using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms;
using System.IO;

namespace ReplayViewer
{
    public class Canvas : Game
    {
        public static Color[] Color_Cursor = new Color[7] { Color.Red, Color.Cyan, Color.Lime, Color.Yellow, Color.Magenta, new Color(128, 128, 255, 255), Color.Honeydew };
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        public Vector2 Size { get; set; }
        public int FirstHitObjectTime { get; set; }
        public int MaxSongTime { get; set; }
        public byte ShowHelp { get; set; }
        private SongPlayer songPlayer = null;
        public Form ParentForm { get; set; }
        public Control XNAForm { get; set; }
        public IntPtr DrawingSurface { get; set; }
        private List<List<ReplayAPI.ReplayFrame>> replayFrames;
        private List<List<ReplayAPI.ReplayFrame>> nearbyFrames;
        private List<ReplayAPI.ReplayFrame> selectedFrames;
        private List<BMAPI.v1.HitObjects.CircleObject> nearbyHitObjects;
        public BMAPI.v1.Beatmap Beatmap { get; set; }
        private int approachRate = 0;
        private int circleDiameter = 0;

        public int State_TimeRange { get; set; }
        public int State_CurveSmoothness { get; set; }
        public float State_PlaybackSpeed
        {
            get { return this.state_PlaybackSpeed; }
            set
            {
                this.state_PlaybackSpeed = value;
                MainForm.self.UpdateSpeedRadio(value);
                this.songPlayer.SetPlaybackSpeed(value);
            }
        }
        public byte State_ReplaySelected
        {
            get { return this.state_ReplaySelected; }
            set
            {
                this.state_ReplaySelected = value;
                MainForm.self.UpdateReplayRadio(value);
            }
        }
        public float State_Volume
        {
            get
            {
                return this.state_volume;
            }
            set
            {
                this.state_volume = value;
                this.songPlayer.SetVolume(value);
            }
        }
        public byte State_PlaybackMode { get; set; }
        public byte State_PlaybackFlow { get; set; }
        public int State_FadeTime { get; set; }
        public Color State_BackgroundColor { get; set; }

        public float Visual_BeatmapAR { get; set; }
        public bool Visual_HardRockAR { get; set; }
        public bool Visual_EasyAR { get; set; }
        public float Visual_BeatmapCS { get; set; }
        public bool Visual_HardRockCS { get; set; }
        public bool Visual_EasyCS { get; set; }
        public bool Visual_MapInvert { get; set; }

        private float state_PlaybackSpeed;
        private byte state_ReplaySelected;
        private float state_volume;

        private Texture2D nodeTexture;
        private Texture2D cursorTexture;
        private Texture2D lineTexture;
        private Texture2D hitCircleTexture;
        private Texture2D sliderFollowCircleTexture;
        private Texture2D spinnerTexture;
        private Texture2D approachCircleTexture;
        private Texture2D helpTexture;
        private Texture2D sliderEdgeTexture;
        private Texture2D sliderBodyTexture;
        private Texture2D reverseArrowTexture;

        public Canvas(IntPtr surface, Form form)
        {
            this.DrawingSurface = surface;
            this.ParentForm = form;
            this.MaxSongTime = 0;
            this.FirstHitObjectTime = 0;
            this.ShowHelp = 2;
            this.songPlayer = new SongPlayer();
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 832;
            this.graphics.PreferredBackBufferHeight = 624;
            this.Size = new Vector2(this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);
            this.Content.RootDirectory = "Content";
            this.nearbyHitObjects = new List<BMAPI.v1.HitObjects.CircleObject>();
            this.replayFrames = new List<List<ReplayAPI.ReplayFrame>>();
            this.nearbyFrames = new List<List<ReplayAPI.ReplayFrame>>();
            for (int i = 0; i < 7; i++)
            {
                this.replayFrames.Add(null);
                this.nearbyFrames.Add(new List<ReplayAPI.ReplayFrame>());
            }
            this.Beatmap = null;
            this.IsFixedTimeStep = false;
            this.graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
            this.XNAForm = Control.FromHandle(this.Window.Handle);
            Mouse.WindowHandle = this.DrawingSurface;
            this.XNAForm.VisibleChanged += XNAForm_VisibleChanged;
            this.ParentForm.FormClosing += ParentForm_FormClosing;

            this.State_TimeRange = 1000;
            this.State_CurveSmoothness = 50;
            this.State_PlaybackSpeed = 1.0f;
            this.State_ReplaySelected = 0;
            this.State_PlaybackMode = 0;
            this.State_PlaybackFlow = 0;
            this.State_FadeTime = 200;
            this.State_BackgroundColor = Color.Black;

            this.Visual_BeatmapAR = 0.0f;
            this.Visual_HardRockAR = false;
            this.Visual_HardRockCS = false;
            this.Visual_EasyAR = false;
            this.Visual_EasyCS = false;
            this.Visual_MapInvert = false;
        }

