using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PluginCore.Managers;

namespace ProjectManager.Projects.Haxe
{
    public class HaxeProject : Project
    {
        public HaxeProject(string path)
            : base(path, new HaxeOptions())
        {
            movieOptions = new HaxeMovieOptions();
        }

        public override string Language { get { return "haxe"; } }
        public override bool HasLibraries { get { return !NoOutput && IsFlashOutput; } }
        public override bool RequireLibrary { get { return IsFlashOutput; } }

        public new HaxeOptions CompilerOptions { get { return (HaxeOptions)base.CompilerOptions; } }

        public bool IsFlashOutput
        {
            get { return MovieOptions.Version < 10; }
        }
        public bool IsJavacriptOutput
        {
            get { return MovieOptions.Version == 10; }
        }
        public bool IsNekoOutput
        {
            get { return MovieOptions.Version == 11; }
        }

        public override string GetInsertFileText(string inFile, string path, string export, string nodeType)
        {
            bool isInjectionTarget = (UsesInjection && path == GetAbsolutePath(InputPath));
            if (export != null) return export;
            if (IsLibraryAsset(path) && !isInjectionTarget)
                return GetAsset(path).ID;
            else if (!NoOutput && FileInspector.IsHaxeFile(inFile))
                return ProjectPaths.GetRelativePath(Path.GetDirectoryName(ProjectPath), path).Replace('\\', '/');
            else
                return ProjectPaths.GetRelativePath(Path.GetDirectoryName(inFile), path).Replace('\\', '/');
        }

        #region Load/Save

        public static HaxeProject Load(string path)
        {
            HaxeProjectReader reader = new HaxeProjectReader(path);

            try
            {
                return reader.ReadProject();
            }
            catch (System.Xml.XmlException exception)
            {
                string format = string.Format("Error in XML Document line {0}, position {1}.",
                    exception.LineNumber, exception.LinePosition);
                throw new Exception(format, exception);
            }
            finally { reader.Close(); }
        }

        public override void Save()
        {
            SaveAs(ProjectPath);
        }

        public override void SaveAs(string fileName)
        {
            try
            {
                HaxeProjectWriter writer = new HaxeProjectWriter(this, fileName);
                writer.WriteProject();
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowWarning(ex.Message, ex);
            }
        }

        #endregion
    }
}
