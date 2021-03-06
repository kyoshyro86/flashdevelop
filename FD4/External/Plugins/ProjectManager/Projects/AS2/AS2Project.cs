using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ProjectManager.Projects.AS2
{
    public class AS2Project : Project
    {
        public AS2Project(string path)
            : base(path, new MtascOptions()) 
        {
            movieOptions = new AS2MovieOptions();
        }

        public override string Language { get { return "as2"; } }
        public override bool IsCompilable { get { return true; } }
        public override bool UsesInjection { get { return InputPath != ""; } }
        public override bool HasLibraries { get { return OutputType == OutputType.Application && !UsesInjection; } }
        public override bool RequireLibrary { get { return true; } }
        public override string DefaultSearchFilter { get { return "*.as"; } }

        public new MtascOptions CompilerOptions { get { return (MtascOptions)base.CompilerOptions; } }

        public override ProjectManager.Controls.PropertiesDialog CreatePropertiesDialog()
        {
            return new ProjectManager.Controls.AS2.AS2PropertiesDialog();
        }

        public override void ValidateBuild(out string error)
        {
            if (CompilerOptions.UseMain && CompileTargets.Count == 0)
                error = "Description.MissingTarget";
            else
                error = null;
        }

        public override string GetInsertFileText(string inFile, string path, string export, string nodeType)
        {
            bool isInjectionTarget = (UsesInjection && path == GetAbsolutePath(InputPath));
            if (export != null) return export;
            if (IsLibraryAsset(path) && !isInjectionTarget)
                return GetAsset(path).ID;

            if (FileInspector.IsActionScript(inFile, Path.GetExtension(inFile).ToLower()))
                return ProjectPaths.GetRelativePath(Path.GetDirectoryName(ProjectPath), path).Replace('\\', '/');
            else
                return ProjectPaths.GetRelativePath(Path.GetDirectoryName(inFile), path).Replace('\\', '/');
        }

        public override CompileTargetType AllowCompileTarget(string path, bool isDirectory)
        {
            if (!isDirectory && Path.GetExtension(path) != ".as") return CompileTargetType.None;

            foreach(string cp in AbsoluteClasspaths)
                if (path.StartsWith(cp, StringComparison.OrdinalIgnoreCase))
                    return CompileTargetType.AlwaysCompile;
            return CompileTargetType.None;
        }

        public override bool Clean()
        {
            try
            {
                if (OutputPath != null && OutputPath.Length > 0 && File.Exists(GetAbsolutePath(OutputPath)))
                    File.Delete(GetAbsolutePath(OutputPath));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string GetOtherIDE(bool runOutput, bool releaseMode, out string error)
        {
            error = null;
            return "FlashIDE";
        }

        #region Load/Save

		public static AS2Project Load(string path)
		{
			AS2ProjectReader reader = new AS2ProjectReader(path);

			try
			{
				return reader.ReadProject();
			}
			catch (System.Xml.XmlException exception)
			{
				string format = string.Format("Error in XML Document line {0}, position {1}.",
					exception.LineNumber,exception.LinePosition);
				throw new Exception(format,exception);
			}
			finally { reader.Close(); }
		}

		public override void Save()
        {
            SaveAs(ProjectPath);
        }

        public override void SaveAs(string fileName)
        {
            if (!AllowedSaving(fileName)) return;
            try
            {
                AS2ProjectWriter writer = new AS2ProjectWriter(this, fileName);
				writer.WriteProject();
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IO Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
		}

		#endregion
    }
}
