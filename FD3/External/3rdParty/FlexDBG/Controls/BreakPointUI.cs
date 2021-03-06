﻿/*
    Copyright (C) 2009  Robert Nelson

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PluginCore;

namespace FlexDbg
{
    class BreakPointUI : DockPanelControl
    {
        private PluginMain pluginMain;
        private BreakPointManager breakPointManager;

        private DataGridView dgv;

        private DataGridViewCheckBoxColumn ColumnBreakPointEnable;
        private DataGridViewTextBoxColumn ColumnBreakPointFilePath;
        private DataGridViewTextBoxColumn ColumnBreakPointFileName;
        private DataGridViewTextBoxColumn ColumnBreakPointLine;
        private DataGridViewTextBoxColumn ColumnBreakPointExp;

        private Color defaultColor;

        public BreakPointUI(PluginMain pluginMain, BreakPointManager breakPointManager)
        {
            init();

            this.pluginMain = pluginMain;
            this.breakPointManager = breakPointManager;
            this.breakPointManager.ChangeBreakPointEvent += new ChangeBreakPointEventHandler(breakPointManager_ChangeBreakPointEvent);
            this.breakPointManager.UpdateBreakPointEvent += new UpdateBreakPointEventHandler(breakPointManager_UpdateBreakPointEvent);
            this.Controls.Add(this.dgv);
        }

        void breakPointManager_UpdateBreakPointEvent(object sender, UpdateBreakPointArgs e)
        {
            int index = ItemIndex(e.FileFullPath, e.OldLine);
            if (index >= 0)
            {
                dgv.Rows[index].Cells["Line"].Value = e.NewLine.ToString();
            }
        }

        void breakPointManager_ChangeBreakPointEvent(object sender, BreakPointArgs e)
        {
            if (e.IsDelete)
            {
                DeleteItem(e.FileFullPath, e.Line + 1);            
            }
            else
            {
                AddItem(e.FileFullPath, e.Line + 1, e.Exp, e.Enable);
            }
        }

        private void init()
        {
            this.dgv = new DataGridView();
            this.dgv.Dock = DockStyle.Fill;
            this.dgv.BorderStyle = BorderStyle.None;
            this.dgv.BackgroundColor = SystemColors.Window;
            this.dgv.Font = PluginBase.Settings.DefaultFont;
            this.dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            this.dgv.EnableHeadersVisualStyles = true;
            this.dgv.RowHeadersVisible = false;

            DataGridViewCellStyle viewStyle = new DataGridViewCellStyle();
            viewStyle.Padding = new Padding(1);
            this.dgv.ColumnHeadersDefaultCellStyle = viewStyle;

            this.ColumnBreakPointEnable = new DataGridViewCheckBoxColumn();
            this.ColumnBreakPointFilePath = new DataGridViewTextBoxColumn();
            this.ColumnBreakPointFileName = new DataGridViewTextBoxColumn();
            this.ColumnBreakPointLine = new DataGridViewTextBoxColumn();
            this.ColumnBreakPointExp = new DataGridViewTextBoxColumn();

            this.ColumnBreakPointEnable.HeaderText = "Enable";
            this.ColumnBreakPointEnable.Name = "Enable";
            this.ColumnBreakPointEnable.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            this.ColumnBreakPointEnable.Width = 70;

            this.ColumnBreakPointFilePath.HeaderText = "Path";
            this.ColumnBreakPointFilePath.Name = "FilePath";
            this.ColumnBreakPointFilePath.ReadOnly = true;

            this.ColumnBreakPointFileName.HeaderText = "File";
            this.ColumnBreakPointFileName.Name = "FileName";
            this.ColumnBreakPointFileName.ReadOnly = true;

            this.ColumnBreakPointLine.HeaderText = "Line";
            this.ColumnBreakPointLine.Name = "Line";
            this.ColumnBreakPointLine.ReadOnly = true;

            this.ColumnBreakPointExp.HeaderText = "Exp";
            this.ColumnBreakPointExp.Name = "Exp";

            this.dgv.AllowUserToAddRows = false;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.ColumnBreakPointEnable,
                this.ColumnBreakPointFilePath,
                this.ColumnBreakPointFileName,
                this.ColumnBreakPointLine,
                this.ColumnBreakPointExp});

            defaultColor = dgv.Rows[dgv.Rows.Add()].DefaultCellStyle.BackColor;
            dgv.Rows.Clear();

            this.dgv.CellEndEdit += new DataGridViewCellEventHandler(dgv_CellEndEdit);
            this.dgv.CellMouseUp += new DataGridViewCellMouseEventHandler(dgv_CellMouseUp);
            this.dgv.CellDoubleClick += new DataGridViewCellEventHandler(dgv_CellDoubleClick);
        }

        void dgv_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgv.Rows[e.RowIndex].Cells["Enable"].ColumnIndex == e.ColumnIndex)
            {
                Boolean enable = !(Boolean)dgv.Rows[e.RowIndex].Cells["Enable"].Value;
                string filefullpath = (string)dgv.Rows[e.RowIndex].Cells["FilePath"].Value;
                int line = int.Parse((string)dgv.Rows[e.RowIndex].Cells["Line"].Value);
                int marker = enable ? 3 : 4;

                ITabbedDocument doc = ScintillaHelper.GetDocument(filefullpath);
                if (doc != null)
                {
                    ScintillaHelper.ToggleMarker(doc.SciControl, 4, line - 1);

                    Int32 lineMask = doc.SciControl.MarkerGet(line - 1);
                    Boolean m = (lineMask & (1 << 4)) == 0 ? true : false;
                    if ((Boolean)dgv.Rows[e.RowIndex].Cells["Enable"].Value != m)
                    {
                        dgv.Rows[e.RowIndex].Cells["Enable"].Value = m;
                    }
                    dgv.RefreshEdit();
                }
            }
        }

        void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgv.Rows[e.RowIndex].Cells["Line"].ColumnIndex == e.ColumnIndex)
            {
                string filename = (string)dgv.Rows[e.RowIndex].Cells["FilePath"].Value;
                int line = int.Parse((string)dgv.Rows[e.RowIndex].Cells["Line"].Value);
                ScintillaHelper.ActivateDocument(filename, line - 1, true);
            }
        }

        void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgv.Rows[e.RowIndex].Cells["Exp"].ColumnIndex == e.ColumnIndex)
            {
                dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = defaultColor;

                string filename = (string)dgv.Rows[e.RowIndex].Cells["FilePath"].Value;
                int line = int.Parse((string)dgv.Rows[e.RowIndex].Cells["Line"].Value);
                string exp = (string)dgv.Rows[e.RowIndex].Cells["Exp"].Value;

                breakPointManager.SetBreakPointCondition(filename, line - 1, exp);
            }
        }

        public void Clear()
        {
            dgv.Rows.Clear();
        }

        public new Boolean Enabled
        {
            get { return dgv.Enabled; }
            set { dgv.Enabled = value; }
        }

        private void AddItem(string filename, int line, string exp, Boolean enable)
        {
            DataGridViewRow dgvrow;
            int i = ItemIndex(filename, line);
            if (i >= 0)
            {
                dgvrow = dgv.Rows[i];
            }
            else
            {
                dgvrow = dgv.Rows[dgv.Rows.Add()];
            }
            dgvrow.Cells["Enable"].Value = enable;
            dgvrow.Cells["FilePath"].Value = filename;
            dgvrow.Cells["FileName"].Value = Path.GetFileName(filename);
            dgvrow.Cells["Line"].Value = line.ToString();
            dgvrow.Cells["Exp"].Value = exp;
        }

        private void DeleteItem(string filename, int line)
        {
            int i = ItemIndex(filename, line);
            if (i >= 0)
            {
                dgv.Rows.RemoveAt(i);
            }
        }

        private int ItemIndex(string filename, int line)
        {
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                if ((string)dgv.Rows[i].Cells["FilePath"].Value == filename
                    && (string)dgv.Rows[i].Cells["Line"].Value == line.ToString())
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
