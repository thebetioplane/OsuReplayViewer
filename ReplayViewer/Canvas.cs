using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using Rectangle = System.Drawing.Rectangle;
using Color = OpenTK.Graphics.Color4;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.IO;

namespace ReplayViewer
{
    public class Canvas : OpenTK.GLControl
    {
        public static Color[] Color_Cursor = new Color[7] { Color.Red, Color.Cyan, Color.Lime, Color.Yellow, Color.Magenta, new Color(128, 128, 255, 255), Color.Honeydew };
        public int FirstHitObjectTime { get; set; }
        public int MaxSongTime { get; set; }
        public byte ShowHelp { get; set; }
        private SongPlayer songPlayer = null;
        public IntPtr DrawingSurface { get; set; }
        private List<List<ReplayAPI.ReplayFrame>> replayFrames;
        private List<List<ReplayAPI.ReplayFrame>> nearbyFrames;
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
                MainForm.self?.UpdateSpeedRadio(value);
                this.songPlayer?.SetPlaybackSpeed(value);
            }
        }
        public byte State_ReplaySelected
        {
            get { return this.state_ReplaySelected; }
            set
            {
                this.state_ReplaySelected = value;
                MainForm.self?.UpdateReplayRadio(value);
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
                this.songPlayer?.SetVolume(value);
            }
        }
        public byte State_PlaybackMode { get; set; }
        public byte State_PlaybackFlow { get; set; }
        public int State_FadeTime { get; set; }

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
        private Texture2D hitCircleTexture;
        private Texture2D sliderFollowCircleTexture;
        private Texture2D spinnerTexture;
        private Texture2D approachCircleTexture;
        private Texture2D helpTexture;
        private Texture2D sliderEdgeTexture;
        private Texture2D sliderBodyTexture;
        private Texture2D reverseArrowTexture;

        public Canvas()
            : base()
        {
            this.MakeCurrent();
            this.Paint += Canvas_Paint;
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            this.Tick();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            this.Draw(true);
            this.Draw(false);
            this.SwapBuffers();
            System.Threading.Thread.Sleep(16);
            this.Resize += Canvas_Resize;
        }

        private void Canvas_Resize(object sender, EventArgs e)
        {
            int w = this.Size.Width;
            int h = this.Size.Height;
            int x = 0;
            int y = 0;
            if (w * 3 / 4 > h)
            {
                w = h * 4 / 3;
                x = (this.Size.Width - w) / 2;
            }
            if (h * 4 / 3 > w)
            {
                h = w * 3 / 4;
                y = (this.Size.Height - h) / 2;
            }
            GL.Viewport(x, y, w, h);
        }

        private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.songPlayer.Stop();
        }

        public void Init()
        {
            this.MaxSongTime = 0;
            this.FirstHitObjectTime = 0;
            this.ShowHelp = 2;
            this.songPlayer = new SongPlayer();
            //this.Size = new Vector2(832, 624);
            this.nearbyHitObjects = new List<BMAPI.v1.HitObjects.CircleObject>();
            this.replayFrames = new List<List<ReplayAPI.ReplayFrame>>();
            this.nearbyFrames = new List<List<ReplayAPI.ReplayFrame>>();
            for (int i = 0; i < 7; i++)
            {
                this.replayFrames.Add(null);
                this.nearbyFrames.Add(new List<ReplayAPI.ReplayFrame>());
            }
            this.Beatmap = null;
            this.ParentForm.FormClosing += ParentForm_FormClosing;

            this.State_TimeRange = 1000;
            this.State_CurveSmoothness = 50;
            this.State_PlaybackSpeed = 1.0f;
            this.State_ReplaySelected = 0;
            this.State_PlaybackMode = 0;
            this.State_PlaybackFlow = 0;
            this.State_FadeTime = 200;

            this.Visual_BeatmapAR = 0.0f;
            this.Visual_HardRockAR = false;
            this.Visual_HardRockCS = false;
            this.Visual_EasyAR = false;
            this.Visual_EasyCS = false;
            this.Visual_MapInvert = false;

            this.nodeTexture = this.TextureFromFile(MainForm.Path_Img_EditorNode);
            this.cursorTexture = this.TextureFromFile(MainForm.Path_Img_Cursor);
            this.hitCircleTexture = this.TextureFromFile(MainForm.Path_Img_Hitcircle);
            this.sliderFollowCircleTexture = this.TextureFromFile(MainForm.Path_Img_SliderFollowCircle);
            this.spinnerTexture = this.TextureFromFile(MainForm.Path_Img_Spinner);
            this.approachCircleTexture = this.TextureFromFile(MainForm.Path_Img_ApproachCircle);
            this.helpTexture = this.TextureFromFile(MainForm.Path_Img_Help);
            this.sliderEdgeTexture = this.TextureFromFile(MainForm.Path_Img_SliderEdge);
            this.sliderBodyTexture = this.TextureFromFile(MainForm.Path_Img_SliderBody);
            this.reverseArrowTexture = this.TextureFromFile(MainForm.Path_Img_ReverseArrow);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            this.Canvas_Resize(this, null);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Vector2 border = new Vector2(4.0f, 3.0f) * 32.0f;
            GL.Ortho(-border.X, 512.0 + border.X, 384.0 + border.Y, -border.Y, 0.0, 1.0);
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        private Texture2D TextureFromFile(string path)
        {
            try
            {
                return new Texture2D(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
            catch
            {
                MainForm.ErrorMessage(String.Format("Could not the load the image found at \"{0}\", please make sure it exists and is a valid .png image.\nRedownloading the .zip file will also restore lost images and update new ones.", path));
                return null;
            }
        }

        private void UnloadContent()
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
            this.reverseArrowTexture.Dispose();
        }

        private void Tick()
        {
            if (Keyboard.GetState().IsKeyDown(Key.H))
            {
                this.ShowHelp = 1;
            }
            else if (this.ShowHelp != 2)
            {
                this.ShowHelp = 0;
            }
            if (Keyboard.GetState().IsKeyDown(Key.D) || Keyboard.GetState().IsKeyDown(Key.Right))
            {
                this.State_PlaybackFlow = 2;
            }
            else if (Keyboard.GetState().IsKeyDown(Key.A) || Keyboard.GetState().IsKeyDown(Key.Left))
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
                //this.songPlayer.JumpTo((long)(this.songPlayer.SongTime - (gameTime.ElapsedGameTime.Milliseconds * this.State_PlaybackSpeed)));
                this.songPlayer.JumpTo((long)(this.songPlayer.SongTime - ((1000.0 / 60.0) * this.State_PlaybackSpeed)));
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
        }

        private void MulFlipMatrix()
        {
            double[] flipMatrix = {
                1.0, 0.0, 0.0, 0.0,
                0.0, -1.0, 0.0, 384.0,
                0.0, 0.0, 1.0, 0.0,
                0.0, 0.0, 0.0, 1.0,
            };
            GL.MultTransposeMatrix(flipMatrix);
        }

        private void Draw(bool isBackground)
        {
            if (this.Visual_MapInvert)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.PushMatrix();
                this.MulFlipMatrix();
            }
            //this.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);
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
                        if (isBackground)
                            this.DrawHitcircle(hitObject, alpha);
                        else
                            this.DrawApproachCircle(hitObject, alpha, approachCircleValue);
                    }
                    else if (hitObject.Type.HasFlag(BMAPI.v1.HitObjectType.Slider))
                    {
                        this.DrawSlider(hitObjectAsSlider, alpha, approachCircleValue, isBackground);
                    }
                    else if (hitObject.Type.HasFlag(BMAPI.v1.HitObjectType.Spinner))
                    {
                        if (!isBackground)
                        {
                            this.DrawSpinner(hitObjectAsSpinner, alpha);
                            this.DrawSpinnerApproachCircle(hitObjectAsSpinner, alpha, (float)(this.songPlayer.SongTime - hitObjectAsSpinner.StartTime) / (hitObjectAsSpinner.EndTime - hitObjectAsSpinner.StartTime));
                        }
                    }
                }
            }
            if (this.Visual_MapInvert)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.PopMatrix();
            }
            if (!isBackground)
            {
                bool forceFlip = false;
                if (this.State_PlaybackMode == 0)
                {
                    if (MainForm.self.CurrentReplays[this.State_ReplaySelected] != null)
                    {
                        forceFlip = MainForm.self.CurrentReplays[this.State_ReplaySelected].AxisFlip;
                    }
                    if (forceFlip)
                    {
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.PushMatrix();
                        this.MulFlipMatrix();
                    }
                    Vector2 currentPos = Vector2.Zero;
                    Vector2 lastPos = new Vector2(-222, 0);
                    for (int i = 0; i < this.nearbyFrames[this.state_ReplaySelected].Count; i++)
                    {
                        ReplayAPI.ReplayFrame currentFrame = this.nearbyFrames[this.state_ReplaySelected][i];
                        float alpha = i / (float)this.nearbyFrames[this.state_ReplaySelected].Count;
                        currentPos = new Vector2(currentFrame.X, currentFrame.Y);
                        if (lastPos.X != -222)
                        {
                            this.DrawLine(lastPos, currentPos, new Color(1.0f, 0.0f, 0.0f, alpha));
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
                        this.nodeTexture.Draw(currentPos - new Vector2(5, 5), Vector2.Zero, nodeColor);
                        lastPos = currentPos;
                    }
                    if (forceFlip)
                    {
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.PopMatrix();
                    }
                }
                else if (this.State_PlaybackMode == 1)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (this.nearbyFrames[i] != null && this.nearbyFrames[i].Count >= 1)
                        {
                            forceFlip = MainForm.self.CurrentReplays[i].AxisFlip;
                            if (forceFlip)
                            {
                                GL.MatrixMode(MatrixMode.Projection);
                                GL.PushMatrix();
                                this.MulFlipMatrix();
                            }
                            Vector2 cursorPos = this.GetInterpolatedFrame(i);
                            float diameter = 32.0f;
                            this.cursorTexture.Draw(cursorPos, diameter, diameter, new Vector2(diameter * 0.5f), Canvas.Color_Cursor[i]);
                            if (forceFlip)
                            {
                                GL.MatrixMode(MatrixMode.Projection);
                                GL.PopMatrix();
                            }
                        }
                    }
                }
                if (this.ShowHelp != 0)
                {
                    GL.MatrixMode(MatrixMode.Projection);
                    GL.PushMatrix();
                    GL.LoadIdentity();
                    GL.Ortho(0.0, 1.0, 1.0, 0.0, 0.0, 1.0);
                    this.helpTexture.Draw(Vector2.Zero, 1.0f, 1.0f, Vector2.Zero, Color.White);
                    GL.MatrixMode(MatrixMode.Projection);
                    GL.PopMatrix();
                }
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

        private void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            GL.Disable(EnableCap.Texture2D);
            GL.Color4(color);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(start.X, start.Y);
            GL.Vertex2(end.X, end.Y);
            GL.End();
            GL.Enable(EnableCap.Texture2D);
        }

        private void DrawHitcircle(BMAPI.v1.HitObjects.CircleObject hitObject, float alpha)
        {
            // TODO: flip when hardrock
            Vector2 pos = hitObject.Location.ToVector2();
            this.DrawHitcircle(pos, this.circleDiameter, alpha);
        }

        private void DrawHitcircle(Vector2 pos, int diameter, float alpha)
        {
            this.hitCircleTexture.Draw(pos, diameter, diameter, new Vector2(diameter*0.5f), new Color(1.0f, 1.0f, 1.0f, alpha));
        }

        private void DrawApproachCircle(BMAPI.v1.HitObjects.CircleObject hitObject, float alpha, float value)
        {
            float smallDiameter = this.circleDiameter;
            float largeDiameter = smallDiameter * 3.0f;
            // linearly interpolate between two diameters
            // makes approach circle shrink
            int diameter = (int)(smallDiameter + (largeDiameter - smallDiameter) * value);
            // TODO: flip when hardrock
            Vector2 pos = new Vector2(hitObject.Location.X, hitObject.Location.Y);
            this.approachCircleTexture.Draw(pos, diameter, diameter, new Vector2(diameter * 0.5f), new Color(1.0f, 1.0f, 1.0f, alpha));
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
            int diameter = (int)(384.0f * (1 - value));
            // TODO: flip when hardrock
            Vector2 pos = new Vector2(hitObject.Location.X, hitObject.Location.Y);
            this.spinnerTexture.Draw(pos, diameter, diameter, new Vector2(diameter * 0.5f), new Color(1.0f, 1.0f, 1.0f, alpha));
        }

        private void DrawSpinner(BMAPI.v1.HitObjects.SpinnerObject hitObject, float alpha)
        {
            float diameter = 384;
            Vector2 pos = new Vector2(hitObject.Location.X, hitObject.Location.Y);
            this.spinnerTexture.Draw(pos, diameter, diameter, new Vector2(diameter * 0.5f), new Color(1.0f, 1.0f, 1.0f, alpha));
        }

        private void DrawSlider(BMAPI.v1.HitObjects.SliderObject hitObject, float alpha, float approachCircleValue, bool isBackground)
        {
            float time = (float)(this.songPlayer.SongTime - hitObject.StartTime) / (float)((hitObject.SegmentEndTime - hitObject.StartTime) * hitObject.RepeatCount);
            byte reverseArrowType = (hitObject.RepeatCount > 1) ? (byte)1 : (byte)0;
            if (time < 0)
            {
                if (isBackground)
                {
                    this.DrawSliderBody(hitObject, alpha, this.circleDiameter / 2, reverseArrowType);
                    this.DrawHitcircle(hitObject, alpha);
                }
                else
                {
                    this.DrawApproachCircle(hitObject, alpha, approachCircleValue);
                }
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
            if (isBackground)
            {
                this.DrawSliderBody(hitObject, alpha, this.circleDiameter / 2, reverseArrowType);
            }
            else
            {
                Vector2 pos = hitObject.PositionAtTime(time);
                float diameter = this.circleDiameter * 2.0f;
                this.sliderFollowCircleTexture.Draw(pos, diameter, diameter, new Vector2(0.5f * diameter), new Color(1.0f, 1.0f, 1.0f, alpha));
            }
        }

        private void DrawSliderBody(BMAPI.v1.HitObjects.SliderObject hitObject, float alpha, int radius, int zindex)
        {
            float smallLength = hitObject.TotalLength;
            Color color = Color.White;
            for (float i = 0; i < smallLength + 10; i += 10)
            {
                if (i > smallLength)
                {
                    i = smallLength;
                }
                // TODO: flip when hardrock
                Vector2 pos = hitObject.PositionAtTime(i / smallLength);
                int diameter = this.circleDiameter;
                color.A = (byte)(255 * alpha);
                this.sliderEdgeTexture.Draw(pos, diameter, diameter, new Vector2(diameter * 0.5f), color);
                if (i == smallLength)
                {
                    break;
                }
            }
            color.A = 255;
            for (float i = 0; i < smallLength + 10; i += 10)
            {
                if (i > smallLength)
                {
                    i = smallLength;
                }
                // TODO: flip when hardrock
                Vector2 pos = hitObject.PositionAtTime(i / smallLength);
                int diameter = this.circleDiameter;
                this.sliderBodyTexture.Draw(pos, diameter, diameter, new Vector2(diameter * 0.5f), color);
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
            for (float i = 0; i < smallLength + 10; i += 10)
            {
                if (i > smallLength)
                {
                    i = smallLength;
                }
                // TODO: flip when hardrock
                Vector2 pos = hitObject.PositionAtTime(i / smallLength);
                int diameter = this.circleDiameter;
                color.A = alphaByte;
                this.sliderEdgeTexture.Draw(pos, diameter, diameter, new Vector2(0.5f * diameter), color);
                //this.spriteBatch.Draw(this.sliderEdgeTexture, rect, null, color, 0f, Vector2.Zero, SpriteEffects.None, 0.4f + depthHigh);
                color.A = 255;
                //this.spriteBatch.Draw(this.sliderBodyTexture, rect, null, color, 0f, Vector2.Zero, SpriteEffects.None, 0.4f + depthMed);
                this.sliderBodyTexture.Draw(pos, diameter, diameter, new Vector2(0.5f * diameter), color);
                if (i == smallLength)
                {
                    break;
                }
            }
            if (reverseArrowType != 0)
            {
                /*
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
                */
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
            //this.circleDiameter = (int)(2 * (40 - 4 * (moddedCS - 2)));
            this.circleDiameter = (int)(109 - 9 * moddedCS);
            if (this.circleDiameter < 5)
                this.circleDiameter = 5;
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
