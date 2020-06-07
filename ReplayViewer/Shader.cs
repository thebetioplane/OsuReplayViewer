using System.IO;
using OpenTK;
using OpenTK.Graphics.ES20;
using System.Runtime.InteropServices;

namespace ReplayViewer
{
    public class Shader
    {
        private int programID;
        private static readonly float ScreenLeft;
        private static readonly float ScreenRight;
        private static readonly float ScreenTop;
        private static readonly float ScreenBottom;
        private static Matrix4 Projection;
        private int uProjection;
        private const float UNIQUE_ENOUGH = -234.432f;

        private int uMode;
        private int uCenter;
        private int uOffCenter;
        private int uCurrentTime;
        private int uStartTime;
        private int uEndTime;
        private int uAR;
        private int uCS;
        private int uSliderVelocity;

        private int mMode = -1;
        public int Mode
        {
            get
            {
                return mMode;
            }
            set
            {
                if (mMode != value)
                {
                    GL.Uniform1(this.uMode, value);
                    mMode = value;
                }
            }
        }

        private Vector2 mCenter = new Vector2(UNIQUE_ENOUGH, UNIQUE_ENOUGH);
        public Vector2 Center
        {
            get
            {
                return mCenter;
            }
            set
            {
                if (mCenter != value)
                {
                    GL.Uniform2(this.uCenter, value);
                    mCenter = value;
                }
            }
        }

        private Vector2 mOffCenter = new Vector2(UNIQUE_ENOUGH, UNIQUE_ENOUGH);
        public Vector2 OffCenter
        {
            get
            {
                return mOffCenter;
            }
            set
            {
                if (mOffCenter != value)
                {
                    GL.Uniform2(this.uOffCenter, value);
                    mOffCenter = value;
                }
            }
        }

        private float mCurrentTime = -1;
        public float CurrentTime
        {
            get
            {
                return mCurrentTime;
            }
            set
            {
                if (mCurrentTime != value)
                {
                    GL.Uniform1(this.uCurrentTime, value);
                    mCurrentTime = value;
                }
            }
        }

        private float mStartTime = UNIQUE_ENOUGH;
        public float StartTime
        {
			get
			{
				return mStartTime;
			}
			set
			{
				if (mStartTime != value)
				{
					GL.Uniform1(this.uStartTime, value);
					mStartTime = value;
				}
			}
        }

        private float mEndTime = UNIQUE_ENOUGH;
        public float EndTime
        {
			get
			{
				return mEndTime;
			}
			set
			{
				if (mEndTime != value)
				{
					GL.Uniform1(this.uEndTime, value);
					mEndTime = value;
				}
			}
        }

        private float mAR = UNIQUE_ENOUGH;
        public float AR
        {
			get
			{
				return mAR;
			}
			set
			{
				if (mAR != value)
				{
					GL.Uniform1(this.uAR, value);
					mAR = value;
				}
			}
        }

        private float mCS = UNIQUE_ENOUGH;
        public float CS
        {
			get
			{
				return mCS;
			}
			set
			{
				if (mCS != value)
				{
					GL.Uniform1(this.uCS, value);
					mCS = value;
				}
			}
        }

        private float mSliderVelocity = UNIQUE_ENOUGH;
        public float SliderVelocity
        {
			get
			{
				return mSliderVelocity;
			}
			set
			{
				if (mSliderVelocity != value)
				{
					GL.Uniform1(this.uSliderVelocity, value);
					mSliderVelocity = value;
				}
			}
        }

        static Shader()
        {
            Vector2 border = new Vector2(4.0f, 3.0f) * 32.0f;
            ScreenLeft = -border.X;
            ScreenRight = 512.0f + border.X;
            ScreenBottom = 384.0f + border.Y;
            ScreenTop = -border.Y;
            Projection = Matrix4.CreateOrthographicOffCenter(ScreenLeft, ScreenRight, ScreenBottom, ScreenTop, 0.0f, 3.0f);
        }

