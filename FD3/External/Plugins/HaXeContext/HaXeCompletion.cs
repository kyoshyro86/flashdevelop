﻿using System;
using System.Text;
using System.Collections;

using System.Diagnostics;
using System.Xml;
using System.Text.RegularExpressions;

using PluginCore;
using PluginCore.Managers;
using ProjectManager.Projects.Haxe;
using System.IO;
using PluginCore.Helpers;
using PluginCore.Localization;

namespace HaXeContext
{
    class HaXeCompletion
    {
        Process p;
        int position;
        ScintillaNet.ScintillaControl sci;
        ArrayList tips;
        int nbErrors;
         
        public HaXeCompletion(ScintillaNet.ScintillaControl sci, int position)
        {
            this.sci = sci;
            this.position = position;
            tips = new ArrayList();
            nbErrors = 0;
        }

        private void initProcess(bool completionMode)
        {
            // check haxe project & context
            if (PluginBase.CurrentProject == null || !(PluginBase.CurrentProject is HaxeProject)
                || !(Context.Context is HaXeContext.Context))
                return;

            PluginBase.MainForm.CallCommand( "SaveAllModified", null );
            
            HaxeProject hp = (PluginBase.CurrentProject as HaxeProject);
          
            // Current file
            string file = PluginBase.MainForm.CurrentDocument.FileName;

            // Locate carret position
            Int32 pos = position + 1; // sci.CurrentPos;
            // locate a . or (
            while (pos > 1 && sci.CharAt(pos - 1) != '.' && sci.CharAt(pos - 1) != '(')
                pos--;

            try
            {
                Byte[] bom = new Byte[4];
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fs.CanSeek)
                {
                    fs.Read(bom, 0, 4);
                    fs.Close();
                    if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                    {
                        pos += 3; // Skip BOM
                    }
                }
            }
            catch {}
                        
            // Build haXe command
            string[] paths = ProjectManager.PluginMain.Settings.GlobalClasspaths.ToArray();
            string hxml = String.Join(" ", hp.BuildHXML(paths, "__nothing__", true));

            // Get the current class edited (ensure completion even if class not reference in the project)
            int start = file.LastIndexOf("\\") + 1;
            int end = file.LastIndexOf(".");
            string package = Context.Context.CurrentModel.Package;
            if (package != "")
            {
                string cl = Context.Context.CurrentModel.Package + "." + file.Substring(start, end - start);
                string libToAdd = file.Split(new string[] { "\\" + String.Join("\\", cl.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)) }, StringSplitOptions.RemoveEmptyEntries).GetValue(0).ToString();
                hxml = hxml+" "+"-cp \"" + libToAdd + "\" "+cl;
            }
            else
                hxml = hxml + " " + file.Substring(start, end - start);

            // Build haXe built-in completion/check syntax command
            string args = completionMode
                ? "--display \"" + file + "\"@" + pos.ToString() + " " + hxml
                : "--no-output " + hxml;

            // compiler path
            string haxePath = Environment.GetEnvironmentVariable("HAXEPATH");
            string customHaxePath = (Context.Context.Settings as HaXeSettings).HaXePath;
            if (customHaxePath != null && customHaxePath.Length > 0)
                haxePath = PathHelper.ResolvePath(customHaxePath);
            
            string process = Path.Combine(haxePath, "haxe.exe");
            if (!File.Exists(process))
            {
                ErrorManager.ShowInfo(String.Format(TextHelper.GetString("Info.HaXeExeError"), "\n"));
                p = null;
                return;
            }

            // Run haXe compiler
            p = new Process();           
            p.StartInfo.FileName = process;
            p.StartInfo.Arguments = args;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.WorkingDirectory = hp.Directory;
            p.EnableRaisingEvents = true;
        }

        private String htmlUnescape(String s)
        {
            return s.Replace("&lt;", "<").Replace("&gt;", ">");
        }

        public ArrayList getList()
        {
            return getList(true);
        }

        public ArrayList getList(bool completionMode)
        {
            // Prepare haXe command
            initProcess(completionMode);
            
            // If project is uncompletly set, return the errors
            if (p == null)
                return tips;
            else
                p.Start();
            
            // Parsing the haXe compiler output
            string[] lines = p.StandardError.ReadToEnd().Split('\n');
            string type = "";
            string error = "";

            for (int i = 0; i < lines.Length; i++)
            {
                String l = lines[i].Trim();
                
                if (l.Length == 0)
                    continue;
                // Get list of properties
                if (l == "<list>")
                {
                    ArrayList content = new ArrayList();
                    string xml = "";
                    while (++i < lines.Length)
                        xml += lines[i];
                    foreach (Match m in Regex.Matches(xml, "<i n=\"([^\"]+)\"><t>([^<]*)</t><d>([^<]*)</d></i>"))
                    {
                        ArrayList seq = new ArrayList();
                        seq.Add(m.Groups[1].Value);
                        seq.Add(htmlUnescape(m.Groups[2].Value));
                        seq.Add(htmlUnescape(m.Groups[3].Value));
                        content.Add(seq);
                    }

                    tips.Add("list");
                    tips.Add(content);

                    break;
                }
                // Get the type
                else if (l == "<type>")
                {
                    type = htmlUnescape(lines[++i].Trim('\r'));

                    tips.Add("type");
                    tips.Add(type);

                    break;
                }
                // Or get the errors (5 max)
                else
                {
                    if (nbErrors == 0)
                        error += l;
                    else if (nbErrors < 5)
                        error += "\n" + l;
                    else if (nbErrors == 5)
                        error += "\n...";

                    nbErrors++;
                }
            }
            p.Close();
            p = null;
            
            if (error != "")
            {
                tips.Clear();
                tips.Add("error");
                tips.Add(error);
            }
            return tips;
        }

    }
}
