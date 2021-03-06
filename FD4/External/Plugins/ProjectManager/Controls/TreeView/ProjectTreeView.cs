using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ProjectManager.Projects;
using System.Collections.Generic;
using System.Collections.Specialized;
using ProjectManager.Projects.AS3;

namespace ProjectManager.Controls.TreeView
{
	public delegate void DragPathEventHandler(string fromPath, string toPath);

	/// <summary>
	/// The Project Explorer TreeView
	/// </summary>
    public class ProjectTreeView : DragDropTreeView
	{
        Dictionary<string, GenericNode> nodeMap;
		List<Project> projects = new List<Project>();
        Project activeProject;
		string pathToSelect;

		public static ProjectTreeView Instance;
		public event DragPathEventHandler MovePath;
		public event DragPathEventHandler CopyPath;
        public new event EventHandler DoubleClick;

		public ProjectTreeView()
		{
			Instance = this;
			MultiSelect = true;
            nodeMap = new Dictionary<string, GenericNode>();
            ShowNodeToolTips = true;
		}

        public Project ProjectOf(string path)
        {
            foreach (GenericNode node in Nodes)
            {
                if (node is ProjectNode && path.StartsWith(node.BackingPath, StringComparison.OrdinalIgnoreCase))
                    return (node as ProjectNode).ProjectRef;
            }
            return null;
        }

        public Project ProjectOf(GenericNode node)
        {
            GenericNode p = node;
            while (p != null && !(p is ProjectNode))
                p = p.Parent as GenericNode;
            if (p is ProjectNode) return (p as ProjectNode).ProjectRef;
            return null;
        }

		public void Select(string path)
		{
            if (nodeMap.ContainsKey(path))
            {
                SelectedNode = nodeMap[path];
            }
            else
            {
                Int32 index = 0;
                String separator = Path.DirectorySeparatorChar.ToString();
                while (true)
                {
                    index = path.IndexOf(separator, index);
                    if (index == -1) break; // Stop, not found
                    String subPath = path.Substring(0, index);
                    if (nodeMap.ContainsKey(subPath)) nodeMap[subPath].Expand();
                    index++;
                }
                if (nodeMap.ContainsKey(path))
                {
                    SelectedNode = nodeMap[path];
                }
            }
		}

		// this is called by GenericNode when a selected node is refreshed, so that
		// the context menu can rebuild itself accordingly.
		public void NotifySelectionChanged()
		{
			OnAfterSelect(new TreeViewEventArgs(SelectedNode));
		}

        public static bool IsFileTypeHidden(string path)
        {
            if (Path.GetFileName(path).StartsWith("~$")) return true;
            string ext = Path.GetExtension(path).ToLower();
            foreach (string exclude in PluginMain.Settings.ExcludedFileTypes)
                if (ext == exclude) return true;
            return false;
        }

        public void RefreshNode(GenericNode node)
        {
            // refresh the first parent that *can* be refreshed
            while (node != null && !node.IsRefreshable)
            {
                node = node.Parent as GenericNode;
            }
            if (node == null) return;
            // if you refresh a SwfFileNode this way (by asking for it), you get
            // special feedback
            SwfFileNode swfNode = node as SwfFileNode;

            if (swfNode != null) swfNode.RefreshWithFeedback(true);
            else node.Refresh(true);
        }

