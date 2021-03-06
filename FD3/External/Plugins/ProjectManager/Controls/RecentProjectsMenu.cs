using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Helpers;

namespace ProjectManager.Controls
{
    public delegate void ProjectEventHandler(string projectPath);

    public class RecentProjectsMenu : ToolStripMenuItem
    {
        private ToolStripMenuItem clearItemMenu;
        private ToolStripMenuItem clearItemToolbar;
        public event ProjectEventHandler ProjectSelected;
        public ToolStripDropDownButton ToolbarSelector;

        public RecentProjectsMenu() : base(TextHelper.GetString("Label.RecentProjects"))
        {
            ToolbarSelector = new ToolStripDropDownButton();
            ToolbarSelector.Text = TextHelper.GetString("Label.RecentProjects").Replace("&", "");
            ToolbarSelector.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ToolbarSelector.Image = Icons.Project.Img;
            ToolbarSelector.Enabled = false;
            string text = TextHelper.GetString("Label.ClearRecentProjects");
            clearItemMenu = new ToolStripMenuItem(text);
            clearItemMenu.Click += delegate{ ClearAllItems(); };
            clearItemToolbar = new ToolStripMenuItem(text);
            clearItemToolbar.Click += delegate { ClearAllItems(); };
            RebuildList();
        }

        public void AddOpenedProject(string projectPath)
        {
            if (PluginMain.Settings.RecentProjects.Contains(projectPath))
            {
                PluginMain.Settings.RecentProjects.Remove(projectPath);
            }
            PluginMain.Settings.RecentProjects.Insert(0, projectPath);
            RebuildList();
        }
        public void RemoveOpenedProject(string projectPath)
        {
            if (PluginMain.Settings.RecentProjects.Contains(projectPath))
            {
                PluginMain.Settings.RecentProjects.Remove(projectPath);
                RebuildList();
            }
        }

        private void RebuildList()
        {
            int count = PluginMain.Settings.RecentProjects.Count;
            int max = PluginMain.Settings.MaxRecentProjects;
            if (count > max) PluginMain.Settings.RecentProjects.RemoveRange(max, count - max);
            if (count == 0)
            {
                ClearAllItems();
                return;
            }
            DropDownItems.Clear();
            ToolbarSelector.DropDownItems.Clear();
            foreach (string projectPath in PluginMain.Settings.RecentProjects)
            {
                DropDownItems.Add(BuildItem(projectPath, true));
                ToolbarSelector.DropDownItems.Add(BuildItem(projectPath, false));
            }
            DropDownItems.Add(new ToolStripSeparator());
            DropDownItems.Add(clearItemMenu);
            ToolbarSelector.DropDownItems.Add(new ToolStripSeparator());
            ToolbarSelector.DropDownItems.Add(clearItemToolbar);
            ToolbarSelector.Enabled = Enabled = true;
        }

        private ToolStripMenuItem BuildItem(string projectPath, bool showPath)
        {
            ToolStripMenuItem item = new ToolStripMenuItem();
            string name = Path.GetFileNameWithoutExtension(projectPath);
            if (name.Length == 0) 
            {
                if (showPath) item.Text = PathHelper.GetCompactPath(Path.GetDirectoryName(projectPath));
                else item.Text = Path.GetFileName(Path.GetDirectoryName(projectPath));
            }
            else 
            {
                if (showPath) item.Text = PathHelper.GetCompactPath(projectPath);
                else item.Text = name;
            }
            item.Tag = projectPath;
            item.Click += delegate { OnProjectSelected(projectPath); };
            return item;
        }

        private void ClearAllItems()
        {
            PluginMain.Settings.RecentProjects.Clear();
            DropDownItems.Clear();
            ToolbarSelector.DropDownItems.Clear();
            base.Enabled = false;
            ToolbarSelector.Enabled = false;
        }

        private void OnProjectSelected(string projectPath)
        {
            // if the project no longer exists, fail with a nice message.
            if (!File.Exists(projectPath))
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.CouldNotFindProject"));
                RemoveOpenedProject(projectPath);
                return;
            }
            if (ProjectSelected != null) ProjectSelected(projectPath);
        }

    }

}