        public Shader()
        {
            int vsShader = CompileShader(File.ReadAllText(MainForm.Path_Vs), ShaderType.VertexShader);
            int fsShader = CompileShader(File.ReadAllText(MainForm.Path_Fs), ShaderType.FragmentShader);
            this.programID = LinkShader(vsShader, fsShader);
            GL.UseProgram(this.programID);
            this.uMode = GL.GetUniformLocation(this.programID, "mode");
            this.uProjection = GL.GetUniformLocation(this.programID, "projection");
            this.uCenter = GL.GetUniformLocation(this.programID, "center");
            this.uOffCenter = GL.GetUniformLocation(this.programID, "off_center");
            this.uCurrentTime = GL.GetUniformLocation(this.programID, "current_time");
            this.uStartTime = GL.GetUniformLocation(this.programID, "start_time");
            this.uEndTime = GL.GetUniformLocation(this.programID, "end_time");
            this.uAR = GL.GetUniformLocation(this.programID, "AR");
            this.uCS = GL.GetUniformLocation(this.programID, "CS");
            this.uSliderVelocity = GL.GetUniformLocation(this.programID, "slider_velocity");
            Vector2 v = new Vector2();
            GL.Uniform2(this.uCenter, ref v);
            int buf = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, buf);
            // triangle strip, order is
            // top-left, bottom-left, top-right, bottom-right
            float[] vertexData = new float[] {
                ScreenLeft, ScreenTop, -1.0f, 1.0f,
                ScreenLeft, ScreenBottom, -1.0f, -1.0f,
                ScreenRight, ScreenTop, 1.0f, 1.0f,
                ScreenRight, ScreenBottom, 1.0f, -1.0f,
            };
            GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * 4, vertexData, BufferUsageHint.StaticDraw);
            int attrib_position = GL.GetAttribLocation(this.programID, "in_position");
            int attrib_uv = GL.GetAttribLocation(this.programID, "in_uv");
            // 4 floats, 4 bytes each
            const int stride = 4*4;
            GL.EnableVertexAttribArray(attrib_position);
            GL.EnableVertexAttribArray(attrib_uv);
            GL.VertexAttribPointer(attrib_position, 2, VertexAttribPointerType.Float, false, stride, 0);
            GL.VertexAttribPointer(attrib_uv, 2, VertexAttribPointerType.Float, false, stride, 2*4);
            GL.UniformMatrix4(this.uProjection, false, ref Projection);
            this.Mode = 2;
            this.Center = new Vector2(100.0f, 100.0f);
            this.OffCenter = new Vector2(300.0f, 150.0f);
            this.CurrentTime = 0.0f;
            this.StartTime = 0.0f;
            this.EndTime = 0.0f;
            this.AR = 8.0f;
            this.CS = 5.0f;
            this.SliderVelocity = 130.0f;
        }

        private static int CompileShader(string source, ShaderType st)
        {
            int handle = GL.CreateShader(st);
            if (handle == 0)
                throw new System.Exception("could not create shader");
            GL.ShaderSource(handle, source);
            GL.CompileShader(handle);
            CheckCompileStatus(handle);
            return handle;
        }

        private static int LinkShader(int shader0, int shader1)
        {
            int handle = GL.CreateProgram();
            if (handle == 0)
                throw new System.Exception("could not create program");
            GL.AttachShader(handle, shader0);
            GL.AttachShader(handle, shader1);
            GL.LinkProgram(handle);
            CheckLinkStatus(handle);
            return handle;
        }

        private static void CheckCompileStatus(int handle)
        {
            GL.GetShader(handle, ShaderParameter.CompileStatus, out int status);
            if (status == 0)
            {
                string msg = GL.GetShaderInfoLog(handle);
                throw new System.Exception("shader did not compile successfully\n\n" + msg);
            }
        }

        private static void CheckLinkStatus(int handle)
        {
            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int status);
            if (status == 0)
            {
                string msg = GL.GetProgramInfoLog(handle);
                throw new System.Exception("shader did not link successfully\n\n" + msg);
            }
        }

        public void Draw()
        {
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        }
    }
}
