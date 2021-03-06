using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.Projects.AS3
{
    public class AS3ProjectReader : ProjectReader
    {
        AS3Project project;

        public AS3ProjectReader(string filename)
            : base(filename, new AS3Project(filename))
        {
            this.project = base.Project as AS3Project;
        }

        public override Project ReadProject()
        {
            AS3Project project = base.ReadProject() as AS3Project;

            // re-import FlashDevelop 4 AIR projects
            if (project.MovieOptions.Version <= 2)
            {
                project.MovieOptions.Version = project.MovieOptions.Version < 2 ? 9 : 10;
                List<String> add = new List<string>();
                add.Add("+configname=air");
                if (project.CompilerOptions.Additional != null) add.AddRange(project.CompilerOptions.Additional);
                project.CompilerOptions.Additional = add.ToArray();

                project.TestMovieBehavior = TestMovieBehavior.Custom;
                project.TestMovieCommand = @"$(FlexSDK)\bin\adl.exe;application.xml bin";
            }
            return project;
        }

        // process AS3-specific stuff
        protected override void ProcessNode(string name)
        {
            if (NodeType == System.Xml.XmlNodeType.Element)
            switch (name)
            {
                case "build": ReadBuildOptions(); break;
                case "library": ReadLibraryAssets(); break;
                case "includeLibraries": ReadIncludeLibraries(); break;
                case "libraryPaths": ReadLibrayPath(); break;
                case "externalLibraryPaths": ReadExternalLibraryPaths(); break;
                case "rslPaths": ReadRSLPaths(); break;
                case "intrinsics": ReadIntrinsicPaths(); break;
                default: base.ProcessNode(name); break;
            }
        }

        private void ReadIntrinsicPaths()
        {
            project.CompilerOptions.IntrinsicPaths = ReadLibrary("intrinsics", SwfAssetMode.Ignore);
        }

        private void ReadRSLPaths()
        {
            project.CompilerOptions.RSLPaths = ReadLibrary("rslPaths", SwfAssetMode.Ignore);
        }

        private void ReadExternalLibraryPaths()
        {
            project.CompilerOptions.ExternalLibraryPaths = ReadLibrary("externalLibraryPaths", SwfAssetMode.ExternalLibrary);
        }

        private void ReadLibrayPath()
        {
            project.CompilerOptions.LibraryPaths = ReadLibrary("libraryPaths", SwfAssetMode.Library);
        }

        private void ReadIncludeLibraries()
        {
            project.CompilerOptions.IncludeLibraries = ReadLibrary("includeLibraries", SwfAssetMode.IncludedLibrary);
        }

        private string[] ReadLibrary(string name, SwfAssetMode mode)
        {
            ReadStartElement(name);
            List<string> elements = new List<string>();
            while (Name == "element")
            {
                string path = GetAttribute("path").Replace('/', '\\');
                elements.Add(path);

                if (mode != SwfAssetMode.Ignore)
                {
                    LibraryAsset asset = new LibraryAsset(project, path);
                    asset.SwfMode = mode;
                    project.SwcLibraries.Add(asset);
                }
                Read();
            }
            ReadEndElement();
            string[] result = new string[elements.Count];
            elements.CopyTo(result);
            return result;
        }

        public void ReadLibraryAssets()
        {
            ReadStartElement("library");
            while (Name == "asset")
            {
                string path = OSPath(GetAttribute("path"));

                if (path == null)
                    throw new Exception("All library assets must have a 'path' attribute.");

                LibraryAsset asset = new LibraryAsset(project, path);
                project.LibraryAssets.Add(asset);

                asset.UpdatePath = OSPath(GetAttribute("update")); // could be null
                asset.FontGlyphs = GetAttribute("glyphs"); // could be null

                Read();
            }
            ReadEndElement();
        }

        public void ReadBuildOptions()
        {
            MxmlcOptions options = project.CompilerOptions;

            ReadStartElement("build");
            while (Name == "option")
            {
                MoveToFirstAttribute();
                switch (Name)
                {
                    case "accessible": options.Accessible = BoolValue; break;
                    case "allowSourcePathOverlap": options.AllowSourcePathOverlap = BoolValue; break;
                    case "benchmark": options.Benchmark = BoolValue; break;
                    case "es": options.ES = BoolValue; break;
                    case "locale": options.Locale = Value; break;
                    case "loadConfig": options.LoadConfig = Value; break;
                    case "optimize": options.Optimize = BoolValue; break;
                    case "showActionScriptWarnings": options.ShowActionScriptWarnings = BoolValue; break;
                    case "showBindingWarnings": options.ShowBindingWarnings = BoolValue; break;
                    case "showInvalidCSS": options.ShowInvalidCSS = BoolValue; break;
                    case "showDeprecationWarnings": options.ShowDeprecationWarnings = BoolValue; break;
                    case "showUnusedTypeSelectorWarnings": options.ShowUnusedTypeSelectorWarnings = BoolValue; break;
                    case "strict": options.Strict = BoolValue; break;
                    case "useNetwork": options.UseNetwork = BoolValue; break;
                    case "useResourceBundleMetadata": options.UseResourceBundleMetadata = BoolValue; break;
                    case "warnings": options.Warnings = BoolValue; break;
                    case "verboseStackTraces": options.VerboseStackTraces = BoolValue; break;
                    case "linkReport": options.LinkReport = Value; break;
                    case "loadExterns": options.LoadExterns = Value; break;
                    case "staticLinkRSL": options.StaticLinkRSL = BoolValue; break;
                    case "additional": options.Additional = Value.Split('\n'); break;
                    case "compilerConstants": options.CompilerConstants = Value.Split('\n'); break;
                    case "customSDK": options.CustomSDK = Value; break;
                }
                Read();
            }
            ReadEndElement();
        }
    }
}
