﻿using System;
using System.Collections.Generic;
using System.Text;
using SourceControl.Sources;
using System.Drawing;
using System.Windows.Forms;
using PluginCore;
using ProjectManager.Controls.TreeView;
using SourceControl.Actions;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;

namespace SourceControl.Managers
{
    class OverlayManager
    {
        public const string META_VC = "SourceControl.VC";
        public const string META_ROOT = "SourceControl.ROOT";
        public const string META_STATUS = "SourceControl.STATUS";

        ProjectTreeView currentTree;
        FSWatchers fsWatchers;

        public OverlayManager(FSWatchers fsWatchers)
        {
            this.fsWatchers = fsWatchers;

            FileNode.OnFileNodeRefresh += new FileNodeRefresh(FileNode_OnFileNodeRefresh);
            DirectoryNode.OnDirectoryNodeRefresh += new DirectoryNodeRefresh(DirectoryNode_OnDirectoryNodeRefresh);

            OverlayMap.Init();
        }

        internal void Dispose()
        {
            OverlayMap.Reset();
            fsWatchers = null;
        }

        internal void Reset()
        {
            OverlayMap.Reset();
            if (currentTree != null) ResetNodes(currentTree.Nodes);
        }

        internal void Refresh()
        {
            if (currentTree != null)
            {
                currentTree.RefreshTree();
                SelectionChanged();
            }
        }

        internal void SelectionChanged()
        {
            ProjectSelectionState state = new ProjectSelectionState(currentTree);
            TreeContextMenuUpdate.SetMenu(currentTree, state);
        }

