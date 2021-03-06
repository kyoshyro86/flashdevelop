﻿using System;
using System.Collections.Generic;
using System.Text;
using PluginCore;
using System.IO;
using SourceControl.Sources;
using SourceControl.Managers;
using ProjectManager.Projects;
using System.Windows.Forms;
using PluginCore.Managers;
using PluginCore.Localization;

namespace SourceControl.Actions
{
    public class ProjectWatcher
    {
        static internal readonly List<IVCManager> VCManagers = new List<IVCManager>();
        static VCManager vcManager;
        static FSWatchers fsWatchers;
        static OverlayManager ovManager;
        static Project currentProject;

        internal static void Init()
        {
            fsWatchers = new FSWatchers();
            ovManager = new OverlayManager(fsWatchers);
            vcManager = new VCManager(ovManager);

            SetProject(PluginBase.CurrentProject as Project);
        }

        internal static void Dispose()
        {
            if (vcManager != null)
            {
                vcManager.Dispose();
                fsWatchers.Dispose();
                ovManager.Dispose();
                currentProject = null;
            }
        }

        internal static void SetProject(Project project)
        {
            currentProject = project;

            fsWatchers.SetProject(project);
            ovManager.Reset();
        }

        internal static void SelectionChanged()
        {
            ovManager.SelectionChanged();
        }

        internal static void ForceRefresh()
        {
            fsWatchers.ForceRefresh();
        }


        #region file actions

        internal static bool HandleFileBeforeRename(string path)
        {
            WatcherVCResult result = fsWatchers.ResolveVC(path, true);
            if (result == null || result.Status == VCItemStatus.Unknown)
                return false;

            return result.Manager.FileActions.FileBeforeRename(path);
        }

        internal static bool HandleFileRename(string[] paths)
        {
            WatcherVCResult result = fsWatchers.ResolveVC(paths[0], true);
            if (result == null || result.Status == VCItemStatus.Unknown)
                return false;

            return result.Manager.FileActions.FileRename(paths[0], paths[1]);
        }

        internal static bool HandleFileDelete(string[] paths, bool confirm)
        {
            if (paths == null || paths.Length == 0) return false;
            WatcherVCResult result = fsWatchers.ResolveVC(Path.GetDirectoryName(paths[0]));
            if (result == null) return false;

            List<string> svnRemove = new List<string>();
            List<string> regularRemove = new List<string>();
            List<string> hasModification = new List<string>();
            List<string> hasUnknown = new List<string>();
            try
            {
                foreach (string path in paths)
                {
                    result = fsWatchers.ResolveVC(path, true);
                    if (result == null || result.Status == VCItemStatus.Unknown) regularRemove.Add(path);
                    else
                    {
                        IVCManager manager = result.Manager;
                        string root = result.Watcher.Path;
                        int p = root.Length + 1;
                        
                        if (Directory.Exists(path))
                        {
                            List<string> files = new List<string>();
                            GetAllFiles(path, files);
                            foreach (string file in files)
                            {
                                VCItemStatus status = manager.GetOverlay(file, root);
                                if (status == VCItemStatus.Unknown)
                                    hasUnknown.Add(file.Substring(p));
                                else if (status > VCItemStatus.UpToDate)
                                    hasModification.Add(file.Substring(p));
                            }
                        }
                        else if (result.Status > VCItemStatus.UpToDate)
                            hasModification.Add(path);

                        if (svnRemove.Count > 0)
                        {
                            if (Path.GetDirectoryName(svnRemove[0]) != Path.GetDirectoryName(path))
                                throw new UnsafeOperationException(TextHelper.GetString("SourceControl.Info.ElementsLocatedInDiffDirs"));
                        }
                        svnRemove.Add(path);
                    }
                }
                if (regularRemove.Count > 0 && svnRemove.Count > 0)
                    throw new UnsafeOperationException(TextHelper.GetString("SourceControl.Info.MixedSelectionOfElements"));

                if (svnRemove.Count == 0 && regularRemove.Count > 0)
                    return false; // regular deletion
            }
            catch (UnsafeOperationException upex)
            {
                MessageBox.Show(upex.Message, TextHelper.GetString("SourceControl.Info.UnsafeDeleteOperation"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return true; // prevent regular deletion
            }

            if (hasUnknown.Count > 0 && confirm)
            {
                string title = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                string msg = TextHelper.GetString("SourceControl.Info.ConfirmUnversionedDelete") + "\n\n" + String.Join("\n", hasUnknown.ToArray());
                if (MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK) return true;
            }

            if (hasModification.Count > 0 && confirm)
            {
                string title = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
                string msg = TextHelper.GetString("SourceControl.Info.ConfirmLocalModsDelete") + "\n\n" + String.Join("\n", hasModification.ToArray());
                if (MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK) return true;
            }

            return result.Manager.FileActions.FileDelete(paths, confirm);
        }

        private static void GetAllFiles(string path, List<string> files)
        {
            string[] search = Directory.GetFiles(path);
            foreach (string file in search)
            {
                string name = Path.GetFileName(file);
                if (name[0] == '.') continue;
                files.Add(file);
            }

            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                string name = Path.GetFileName(dir);
                if (name[0] == '.') continue;
                FileInfo info = new FileInfo(dir);
                if ((info.Attributes & FileAttributes.Hidden) > 0) continue;
                GetAllFiles(dir, files);
            }
        }

        internal static bool HandleFileMove(string[] paths)
        {
            WatcherVCResult result = fsWatchers.ResolveVC(paths[0], true);
            if (result == null || result.Status == VCItemStatus.Unknown)
                return false; // origin not under VC, ignore
            WatcherVCResult result2 = fsWatchers.ResolveVC(paths[1], true);
            if (result2 == null || result2.Status == VCItemStatus.Unknown)
                return false; // target dir not under VC, ignore

            return result.Manager.FileActions.FileMove(paths[0], paths[1]);
        }

        #endregion

    }
    
    class UnsafeOperationException:Exception
    {
        public UnsafeOperationException(string message)
            : base(message)
        {
        }
    }

}
