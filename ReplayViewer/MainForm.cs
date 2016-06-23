using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Xna.Framework;

namespace ReplayViewer
{
    public partial class MainForm : Form
    {
        public static string Path_Settings = "settings.txt";
        public static string Path_OsuDb = "";
        public static string Path_Songs = "";
        public static string Path_Replays = "";
        public static string Path_Img_EditorNode = @"img/editornode.png";
        public static string Path_Img_Cursor = @"img/cursor.png";
        public static string Path_Img_Hitcircle = @"img/hitcircle.png";
        public static string Path_Img_SliderFollowCircle = @"img/sliderfollowcircle.png";
        public static string Path_Img_Spinner = @"img/spinner.png";
        public static string Path_Img_ApproachCircle = @"img/approachcircle.png";
        public static string Path_Img_Help = @"img/help.png";
        public static string Path_Img_SliderEdge = @"img/slideredge.png";
        public static string Path_Img_SliderBody = @"img/sliderbody.png";
        public static MainForm self;
        public List<ReplayAPI.Replay> CurrentReplays;
        public Canvas Canvas { get; set; }
        public OsuDbAPI.OsuDbFile OsuDbFile { get; set; }

        public MainForm()
        {
            MainForm.self = this;
            this.CurrentReplays = new List<ReplayAPI.Replay>();
            for (int i = 0; i < 7; i++)
            {
                this.CurrentReplays.Add(null);
            }
            this.Canvas = null;
            InitializeComponent();
        }

        public IntPtr GetPictureBoxHandle()
        {
            return this.pictureBox.Handle;
        }

        private void pictureBox_Resize(object sender, EventArgs e)
        {
            // do we even need to do anything here ?
        }