        void ResetNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node is GenericNode)
                {
                    GenericNode gnode = (GenericNode)node;
                    if (gnode.Meta != null && gnode.Meta.ContainsKey(META_VC))
                    {
                        gnode.Meta.Remove(META_VC);
                        gnode.Meta.Remove(META_ROOT);
                        gnode.Meta.Remove(META_STATUS);
                    }
                }
                if (node.Nodes.Count > 0) ResetNodes(node.Nodes);
            }
        }

        void DirectoryNode_OnDirectoryNodeRefresh(DirectoryNode node)
        {
            if (node is ProjectNode)
                currentTree = node.TreeView as ProjectTreeView;

            UpdateNodeStatus(node);
        }

        void FileNode_OnFileNodeRefresh(FileNode node)
        {
            UpdateNodeStatus(node);
        }

        void UpdateNodeStatus(GenericNode node)
        {
            if (node.Meta == null)
                node.Meta = new Dictionary<string, object>();

            if (!node.Meta.ContainsKey(META_VC))
                LocateVC(node);

            IVCManager currentVC = node.Meta[META_VC] as IVCManager;
            string root = (string)node.Meta[META_ROOT];

            if (currentVC != null)
            {
                VCItemStatus status = currentVC.GetOverlay(node.BackingPath, root);
                node.Meta[META_STATUS] = status;
                OverlayMap.SetOverlay(status, node, currentTree);
            }
        }

        void LocateVC(GenericNode node)
        {
            node.Meta[META_VC] = null;
            node.Meta[META_ROOT] = null;
            node.Meta[META_STATUS] = VCItemStatus.Unknown;

            if (node.Parent != null && node.Parent is GenericNode)
            {
                GenericNode parent = (GenericNode)node.Parent;
                if (parent.Meta != null && parent.Meta.ContainsKey(META_VC))
                {
                    node.Meta[META_VC] = parent.Meta[META_VC];
                    node.Meta[META_ROOT] = parent.Meta[META_ROOT];
                    return;
                }
            }

            WatcherVCResult result = fsWatchers.ResolveVC(node.BackingPath);
            if (result != null)
            {
                node.Meta[META_VC] = result.Manager;
                node.Meta[META_ROOT] = result.Watcher.Path;
            }
        }
    }

    #region Overlay builder

    class OverlayMap: Dictionary<int, int>
    {
        static Image iconSkin;
        static Dictionary<VCItemStatus, OverlayMap> maps = new Dictionary<VCItemStatus, OverlayMap>();

        static public void Init()
        {
            iconSkin = GetSkin();

            AddOverlay(VCItemStatus.UpToDate);
            AddOverlay(VCItemStatus.Modified);
            AddOverlay(VCItemStatus.Ignored);
            AddOverlay(VCItemStatus.Added);
            AddOverlay(VCItemStatus.Conflicted);
            AddOverlay(VCItemStatus.Deleted);
            AddOverlay(VCItemStatus.Replaced);
            AddOverlay(VCItemStatus.External);
            AddOverlay(VCItemStatus.Unknown);
        }

        private static Image GetSkin()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return new Bitmap(assembly.GetManifestResourceStream("SourceControl.Resources.icons.png"));
            }
            catch
            {
                return new Bitmap(160, 16);
            }
        }

        static public void Reset()
        {
            foreach (OverlayMap map in maps.Values)
                map.Clear();
        }

        private static void AddOverlay(VCItemStatus status)
        {
            maps.Add(status, new OverlayMap());
        }

        static public void SetOverlay(VCItemStatus status, TreeNode node, TreeView tree)
        {
            if (!maps.ContainsKey(status)) return;
            OverlayMap map = maps[status];

            if (map.ContainsKey(node.ImageIndex))
            {
                node.SelectedImageIndex = node.ImageIndex = map[node.ImageIndex];
                return;
            }

            if (tree.ImageList.Images.Count <= node.ImageIndex)
                return;

            Image original = tree.ImageList.Images[node.ImageIndex];
            Bitmap composed = original.Clone() as Bitmap;
            using (Graphics destination = Graphics.FromImage(composed))
            {
                destination.DrawImage(iconSkin, new Rectangle(0, 0, 16, 16), 
                    new Rectangle((int)status * 16, 0, 16, 16), GraphicsUnit.Pixel);
            }
            int index = tree.ImageList.Images.Count;
            tree.ImageList.Images.Add(composed);
            map[node.ImageIndex] = index;
            node.SelectedImageIndex = node.ImageIndex = index;
        }

        public OverlayMap()
        {
        }
    }

    #endregion

    #region Project Selection State

    class ProjectSelectionState
    {
        public int Files = 0;
        public int Dirs = 0;
        public int Unknown = 0;
        public int Ignored = 0;
        public int Added = 0;
        public int Revert = 0;
        public int Diff = 0;
        public int Conflict = 0;
        public int Other = 0;
        public int Total = 0;
        public IVCManager Manager = null;

        public ProjectSelectionState(ProjectTreeView tree)
        {
            if (tree == null || tree.SelectedNodes.Count == 0)
                return;

            foreach (TreeNode node in tree.SelectedNodes)
            {
                if (node is FileNode) Files++;
                else if (node is DirectoryNode) Dirs++;
                else return; // unknown node in selection - no action

                GenericNode gnode = (GenericNode)node;
                if (gnode.Meta == null || !gnode.Meta.ContainsKey(OverlayManager.META_STATUS)
                    || !gnode.Meta.ContainsKey(OverlayManager.META_VC))
                    return; // incomplete status

                if (Manager == null) Manager = gnode.Meta[OverlayManager.META_VC] as IVCManager;
                else if (gnode.Meta[OverlayManager.META_VC] != Manager)
                    return; // several managers...

                VCItemStatus status = (VCItemStatus)(gnode.Meta[OverlayManager.META_STATUS]);
                if (status == VCItemStatus.Unknown) Unknown++;
                else if (status == VCItemStatus.Ignored) Ignored++;
                else if (status == VCItemStatus.Added) Added++;
                else if (status == VCItemStatus.Conflicted) Conflict++;
                else if (status == VCItemStatus.Modified || status == VCItemStatus.Deleted) { Revert++; Diff++; }
                else if (status == VCItemStatus.Replaced) Revert++;
                else Other++;
                Total++;
            }
        }
    }

    #endregion
}