        private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.songPlayer.Stop();
            this.Exit();
        }

        private void XNAForm_VisibleChanged(object sender, EventArgs e)
        {
            if (XNAForm.Visible)
            {
                XNAForm.Visible = false;
            }
        }

        private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = this.DrawingSurface;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            this.nodeTexture = this.TextureFromFile(MainForm.Path_Img_EditorNode);
            this.cursorTexture = this.TextureFromFile(MainForm.Path_Img_Cursor);
            this.hitCircleTexture = this.TextureFromFile(MainForm.Path_Img_Hitcircle);
            this.sliderFollowCircleTexture = this.TextureFromFile(MainForm.Path_Img_SliderFollowCircle);
            this.spinnerTexture = this.TextureFromFile(MainForm.Path_Img_Spinner);
            this.approachCircleTexture = this.TextureFromFile(MainForm.Path_Img_ApproachCircle);
            this.helpTexture = this.TextureFromFile(MainForm.Path_Img_Help);
            this.sliderEdgeTexture = this.TextureFromFile(MainForm.Path_Img_SliderEdge);
            this.sliderBodyTexture = this.TextureFromFile(MainForm.Path_Img_SliderBody);
            this.lineTexture = this.TextureFromColor(Color.White);
            this.reverseArrowTexture = this.TextureFromFile(MainForm.Path_Img_ReverseArrow);
        }

        private Texture2D TextureFromFile(string path)
        {
            try
            {
                return Texture2D.FromStream(this.GraphicsDevice, new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
            catch
            {
                MainForm.ErrorMessage(String.Format("Could not the load the image found at \"{0}\", please make sure it exists and is a valid .png image.\nRedownloading the .zip file will also restore lost images and update new ones.", path));
                return this.TextureFromColor(Color.Magenta);
            }
        }

        private Texture2D TextureFromColor(Color color, int w = 1, int h = 1)
        {
            Texture2D texture = new Texture2D(this.GraphicsDevice, w, h);
            Color[] data = new Color[w * h];
            for (int i = 0; i < w * h; i++)
            {
                data[i] = color;
            }
            texture.SetData<Color>(data);
            return texture;
        }

        protected override void UnloadContent()
        {
            this.nodeTexture.Dispose();
            this.cursorTexture.Dispose();
            this.hitCircleTexture.Dispose();
            this.sliderFollowCircleTexture.Dispose();
            this.spinnerTexture.Dispose();
            this.approachCircleTexture.Dispose();
            this.helpTexture.Dispose();
            this.sliderEdgeTexture.Dispose();
            this.sliderBodyTexture.Dispose();
            this.lineTexture.Dispose();
            this.reverseArrowTexture.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.H))
            {
                this.ShowHelp = 1;
            }
            else if (this.ShowHelp != 2)
            {
                this.ShowHelp = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D) || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                this.State_PlaybackFlow = 2;
            }
            else if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A) || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                this.State_PlaybackFlow = 1;
            }
            else if (this.State_PlaybackFlow != 3)
            {
                this.State_PlaybackFlow = 0;
            }
            MainForm.self.SetPlayPause("Play");
            if (this.State_PlaybackFlow == 0)
            {
                this.songPlayer.Pause();
            }
            else if (this.State_PlaybackFlow == 1)
            {
                this.songPlayer.Pause();
                this.songPlayer.JumpTo((long)(this.songPlayer.SongTime - (gameTime.ElapsedGameTime.Milliseconds * this.State_PlaybackSpeed)));
            }
            else if (this.State_PlaybackFlow == 2)
            {
                this.songPlayer.Play();
            }
            else if (this.State_PlaybackFlow == 3)
            {
                MainForm.self.SetPlayPause("Pause");
                this.songPlayer.Play();
            }
            if (this.MaxSongTime != 0)
            {
                MainForm.self.SetTimelinePercent((float)this.songPlayer.SongTime / (float)this.MaxSongTime);
            }
            MainForm.self.SetSongTimeLabel((int)this.songPlayer.SongTime);
            this.nearbyHitObjects = new List<BMAPI.v1.HitObjects.CircleObject>();
            if (this.Beatmap != null)
            {
                // we take advantage of the fact that the hitobjects are listed in chronological order and implement a binary search
                // this will the index of the hitobject closest (rounded down) to the time
                // we will get all the hitobjects a couple seconds after and before the current time
                int startIndex = this.BinarySearchHitObjects((float)(this.songPlayer.SongTime - 10000)) - 5;
                int endIndex = this.BinarySearchHitObjects((float)(this.songPlayer.SongTime + 2000)) + 5;
                for (int k = startIndex; k < endIndex; k++)
                {
                    if (k < 0)
                    {
                        continue;
                    }
                    else if (k >= this.Beatmap.HitObjects.Count)
                    {
                        break;
                    }
                    this.nearbyHitObjects.Add(this.Beatmap.HitObjects[k]);
                }
            }
            for (int j = 0; j < 7; j++)
            {
                if (this.replayFrames[j] != null)
                {
                    if (this.replayFrames[j].Count == 0)
                    {
                        MainForm.ErrorMessage("This replay contains no cursor data.");
                        this.replayFrames[j] = null;
                        continue;
                    }
                    // like the hitobjects, the replay frames are also in chronological order
                    // so we use more binary searches to efficiently get the index of the replay frame at a time
                    this.nearbyFrames[j] = new List<ReplayAPI.ReplayFrame>();
                    if (this.State_PlaybackMode == 0)
                    {
                        int lowIndex = this.BinarySearchReplayFrame(j, (int)(this.songPlayer.SongTime) - this.State_TimeRange);
                        int highIndex = this.BinarySearchReplayFrame(j, (int)this.songPlayer.SongTime) + 1;
                        for (int i = lowIndex; i <= highIndex; i++)
                        {
                            this.nearbyFrames[j].Add(this.replayFrames[j][i]);
                        }
                    }
                    else if (this.State_PlaybackMode == 1)
                    {
                        int nearestIndex = this.BinarySearchReplayFrame(j, (int)this.songPlayer.SongTime);
                        this.nearbyFrames[j].Add(this.replayFrames[j][nearestIndex]);
                        if (nearestIndex + 1 < this.replayFrames[j].Count)
                        {
                            this.nearbyFrames[j].Add(this.replayFrames[j][nearestIndex + 1]);
                        }
                    }
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(this.State_BackgroundColor);
            this.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
            for (int b = this.nearbyHitObjects.Count - 1; b >= 0; b--)
            {
                BMAPI.v1.HitObjects.CircleObject hitObject = this.nearbyHitObjects[b];
                // the song time relative to the hitobject start time
                float diff = (float)(hitObject.StartTime - this.songPlayer.SongTime);
                // transparency of hitobject 
                float alpha = 1.0f;
                // a percentage of how open a slider is, 1.0 is closed, 0.0 is open
                float approachCircleValue = 0.0f;
                // length in time of hit object (only applies to sliders and spinners)
                int hitObjectLength = 0;
                // Use these types if the hitobject is a slider or spinner
                BMAPI.v1.HitObjects.SliderObject hitObjectAsSlider = null;
                BMAPI.v1.HitObjects.SpinnerObject hitObjectAsSpinner = null;
                if (hitObject.Type.HasFlag(BMAPI.v1.HitObjectType.Spinner))
                {
                    hitObjectAsSpinner = (BMAPI.v1.HitObjects.SpinnerObject)hitObject;
                    hitObjectLength = (int)(hitObjectAsSpinner.EndTime - hitObjectAsSpinner.StartTime);
                }
                else if (hitObject.Type.HasFlag(BMAPI.v1.HitObjectType.Slider))
                {
                    hitObjectAsSlider = (BMAPI.v1.HitObjects.SliderObject)hitObject;
                    hitObjectLength = (int)((hitObjectAsSlider.SegmentEndTime - hitObjectAsSlider.StartTime) * hitObjectAsSlider.RepeatCount);
                    //hitObjectLength = 500;
                }
                // for reference: this.approachRate is the time in ms it takes for approach circle to close
                if (diff < this.approachRate + this.State_FadeTime && diff > -(hitObjectLength + this.State_FadeTime))
                {
                    if (diff < -hitObjectLength)
                    {
                        // fade out
                        alpha = 1 - ((diff + hitObjectLength) / -(float)this.State_FadeTime);
                    }
                    else if (!hitObject.Type.HasFlag(BMAPI.v1.HitObjectType.Spinner) && diff >= this.approachRate && diff < this.approachRate + this.State_FadeTime)
                    {
                        // fade in
                        alpha = 1 - (diff - this.approachRate) / (float)this.State_FadeTime;
                    }
                    else if (hitObject.Type.HasFlag(BMAPI.v1.HitObjectType.Spinner))
                    {
                        // because spinners don't have an approach circle before they appear
                        if (diff >= 0 && diff < this.State_FadeTime)
                        {
                            alpha = 1 - diff / (float)this.State_FadeTime;
                        }
                        else if (diff > 0)
                        {
                            alpha = 0;
                        }
                    }
                    if (diff < this.approachRate + this.State_FadeTime && diff > 0)
                    {
                        // hitcircle percentage from open to closed
                        approachCircleValue = diff / (float)(this.approachRate + this.State_FadeTime);
                    }
                    else if (diff > 0)
                    {
                        approachCircleValue = 1.0f;
                    }
                    if (hitObject.Type.HasFlag(BMAPI.v1.HitObjectType.Circle))
                    {
                        this.DrawHitcircle(hitObject, alpha, b);
                        this.DrawApproachCircle(hitObject, alpha, approachCircleValue);
                    }
                    else if (hitObject.Type.HasFlag(BMAPI.v1.HitObjectType.Slider))
                    {
                        this.DrawSlider(hitObjectAsSlider, alpha, b, approachCircleValue);
                    }
                    else if (hitObject.Type.HasFlag(BMAPI.v1.HitObjectType.Spinner))
                    {
                        this.DrawSpinner(hitObjectAsSpinner, alpha);
                        this.DrawSpinnerApproachCircle(hitObjectAsSpinner, alpha, (float)(this.songPlayer.SongTime - hitObjectAsSpinner.StartTime) / (hitObjectAsSpinner.EndTime - hitObjectAsSpinner.StartTime));
                    }
                }
            }
            byte forceFlip = 0xff;
            if (this.State_PlaybackMode == 0)
            {
                if (MainForm.self.CurrentReplays[this.State_ReplaySelected] != null)
                {
                    forceFlip = this.BoolToByte(MainForm.self.CurrentReplays[this.State_ReplaySelected].AxisFlip);
                }
                Vector2 currentPos = Vector2.Zero;
                Vector2 lastPos = new Vector2(-222, 0);
                for (int i = 0; i < this.nearbyFrames[this.state_ReplaySelected].Count; i++)
                {
                    ReplayAPI.ReplayFrame currentFrame = this.nearbyFrames[this.state_ReplaySelected][i];
                    float alpha = i / (float)this.nearbyFrames[this.state_ReplaySelected].Count;
                    currentPos = this.InflateVector(new Vector2(currentFrame.X, currentFrame.Y), false, forceFlip);
                    bool selected = false;
                    if (this.selectedFrames != null)
                    {
                        selected = this.selectedFrames.Contains(currentFrame);
                    }
                    if (lastPos.X != -222)
                    {
                        Color linecolor;
                        if (selected)
                        {
                            linecolor = Color.White;
                        }
                        else
                        {
                            linecolor = new Color(1.0f, 0.0f, 0.0f, alpha);
                        }
                        this.DrawLine(lastPos, currentPos, linecolor);
                    }
                    Color nodeColor = Color.Gray;
                    if (currentFrame.Keys.HasFlag(ReplayAPI.Keys.K1))
                    {
                        nodeColor = Color.Cyan;
                    }
                    else if (currentFrame.Keys.HasFlag(ReplayAPI.Keys.K2))
                    {
                        nodeColor = Color.Magenta;
                    }
                    else if (currentFrame.Keys.HasFlag(ReplayAPI.Keys.M1))
                    {
                        nodeColor = Color.Lime;
                    }
                    else if (currentFrame.Keys.HasFlag(ReplayAPI.Keys.M2))
                    {
                        nodeColor = Color.Yellow;
                    }
                    if (! selected)
                    {
                        nodeColor.A = (byte)(alpha * 255);
                    }
                    this.spriteBatch.Draw(this.nodeTexture, currentPos - new Vector2(5, 5), nodeColor);
                    lastPos = currentPos;
                }
            }
            else if (this.State_PlaybackMode == 1)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (this.nearbyFrames[i] != null && this.nearbyFrames[i].Count >= 1)
                    {
                        forceFlip = this.BoolToByte(MainForm.self.CurrentReplays[i].AxisFlip);
                        this.spriteBatch.Draw(this.cursorTexture, this.InflateVector(this.GetInterpolatedFrame(i), false, forceFlip) - new Vector2(this.cursorTexture.Width, this.cursorTexture.Height) / 2f, Canvas.Color_Cursor[i]);
                    }
                }
            }
            if (this.ShowHelp != 0)
            {
                this.spriteBatch.Draw(this.helpTexture, Vector2.Zero, Color.White);
            }
            this.spriteBatch.End();
            base.Draw(gameTime);
        }

        private byte BoolToByte(bool value)
        {
            if (value)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private int BinarySearchReplayFrame(int replaynum, int target)
        {
            int high = this.replayFrames[replaynum].Count - 1;
            int low = 0;
            while (low <= high)
            {
                int mid = (high + low) / 2;
                if (mid == high || mid == low)
                {
                    return mid;
                }
                if (this.replayFrames[replaynum][mid].Time >= target)
                {
                    high = mid;
                }
                else
                {
                    low = mid;
                }
            }
            return 0;
        }

        private int BinarySearchHitObjects(float target)
        {
            if (this.Beatmap == null)
            {
                return -1;
            }
            int high = this.Beatmap.HitObjects.Count - 1;
            int low = 0;
            while (low <= high)
            {
                int mid = (high + low) / 2;
                if (mid == high || mid == low)
                {
                    return mid;
                }
                else if (this.Beatmap.HitObjects[mid].StartTime > target)
                {
                    high = mid;
                }
                else
                {
                    low = mid;
                }
            }
            return 0;
        }

        private void DrawLine(Vector2 start, Vector2 end, Color color, int width = 1)
        {
            Rectangle r = new Rectangle((int)start.X, (int)start.Y, (int)(end - start).Length() + width, width);
            Vector2 v = Vector2.Normalize(start - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (start.Y > end.Y)
            {
                angle = 6.28318530717f - angle;
            }
            spriteBatch.Draw(this.lineTexture, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        private void DrawHitcircle(BMAPI.v1.HitObjects.CircleObject hitObject, float alpha, int zindex)
        {
            int diameter = (int)(this.circleDiameter * this.Size.X / 512f);
            Vector2 pos = this.InflateVector(hitObject.Location.ToVector2(), true);
            this.DrawHitcircle(pos, diameter, alpha, zindex);
        }

        private void DrawHitcircle(Vector2 pos, int diameter, float alpha, int zindex)
        {
            Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, diameter, diameter);
            rect.X -= rect.Width / 2;
            rect.Y -= rect.Height / 2;
            float depth = zindex / 1000.0f;
            this.spriteBatch.Draw(this.hitCircleTexture, rect, null, new Color(1.0f, 1.0f, 1.0f, alpha), 0f, Vector2.Zero, SpriteEffects.None, 0.1f + depth);
        }

        private void DrawApproachCircle(BMAPI.v1.HitObjects.CircleObject hitObject, float alpha, float value)
        {
            float smallDiameter = this.circleDiameter * this.Size.X / 512f;
            float largeDiameter = smallDiameter * 3.0f;
            // linearly interpolate between two diameters
            // makes approach circle shrink
            int diameter = (int)(smallDiameter + (largeDiameter - smallDiameter) * value);
            Vector2 pos = this.InflateVector(new Vector2(hitObject.Location.X, hitObject.Location.Y), true);
            Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, diameter, diameter);
            rect.X -= rect.Width / 2;
            rect.Y -= rect.Height / 2;
            this.spriteBatch.Draw(this.approachCircleTexture, rect, null, new Color(1.0f, 1.0f, 1.0f, alpha), 0f, Vector2.Zero, SpriteEffects.None, 0);
        }

        private void DrawSpinnerApproachCircle(BMAPI.v1.HitObjects.SpinnerObject hitObject, float alpha, float value)
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value > 1)
            {
                value = 1;
            }
            int diameter = (int)(this.spinnerTexture.Width * (1 - value) * 0.9);
            Vector2 pos = this.InflateVector(new Vector2(hitObject.Location.X, hitObject.Location.Y), true);
            Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, diameter, diameter);
            rect.X -= rect.Width / 2;
            rect.Y -= rect.Height / 2;
            this.spriteBatch.Draw(this.spinnerTexture, rect, null, new Color(1.0f, 1.0f, 1.0f, alpha), 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
        }

        private void DrawSpinner(BMAPI.v1.HitObjects.SpinnerObject hitObject, float alpha)
        {
            int diameter = (int)this.Size.Y;
            Vector2 pos = this.InflateVector(new Vector2(hitObject.Location.X, hitObject.Location.Y));
            Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, diameter, diameter);
            rect.X -= rect.Width / 2;
            rect.Y -= rect.Height / 2;
            this.spriteBatch.Draw(this.spinnerTexture, rect, null, new Color(1.0f, 1.0f, 1.0f, alpha), 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
        }

        private void DrawSlider(BMAPI.v1.HitObjects.SliderObject hitObject, float alpha, int zindex, float approachCircleValue)
        {
            float time = (float)(this.songPlayer.SongTime - hitObject.StartTime) / (float)((hitObject.SegmentEndTime - hitObject.StartTime) * hitObject.RepeatCount);
            byte reverseArrowType = (hitObject.RepeatCount > 1) ? (byte)1 : (byte)0;
            if (time < 0)
            {
                this.DrawHitcircle(hitObject, alpha, zindex);
                this.DrawApproachCircle(hitObject, alpha, approachCircleValue);
                this.DrawSliderBody(hitObject, alpha, this.circleDiameter / 2, zindex, reverseArrowType);
                return;
            }
            else if (time > 1)
            {
                time = 1;
            }
            time *= hitObject.RepeatCount;
            if (time > 1)
            {
                bool drawArrow = time - (hitObject.RepeatCount - 1) < 0;
                int order = 0;
                while (time > 1)
                {
                    time -= 1;
                    order++;
                    reverseArrowType = drawArrow ? (byte)1 : (byte)0;
                }
                if (order % 2 != 0)
                {
                    time = 1 - time;
                    reverseArrowType = drawArrow ? (byte)2 : (byte)0;
                }
            }
            this.DrawSliderBody(hitObject, alpha, this.circleDiameter / 2, zindex, reverseArrowType);
            Vector2 pos = this.InflateVector(hitObject.PositionAtTime(time), true);
            // 128x128 is the size of the sprite image for hitcircles
            int diameter = (int)(this.circleDiameter / 128f * this.sliderFollowCircleTexture.Width * this.Size.X / 512f);
            Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, diameter, diameter);
            rect.X -= rect.Width / 2;
            rect.Y -= rect.Height / 2;
            this.spriteBatch.Draw(this.sliderFollowCircleTexture, rect, null, new Color(1.0f, 1.0f, 1.0f, alpha), 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }

        private void DrawSliderBody(BMAPI.v1.HitObjects.SliderObject hitObject, float alpha, int radius, int zindex)
        {
            float smallLength = hitObject.TotalLength;
            Color color = Color.White;
            float depthHigh = (zindex * 2 + 1) / 1000.0f;
            float depthLow = (zindex * 2) / 1000.0f;
            for (float i = 0; i < smallLength + 10; i += 10)
            {
                if (i > smallLength)
                {
                    i = smallLength;
                }
                Vector2 pos = this.InflateVector(hitObject.PositionAtTime(i / smallLength), true);
                int diameter = (int)(this.circleDiameter * this.Size.X / 512f);
                Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, diameter, diameter);
                rect.X -= rect.Width / 2;
                rect.Y -= rect.Height / 2;
                color.A = (byte)(255 * alpha);
                this.spriteBatch.Draw(this.sliderEdgeTexture, rect, null, color, 0f, Vector2.Zero, SpriteEffects.None, 0.4f + depthHigh);
                color.A = 255;
                this.spriteBatch.Draw(this.sliderBodyTexture, rect, null, color, 0f, Vector2.Zero, SpriteEffects.None, 0.4f + depthLow);
                if (i == smallLength)
                {
                    break;
                }
            }
        }

        private void DrawSliderBody(BMAPI.v1.HitObjects.SliderObject hitObject, float alpha, int radius, int zindex, byte reverseArrowType)
        {
            float smallLength = hitObject.TotalLength;
            Color color = Color.White;
            float depthHigh = (zindex * 3 + 2) / 1000.0f;
            float depthMed = (zindex * 3 + 1) / 1000.0f;
            float depthLow = (zindex * 3) / 1000.0f;
            byte alphaByte = (byte)(255 * alpha);
            Rectangle firstRect = new Rectangle(-222, -222, 222, 222);
            Rectangle lastRect = new Rectangle(-222, -222, 222, 222);
            Rectangle firstRectDiff = new Rectangle(-222, -222, 222, 222);
            Rectangle lastRectDiff = new Rectangle(-222, -222, 222, 222);
            for (float i = 0; i < smallLength + 10; i += 10)
            {
                if (i > smallLength)
                {
                    i = smallLength;
                }
                Vector2 pos = this.InflateVector(hitObject.PositionAtTime(i / smallLength), true);
                int diameter = (int)(this.circleDiameter * this.Size.X / 512f);
                Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, diameter, diameter);
                rect.X -= rect.Width / 2;
                rect.Y -= rect.Height / 2;
                if (firstRect.X == -222)
                {
                    firstRect = rect;
                }
                else if (firstRectDiff.X == -222)
                {
                    firstRectDiff = rect;
                }
                lastRectDiff = lastRect;
                lastRect = rect;
                color.A = alphaByte;
                this.spriteBatch.Draw(this.sliderEdgeTexture, rect, null, color, 0f, Vector2.Zero, SpriteEffects.None, 0.4f + depthHigh);
                color.A = 255;
                this.spriteBatch.Draw(this.sliderBodyTexture, rect, null, color, 0f, Vector2.Zero, SpriteEffects.None, 0.4f + depthMed);
                if (i == smallLength)
                {
                    break;
                }
            }
            if (reverseArrowType != 0)
            {
                color.A = alphaByte;
                Rectangle mode;
                float rotation;
                if (reverseArrowType == 1)
                {
                    mode = lastRect;
                    rotation = (float)Math.Atan2(lastRectDiff.Y - lastRect.Y, lastRectDiff.X - lastRect.X);
                }
                else
                {
                    rotation = (float)Math.Atan2(firstRectDiff.Y - firstRect.Y, firstRectDiff.X - firstRect.X);
                    mode = firstRect;
                }
                mode.X += 52;
                mode.Y += 52;
                this.spriteBatch.Draw(this.reverseArrowTexture, mode, null, color, rotation, new Vector2(64.0f), SpriteEffects.None, 0.4f + depthLow);
            }
        }

        private void DrawBoxOutline(Rectangle rect, Color color)
        {
            Vector2 rp0 = new Vector2(rect.X, rect.Y);
            Vector2 rp1 = new Vector2(rect.X + rect.Width, rect.Y);
            Vector2 rp2 = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
            Vector2 rp3 = new Vector2(rect.X, rect.Y + rect.Height);
            this.DrawLine(rp0, rp1, color);
            this.DrawLine(rp1, rp2, color);
            this.DrawLine(rp2, rp3, color);
            this.DrawLine(rp3, rp0, color);
        }

        public void LoadReplay(ReplayAPI.Replay replay)
        {
            this.ShowHelp = 0;
            this.replayFrames[this.state_ReplaySelected] = replay.ReplayFrames;
            this.JumpTo(0);
            this.State_PlaybackFlow = 0;
            if (replay.ReplayFrames.Count > 0)
            {
                this.MaxSongTime = replay.ReplayFrames[replay.ReplayFrames.Count - 1].Time;
            }
            else
            {
                this.MaxSongTime = 0;
            }
            if (this.Beatmap != null)
            {
                this.FirstHitObjectTime = (int)this.Beatmap.HitObjects[0].StartTime;
                this.Visual_BeatmapAR = this.Beatmap.ApproachRate;
                this.Visual_BeatmapCS = this.Beatmap.CircleSize;
                this.songPlayer.Start(Path.Combine(this.Beatmap.Folder, this.Beatmap.AudioFilename));
                this.JumpTo(this.FirstHitObjectTime - 1000);
            }
            this.ApplyMods(replay);
        }

        public void ApplyMods(ReplayAPI.Replay replay)
        {
            if (replay.Mods.HasFlag(ReplayAPI.Mods.Easy))
            {
                this.Visual_EasyAR = true;
                this.Visual_EasyCS = true;
                this.Visual_HardRockAR = false;
                this.Visual_HardRockCS = false;
            }
            else if (replay.Mods.HasFlag(ReplayAPI.Mods.HardRock))
            {
                this.Visual_EasyAR = false;
                this.Visual_EasyCS = false;
                this.Visual_HardRockAR = true;
                this.Visual_HardRockCS = true;
            }
            else
            {
                this.Visual_EasyAR = false;
                this.Visual_EasyCS = false;
                this.Visual_HardRockAR = false;
                this.Visual_HardRockCS = false;
            }
            if (replay.Mods.HasFlag(ReplayAPI.Mods.DoubleTime))
            {
                this.State_PlaybackSpeed = 1.5f;
            }
            else if (replay.Mods.HasFlag(ReplayAPI.Mods.HalfTime))
            {
                State_PlaybackSpeed = 0.75f;
            }
            else
            {
                State_PlaybackSpeed = 1.0f;
            }
            this.UpdateApproachRate();
            this.UpdateCircleSize();
            this.Visual_MapInvert = this.Visual_HardRockAR && this.Visual_HardRockCS;
        }

        public void UnloadReplay(byte pos)
        {
            this.replayFrames[pos] = null;
            this.nearbyFrames[pos] = new List<ReplayAPI.ReplayFrame>();
        }

        public void UpdateApproachRate()
        {
            // from beatmap approach rate to actual ms in approach rate
            float moddedAR = this.Visual_BeatmapAR;
            if (this.Visual_HardRockAR && !this.Visual_EasyAR)
            {
                moddedAR *= 1.4f;
            }
            else if (!this.Visual_HardRockAR && this.Visual_EasyAR)
            {
                moddedAR /= 2.0f;
            }
            if (moddedAR > 10)
            {
                moddedAR = 10;
            }
            this.approachRate = (int)(-150 * moddedAR + 1950);
        }

        public void UpdateCircleSize()
        {
            // from beatmap circle size to actual pixels in radius
            float moddedCS = this.Visual_BeatmapCS;
            if (this.Visual_HardRockCS && !this.Visual_EasyCS)
            {
                moddedCS *= 1.3f;
            }
            else if (!this.Visual_HardRockCS && this.Visual_EasyCS)
            {
                moddedCS /= 2.0f;
            }
            this.circleDiameter = (int)(2 * (40 - 4 * (moddedCS - 2)));
        }

        private Vector2 InflateVector(Vector2 vector, bool flipWhenHardrock = false, byte forceFlip = 0xff)
        {
            // takes a vector with x: 0 - 512 and y: 0 - 384 and turns them into coordinates for whole canvas size 
            bool shouldInvert = this.Visual_MapInvert && flipWhenHardrock;
            if (forceFlip == 1)
            {
                shouldInvert = true;
            }
            else if (forceFlip == 0)
            {
                shouldInvert = false;
            }
            if (shouldInvert)
            {
                return new Vector2(vector.X / 512f * this.Size.X, (384f - vector.Y) / 384f * this.Size.Y);
            }
            else
            {
                return new Vector2(vector.X / 512f * this.Size.X, vector.Y / 384f * this.Size.Y);
            }
        }

        public void SetSongTimePercent(float percent)
        {
            // for when timeline is clicked, sets the song time in ms from percentage into the song
            this.JumpTo((long)(percent * (float)this.MaxSongTime));
        }

        private Vector2 GetInterpolatedFrame(int replayNum)
        {
            // gets the cursor position at a given time based on the replay data
            // if between two points, interpolate between
            Vector2 p1 = new Vector2(this.nearbyFrames[replayNum][0].X, this.nearbyFrames[replayNum][0].Y);
            Vector2 p2 = Vector2.Zero;
            int t1 = this.nearbyFrames[replayNum][0].Time;
            int t2 = t1 + 1;
            // check to make sure it is not the final replay frame in the replay
            if (this.nearbyFrames[replayNum].Count > 1)
            {
                p2.X = this.nearbyFrames[replayNum][1].X;
                p2.Y = this.nearbyFrames[replayNum][1].Y;
                t2 = this.nearbyFrames[replayNum][1].Time;
                // While I don't think there would ever be two replay frames at the same time,
                // this will prevent ever dividing by zero when calculating 'm'
                if (t1 == t2)
                {
                    t2++;
                }
            }
            // 't' is the percentage (from 0.0 to 1.0) of time completed from one point to other
            float t = ((float)this.songPlayer.SongTime - t1) / (float)(t2 - t1);
            // Linearly interpolate between point 1 and point 2 based off the time percentage 'm'
            return new Vector2(p1.X + (p2.X - p1.X) * t, p1.Y + (p2.Y - p1.Y) * t);
        }

        private void JumpTo(long value)
        {
            if (value < 0)
            {
                this.songPlayer.JumpTo(0);
                this.State_PlaybackFlow = 0;
            }
            else if (value > this.MaxSongTime)
            {
                this.songPlayer.Stop();
                this.State_PlaybackFlow = 0;
            }
            else
            {
                this.songPlayer.JumpTo(value);
            }
        }
    }
}