        #region Custom Double-Click Behavior

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == 515 && DoubleClick != null) // WM_LBUTTONDBLCLK - &H203
            {
                // ok, we only want the base treeview to handle double-clicking to expand things
                // if there's one node selected and it's a folder.
                if (SelectedNodes.Count == 1 && SelectedNode is DirectoryNode)
                    base.DefWndProc(ref m);
                else
                    DoubleClick(this, EventArgs.Empty); // let someone else handle it!
            }
            else base.DefWndProc(ref m);
        }

        #endregion

        #region Properties

        public IDictionary<string, GenericNode> NodeMap
		{
			get { return nodeMap; }
		}

		public Project Project
		{
			get { return activeProject; }
			set
			{
                activeProject = value;
                string path = activeProject != null ? activeProject.Directory : null;
                SelectedNode = null;
                foreach (GenericNode node in Nodes)
                {
                    if (!(node is ProjectNode)) continue;
                    if (node.BackingPath == path)
                    {
                        (node as ProjectNode).IsActive = true;
                        SelectedNode = node as GenericNode;
                    }
                    else (node as ProjectNode).IsActive = false;
                }
            }
		}

        public List<Project> Projects
        {
            get { return projects; }
            set
            {
                projects = value != null ? new List<Project>(value) : new List<Project>();

                BeginUpdate();
                try
                {
                    Visible = false;
                    RebuildTree(false, false);

                    if (projects.Count > 0)
                    {
                        //Project = projects[0];
                        ExpandedPaths = PluginMain.Settings.GetPrefs(projects[0]).ExpandedPaths;
                        Win32.Scrolling.SetScrollPos(this, new Point());
                    }
                    else Project = null;

                    if (Nodes.Count > 0) SelectedNode = Nodes[0] as GenericNode;
                }
                finally
                {
                    Visible = true;
                    EndUpdate();
                }
            }
        }

		public string PathToSelect
		{
			get { return pathToSelect; }
			set { pathToSelect = value; }
		}

		public new GenericNode SelectedNode
		{
			get { return base.SelectedNode as GenericNode; }
			set { base.SelectedNode = value; }
		}

		public string SelectedPath
		{
			get
			{
				if (SelectedNode != null) return SelectedNode.BackingPath;
				else return null;
			}
		}

		public string[] SelectedPaths
		{
			get
			{
				ArrayList paths = new ArrayList();
                foreach (GenericNode node in SelectedNodes)
                {
                    paths.Add(node.BackingPath);

                    // if this is a "mapped" file, that is a file that "hides" other related files,
                    // make sure we select the related files also.
                    // DISABLED - causes inconsistent behavior
                    /*if (node is FileNode)
                        foreach (GenericNode mappedNode in node.Nodes)
                            if (mappedNode is FileNode)
                                paths.Add(mappedNode.BackingPath);*/
                }
				return paths.ToArray(typeof(string)) as string[];
			}
			set
			{
				ArrayList nodes = new ArrayList();
				foreach (string path in value)
					if (nodeMap.ContainsKey(path))
						nodes.Add(nodeMap[path]);
				SelectedNodes = nodes;
			}
		}

		public LibraryAsset SelectedAsset 
		{ 
			get { return activeProject.GetAsset(SelectedPath); }
		}

		public List<string> ExpandedPaths
		{
			get
			{
                List<string> expanded = new List<string>();
				AddExpanded(Nodes, expanded); // add in the correct order - top-down
                return expanded;
			}
			set
			{
				foreach (string path in value)
					if (nodeMap.ContainsKey(path))
					{
                        GenericNode node = nodeMap[path];
						if (!(node is SwfFileNode))
						{
							node.Expand();
							node.Refresh(false);
						}
					}
			}
		}

		private void AddExpanded(TreeNodeCollection nodes, List<string> list)
		{
			foreach (GenericNode node in nodes)
				if (node.IsExpanded)
				{
					list.Add(node.BackingPath);
					AddExpanded(node.Nodes,list);
				}
		}

		#endregion

		#region TreeView Population
        /// <summary>
		/// Rebuilds the tree from scratch.
		/// </summary>
        public void RebuildTree(bool saveState)
        {
            RebuildTree(saveState, true);
        }

		/// <summary>
		/// Rebuilds the tree from scratch.
		/// </summary>
		public void RebuildTree(bool saveState, bool restoreExpanded)
		{
			// store old tree state
            List<string> previouslyExpanded = restoreExpanded ? ExpandedPaths : null;
            Point scrollPos = restoreExpanded ? Win32.Scrolling.GetScrollPos(this) : new Point();

            BeginUpdate();
            try
            {
                foreach (GenericNode node in Nodes)
                    node.Dispose();

                SelectedNodes = null;
                Nodes.Clear();
                nodeMap.Clear();
                ShowRootLines = true;

                if (projects.Count == 0)
                    return;

                // cleanup
                EndUpdate();
                Refresh();
                BeginUpdate();

                foreach (Project project in projects)
                    RebuildProjectNode(project);

                // restore tree state
                if (restoreExpanded)
                {
                    ExpandedPaths = previouslyExpanded;
                    SelectedNode = Nodes[0] as GenericNode;// projectNode;
                    Win32.Scrolling.SetScrollPos(this, scrollPos);
                }
            }
            finally
            {
                EndUpdate();
            }
		}

        private void RebuildProjectNode(Project project)
        {
            activeProject = project;

            // create the top-level project node
            ProjectNode projectNode = new ProjectNode(project);
            Nodes.Add(projectNode);
            projectNode.Refresh(true);
            projectNode.Expand();

            ArrayList projectClasspaths = new ArrayList();
            ArrayList globalClasspaths = new ArrayList();

            if (PluginMain.Settings.ShowProjectClasspaths)
            {
                projectClasspaths.AddRange(project.Classpaths);
                if (project.AdditionalPaths != null) projectClasspaths.AddRange(project.AdditionalPaths);
            }

            if (PluginMain.Settings.ShowGlobalClasspaths)
                globalClasspaths.AddRange(PluginMain.Settings.GlobalClasspaths);

            ReferencesNode refs = new ReferencesNode(project.Directory, "References");
            projectNode.References = refs;
            ClasspathNode cpNode;

            foreach (string projectClasspath in projectClasspaths)
            {
                string absolute = projectClasspath;
                if (!Path.IsPathRooted(absolute))
                    absolute = project.GetAbsolutePath(projectClasspath);
                if ((absolute + "\\").StartsWith(project.Directory + "\\"))
                    continue;

                cpNode = new ProjectClasspathNode(project, absolute, projectClasspath);
                refs.Nodes.Add(cpNode);
                cpNode.Refresh(true);
            }

            foreach (string globalClasspath in globalClasspaths)
            {
                string absolute = globalClasspath;
                if (!Path.IsPathRooted(absolute))
                    absolute = project.GetAbsolutePath(globalClasspath);
                if (absolute.StartsWith(project.Directory + Path.DirectorySeparatorChar.ToString()))
                    continue;

                cpNode = new ClasspathNode(project, absolute, globalClasspath);
                refs.Nodes.Add(cpNode);
                cpNode.Refresh(true);
            }

            // add external libraries at the top level also
            if (project is AS3Project)
                foreach (LibraryAsset asset in (project as AS3Project).SwcLibraries)
                {
                    if (!asset.IsSwc) continue;
                    // check if SWC is inside the project or inside a classpath
                    string absolute = asset.Path;
                    if (!Path.IsPathRooted(absolute))
                        absolute = project.GetAbsolutePath(asset.Path);

                    bool showNode = true;
                    if (absolute.StartsWith(project.Directory))
                        showNode = false;
                    foreach (string path in project.AbsoluteClasspaths)
                        if (absolute.StartsWith(path))
                        {
                            showNode = false;
                            break;
                        }
                    foreach (string path in PluginMain.Settings.GlobalClasspaths)
                        if (absolute.StartsWith(path))
                        {
                            showNode = false;
                            break;
                        }

                    if (showNode && File.Exists(absolute))
                    {
                        SwfFileNode swcNode = new SwfFileNode(absolute);
                        refs.Nodes.Add(swcNode);
                        swcNode.Refresh(true);
                    }
                }
        }

		/// <summary>
		/// Refreshes all visible nodes in-place.
		/// </summary>
		public void RefreshTree()
		{
			RefreshTree(null);
		}

		/// <summary>
		/// Refreshes only the nodes representing the given paths, or all nodes if
		/// paths is null.
		/// </summary>
		public void RefreshTree(string[] paths)
		{
			BeginUpdate();

            try
            {
                if (paths == null)
                {
                    // full recursive refresh
                    foreach (GenericNode node in Nodes)
                        node.Refresh(true);
                }
                else
                {
                    // selective refresh
                    foreach (string path in paths)
                        if (nodeMap.ContainsKey(path))
                            nodeMap[path].Refresh(false);
                }
            }
            catch { }
            finally { EndUpdate(); }
		}

		#endregion

		#region TreeView Overrides

		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
            // signal the node about to expand that the expansion is coming
            GenericNode node = e.Node as GenericNode;
            if (node != null) node.BeforeExpand();

			base.OnBeforeExpand(e);
		}

		protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
		{
			base.OnBeforeLabelEdit(e);
			GenericNode node = e.Node as GenericNode;

            if (pathToSelect == node.BackingPath)
            {
                e.CancelEdit = false;
                pathToSelect = null;
            }

			if (!node.IsRenamable)
				e.CancelEdit = true;
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			base.OnAfterSelect(e);
			HideSelection = (SelectedNode is ProjectNode);
		}

		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			if (e.Item is GenericNode && (e.Item as GenericNode).IsDraggable)
				base.OnItemDrag(e);
		}

        protected override DataObject BeginDragNodes(ArrayList nodes)
        {
            DataObject data = base.BeginDragNodes(nodes);

            // we also want to drag files, not just nodes, so that we can drop
            // them on explorer, etc.
            StringCollection paths = new StringCollection();

            foreach (GenericNode node in nodes)
                paths.Add(node.BackingPath);

            data.SetFileDropList(paths);
            return data;
        }

		protected override void OnMoveNode(TreeNode node, TreeNode targetNode)
		{
			if (MovePath != null && node is GenericNode)
			{
				string fromPath = (node as GenericNode).BackingPath;
				string toPath = (targetNode as GenericNode).BackingPath;

				MovePath(fromPath,toPath);
			}
		}

		protected override void OnCopyNode(TreeNode node, TreeNode targetNode)
		{
            if (CopyPath != null && node is GenericNode)
			{
				string fromPath = (node as GenericNode).BackingPath;
				string toPath = (targetNode as GenericNode).BackingPath;

				CopyPath(fromPath,toPath);
			}			
		}

		protected override void OnFileDrop(string[] paths, TreeNode targetNode)
		{
            if (CopyPath != null && targetNode is GenericNode)
			{
				string toPath = (targetNode as GenericNode).BackingPath;
				foreach (string fromPath in paths)
					CopyPath(fromPath,toPath);
			}
		}

		protected override TreeNode ChangeDropTarget(TreeNode targetNode)
		{
			// you can only drop things into folders
			GenericNode node = targetNode as GenericNode;

			while (node != null && (!node.IsDropTarget || node.IsInvalid))
				node = node.Parent as GenericNode;

			return node;
		}


		#endregion

	}

}
