using System;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectManager.Projects;
using ProjectManager.Helpers;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Localization;
using ProjectManager.Controls.AS2;
using ProjectManager.Controls.AS3;

namespace ProjectManager.Controls
{
	public class PropertiesDialog : Form
	{
		#region Form Designer

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TabPage movieTab;
		private System.Windows.Forms.TextBox outputSwfBox;
		private System.Windows.Forms.Label exportinLabel;
		private System.Windows.Forms.Label pxLabel;
		private System.Windows.Forms.Label fpsLabel;
        private System.Windows.Forms.Label bgcolorLabel;
		private System.Windows.Forms.Label framerateLabel;
		private System.Windows.Forms.Label dimensionsLabel;
        private System.Windows.Forms.Label xLabel;
		private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button outputBrowseButton;
		private System.Windows.Forms.GroupBox generalGroupBox;
		private System.Windows.Forms.GroupBox playGroupBox;
        private System.Windows.Forms.TabPage classpathsTab;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnGlobalClasspaths;
        private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TabPage buildTab;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.TextBox preBuildBox;
		private System.Windows.Forms.Button preBuilderButton;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Button postBuilderButton;
		private System.Windows.Forms.TextBox postBuildBox;
		private System.Windows.Forms.ToolTip agressiveTip;
        private System.Windows.Forms.CheckBox alwaysExecuteCheckBox;
        private System.Windows.Forms.ComboBox testMovieCombo;
        private System.Windows.Forms.TabPage compilerTab;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.CheckBox noOutputCheckBox;
        private System.Windows.Forms.GroupBox platformGroupBox;
        private System.Windows.Forms.ComboBox versionCombo;
        private System.Windows.Forms.Label exportforLabel;
        private System.Windows.Forms.Button editCommandButton;
        protected System.Windows.Forms.TabControl tabControl;
        protected System.Windows.Forms.TextBox colorTextBox;
        protected System.Windows.Forms.Label colorLabel;
        protected System.Windows.Forms.TextBox fpsTextBox;
        protected System.Windows.Forms.TextBox heightTextBox;
        protected System.Windows.Forms.TextBox widthTextBox;
        private ClasspathControl classpathControl;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer Generated Code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.movieTab = new System.Windows.Forms.TabPage();
            this.platformGroupBox = new System.Windows.Forms.GroupBox();
            this.versionCombo = new System.Windows.Forms.ComboBox();
            this.exportforLabel = new System.Windows.Forms.Label();
            this.noOutputCheckBox = new System.Windows.Forms.CheckBox();
            this.generalGroupBox = new System.Windows.Forms.GroupBox();
            this.widthTextBox = new System.Windows.Forms.TextBox();
            this.outputBrowseButton = new System.Windows.Forms.Button();
            this.heightTextBox = new System.Windows.Forms.TextBox();
            this.xLabel = new System.Windows.Forms.Label();
            this.dimensionsLabel = new System.Windows.Forms.Label();
            this.colorTextBox = new System.Windows.Forms.TextBox();
            this.framerateLabel = new System.Windows.Forms.Label();
            this.outputSwfBox = new System.Windows.Forms.TextBox();
            this.fpsTextBox = new System.Windows.Forms.TextBox();
            this.exportinLabel = new System.Windows.Forms.Label();
            this.colorLabel = new System.Windows.Forms.Label();
            this.pxLabel = new System.Windows.Forms.Label();
            this.bgcolorLabel = new System.Windows.Forms.Label();
            this.fpsLabel = new System.Windows.Forms.Label();
            this.playGroupBox = new System.Windows.Forms.GroupBox();
            this.testMovieCombo = new System.Windows.Forms.ComboBox();
            this.editCommandButton = new System.Windows.Forms.Button();
            this.classpathsTab = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnGlobalClasspaths = new System.Windows.Forms.Button();
            this.buildTab = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.alwaysExecuteCheckBox = new System.Windows.Forms.CheckBox();
            this.postBuilderButton = new System.Windows.Forms.Button();
            this.postBuildBox = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.preBuilderButton = new System.Windows.Forms.Button();
            this.preBuildBox = new System.Windows.Forms.TextBox();
            this.compilerTab = new System.Windows.Forms.TabPage();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.agressiveTip = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl.SuspendLayout();
            this.movieTab.SuspendLayout();
            this.platformGroupBox.SuspendLayout();
            this.generalGroupBox.SuspendLayout();
            this.playGroupBox.SuspendLayout();
            this.classpathsTab.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.buildTab.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.compilerTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(116, 316);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 21);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(197, 316);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 21);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(278, 316);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 21);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "&Apply";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.movieTab);
            this.tabControl.Controls.Add(this.classpathsTab);
            this.tabControl.Controls.Add(this.buildTab);
            this.tabControl.Controls.Add(this.compilerTab);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(342, 298);
            this.tabControl.TabIndex = 0;
            // 
            // movieTab
            // 
            this.movieTab.Controls.Add(this.platformGroupBox);
            this.movieTab.Controls.Add(this.noOutputCheckBox);
            this.movieTab.Controls.Add(this.generalGroupBox);
            this.movieTab.Controls.Add(this.playGroupBox);
            this.movieTab.Location = new System.Drawing.Point(4, 22);
            this.movieTab.Name = "movieTab";
            this.movieTab.Size = new System.Drawing.Size(334, 272);
            this.movieTab.TabIndex = 0;
            this.movieTab.Text = "Output";
            this.movieTab.UseVisualStyleBackColor = true;
            // 
            // platformGroupBox
            // 
            this.platformGroupBox.Controls.Add(this.versionCombo);
            this.platformGroupBox.Controls.Add(this.exportforLabel);
            this.platformGroupBox.Location = new System.Drawing.Point(8, 3);
            this.platformGroupBox.Name = "platformGroupBox";
            this.platformGroupBox.Size = new System.Drawing.Size(319, 49);
            this.platformGroupBox.TabIndex = 41;
            this.platformGroupBox.TabStop = false;
            this.platformGroupBox.Text = "Platform";
            // 
            // versionCombo
            // 
            this.versionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.versionCombo.Location = new System.Drawing.Point(109, 18);
            this.versionCombo.Name = "versionCombo";
            this.versionCombo.Size = new System.Drawing.Size(121, 21);
            this.versionCombo.TabIndex = 13;
            this.versionCombo.SelectedIndexChanged += new System.EventHandler(this.versionCombo_SelectedIndexChanged);
            // 
            // exportforLabel
            // 
            this.exportforLabel.Location = new System.Drawing.Point(14, 21);
            this.exportforLabel.Name = "exportforLabel";
            this.exportforLabel.Size = new System.Drawing.Size(90, 20);
            this.exportforLabel.TabIndex = 12;
            this.exportforLabel.Text = "&Target:";
            this.exportforLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // noOutputCheckBox
            // 
            this.noOutputCheckBox.AutoSize = true;
            this.noOutputCheckBox.Location = new System.Drawing.Point(11, 61);
            this.noOutputCheckBox.Name = "noOutputCheckBox";
            this.noOutputCheckBox.Size = new System.Drawing.Size(241, 17);
            this.noOutputCheckBox.TabIndex = 40;
            this.noOutputCheckBox.Text = "No output, only run pre/post build commands.";
            this.noOutputCheckBox.UseVisualStyleBackColor = true;
            this.noOutputCheckBox.CheckedChanged += new System.EventHandler(this.noOutputCheckBox_CheckedChanged);
            // 
            // generalGroupBox
            // 
            this.generalGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.generalGroupBox.Controls.Add(this.widthTextBox);
            this.generalGroupBox.Controls.Add(this.outputBrowseButton);
            this.generalGroupBox.Controls.Add(this.heightTextBox);
            this.generalGroupBox.Controls.Add(this.xLabel);
            this.generalGroupBox.Controls.Add(this.dimensionsLabel);
            this.generalGroupBox.Controls.Add(this.colorTextBox);
            this.generalGroupBox.Controls.Add(this.framerateLabel);
            this.generalGroupBox.Controls.Add(this.outputSwfBox);
            this.generalGroupBox.Controls.Add(this.fpsTextBox);
            this.generalGroupBox.Controls.Add(this.exportinLabel);
            this.generalGroupBox.Controls.Add(this.colorLabel);
            this.generalGroupBox.Controls.Add(this.pxLabel);
            this.generalGroupBox.Controls.Add(this.bgcolorLabel);
            this.generalGroupBox.Controls.Add(this.fpsLabel);
            this.generalGroupBox.Location = new System.Drawing.Point(8, 81);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(319, 129);
            this.generalGroupBox.TabIndex = 0;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "General";
            // 
            // widthTextBox
            // 
            this.widthTextBox.Location = new System.Drawing.Point(108, 44);
            this.widthTextBox.MaxLength = 4;
            this.widthTextBox.Name = "widthTextBox";
            this.widthTextBox.Size = new System.Drawing.Size(32, 20);
            this.widthTextBox.TabIndex = 4;
            this.widthTextBox.Text = "500";
            this.widthTextBox.TextChanged += new System.EventHandler(this.widthTextBox_TextChanged);
            // 
            // outputBrowseButton
            // 
            this.outputBrowseButton.Location = new System.Drawing.Point(233, 15);
            this.outputBrowseButton.Name = "outputBrowseButton";
            this.outputBrowseButton.Size = new System.Drawing.Size(75, 21);
            this.outputBrowseButton.TabIndex = 2;
            this.outputBrowseButton.Text = "&Browse...";
            this.outputBrowseButton.Click += new System.EventHandler(this.outputBrowseButton_Click);
            // 
            // heightTextBox
            // 
            this.heightTextBox.Location = new System.Drawing.Point(161, 44);
            this.heightTextBox.MaxLength = 4;
            this.heightTextBox.Name = "heightTextBox";
            this.heightTextBox.Size = new System.Drawing.Size(32, 20);
            this.heightTextBox.TabIndex = 5;
            this.heightTextBox.Text = "300";
            this.heightTextBox.TextChanged += new System.EventHandler(this.heightTextBox_TextChanged);
            // 
            // xLabel
            // 
            this.xLabel.Location = new System.Drawing.Point(145, 45);
            this.xLabel.Name = "xLabel";
            this.xLabel.Size = new System.Drawing.Size(13, 17);
            this.xLabel.TabIndex = 21;
            this.xLabel.Text = "x";
            this.xLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dimensionsLabel
            // 
            this.dimensionsLabel.Location = new System.Drawing.Point(8, 47);
            this.dimensionsLabel.Name = "dimensionsLabel";
            this.dimensionsLabel.Size = new System.Drawing.Size(96, 13);
            this.dimensionsLabel.TabIndex = 3;
            this.dimensionsLabel.Text = "&Dimensions:";
            this.dimensionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // colorTextBox
            // 
            this.colorTextBox.Location = new System.Drawing.Point(139, 71);
            this.colorTextBox.MaxLength = 7;
            this.colorTextBox.Name = "colorTextBox";
            this.colorTextBox.Size = new System.Drawing.Size(55, 20);
            this.colorTextBox.TabIndex = 37;
            this.colorTextBox.Text = "#FFFFFF";
            this.colorTextBox.TextChanged += new System.EventHandler(this.colorTextBox_TextChanged);
            // 
            // framerateLabel
            // 
            this.framerateLabel.Location = new System.Drawing.Point(16, 98);
            this.framerateLabel.Name = "framerateLabel";
            this.framerateLabel.Size = new System.Drawing.Size(88, 17);
            this.framerateLabel.TabIndex = 8;
            this.framerateLabel.Text = "&Framerate:";
            this.framerateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // outputSwfBox
            // 
            this.outputSwfBox.Location = new System.Drawing.Point(108, 17);
            this.outputSwfBox.Name = "outputSwfBox";
            this.outputSwfBox.Size = new System.Drawing.Size(121, 20);
            this.outputSwfBox.TabIndex = 1;
            this.outputSwfBox.TextChanged += new System.EventHandler(this.outputSwfBox_TextChanged);
            // 
            // fpsTextBox
            // 
            this.fpsTextBox.Location = new System.Drawing.Point(109, 98);
            this.fpsTextBox.MaxLength = 3;
            this.fpsTextBox.Name = "fpsTextBox";
            this.fpsTextBox.Size = new System.Drawing.Size(27, 20);
            this.fpsTextBox.TabIndex = 9;
            this.fpsTextBox.Text = "30";
            this.fpsTextBox.TextChanged += new System.EventHandler(this.fpsTextBox_TextChanged);
            // 
            // exportinLabel
            // 
            this.exportinLabel.Location = new System.Drawing.Point(8, 17);
            this.exportinLabel.Name = "exportinLabel";
            this.exportinLabel.Size = new System.Drawing.Size(96, 18);
            this.exportinLabel.TabIndex = 0;
            this.exportinLabel.Text = "&Output File:";
            this.exportinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // colorLabel
            // 
            this.colorLabel.BackColor = System.Drawing.Color.White;
            this.colorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.colorLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.colorLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.colorLabel.Location = new System.Drawing.Point(109, 72);
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Size = new System.Drawing.Size(17, 16);
            this.colorLabel.TabIndex = 7;
            this.colorLabel.Click += new System.EventHandler(this.colorLabel_Click);
            // 
            // pxLabel
            // 
            this.pxLabel.Location = new System.Drawing.Point(199, 47);
            this.pxLabel.Name = "pxLabel";
            this.pxLabel.Size = new System.Drawing.Size(19, 14);
            this.pxLabel.TabIndex = 30;
            this.pxLabel.Text = "px";
            // 
            // bgcolorLabel
            // 
            this.bgcolorLabel.Location = new System.Drawing.Point(5, 70);
            this.bgcolorLabel.Name = "bgcolorLabel";
            this.bgcolorLabel.Size = new System.Drawing.Size(99, 18);
            this.bgcolorLabel.TabIndex = 6;
            this.bgcolorLabel.Text = "Background &color:";
            this.bgcolorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fpsLabel
            // 
            this.fpsLabel.Location = new System.Drawing.Point(141, 101);
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(32, 17);
            this.fpsLabel.TabIndex = 28;
            this.fpsLabel.Text = "fps";
            // 
            // playGroupBox
            // 
            this.playGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.playGroupBox.Controls.Add(this.testMovieCombo);
            this.playGroupBox.Controls.Add(this.editCommandButton);
            this.playGroupBox.Location = new System.Drawing.Point(8, 216);
            this.playGroupBox.Name = "playGroupBox";
            this.playGroupBox.Size = new System.Drawing.Size(319, 47);
            this.playGroupBox.TabIndex = 1;
            this.playGroupBox.TabStop = false;
            this.playGroupBox.Text = "Test &Movie";
            // 
            // testMovieCombo
            // 
            this.testMovieCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.testMovieCombo.Items.AddRange(new object[] {
            "Document",
            "External",
            "Popup",
            "Custom"});
            this.testMovieCombo.Location = new System.Drawing.Point(11, 17);
            this.testMovieCombo.Name = "testMovieCombo";
            this.testMovieCombo.Size = new System.Drawing.Size(219, 21);
            this.testMovieCombo.TabIndex = 12;
            this.testMovieCombo.SelectedIndexChanged += new System.EventHandler(this.testMovieCombo_SelectedIndexChanged);
            // 
            // editCommandButton
            // 
            this.editCommandButton.Location = new System.Drawing.Point(234, 15);
            this.editCommandButton.Name = "editCommandButton";
            this.editCommandButton.Size = new System.Drawing.Size(75, 21);
            this.editCommandButton.TabIndex = 2;
            this.editCommandButton.Text = "&Edit...";
            this.editCommandButton.Visible = false;
            this.editCommandButton.Click += new System.EventHandler(this.editCommandButton_Click);
            // 
            // classpathsTab
            // 
            this.classpathsTab.Controls.Add(this.groupBox3);
            this.classpathsTab.Controls.Add(this.label3);
            this.classpathsTab.Controls.Add(this.btnGlobalClasspaths);
            this.classpathsTab.Location = new System.Drawing.Point(4, 22);
            this.classpathsTab.Name = "classpathsTab";
            this.classpathsTab.Size = new System.Drawing.Size(334, 272);
            this.classpathsTab.TabIndex = 3;
            this.classpathsTab.Text = "Classpaths";
            this.classpathsTab.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(8, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(319, 192);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "&Project Classpaths";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(16, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(288, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Project classpaths are relative to the project location";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Location = new System.Drawing.Point(14, 204);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(313, 31);
            this.label3.TabIndex = 1;
            this.label3.Text = "Global classpaths are specific to your machine\r\nand are not stored in the project file.";
            // 
            // btnGlobalClasspaths
            // 
            this.btnGlobalClasspaths.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGlobalClasspaths.Location = new System.Drawing.Point(15, 237);
            this.btnGlobalClasspaths.Name = "btnGlobalClasspaths";
            this.btnGlobalClasspaths.Size = new System.Drawing.Size(150, 21);
            this.btnGlobalClasspaths.TabIndex = 2;
            this.btnGlobalClasspaths.Text = "&Edit Global Classpaths...";
            this.btnGlobalClasspaths.Click += new System.EventHandler(this.btnGlobalClasspaths_Click);
            // 
            // buildTab
            // 
            this.buildTab.Controls.Add(this.groupBox5);
            this.buildTab.Controls.Add(this.groupBox4);
            this.buildTab.Location = new System.Drawing.Point(4, 22);
            this.buildTab.Name = "buildTab";
            this.buildTab.Size = new System.Drawing.Size(334, 272);
            this.buildTab.TabIndex = 4;
            this.buildTab.Text = "Build";
            this.buildTab.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.alwaysExecuteCheckBox);
            this.groupBox5.Controls.Add(this.postBuilderButton);
            this.groupBox5.Controls.Add(this.postBuildBox);
            this.groupBox5.Location = new System.Drawing.Point(8, 145);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(319, 118);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Post-Build Command Line";
            // 
            // alwaysExecuteCheckBox
            // 
            this.alwaysExecuteCheckBox.Location = new System.Drawing.Point(13, 84);
            this.alwaysExecuteCheckBox.Name = "alwaysExecuteCheckBox";
            this.alwaysExecuteCheckBox.Size = new System.Drawing.Size(144, 17);
            this.alwaysExecuteCheckBox.TabIndex = 2;
            this.alwaysExecuteCheckBox.Text = "Always execute";
            this.agressiveTip.SetToolTip(this.alwaysExecuteCheckBox, "Execute the Post-Build Command Line even after a failed build");
            this.alwaysExecuteCheckBox.CheckedChanged += new System.EventHandler(this.alwaysExecuteCheckBox_CheckedChanged);
            // 
            // postBuilderButton
            // 
            this.postBuilderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.postBuilderButton.Location = new System.Drawing.Point(232, 81);
            this.postBuilderButton.Name = "postBuilderButton";
            this.postBuilderButton.Size = new System.Drawing.Size(75, 21);
            this.postBuilderButton.TabIndex = 1;
            this.postBuilderButton.Text = "Builder...";
            this.postBuilderButton.Click += new System.EventHandler(this.postBuilderButton_Click);
            // 
            // postBuildBox
            // 
            this.postBuildBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.postBuildBox.Location = new System.Drawing.Point(13, 31);
            this.postBuildBox.Multiline = true;
            this.postBuildBox.Name = "postBuildBox";
            this.postBuildBox.Size = new System.Drawing.Size(293, 45);
            this.postBuildBox.TabIndex = 0;
            this.postBuildBox.TextChanged += new System.EventHandler(this.postBuildBox_TextChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.preBuilderButton);
            this.groupBox4.Controls.Add(this.preBuildBox);
            this.groupBox4.Location = new System.Drawing.Point(8, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(319, 140);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Pre-Build Command Line";
            // 
            // preBuilderButton
            // 
            this.preBuilderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.preBuilderButton.Location = new System.Drawing.Point(232, 102);
            this.preBuilderButton.Name = "preBuilderButton";
            this.preBuilderButton.Size = new System.Drawing.Size(75, 21);
            this.preBuilderButton.TabIndex = 1;
            this.preBuilderButton.Text = "Builder...";
            this.preBuilderButton.Click += new System.EventHandler(this.preBuilderButton_Click);
            // 
            // preBuildBox
            // 
            this.preBuildBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.preBuildBox.Location = new System.Drawing.Point(13, 30);
            this.preBuildBox.Multiline = true;
            this.preBuildBox.Name = "preBuildBox";
            this.preBuildBox.Size = new System.Drawing.Size(293, 68);
            this.preBuildBox.TabIndex = 0;
            this.preBuildBox.TextChanged += new System.EventHandler(this.preBuildBox_TextChanged);
            // 
            // compilerTab
            // 
            this.compilerTab.Controls.Add(this.propertyGrid);
            this.compilerTab.Location = new System.Drawing.Point(4, 22);
            this.compilerTab.Name = "compilerTab";
            this.compilerTab.Padding = new System.Windows.Forms.Padding(3);
            this.compilerTab.Size = new System.Drawing.Size(334, 272);
            this.compilerTab.TabIndex = 1;
            this.compilerTab.Text = "Compiler Options";
            this.compilerTab.UseVisualStyleBackColor = true;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(328, 266);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // agressiveTip
            // 
            this.agressiveTip.AutomaticDelay = 0;
            // 
            // PropertiesDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(366, 348);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertiesDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Project Properties";
            this.tabControl.ResumeLayout(false);
            this.movieTab.ResumeLayout(false);
            this.movieTab.PerformLayout();
            this.platformGroupBox.ResumeLayout(false);
            this.generalGroupBox.ResumeLayout(false);
            this.generalGroupBox.PerformLayout();
            this.playGroupBox.ResumeLayout(false);
            this.classpathsTab.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.buildTab.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.compilerTab.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		#endregion

		private Project project;
        private CompilerOptions optionsCopy;
        private Boolean propertiesChanged;
        private Boolean classpathsChanged;
        private Boolean assetsChanged;

		public event EventHandler OpenGlobalClasspaths;

        public PropertiesDialog() 
        { 
            this.InitializeComponent();
            this.CreateClassPathControl();
            this.InitializeLocalization();
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            this.Font = PluginBase.Settings.DefaultFont;
        }

        private void CreateClassPathControl()
        {
            this.classpathControl = new ProjectManager.Controls.ClasspathControl();
            this.classpathControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.classpathControl.Classpaths = new string[0];
            this.classpathControl.Language = null;
            this.classpathControl.Location = new System.Drawing.Point(17, 22);
            this.classpathControl.Name = "classpathControl";
            this.classpathControl.Project = null;
            this.classpathControl.Size = new System.Drawing.Size(287, 134);
            this.classpathControl.TabIndex = 0;
            this.groupBox3.Controls.Add(this.classpathControl);
        }

        protected Project BaseProject { get { return project; } }

        public void SetProject(Project project)
        {
            this.project = project;
            BuildDisplay();
        }

        #region Change Tracking

        public bool PropertiesChanged
        {
            get { return propertiesChanged; }
            protected set { propertiesChanged = value; }
        }
        
        public bool ClasspathsChanged
        {
            get { return classpathsChanged; }
            protected set { classpathsChanged = value; }
        }
		public bool AssetsChanged
        {
            get { return assetsChanged; }
            protected set { assetsChanged = value; }
        }

        #endregion

        private void InitializeLocalization()
        {
            this.btnOK.Text = TextHelper.GetString("Label.OK");
            this.buildTab.Text = TextHelper.GetString("Info.Build");
            this.movieTab.Text = TextHelper.GetString("Info.Output");
            this.btnApply.Text = TextHelper.GetString("Label.Apply");
            this.btnCancel.Text = TextHelper.GetString("Label.Cancel");
            this.preBuilderButton.Text = TextHelper.GetString("Info.Builder");
            this.postBuilderButton.Text = TextHelper.GetString("Info.Builder");
            this.label2.Text = TextHelper.GetString("Info.ProjectClasspaths");
            this.noOutputCheckBox.Text = TextHelper.GetString("Info.NoOutput");
            this.outputBrowseButton.Text = TextHelper.GetString("Label.Browse");
            this.groupBox3.Text = TextHelper.GetString("Label.ProjectClasspaths").Replace("...", "");
            this.groupBox5.Text = TextHelper.GetString("Info.PostBuildCmdLine");
            this.dimensionsLabel.Text = TextHelper.GetString("Label.Dimensions");
            this.label3.Text = String.Format(TextHelper.GetString("Info.GlobalClasspaths"), "\n");
            this.agressiveTip.SetToolTip(this.alwaysExecuteCheckBox, TextHelper.GetString("ToolTip.AlwaysExecute"));
            this.btnGlobalClasspaths.Text = TextHelper.GetString("Label.EditGlobalClasspaths");
            this.alwaysExecuteCheckBox.Text = TextHelper.GetString("Info.AlwaysExecute");
            this.bgcolorLabel.Text = TextHelper.GetString("Label.BackgroundColor");
            this.exportforLabel.Text = TextHelper.GetString("Info.TargetVersion");
            this.framerateLabel.Text = TextHelper.GetString("Label.FrameRate");
            this.compilerTab.Text = TextHelper.GetString("Info.CompilerOptions");
            this.platformGroupBox.Text = TextHelper.GetString("Info.Platform");
            this.groupBox4.Text = TextHelper.GetString("Info.PreBuildCmdLine");
            this.exportinLabel.Text = TextHelper.GetString("Label.OutputFile");
            this.classpathsTab.Text = TextHelper.GetString("Info.Classpaths");
            this.Text = " " + TextHelper.GetString("Title.ProjectProperties");
            this.generalGroupBox.Text = TextHelper.GetString("Info.General");
            this.playGroupBox.Text = TextHelper.GetString("Label.TestMovie");
            this.testMovieCombo.Items.Clear();
            this.testMovieCombo.Items.AddRange(new Object[] { 
                TextHelper.GetString("Info.Default"),
                TextHelper.GetString("Info.Document"), 
                TextHelper.GetString("Info.Popup"),
                TextHelper.GetString("Info.External"), 
                TextHelper.GetString("Info.OpenDocument"),
                TextHelper.GetString("Info.Custom")
            });
            this.editCommandButton.Text = TextHelper.GetString("Info.EditCommand");
        }

        protected virtual void BuildDisplay()
		{
            this.versionCombo.Items.AddRange(project.MovieOptions.TargetPlatforms);

            this.Text = " " + project.Name + " (" + project.Language.ToUpper() + ") " + TextHelper.GetString("Info.Properties");

			MovieOptions options = project.MovieOptions;

            noOutputCheckBox.Checked = project.NoOutput;
			outputSwfBox.Text = project.OutputPath;
			widthTextBox.Text = options.Width.ToString();
			heightTextBox.Text = options.Height.ToString();
			
			// bugfix -- direct color assignment doesn't work (copies by ref?)
			//colorLabel.BackColor = Color.FromArgb(255,options.BackgroundColor);

			colorTextBox.Text = options.Background;
			fpsTextBox.Text = options.Fps.ToString();

            versionCombo.SelectedIndex = Math.Max(0, Math.Min(versionCombo.Items.Count - 1, options.Platform));

            if (project.TestMovieBehavior == TestMovieBehavior.NewTab) testMovieCombo.SelectedIndex = 1;
            else if (project.TestMovieBehavior == TestMovieBehavior.NewWindow) testMovieCombo.SelectedIndex = 2;
            else if (project.TestMovieBehavior == TestMovieBehavior.ExternalPlayer) testMovieCombo.SelectedIndex = 3;
            else if (project.TestMovieBehavior == TestMovieBehavior.OpenDocument) testMovieCombo.SelectedIndex = 4;
            else if (project.TestMovieBehavior == TestMovieBehavior.Custom) testMovieCombo.SelectedIndex = 5;
            else testMovieCombo.SelectedIndex = 0;
            editCommandButton.Visible = testMovieCombo.SelectedIndex >= 4;

			classpathControl.Changed += new EventHandler(classpathControl_Changed);
			classpathControl.Project = project;
            classpathControl.Classpaths = project.Classpaths.ToArray();
            classpathControl.Language = project.Language;
            classpathControl.LanguageBox.Visible = false;

			preBuildBox.Text = project.PreBuildEvent;
			postBuildBox.Text = project.PostBuildEvent;
			alwaysExecuteCheckBox.Checked = project.AlwaysRunPostBuild;

            // clone the compiler options object because the PropertyGrid modifies its
            // object directly
            optionsCopy = project.CompilerOptions.Clone();
            propertyGrid.SelectedObject = optionsCopy;

			propertiesChanged = false;
			classpathsChanged = false;
			assetsChanged = false;
			btnApply.Enabled = false;
		}

		void classpathControl_Changed(object sender, EventArgs e)
		{
			classpathsChanged = true; // keep special track of this, it's a big deal
			Modified();
		}

		protected void Modified()
		{
			btnApply.Enabled = true;
		}

		protected virtual bool Apply()
		{
			MovieOptions options = project.MovieOptions;

			try
			{
                project.NoOutput = noOutputCheckBox.Checked;
				project.OutputPath = outputSwfBox.Text;
                if (project.OutputPath.Length > 0 
                    && Path.GetExtension(project.OutputPath).Length == 0
                    && (project.Language == "as2" || project.Language == "as3"))
                {
                    project.OutputPath += ".swf";
                }
				project.Classpaths.Clear();
				project.Classpaths.AddRange(classpathControl.Classpaths);
				options.Width = int.Parse(widthTextBox.Text);
				options.Height = int.Parse(heightTextBox.Text);
				options.BackgroundColor = Color.FromArgb(255, colorLabel.BackColor);
				options.Fps = int.Parse(fpsTextBox.Text);
                options.Platform = versionCombo.SelectedIndex;
				project.PreBuildEvent = preBuildBox.Text;
				project.PostBuildEvent = postBuildBox.Text;
				project.AlwaysRunPostBuild = alwaysExecuteCheckBox.Checked;

                if (testMovieCombo.SelectedIndex == 1) project.TestMovieBehavior = TestMovieBehavior.NewTab;
                else if (testMovieCombo.SelectedIndex == 2) project.TestMovieBehavior = TestMovieBehavior.NewWindow;
                else if (testMovieCombo.SelectedIndex == 3) project.TestMovieBehavior = TestMovieBehavior.ExternalPlayer;
                else if (testMovieCombo.SelectedIndex == 4) project.TestMovieBehavior = TestMovieBehavior.OpenDocument;
                else if (testMovieCombo.SelectedIndex == 5) project.TestMovieBehavior = TestMovieBehavior.Custom;
                else if (testMovieCombo.SelectedIndex == 0) project.TestMovieBehavior = TestMovieBehavior.Default;
			}
			catch (Exception exception)
			{
                ErrorManager.ShowError(exception);
				return false;
			}
            // copy compiler option values
            project.CompilerOptions = optionsCopy;
			btnApply.Enabled = false;
			propertiesChanged = true;
			return true;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (btnApply.Enabled) if (!Apply()) return;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnApply_Click(object sender, EventArgs e)
		{
			Apply();
		}

		private void outputSwfBox_TextChanged(object sender, EventArgs e)
		{
			classpathsChanged = true;
			Modified();
		}

		private void widthTextBox_TextChanged(object sender, EventArgs e) { Modified(); }

		private void heightTextBox_TextChanged(object sender, EventArgs e) { Modified(); }

		private void colorTextBox_TextChanged(object sender, EventArgs e) 
        {
            string rgb = colorTextBox.Text;
            if (rgb.Length == 0) rgb = "#000000";
            if (rgb[0] != '#') rgb = '#' + rgb;
            if (rgb.Length > 7) rgb = rgb.Substring(0, 7);
            else while (rgb.Length < 7) rgb = "#0" + rgb.Substring(1);
            try
            {
                colorLabel.BackColor = ColorTranslator.FromHtml(rgb);
            }
            catch { colorLabel.BackColor = Color.Black; }
            Modified(); 
        }
		private void fpsTextBox_TextChanged(object sender, EventArgs e) { Modified(); }

		private void preBuildBox_TextChanged(object sender, System.EventArgs e) { Modified(); }

		private void postBuildBox_TextChanged(object sender, System.EventArgs e) { Modified(); }

		private void alwaysExecuteCheckBox_CheckedChanged(object sender, System.EventArgs e) { Modified(); }

		private void testMovieCombo_SelectedIndexChanged(object sender, System.EventArgs e) 
        { 
            Modified();
            editCommandButton.Visible = testMovieCombo.SelectedIndex >= 4;
        }

		private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{ 
            Modified(); 
        }

		private void versionCombo_SelectedIndexChanged(object sender, EventArgs e)
		{
            classpathsChanged = true; // keep special track of this, it's a big deal
            Modified();
		}

		private void colorLabel_Click(object sender, EventArgs e)
		{
			if (this.colorDialog.ShowDialog() == DialogResult.OK)
			{
				this.colorLabel.BackColor = this.colorDialog.Color;
				this.colorTextBox.Text = this.ToHtml(this.colorLabel.BackColor);
				Modified();
			}
		}

		private string ToHtml(Color c)
		{
			return string.Format("#{0:X6}", (c.R << 16) + (c.G << 8) + c.B);
		}

		private void btnGlobalClasspaths_Click(object sender, EventArgs e)
		{
			if (OpenGlobalClasspaths != null) OpenGlobalClasspaths(this,new EventArgs());
		}

		private void outputBrowseButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = TextHelper.GetString("Info.FlashMovieFilter");
			dialog.OverwritePrompt = false;
			dialog.InitialDirectory = project.Directory;
			// try pre-setting the current output path
			try
			{
				string path = project.GetAbsolutePath(outputSwfBox.Text);
				if (File.Exists(path)) dialog.FileName = path;
			}
			catch { }
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                outputSwfBox.Text = project.GetRelativePath(dialog.FileName);
            }
		}

        private void editCommandButton_Click(object sender, System.EventArgs e)
        {
            string caption;
            string label;
            if (testMovieCombo.SelectedIndex == 4)
            {
                caption = TextHelper.GetString("Title.CustomTestMovieDocument");
                label = TextHelper.GetString("Label.CustomTestMovieDocument");
            }
            else
            {
                caption = TextHelper.GetString("Title.CustomTestMovieCommand");
                label = TextHelper.GetString("Label.CustomTestMovieCommand");
            }
            LineEntryDialog dialog = new LineEntryDialog(caption, label, project.TestMovieCommand ?? "");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                project.TestMovieCommand = dialog.Line;
                Modified();
            }
        }

		private void preBuilderButton_Click(object sender, System.EventArgs e)
		{
			using (BuildEventDialog dialog = new BuildEventDialog(project))
			{
				dialog.CommandLine = preBuildBox.Text;
				if (dialog.ShowDialog(this) == DialogResult.OK) preBuildBox.Text = dialog.CommandLine;
			}
		}

		private void postBuilderButton_Click(object sender, System.EventArgs e)
		{
			using (BuildEventDialog dialog = new BuildEventDialog(project))
			{
				dialog.CommandLine = postBuildBox.Text;
				if (dialog.ShowDialog(this) == DialogResult.OK) postBuildBox.Text = dialog.CommandLine;
			}
		}

        private void noOutputCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            generalGroupBox.Enabled = !noOutputCheckBox.Checked;
            Modified();
        }

	}

}