        public void SetSettings(string[] settings)
        {
            MainForm.Path_OsuDb = settings[0];
            MainForm.Path_Songs = settings[1];
            MainForm.Path_Replays = settings[2];
            if (Directory.Exists(MainForm.Path_Songs))
            {
                if (MainForm.Path_Songs[MainForm.Path_Songs.Length - 1] != '\\')
                {
                    MainForm.Path_Songs += '\\';
                }
                if (File.Exists(MainForm.Path_OsuDb))
                {
                    try
                    {
                        this.OsuDbFile = new OsuDbAPI.OsuDbFile(MainForm.Path_OsuDb);
                    }
                    catch
                    {
                        MainForm.ErrorMessage("Could not load osu!.db. Try deleting it and letting osu! rebuild it when you relaunch the game.");
                    }
                }
                else
                {
                    MainForm.ErrorMessage("Path to osu!.db file does not exist. You will not be able to view beatmaps, only the cursor data.");
                }
            }
            else
            {
                MainForm.ErrorMessage("Path to songs folder does not exist. You will not be able to view beatmaps, only the cursor data.");
            }
            if (Directory.Exists(MainForm.Path_Replays))
            {
                if (MainForm.Path_Replays[MainForm.Path_Replays.Length - 1] != '\\')
                {
                    MainForm.Path_Replays += '\\';
                }
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                string version = " build " + File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "files", "version.txt"));
                if (version.Length > 40)
                {
                    version = version.Substring(0, 40);
                }
                this.Text += version;
            }
            catch
            {
            }
            this.UpdateAxisFlipBtnText(false);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.O))
            {
                this.Open();
                return true;
            }
            else if (keyData == Keys.S)
            {
                this.playBtn_Click(null, null);
            }
            else if (keyData == Keys.Q)
            {
                this.viewBtn_Click(null, null);
            }
            else if (keyData == Keys.W)
            {
                this.editBtn_Click(null, null);
            }
            else if (keyData == Keys.D1)
            {
                this.replay1Radio.Checked = true;
            }
            else if (keyData == Keys.D2)
            {
                this.replay2Radio.Checked = true;
            }
            else if (keyData == Keys.D3)
            {
                this.replay3Radio.Checked = true;
            }
            else if (keyData == Keys.D4)
            {
                this.replay4Radio.Checked = true;
            }
            else if (keyData == Keys.D5)
            {
                this.replay5Radio.Checked = true;
            }
            else if (keyData == Keys.D6)
            {
                this.replay6Radio.Checked = true;
            }
            else if (keyData == Keys.D7)
            {
                this.replay7Radio.Checked = true;
            }
            else if (keyData == Keys.Z)
            {
                this.speed025Radio.Checked = true;
            }
            else if (keyData == Keys.X)
            {
                this.speed050Radio.Checked = true;
            }
            else if (keyData == Keys.C)
            {
                this.speed075Radio.Checked = true;
            }
            else if (keyData == Keys.V)
            {
                this.speed100Radio.Checked = true;
            }
            else if (keyData == Keys.B)
            {
                this.speed150Radio.Checked = true;
            }
            else if (keyData == Keys.N)
            {
                this.speed200Radio.Checked = true;
            }
            else if (keyData == Keys.M)
            {
                this.speed400Radio.Checked = true;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            this.Open(files[0]);
        }

        public static void ErrorMessage(string msg)
        {
            MessageBox.Show(msg, "Error");
        }
        
        public void Open()
        {
            if (this.openFileDialog.ShowDialog() != DialogResult.Cancel)
            {
                this.Open(this.openFileDialog.FileName);
            }
        }

        public void Open(string replayPath)
        {
            try
            {
                if (this.CurrentReplays[this.Canvas.State_ReplaySelected] != null)
                {
                    this.CurrentReplays[this.Canvas.State_ReplaySelected].Dispose();
                }
                this.CurrentReplays[this.Canvas.State_ReplaySelected] = new ReplayAPI.Replay(replayPath, true);
                if (this.OsuDbFile == null)
                {
                    ErrorMessage("Can not read osu!.db.");
                    return;
                }
                string beatmapPath = "";
                foreach (OsuDbAPI.Beatmap dbBeatmap in this.OsuDbFile.Beatmaps)
                {
                    if (dbBeatmap.Hash == this.CurrentReplays[this.Canvas.State_ReplaySelected].MapHash)
                    {
                        beatmapPath = MainForm.Path_Songs + dbBeatmap.FolderName + "\\" + dbBeatmap.OsuFile;
                        break;
                    }
                }
                if (beatmapPath.Length > 0)
                {
                    this.Canvas.Beatmap = new BMAPI.v1.Beatmap(beatmapPath);
                }
                else
                {
                    this.Canvas.Beatmap = null;
                    this.SetTimelinePercent(0);
                    MainForm.ErrorMessage("Could not locate .osu file.");
                }
                this.UpdateTitle();
                this.Canvas.LoadReplay(this.CurrentReplays[this.Canvas.State_ReplaySelected]);
                if (this.GetReplayRadioBtn(this.Canvas.State_ReplaySelected).Text[0] == ' ')
                {
                    char[] eax = this.GetReplayRadioBtn(this.Canvas.State_ReplaySelected).Text.ToCharArray();
                    eax[0] = '*';
                    this.GetReplayRadioBtn(this.Canvas.State_ReplaySelected).Text = new string(eax);
                }
                this.volumeBar_Scroll(null, null);
            }
            catch
            {
                MainForm.ErrorMessage("The file you attempted to load was not a replay file.");
            }
        }

        public void UpdateTitle()
        {
            string playerName = "*";
            if (this.CurrentReplays[this.Canvas.State_ReplaySelected] != null)
            {
                playerName = this.CurrentReplays[this.Canvas.State_ReplaySelected].PlayerName;
            }
            if (this.Canvas.Beatmap == null)
            {
                this.Text = playerName + " playing:  an unknown map";
                this.replayInfoLabel.Text = "Player:  " + playerName;
            }
            else
            {
                this.Text = String.Format("{0} playing:  {1} - {2} [{3}] (mapped by {4})", playerName, this.Canvas.Beatmap.Artist, this.Canvas.Beatmap.Title, this.Canvas.Beatmap.Version, this.Canvas.Beatmap.Creator);
                this.replayInfoLabel.Text = String.Format("{0} playing:\n{1} - {2} [{3}]\n(mapped by {4})", playerName, this.Canvas.Beatmap.Artist, this.Canvas.Beatmap.Title, this.Canvas.Beatmap.Version, this.Canvas.Beatmap.Creator);
            }
            Microsoft.Xna.Framework.Color c = Canvas.Color_Cursor[this.Canvas.State_ReplaySelected];
            this.cursorColorPanel.BackColor = System.Drawing.Color.FromArgb(c.R, c.G, c.B);
        }

        public RadioButton GetReplayRadioBtn(int n)
        {
            if (n == 0)
            {
                return this.replay1Radio;
            }
            else if (n == 1)
            {
                return this.replay2Radio;
            }
            else if (n == 2)
            {
                return this.replay3Radio;
            }
            else if (n == 3)
            {
                return this.replay4Radio;
            }
            else if (n == 4)
            {
                return this.replay5Radio;
            }
            else if (n == 5)
            {
                return this.replay6Radio;
            }
            else if (n == 6)
            {
                return this.replay7Radio;
            }
            else
            {
                return null;
            }
        }

        private void timeline_MouseClick(object sender, MouseEventArgs e)
        {
            this.Canvas.SetSongTimePercent(e.X / (float)this.timeline.Width);
        }

        public void SetTimelinePercent(float percent)
        {
            if (percent < 0)
            {
                percent = 0;
            }
            else if (percent > 1)
            {
                percent = 1;
            }
            this.timeline.Value = percent;
        }

        public void SetSongTimeLabel(int ms)
        {
            this.songTimeLabel.Text = ms + " ms";
        }

        private void playBtn_Click(object sender, EventArgs e)
        {
            if (this.playBtn.Text == "Play")
            {
                this.Canvas.State_PlaybackFlow = 3;
            }
            else
            {
                this.Canvas.State_PlaybackFlow = 0;
            }
        }
        
        public void SetPlayPause(string value)
        {
            if (this.playBtn.Text != value)
            {
                this.playBtn.Text = value;
            }
        }

        private void viewBtn_Click(object sender, EventArgs e)
        {
            this.Canvas.State_PlaybackMode = 1;
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            this.Canvas.State_PlaybackMode = 0;
        }

        private void timeWindowBar_Scroll(object sender, EventArgs e)
        {
            this.Canvas.State_TimeRange = this.timeWindowBar.Value * 100;
            this.timeWindowLabel.Text = this.Canvas.State_TimeRange + " ms";
        }

        private void volumeBar_Scroll(object sender, EventArgs e)
        {
            this.Canvas.State_Volume = this.volumeBar.Value * 0.1f;
            this.volumeBarLabel.Text = String.Format("{0} %", this.volumeBar.Value * 10);
        }

        private void speed025Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateSpeedRadio();
        }

        private void speed050Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateSpeedRadio();
        }

        private void speed075Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateSpeedRadio();
        }

        private void speed100Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateSpeedRadio();
        }

        private void speed150Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateSpeedRadio();
        }

        private void speed200Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateSpeedRadio();
        }

        private void speed400Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateSpeedRadio();
        }

        public void UpdateSpeedRadio()
        {
            if (this.speed025Radio.Checked)
            {
                this.Canvas.State_PlaybackSpeed = 0.25f;
            }
            else if (this.speed050Radio.Checked)
            {
                this.Canvas.State_PlaybackSpeed = 0.50f;
            }
            else if (this.speed075Radio.Checked)
            {
                this.Canvas.State_PlaybackSpeed = 0.75f;
            }
            else if (this.speed100Radio.Checked)
            {
                this.Canvas.State_PlaybackSpeed = 1.00f;
            }
            else if (this.speed150Radio.Checked)
            {
                this.Canvas.State_PlaybackSpeed = 1.50f;
            }
            else if (this.speed200Radio.Checked)
            {
                this.Canvas.State_PlaybackSpeed = 2.00f;
            }
            else if (this.speed400Radio.Checked)
            {
                this.Canvas.State_PlaybackSpeed = 4.00f;
            }
        }

        public void UpdateSpeedRadio(float value)
        {
            if (value == 0.25f)
            {
                this.speed025Radio.Checked = true;
            }
            else if (value == 0.50f)
            {
                this.speed050Radio.Checked = true;
            }
            else if (value == 0.75f)
            {
                this.speed075Radio.Checked = true;
            }
            else if (value == 1.00f)
            {
                this.speed100Radio.Checked = true;
            }
            else if (value == 1.50f)
            {
                this.speed150Radio.Checked = true;
            }
            else if (value == 2.00f)
            {
                this.speed200Radio.Checked = true;
            }
            else if (value == 4.00f)
            {
                this.speed400Radio.Checked = true;
            }

        }

        private void replay1Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateReplayRadio();
        }

        private void replay2Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateReplayRadio();
        }

        private void replay3Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateReplayRadio();
        }

        private void replay4Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateReplayRadio();
        }

        private void replay5Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateReplayRadio();
        }

        private void replay6Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateReplayRadio();
        }

        private void replay7Radio_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateReplayRadio();
        }

        public void UpdateReplayRadio()
        {
            for (byte i = 0; i < 7; i++)
            {
                if (this.GetReplayRadioBtn(i).Checked)
                {
                    this.Canvas.State_ReplaySelected = i;
                    break;
                }
            }
            this.UpdateTitle();
            this.UpdateAxisFlipBtnText();
        }

        public void UpdateReplayRadio(byte value)
        {
            this.GetReplayRadioBtn(value).Checked = true;
        }

        private void unloadBtn_Click(object sender, EventArgs e)
        {
            if (this.CurrentReplays[this.Canvas.State_ReplaySelected] != null)
            {
                this.CurrentReplays[this.Canvas.State_ReplaySelected].Dispose();
                this.CurrentReplays[this.Canvas.State_ReplaySelected] = null;
                this.Canvas.UnloadReplay(this.Canvas.State_ReplaySelected);
                if (this.GetReplayRadioBtn(this.Canvas.State_ReplaySelected).Text[0] == '*')
                {
                    char[] eax = this.GetReplayRadioBtn(this.Canvas.State_ReplaySelected).Text.ToCharArray();
                    eax[0] = ' ';
                    this.GetReplayRadioBtn(this.Canvas.State_ReplaySelected).Text = new string(eax);
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Open();
        }

        private void openSettingsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.OpenSettings();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.SetSettings(Program.LoadSettings());
            }
            catch
            {
                MainForm.ErrorMessage("The settings file was deleted. A new one will be created when you relaunch the program.");
            }
        }

        private void sourceCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/thebetioplane/OsuReplayViewer");
        }

        private void quickLoadToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            this.quickLoadToolStripMenuItem.DropDownItems.Clear();
            if (Directory.Exists(MainForm.Path_Replays))
            {
                string[] files = Directory.GetFiles(MainForm.Path_Replays, "*.osr", SearchOption.TopDirectoryOnly);
                ToolStripMenuItem parentItem = this.quickLoadToolStripMenuItem;
                int groupBy = 30;
                int maxItems = files.Length;
                int counter = 0;
                foreach (string fullpath in files)
                {
                    if (maxItems > groupBy && counter % groupBy == 0)
                    {
                        parentItem = new ToolStripMenuItem(String.Format("{0} to {1}", counter + 1, Math.Min(counter + groupBy, maxItems)));
                        this.quickLoadToolStripMenuItem.DropDownItems.Add(parentItem);
                    }
                    string[] split = fullpath.Split('\\');
                    ToolStripMenuItem item = new ToolStripMenuItem(split[split.Length - 1]);
                    item.Click += item_Click;
                    parentItem.DropDownItems.Add(item);
                    counter++;
                }
            }
            else
            {
                MainForm.ErrorMessage("You do not have a replay folder specified. You can change this in the settings file.");
            }
        }

        private void item_Click(object sender, EventArgs e)
        {
            this.quickLoadToolStripMenuItem.DropDownItems.Clear();
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                this.Open(Path.Combine(MainForm.Path_Replays, item.Text));
            }
        }

        private void onscreenHelpCtrlHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Canvas.ShowHelp = 2;
        }

        public void UpdateFromMetadata(ReplayAPI.Replay replay)
        {
            this.Canvas.ApplyMods(replay);
            this.CurrentReplays[this.Canvas.State_ReplaySelected] = replay;
            this.UpdateTitle();
        }

        private void axisFlipBtn_Click(object sender, EventArgs e)
        {
            if (this.CurrentReplays[this.Canvas.State_ReplaySelected] == null)
            {
                return;
            }
            this.CurrentReplays[this.Canvas.State_ReplaySelected].AxisFlip = !this.CurrentReplays[this.Canvas.State_ReplaySelected].AxisFlip;
            this.UpdateAxisFlipBtnText();
        }
        
        private void UpdateAxisFlipBtnText()
        {
            bool value = false;
            if (this.CurrentReplays[this.Canvas.State_ReplaySelected] != null)
            {
                value = this.CurrentReplays[this.Canvas.State_ReplaySelected].AxisFlip;
            }
            this.UpdateAxisFlipBtnText(value);
        }

        private void UpdateAxisFlipBtnText(bool value)
        {
            string displayText;
            if (value)
            {
                displayText = "Inverted";
            }
            else
            {
                displayText = "Normal";
            }
            this.axisFlipBtn.Text = displayText;
        }
    }
}
