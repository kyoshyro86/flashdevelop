using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ProjectManager.Projects.Haxe
{
    public class HaxeProject : Project
    {
        // hack : we cannot reference settings HaxeProject is also used by FDBuild
        public static bool saveHXML = false;

        public HaxeProject(string path)
            : base(path, new HaxeOptions())
        {
            movieOptions = new HaxeMovieOptions();
        }

        public override string Language { get { return "haxe"; } }
        public override bool HasLibraries { get { return !NoOutput && IsFlashOutput; } }
        public override bool RequireLibrary { get { return IsFlashOutput; } }
        
        public override bool EnableInteractiveDebugger 
        { 
            get 
            {
                return (movieOptions.Version == 9 || movieOptions.Version == 10) 
                    && CompilerOptions.EnableDebug;
            } 
        }

        public override String LibrarySWFPath
        {
            get
            {
                string projectName = RemoveDiacritics(Name);
                return Path.Combine("obj", projectName + "Resources.swf");
            }
        }

        public new HaxeOptions CompilerOptions { get { return (HaxeOptions)base.CompilerOptions; } }

        public bool IsFlashOutput
        {
            get { return MovieOptions.Version < 11; }
        }
        public bool IsJavacriptOutput
        {
            get { return MovieOptions.Version == 11; }
        }
        public bool IsNekoOutput
        {
            get { return MovieOptions.Version == 12; }
        }
        public bool IsPhpOutput
        {
            get { return MovieOptions.Version == 13; }
        }
        public bool IsCppOutput
        {
            get { return MovieOptions.Version == 14; }
        }

        public override string GetInsertFileText(string inFile, string path, string export, string nodeType)
        {
            bool isInjectionTarget = (UsesInjection && path == GetAbsolutePath(InputPath));
            if (export != null) return export;
            if (IsLibraryAsset(path) && !isInjectionTarget)
                return GetAsset(path).ID;
            else if (!NoOutput && FileInspector.IsHaxeFile(inFile, Path.GetExtension(inFile).ToLower()))
                return ProjectPaths.GetRelativePath(Path.GetDirectoryName(ProjectPath), path).Replace('\\', '/');
            else
                return ProjectPaths.GetRelativePath(Path.GetDirectoryName(inFile), path).Replace('\\', '/');
        }

        string Quote(string s)
        {
            if (s.IndexOf(" ") >= 0)
                return "\"" + s + "\"";
            return s;
        }

        public string[] BuildHXML(string[] paths, string outfile, bool release )
        {
            List<String> pr = new List<String>();

            // class paths
            List<String> classPaths = new List<String>();
            foreach (string cp in paths)
                classPaths.Add(cp);
            foreach (string cp in this.Classpaths)
                classPaths.Add(cp);
            foreach (string cp in classPaths) {
                String ccp = String.Join("/",cp.Split('\\'));
                pr.Add("-cp " + Quote(ccp));
            }

            // libraries
            foreach (string lib in CompilerOptions.Libraries)
                pr.Add("-lib " + lib);

            // compilation mode
            string mode = null;
            if (IsFlashOutput) mode = "swf";
            else if (IsJavacriptOutput) mode = "js";
            else if (IsNekoOutput) mode = "neko";
            else if (IsPhpOutput) mode = "php";
            else if (IsCppOutput) mode = "cpp";
            else throw new SystemException("Unknown mode");

            outfile = String.Join("/",outfile.Split('\\'));
            pr.Add("-" + mode + " " + Quote(outfile));

            // flash options
            if (IsFlashOutput)
            {
                string htmlColor = this.MovieOptions.Background.Substring(1);

                if( htmlColor.Length > 0 )
                    htmlColor = ":" + htmlColor;

                pr.Add("-swf-version " + MovieOptions.Version);
                pr.Add("-swf-header " + string.Format("{0}:{1}:{2}{3}", MovieOptions.Width, MovieOptions.Height, MovieOptions.Fps, htmlColor));

                if( !UsesInjection && LibraryAssets.Count > 0 )
                    pr.Add("-swf-lib " + Quote(LibrarySWFPath));

                if( CompilerOptions.FlashStrict )
                    pr.Add("--flash-strict");

            }

            // debug 
            if (!release)
            {
                pr.Add("-debug");
                if( IsFlashOutput && MovieOptions.Version >= 9 && CompilerOptions.EnableDebug )
                    pr.Add("-D fdb");
            }

            // defines
            foreach (string def in CompilerOptions.Directives)
                pr.Add("-D "+Quote(def));

            // add project files marked as "always compile"
            foreach( string relTarget in CompileTargets )
            {
                string absTarget = GetAbsolutePath(relTarget);
                // guess the class name from the file name
                foreach (string cp in classPaths)
                    if( absTarget.StartsWith(cp, StringComparison.OrdinalIgnoreCase) ) {
                        string className = absTarget.Substring(cp.Length);
                        className = className.Substring(0, className.LastIndexOf('.'));
                        className = Regex.Replace(className, "[\\\\/]+", ".");
                        if( className.StartsWith(".") ) className = className.Substring(1);
                        if( CompilerOptions.MainClass != className )
                            pr.Add(className);
                    }
            }

            // add main class
            if( CompilerOptions.MainClass != null && CompilerOptions.MainClass.Length > 0)
                pr.Add("-main " + CompilerOptions.MainClass);


            // extra options
            foreach (string opt in CompilerOptions.Additional) {
                String p = opt.Trim();                   
                if( p == "" || p[0] == '#' )
                    continue;    
                char[] space = {' '};
                string[] parts = p.Split(space, 2);
                if (parts.Length == 1)
                    pr.Add(p);
                else
                    pr.Add(parts[0] + ' ' + Quote(parts[1]));
            }

            return pr.ToArray();
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
                if (saveHXML) {
                    StreamWriter hxml = File.CreateText(Path.ChangeExtension(fileName, "hxml"));
                    foreach( string e in BuildHXML(new string[0],this.OutputPath,true) )
                        hxml.WriteLine(e);
                    hxml.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IO Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
