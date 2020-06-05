namespace ReplayViewer
{
    partial class MainForm
    {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.playBtn = new System.Windows.Forms.Button();
            this.dataBtn = new System.Windows.Forms.Button();
            this.timeWindowBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.timeWindowLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.speed400Radio = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.speed200Radio = new System.Windows.Forms.RadioButton();
            this.speed150Radio = new System.Windows.Forms.RadioButton();
            this.speed100Radio = new System.Windows.Forms.RadioButton();
            this.speed050Radio = new System.Windows.Forms.RadioButton();
            this.speed025Radio = new System.Windows.Forms.RadioButton();
            this.speed075Radio = new System.Windows.Forms.RadioButton();
            this.viewBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.replay7Radio = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.replay6Radio = new System.Windows.Forms.RadioButton();
            this.replay5Radio = new System.Windows.Forms.RadioButton();
            this.replay4Radio = new System.Windows.Forms.RadioButton();
            this.replay2Radio = new System.Windows.Forms.RadioButton();
            this.replay1Radio = new System.Windows.Forms.RadioButton();
            this.replay3Radio = new System.Windows.Forms.RadioButton();
            this.unloadBtn = new System.Windows.Forms.Button();
            this.songTimeLabel = new System.Windows.Forms.Label();
            this.volumeBar = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.volumeBarLabel = new System.Windows.Forms.Label();
            this.cursorColorPanel = new System.Windows.Forms.Panel();
            this.replayInfoLabel = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSettingsFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onscreenHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.axisFlipBtn = new System.Windows.Forms.Button();
            this.axisFlipLabel = new System.Windows.Forms.Label();
            this.coloredPanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.pressBtn = new System.Windows.Forms.Button();
            this.Canvas = new ReplayViewer.Canvas();
            this.timeline = new ReplayViewer.Timeline();
            ((System.ComponentModel.ISupportInitialize)(this.timeWindowBar)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBar)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // playBtn
            // 
            this.playBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.playBtn.Location = new System.Drawing.Point(78, 603);
            this.playBtn.Name = "playBtn";
            this.playBtn.Size = new System.Drawing.Size(126, 44);
            this.playBtn.TabIndex = 2;
            this.playBtn.Text = "Play";
            this.playBtn.UseVisualStyleBackColor = true;
            this.playBtn.Click += new System.EventHandler(this.playBtn_Click);
            // 
            // dataBtn
            // 
            this.dataBtn.Location = new System.Drawing.Point(251, 24);
            this.dataBtn.Name = "dataBtn";
            this.dataBtn.Size = new System.Drawing.Size(138, 37);
            this.dataBtn.TabIndex = 5;
            this.dataBtn.Text = "Cursor Data";
            this.dataBtn.UseVisualStyleBackColor = true;
            this.dataBtn.Click += new System.EventHandler(this.dataBtn_Click);
            // 
            // timeWindowBar
            // 
            this.timeWindowBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.timeWindowBar.Location = new System.Drawing.Point(210, 618);
            this.timeWindowBar.Maximum = 20;
            this.timeWindowBar.Minimum = 1;
            this.timeWindowBar.Name = "timeWindowBar";
            this.timeWindowBar.Size = new System.Drawing.Size(618, 45);
            this.timeWindowBar.TabIndex = 7;
            this.timeWindowBar.Value = 10;
            this.timeWindowBar.Scroll += new System.EventHandler(this.timeWindowBar_Scroll);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(822, 627);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Cursor data length";
            // 
            // timeWindowLabel
            // 
            this.timeWindowLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.timeWindowLabel.AutoSize = true;
            this.timeWindowLabel.Location = new System.Drawing.Point(917, 627);
            this.timeWindowLabel.Name = "timeWindowLabel";
            this.timeWindowLabel.Size = new System.Drawing.Size(47, 13);
            this.timeWindowLabel.TabIndex = 9;
            this.timeWindowLabel.Text = "1000 ms";
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.panel1.Controls.Add(this.speed400Radio);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.speed200Radio);
            this.panel1.Controls.Add(this.speed150Radio);
            this.panel1.Controls.Add(this.speed100Radio);
            this.panel1.Controls.Add(this.speed050Radio);
            this.panel1.Controls.Add(this.speed025Radio);
            this.panel1.Controls.Add(this.speed075Radio);
            this.panel1.Location = new System.Drawing.Point(9, 87);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(92, 203);
            this.panel1.TabIndex = 10;
            // 
            // speed400Radio
            // 
            this.speed400Radio.AutoSize = true;
            this.speed400Radio.Location = new System.Drawing.Point(1, 176);
            this.speed400Radio.Name = "speed400Radio";
            this.speed400Radio.Size = new System.Drawing.Size(54, 17);
            this.speed400Radio.TabIndex = 18;
            this.speed400Radio.Text = "4.00 x";
            this.speed400Radio.UseVisualStyleBackColor = true;
            this.speed400Radio.CheckedChanged += new System.EventHandler(this.speed400Radio_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-3, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Playback Speed:";
            // 
            // speed200Radio
            // 
            this.speed200Radio.AutoSize = true;
            this.speed200Radio.Location = new System.Drawing.Point(1, 153);
            this.speed200Radio.Name = "speed200Radio";
            this.speed200Radio.Size = new System.Drawing.Size(54, 17);
            this.speed200Radio.TabIndex = 17;
            this.speed200Radio.Text = "2.00 x";
            this.speed200Radio.UseVisualStyleBackColor = true;
            this.speed200Radio.CheckedChanged += new System.EventHandler(this.speed200Radio_CheckedChanged);
            // 
            // speed150Radio
            // 
            this.speed150Radio.AutoSize = true;
            this.speed150Radio.Location = new System.Drawing.Point(1, 130);
            this.speed150Radio.Name = "speed150Radio";
            this.speed150Radio.Size = new System.Drawing.Size(78, 17);
            this.speed150Radio.TabIndex = 16;
            this.speed150Radio.Text = "1.50 x (DT)";
            this.speed150Radio.UseVisualStyleBackColor = true;
            this.speed150Radio.CheckedChanged += new System.EventHandler(this.speed150Radio_CheckedChanged);
            // 
            // speed100Radio
            // 
            this.speed100Radio.AutoSize = true;
            this.speed100Radio.Checked = true;
            this.speed100Radio.Location = new System.Drawing.Point(1, 107);
            this.speed100Radio.Name = "speed100Radio";
            this.speed100Radio.Size = new System.Drawing.Size(54, 17);
            this.speed100Radio.TabIndex = 15;
            this.speed100Radio.TabStop = true;
            this.speed100Radio.Text = "1.00 x";
            this.speed100Radio.UseVisualStyleBackColor = true;
            this.speed100Radio.CheckedChanged += new System.EventHandler(this.speed100Radio_CheckedChanged);
            // 
            // speed050Radio
            // 
            this.speed050Radio.AutoSize = true;
            this.speed050Radio.Location = new System.Drawing.Point(1, 61);
            this.speed050Radio.Name = "speed050Radio";
            this.speed050Radio.Size = new System.Drawing.Size(54, 17);
            this.speed050Radio.TabIndex = 14;
            this.speed050Radio.Text = "0.50 x";
            this.speed050Radio.UseVisualStyleBackColor = true;
            this.speed050Radio.CheckedChanged += new System.EventHandler(this.speed050Radio_CheckedChanged);
            // 
            // speed025Radio
            // 
            this.speed025Radio.AutoSize = true;
            this.speed025Radio.Location = new System.Drawing.Point(1, 38);
            this.speed025Radio.Name = "speed025Radio";
            this.speed025Radio.Size = new System.Drawing.Size(54, 17);
            this.speed025Radio.TabIndex = 14;
            this.speed025Radio.Text = "0.25 x";
            this.speed025Radio.UseVisualStyleBackColor = true;
            this.speed025Radio.CheckedChanged += new System.EventHandler(this.speed025Radio_CheckedChanged);
            // 
            // speed075Radio
            // 
            this.speed075Radio.AutoSize = true;
            this.speed075Radio.Location = new System.Drawing.Point(1, 84);
            this.speed075Radio.Name = "speed075Radio";
            this.speed075Radio.Size = new System.Drawing.Size(78, 17);
            this.speed075Radio.TabIndex = 13;
            this.speed075Radio.Text = "0.75 x (HT)";
            this.speed075Radio.UseVisualStyleBackColor = true;
            this.speed075Radio.CheckedChanged += new System.EventHandler(this.speed075Radio_CheckedChanged);
            // 
            // viewBtn
            // 
            this.viewBtn.Location = new System.Drawing.Point(107, 24);
            this.viewBtn.Name = "viewBtn";
            this.viewBtn.Size = new System.Drawing.Size(138, 37);
            this.viewBtn.TabIndex = 12;
            this.viewBtn.Text = "Watch";
            this.viewBtn.UseVisualStyleBackColor = true;
            this.viewBtn.Click += new System.EventHandler(this.viewBtn_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.panel2.Controls.Add(this.replay7Radio);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.replay6Radio);
            this.panel2.Controls.Add(this.replay5Radio);
            this.panel2.Controls.Add(this.replay4Radio);
            this.panel2.Controls.Add(this.replay2Radio);
            this.panel2.Controls.Add(this.replay1Radio);
            this.panel2.Controls.Add(this.replay3Radio);
            this.panel2.Location = new System.Drawing.Point(7, 325);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(92, 203);
            this.panel2.TabIndex = 19;
            // 
            // replay7Radio
            // 
            this.replay7Radio.AutoSize = true;
            this.replay7Radio.Location = new System.Drawing.Point(1, 176);
            this.replay7Radio.Name = "replay7Radio";
            this.replay7Radio.Size = new System.Drawing.Size(59, 17);
            this.replay7Radio.TabIndex = 18;
            this.replay7Radio.Text = "  White";
            this.replay7Radio.UseVisualStyleBackColor = true;
            this.replay7Radio.CheckedChanged += new System.EventHandler(this.replay7Radio_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-1, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Multi Replay";
            // 
            // replay6Radio
            // 
            this.replay6Radio.AutoSize = true;
            this.replay6Radio.Location = new System.Drawing.Point(1, 153);
            this.replay6Radio.Name = "replay6Radio";
            this.replay6Radio.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.replay6Radio.Size = new System.Drawing.Size(61, 17);
            this.replay6Radio.TabIndex = 17;
            this.replay6Radio.Text = "  Purple";
            this.replay6Radio.UseVisualStyleBackColor = true;
            this.replay6Radio.CheckedChanged += new System.EventHandler(this.replay6Radio_CheckedChanged);
            // 
            // replay5Radio
            // 
            this.replay5Radio.AutoSize = true;
            this.replay5Radio.Location = new System.Drawing.Point(1, 130);
            this.replay5Radio.Name = "replay5Radio";
            this.replay5Radio.Size = new System.Drawing.Size(73, 17);
            this.replay5Radio.TabIndex = 16;
            this.replay5Radio.Text = "  Magenta";
            this.replay5Radio.UseVisualStyleBackColor = true;
            this.replay5Radio.CheckedChanged += new System.EventHandler(this.replay5Radio_CheckedChanged);
            // 
            // replay4Radio
            // 
            this.replay4Radio.AutoSize = true;
            this.replay4Radio.Location = new System.Drawing.Point(1, 107);
            this.replay4Radio.Name = "replay4Radio";
            this.replay4Radio.Size = new System.Drawing.Size(62, 17);
            this.replay4Radio.TabIndex = 15;
            this.replay4Radio.Text = "  Yellow";
            this.replay4Radio.UseVisualStyleBackColor = true;
            this.replay4Radio.CheckedChanged += new System.EventHandler(this.replay4Radio_CheckedChanged);
            // 
            // replay2Radio
            // 
            this.replay2Radio.AutoSize = true;
            this.replay2Radio.Location = new System.Drawing.Point(1, 61);
            this.replay2Radio.Name = "replay2Radio";
            this.replay2Radio.Size = new System.Drawing.Size(52, 17);
            this.replay2Radio.TabIndex = 14;
            this.replay2Radio.Text = "  Blue";
            this.replay2Radio.UseVisualStyleBackColor = true;
            this.replay2Radio.CheckedChanged += new System.EventHandler(this.replay2Radio_CheckedChanged);
            // 
            // replay1Radio
            // 
            this.replay1Radio.AutoSize = true;
            this.replay1Radio.Checked = true;
            this.replay1Radio.Location = new System.Drawing.Point(1, 38);
            this.replay1Radio.Name = "replay1Radio";
            this.replay1Radio.Size = new System.Drawing.Size(51, 17);
            this.replay1Radio.TabIndex = 14;
            this.replay1Radio.TabStop = true;
            this.replay1Radio.Text = "  Red";
            this.replay1Radio.UseVisualStyleBackColor = true;
            this.replay1Radio.CheckedChanged += new System.EventHandler(this.replay1Radio_CheckedChanged);
            // 
            // replay3Radio
            // 
            this.replay3Radio.AutoSize = true;
            this.replay3Radio.Location = new System.Drawing.Point(1, 84);
            this.replay3Radio.Name = "replay3Radio";
            this.replay3Radio.Size = new System.Drawing.Size(60, 17);
            this.replay3Radio.TabIndex = 13;
            this.replay3Radio.Text = "  Green";
            this.replay3Radio.UseVisualStyleBackColor = true;
            this.replay3Radio.CheckedChanged += new System.EventHandler(this.replay3Radio_CheckedChanged);
            // 
            // unloadBtn
            // 
            this.unloadBtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.unloadBtn.Location = new System.Drawing.Point(18, 536);
            this.unloadBtn.Name = "unloadBtn";
            this.unloadBtn.Size = new System.Drawing.Size(75, 23);
            this.unloadBtn.TabIndex = 20;
            this.unloadBtn.Text = "Remove";
            this.unloadBtn.UseVisualStyleBackColor = true;
            this.unloadBtn.Click += new System.EventHandler(this.unloadBtn_Click);
            // 
            // songTimeLabel
            // 
            this.songTimeLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.songTimeLabel.AutoSize = true;
            this.songTimeLabel.Location = new System.Drawing.Point(917, 604);
            this.songTimeLabel.Name = "songTimeLabel";
            this.songTimeLabel.Size = new System.Drawing.Size(29, 13);
            this.songTimeLabel.TabIndex = 21;
            this.songTimeLabel.Text = "0 ms";
            // 
            // volumeBar
            // 
            this.volumeBar.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.volumeBar.Location = new System.Drawing.Point(903, 112);
            this.volumeBar.Name = "volumeBar";
            this.volumeBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.volumeBar.Size = new System.Drawing.Size(45, 416);
            this.volumeBar.TabIndex = 22;
            this.volumeBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.volumeBar.Value = 10;
            this.volumeBar.Scroll += new System.EventHandler(this.volumeBar_Scroll);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(896, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Volume:";
            // 
            // volumeBarLabel
            // 
            this.volumeBarLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.volumeBarLabel.AutoSize = true;
            this.volumeBarLabel.Location = new System.Drawing.Point(938, 94);
            this.volumeBarLabel.Name = "volumeBarLabel";
            this.volumeBarLabel.Size = new System.Drawing.Size(36, 13);
            this.volumeBarLabel.TabIndex = 24;
            this.volumeBarLabel.Text = "100 %";
            this.volumeBarLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cursorColorPanel
            // 
            this.cursorColorPanel.BackColor = System.Drawing.Color.Red;
            this.cursorColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.cursorColorPanel.Location = new System.Drawing.Point(539, 25);
            this.cursorColorPanel.Name = "cursorColorPanel";
            this.cursorColorPanel.Size = new System.Drawing.Size(36, 36);
            this.cursorColorPanel.TabIndex = 25;
            // 
            // replayInfoLabel
            // 
            this.replayInfoLabel.AutoSize = true;
            this.replayInfoLabel.Location = new System.Drawing.Point(582, 25);
            this.replayInfoLabel.Name = "replayInfoLabel";
            this.replayInfoLabel.Size = new System.Drawing.Size(0, 13);
            this.replayInfoLabel.TabIndex = 26;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.openToolStripMenuItem.Text = "Open     (Ctrl + O)";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSettingsFileToolStripMenuItem,
            this.reloadToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // openSettingsFileToolStripMenuItem
            // 
            this.openSettingsFileToolStripMenuItem.Name = "openSettingsFileToolStripMenuItem";
            this.openSettingsFileToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openSettingsFileToolStripMenuItem.Text = "Open settings file";
            this.openSettingsFileToolStripMenuItem.Click += new System.EventHandler(this.openSettingsFileToolStripMenuItem_Click);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.reloadToolStripMenuItem.Text = "Reload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // quickLoadToolStripMenuItem
            // 
            this.quickLoadToolStripMenuItem.Name = "quickLoadToolStripMenuItem";
            this.quickLoadToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.quickLoadToolStripMenuItem.Text = "Quick Load";
            this.quickLoadToolStripMenuItem.DropDownOpening += new System.EventHandler(this.quickLoadToolStripMenuItem_DropDownOpening);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.onscreenHelpToolStripMenuItem,
            this.sourceCodeToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // onscreenHelpToolStripMenuItem
            // 
            this.onscreenHelpToolStripMenuItem.Name = "onscreenHelpToolStripMenuItem";
            this.onscreenHelpToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.onscreenHelpToolStripMenuItem.Text = "On-screen Help (H)";
            this.onscreenHelpToolStripMenuItem.Click += new System.EventHandler(this.onscreenHelpCtrlHToolStripMenuItem_Click);
            // 
            // sourceCodeToolStripMenuItem
            // 
            this.sourceCodeToolStripMenuItem.Name = "sourceCodeToolStripMenuItem";
            this.sourceCodeToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.sourceCodeToolStripMenuItem.Text = "Source Code";
            this.sourceCodeToolStripMenuItem.Click += new System.EventHandler(this.sourceCodeToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.quickLoadToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(984, 24);
            this.menuStrip1.TabIndex = 27;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // axisFlipBtn
            // 
            this.axisFlipBtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.axisFlipBtn.Location = new System.Drawing.Point(19, 579);
            this.axisFlipBtn.Name = "axisFlipBtn";
            this.axisFlipBtn.Size = new System.Drawing.Size(75, 23);
            this.axisFlipBtn.TabIndex = 28;
            this.axisFlipBtn.Text = "Axis Flip";
            this.axisFlipBtn.UseVisualStyleBackColor = true;
            this.axisFlipBtn.Click += new System.EventHandler(this.axisFlipBtn_Click);
            // 
            // axisFlipLabel
            // 
            this.axisFlipLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.axisFlipLabel.AutoSize = true;
            this.axisFlipLabel.Location = new System.Drawing.Point(26, 563);
            this.axisFlipLabel.Name = "axisFlipLabel";
            this.axisFlipLabel.Size = new System.Drawing.Size(55, 13);
            this.axisFlipLabel.TabIndex = 29;
            this.axisFlipLabel.Text = "Y-Axis Flip";
            // 
            // coloredPanel
            // 
            this.coloredPanel.BackColor = System.Drawing.Color.Red;
            this.coloredPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.coloredPanel.Location = new System.Drawing.Point(539, 25);
            this.coloredPanel.Name = "coloredPanel";
            this.coloredPanel.Size = new System.Drawing.Size(36, 36);
            this.coloredPanel.TabIndex = 25;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(582, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 13);
            this.label5.TabIndex = 26;
            // 
            // pressBtn
            // 
            this.pressBtn.Location = new System.Drawing.Point(395, 24);
            this.pressBtn.Name = "pressBtn";
            this.pressBtn.Size = new System.Drawing.Size(138, 37);
            this.pressBtn.TabIndex = 31;
            this.pressBtn.Text = "Press Data";
            this.pressBtn.UseVisualStyleBackColor = true;
            this.pressBtn.Click += new System.EventHandler(this.pressBtn_Click);
            // 
            // Canvas
            // 
            this.Canvas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Canvas.BackColor = System.Drawing.Color.Black;
            this.Canvas.Beatmap = null;
// TODO: Code generation for '' failed because of Exception 'Invalid Primitive Type: System.IntPtr. Consider using CodeObjectCreateExpression.'.
            this.Canvas.FirstHitObjectTime = 0;
            this.Canvas.Location = new System.Drawing.Point(107, 67);
            this.Canvas.MaxSongTime = 0;
            this.Canvas.Name = "Canvas";
            this.Canvas.ShowHelp = ((byte)(0));
            this.Canvas.Size = new System.Drawing.Size(773, 532);
            this.Canvas.State_CurveSmoothness = 0;
            this.Canvas.State_FadeTime = 0;
            this.Canvas.State_PlaybackFlow = ((byte)(0));
            this.Canvas.State_PlaybackMode = ((byte)(0));
            this.Canvas.State_PlaybackSpeed = 0F;
            this.Canvas.State_ReplaySelected = ((byte)(0));
            this.Canvas.State_TimeRange = 0;
            this.Canvas.State_Volume = 0F;
            this.Canvas.TabIndex = 30;
            this.Canvas.Visual_BeatmapAR = 0F;
            this.Canvas.Visual_BeatmapCS = 0F;
            this.Canvas.Visual_EasyAR = false;
            this.Canvas.Visual_EasyCS = false;
            this.Canvas.Visual_HardRockAR = false;
            this.Canvas.Visual_HardRockCS = false;
            this.Canvas.Visual_MapInvert = false;
            this.Canvas.VSync = false;
            // 
            // timeline
            // 
            this.timeline.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.timeline.Location = new System.Drawing.Point(210, 603);
            this.timeline.Name = "timeline";
            this.timeline.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.timeline.Size = new System.Drawing.Size(700, 15);
            this.timeline.TabIndex = 1;
            this.timeline.Value = 0F;
            this.timeline.MouseClick += new System.Windows.Forms.MouseEventHandler(this.timeline_MouseClick);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.pressBtn);
            this.Controls.Add(this.Canvas);
            this.Controls.Add(this.axisFlipBtn);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.replayInfoLabel);
            this.Controls.Add(this.coloredPanel);
            this.Controls.Add(this.cursorColorPanel);
            this.Controls.Add(this.volumeBarLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.volumeBar);
            this.Controls.Add(this.songTimeLabel);
            this.Controls.Add(this.unloadBtn);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.viewBtn);
            this.Controls.Add(this.timeline);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.timeWindowLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.timeWindowBar);
            this.Controls.Add(this.dataBtn);
            this.Controls.Add(this.playBtn);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.axisFlipLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "osu! Replay Viewer";
            this.Load += new System.EventHandler(this.Main_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Main_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Main_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.timeWindowBar)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBar)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Timeline timeline;
        private System.Windows.Forms.Button playBtn;
        private System.Windows.Forms.Button dataBtn;
        private System.Windows.Forms.TrackBar timeWindowBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label timeWindowLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton speed075Radio;
        private System.Windows.Forms.RadioButton speed400Radio;
        private System.Windows.Forms.RadioButton speed200Radio;
        private System.Windows.Forms.RadioButton speed150Radio;
        private System.Windows.Forms.RadioButton speed100Radio;
        private System.Windows.Forms.RadioButton speed050Radio;
        private System.Windows.Forms.RadioButton speed025Radio;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button viewBtn;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton replay7Radio;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton replay6Radio;
        private System.Windows.Forms.RadioButton replay5Radio;
        private System.Windows.Forms.RadioButton replay4Radio;
        private System.Windows.Forms.RadioButton replay2Radio;
        private System.Windows.Forms.RadioButton replay1Radio;
        private System.Windows.Forms.RadioButton replay3Radio;
        private System.Windows.Forms.Button unloadBtn;
        private System.Windows.Forms.Label songTimeLabel;
        private System.Windows.Forms.TrackBar volumeBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label volumeBarLabel;
        private System.Windows.Forms.Panel cursorColorPanel;
        private System.Windows.Forms.Label replayInfoLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSettingsFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quickLoadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sourceCodeToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem onscreenHelpToolStripMenuItem;
        private System.Windows.Forms.Button axisFlipBtn;
        private System.Windows.Forms.Label axisFlipLabel;
        private Canvas Canvas;
        private System.Windows.Forms.Panel coloredPanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button pressBtn;
    }
}

