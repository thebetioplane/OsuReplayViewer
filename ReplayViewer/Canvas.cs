using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using Rectangle = System.Drawing.Rectangle;
using Color = OpenTK.Graphics.Color4;
using OpenTK.Graphics.ES20;
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

        public bool Visual_MapInvert { get; set; }

        private float state_PlaybackSpeed;
        private byte state_ReplaySelected;
        private float state_volume;
        private float AnimationStartOffset;
        private float AnimationEndOffset;

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

        private Shader shader;

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
            for (int i = 0; i < 3; ++i)
            {
                this.Draw(i);
            }
            //shader.Draw();
            this.SwapBuffers();
            System.Threading.Thread.Sleep(16);
            this.Resize += Canvas_Resize;
        }

        private void Canvas_Resize(object sender, EventArgs e)
        {
            int w = this.Size.Width;
            int h = this.Size.Height;
            if (w < 5)
                w = 5;
            if (h < 5)
                h = 5;
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

            try
            {
                this.shader = new Shader();
            }
            catch (FileNotFoundException e)
            {
                MainForm.ErrorMessage(string.Format("Cannot load the shader found at \"{0}\".\nDeleting the \"manifest\" file will restore lost files and update new ones.", e.FileName));
                throw e;
            }

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            this.Canvas_Resize(this, null);
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
                MainForm.ErrorMessage(String.Format("Could not the load the image found at \"{0}\", please make sure it exists and is a valid .png image.\nDeleting the \"manifest\" file will restore lost images and update new ones.", path));
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
                float time = (float)this.songPlayer.SongTime;
                int startIndex = this.HitObjectsLowerBound(time);
                int endIndex = this.HitObjectsUpperBound(time);
                for (int k = startIndex; k <= endIndex; k++)
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
                    if (this.State_PlaybackMode == 0 || this.State_PlaybackMode == 2)
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

        /// <summary>
        /// draw
        /// </summary>
        /// <param name="backgroundLevel">0 -> background (slider body), 1 -> middle (hitcircle), 2 -> foreground (approach circle)</param>
        private void Draw(int backgroundLevel)
        {
            shader.CurrentTime = (float)this.songPlayer.SongTime;
            for (int b = this.nearbyHitObjects.Count - 1; b >= 0; b--)
            {
                var hitObject = this.nearbyHitObjects[b];
                bool isSlider = hitObject.Type.HasFlag(BMAPI.v1.HitObjectType.Slider);
                bool isSpinner = hitObject.Type.HasFlag(BMAPI.v1.HitObjectType.Spinner);
                shader.Center = hitObject.Location.ToVector2();
                shader.StartTime = hitObject.StartTime;
                if (isSlider)
                {
                    var slider = hitObject as BMAPI.v1.HitObjects.SliderObject;
                    shader.EndTime = slider.SegmentEndTime + (slider.SegmentEndTime - slider.StartTime) * slider.RepeatCount;
                    if (backgroundLevel == 1)
                    {
                        shader.Mode = 1;
                        shader.Draw();
                    }
                    else if (backgroundLevel == 0)
                    {
                        shader.Mode = 2;
                        foreach (var curve in slider.Curves)
                        {
                            for (int i = 1; i < curve.DrawingPoints.Count; ++i)
                            {
                                shader.OffCenter = curve.DrawingPoints[i - 1];
                                shader.Center = curve.DrawingPoints[i];
                                shader.Draw();
                            }
                        }
                    }
                }
                else if (isSpinner)
                {
                    var spinner = hitObject as BMAPI.v1.HitObjects.SpinnerObject;
                    shader.EndTime = spinner.EndTime;
                }
                else
                {
                    shader.EndTime = shader.StartTime;
                    if (backgroundLevel == 1)
                    {
                        shader.Mode = 1;
                        shader.Draw();
                    }
                }
#if RENDER_CURSOR
                if (this.State_PlaybackMode == 0)
                {
                    if (MainForm.self.CurrentReplays[this.State_ReplaySelected] != null)
                    {
                        forceFlip = MainForm.self.CurrentReplays[this.State_ReplaySelected].AxisFlip;
                    }
                    if (forceFlip)
                    {
                        /*
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.PushMatrix();
                        this.MulFlipMatrix();
                        */
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
                        /*
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.PopMatrix();
                        */
                    }
                }
                if (this.State_PlaybackMode == 2 && this.nearbyFrames.Count > 2)
                {
                    if (MainForm.self.CurrentReplays[this.State_ReplaySelected] != null)
                    {
                        forceFlip = MainForm.self.CurrentReplays[this.State_ReplaySelected].AxisFlip;
                    }
                    if (forceFlip)
                    {
                        /*
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.PushMatrix();
                        this.MulFlipMatrix();
                        */
                    }
                    for (int i = 1; i < this.nearbyFrames[this.state_ReplaySelected].Count; i++)
                    {
                        var lastFrame = this.nearbyFrames[this.state_ReplaySelected][i - 1];
                        var currentFrame = this.nearbyFrames[this.state_ReplaySelected][i];
                        float alpha = i / (float)this.nearbyFrames[this.state_ReplaySelected].Count;
                        var lastPos = new Vector2(lastFrame.X, lastFrame.Y);
                        var currentPos = new Vector2(currentFrame.X, currentFrame.Y);
                        this.DrawLine(lastPos, currentPos, new Color(1.0f, 0.0f, 0.0f, alpha));
                        Color nodeColor = Color.Black;
                        if (currentFrame.Keys.HasFlag(ReplayAPI.Keys.K1) && !lastFrame.Keys.HasFlag(ReplayAPI.Keys.K1))
                        {
                            nodeColor = Color.Yellow;
                        }
                        else if (currentFrame.Keys.HasFlag(ReplayAPI.Keys.K2) && !lastFrame.Keys.HasFlag(ReplayAPI.Keys.K2))
                        {
                            nodeColor = Color.Yellow;
                        }
                        else if (currentFrame.Keys.HasFlag(ReplayAPI.Keys.M1) && !lastFrame.Keys.HasFlag(ReplayAPI.Keys.M1))
                        {
                            nodeColor = Color.Yellow;
                        }
                        else if (currentFrame.Keys.HasFlag(ReplayAPI.Keys.M2) && !lastFrame.Keys.HasFlag(ReplayAPI.Keys.M2))
                        {
                            nodeColor = Color.Yellow;
                        }
                        else if (currentFrame.Keys != lastFrame.Keys && !currentFrame.Keys.HasFlag(ReplayAPI.Keys.K1) && !currentFrame.Keys.HasFlag(ReplayAPI.Keys.K2) && !currentFrame.Keys.HasFlag(ReplayAPI.Keys.M1) && !currentFrame.Keys.HasFlag(ReplayAPI.Keys.M2))
                        {
                            nodeColor = Color.Gray;
                        }
                        if (nodeColor != Color.Black)
                        {
                            this.nodeTexture.Draw(currentPos - new Vector2(5, 5), Vector2.Zero, nodeColor);
                        }
                    }
                    if (forceFlip)
                    {
                        /*
                        GL.MatrixMode(MatrixMode.Projection);
                        GL.PopMatrix();
                        */
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
                                /*
                                GL.MatrixMode(MatrixMode.Projection);
                                GL.PushMatrix();
                                this.MulFlipMatrix();
                                */
                            }
                            Vector2 cursorPos = this.GetInterpolatedFrame(i);
                            float diameter = 32.0f;
                            this.cursorTexture.Draw(cursorPos, diameter, diameter, new Vector2(diameter * 0.5f), Canvas.Color_Cursor[i]);
                            if (forceFlip)
                            {
                                /*
                                GL.MatrixMode(MatrixMode.Projection);
                                GL.PopMatrix();
                                */
                            }
                        }
                    }
                }
#endif
                if (this.ShowHelp != 0)
                {
                    /*
                    GL.MatrixMode(MatrixMode.Projection);
                    GL.PushMatrix();
                    GL.LoadIdentity();
                    GL.Ortho(0.0, 1.0, 1.0, 0.0, 0.0, 1.0);
                    this.helpTexture.Draw(Vector2.Zero, 1.0f, 1.0f, Vector2.Zero, Color.White);
                    GL.MatrixMode(MatrixMode.Projection);
                    GL.PopMatrix();
                    */
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

        private int HitObjectsLowerBound(float target)
        {
            int first = 0;
            int last = this.Beatmap.HitObjects.Count;
            int count = last - first;
            while (count > 0)
            {
                int step = count / 2;
                int it = first + step;
                var hitObject = this.Beatmap.HitObjects[it];
                float endTime = hitObject.StartTime;
                if (hitObject.Type.HasFlag(BMAPI.v1.HitObjectType.Spinner))
                {
                    endTime = ((BMAPI.v1.HitObjects.SpinnerObject)hitObject).EndTime;
                }
                else if (hitObject.Type.HasFlag(BMAPI.v1.HitObjectType.Slider))
                {
                    var slider = (BMAPI.v1.HitObjects.SliderObject)hitObject;
                    endTime = slider.SegmentEndTime + (slider.SegmentEndTime - slider.StartTime)*slider.RepeatCount;
                }
                float animationEnd = endTime + this.AnimationEndOffset;
                if (animationEnd < target)
                {
                    first = ++it;
                    count -= step + 1;
                }
                else
                {
                    count = step;
                }
            }
            return first;
        }

        private int HitObjectsUpperBound(float target)
        {
            int first = 0;
            int last = this.Beatmap.HitObjects.Count;
            int count = last - first;
            while (count > 0)
            {
                int step = count / 2;
                int it = first + step;
                float animationStart = this.Beatmap.HitObjects[it].StartTime - this.AnimationStartOffset;
                if (!(target < animationStart))
                {
                    first = ++it;
                    count -= step + 1;
                }
                else
                {
                    count = step;
                }
            }
            return first;
        }

        private void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            /*
            GL.Disable(EnableCap.Texture2D);
            GL.Color4(color);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(start.X, start.Y);
            GL.Vertex2(end.X, end.Y);
            GL.End();
            GL.Enable(EnableCap.Texture2D);
            */
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
                this.songPlayer.Start(Path.Combine(this.Beatmap.Folder, this.Beatmap.AudioFilename));
                this.JumpTo(this.FirstHitObjectTime - 1000);
                this.ApplyMods(replay, this.Beatmap.ApproachRate, this.Beatmap.CircleSize);
            }
            else
            {
                this.ApplyMods(replay, 5.0f, 5.0f);
            }
#if DEBUG
            if (this.Beatmap != null)
            {
                int segSum = 0;
                int sliderCount = 0;
                int biggestSeg = 0;
                int smallestSeg = int.MaxValue;
                foreach (var hitobj in this.Beatmap.HitObjects)
                {
                    if (hitobj.Type.HasFlag(BMAPI.v1.HitObjectType.Slider))
                    {
                        var slider = hitobj as BMAPI.v1.HitObjects.SliderObject;
                        int n = 0;
                        foreach (var curve in slider.Curves)
                        {
                            n += curve.DrawingPoints.Count;
                        }
                        segSum += n;
                        ++sliderCount;
                        if (n > biggestSeg)
                            biggestSeg = n;
                        if (n < smallestSeg)
                            smallestSeg = n;
                    }
                }
                Console.WriteLine("replay loaded");
                Console.WriteLine("slider segments: min {0}, max {1}, avg {2}", smallestSeg, biggestSeg, segSum / (float)sliderCount);
            }
#endif
        }

        /// <summary>
        /// this is the way osu maps difficulty levels from 0 -> 10 to output
        /// </summary>
        /// <param name="difficulty"></param>
        /// <param name="v0">output when difficulty = 0</param>
        /// <param name="v1">output when difficulty = 5</param>
        /// <param name="v2">output when difficulty = 10</param>
        /// <returns></returns>
        private float DifficultyRange(float difficulty, float v0, float v1, float v2)
        {
            if (difficulty > 5)
                return v1 + (v2 - v1) * (difficulty - 5) / 5;
            if (difficulty < 5)
                return v1 - (v1 - v0) * (5 - difficulty) / 5;
            return v1;
        }

        public void ApplyMods(ReplayAPI.Replay replay, float baseAR, float baseCS)
        {
            if (replay.Mods.HasFlag(ReplayAPI.Mods.Easy))
            {
                shader.AR = baseAR * 0.5f;
                shader.CS = baseCS * 0.5f;
            }
            else if (replay.Mods.HasFlag(ReplayAPI.Mods.HardRock))
            {
                shader.AR = Math.Min(baseAR * 1.4f, 10.0f);
                shader.CS = baseCS * 1.3f;
                this.Visual_MapInvert = true;
            }
            else
            {
                shader.AR = baseAR;
                shader.CS = baseCS;
            }
            this.AnimationStartOffset = DifficultyRange(shader.AR, 1800, 1200, 450);
            this.AnimationEndOffset = this.AnimationStartOffset * 2.0f / 3.0f;
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
        }

        public void UnloadReplay(byte pos)
        {
            this.replayFrames[pos] = null;
            this.nearbyFrames[pos] = new List<ReplayAPI.ReplayFrame>();
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
