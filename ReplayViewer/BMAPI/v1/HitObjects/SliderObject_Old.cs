using System;
using System.Collections.Generic;
using System.Linq;

namespace BMAPI.v1.HitObjects
{
    internal struct DistanceTime
    {
        public float distance;
        public float t;
        public Point2 point;
    }
    public class SliderObject : CircleObject
    {
        public SliderObject() { }
        public SliderObject(CircleObject baseInstance) : base(baseInstance) { }

        public new SliderType Type = SliderType.Linear;
        public List<Point2> Points = new List<Point2>();
        public int RepeatCount { get; set; }
        public float PixelLength
        {
            get { return this._PixelLength; }
            set
            {
                this._PixelLength = value;
            }
        }
        private float _PixelLength = 0f;
        public float Velocity { get; set; }
        public float MaxPoints { get; set; }
        private List<DistanceTime> distanceTime = new List<DistanceTime>();
        private float? S_Length { get; set; }
        private float? S_SegmentLength { get; set; }
        public float Length
        {
            get
            {
                if (S_Length != null) return (float)S_Length;
                switch (Type)
                {
                    case SliderType.Linear:
                        S_Length = (float)Math.Sqrt(Math.Pow(Points[1].X - Points[0].X, 2) + Math.Pow(Points[1].Y - Points[0].Y, 2)) * RepeatCount;
                        break;
                    case SliderType.CSpline:
                        Catmull c = new Catmull(Points, this);
                        return c.Length() * RepeatCount;
                    case SliderType.PSpline:
                        S_Length = PassThroughLength(Points) * RepeatCount;
                        break;
                    case SliderType.Bezier:
                        S_Length = BezierLength(Points) * RepeatCount;
                        break;
                    default:
                        S_Length = 0;
                        break;
                }
                return (float)S_Length;
            }
        }
        public float SegmentLength
        {
            get
            {
                if (S_SegmentLength != null) return (float)S_SegmentLength;
                switch (Type)
                {
                    case SliderType.Linear:
                        S_SegmentLength = LinearLength(Points);
                        break;
                    case SliderType.CSpline:
                        Catmull c = new Catmull(Points, this);
                        return c.Length();
                    case SliderType.PSpline:
                        S_SegmentLength = PassThroughLength(Points);
                        break;
                    case SliderType.Bezier:
                        S_SegmentLength = BezierLength(Points);
                        break;
                    default:
                        S_SegmentLength = 0;
                        break;
                }
                return (float)S_SegmentLength;
            }
        }
        public float SegmentEndTime(int SegmentNumber)
        {
            switch (Type)
            {
                case SliderType.Linear:
                    return StartTime + (SegmentLength * SegmentNumber) / Velocity;
                case SliderType.CSpline:
                case SliderType.PSpline:
                    return StartTime + (SegmentLength * SegmentNumber) / Velocity;
                case SliderType.Bezier:
                    return StartTime + (SegmentLength * SegmentNumber) / Velocity;
                default:
                    return 0;
            }
        }
        public Point2 PositionAtTime(float T)
        {
            /*
            if (this.Type == SliderType.Linear)
            {
                return Points[0].Lerp(Points[1], T);
            }
             */
            return UniformSpeed(Points, this.Length / this.RepeatCount * T);
        }

        public Point2 PassThroughInterpolate(float t)
        {
            if (Points.Count == 3)
            {
                Point2 center = this.circleCenter(Points[0], Points[1], Points[2]);
                float radius = Points[0].DistanceTo(center);
                float start = (Points[0] - center).Atan2();
                //float middle = (Points[1] - center).Atan2();
                float end = (Points[2] - center).Atan2();
                float twopi = (float)(2 * Math.PI);
                if (isClockwise(Points[0], Points[1], Points[2]))
                {
                    while (end < start)
                    {
                        end += twopi;
                    }
                }
                else
                {
                    while (start < end)
                    {
                        start += twopi;
                    }
                }
                t = start + (end - start) * t;
                return new Point2((float)(Math.Cos(t) * radius), (float)(Math.Sin(t) * radius)) + center;
            }
            else
            {
                return BezierInterpolate(Points, t);
            }
        }

        private bool isClockwise(Point2 a, Point2 b, Point2 c)
        {
            return a.X * b.Y - b.X * a.Y + b.X * c.Y - c.X * b.Y + c.X * a.Y - a.X * c.Y > 0;
        }

        private Point2 circleCenter(Point2 A, Point2 B, Point2 C)
        {
            float yDelta_a = B.Y - A.Y;
            float xDelta_a = B.X - A.X;
            float yDelta_b = C.Y - B.Y;
            float xDelta_b = C.X - B.X;
            Point2 center = new Point2();
            if (xDelta_a == 0)
            {
                xDelta_a = 0.00001f;
            }
            if (xDelta_b == 0)
            {
                xDelta_b = 0.00001f;
            }
            float aSlope = yDelta_a / xDelta_a;
            float bSlope = yDelta_b / xDelta_b;
            center.X = (aSlope * bSlope * (A.Y - C.Y) + bSlope * (A.X + B.X) - aSlope * (B.X + C.X)) / (2 * (bSlope - aSlope));
            center.Y = -1 * (center.X - (A.X + B.X) / 2) / aSlope + (A.Y + B.Y) / 2;
            return center;
        }

        public float PassThroughLength(List<Point2> Pts, float prec = 0.01f)
        {
            float sum = 0;
            for (float f = 0; f < 1f; f += prec)
            {
                Point2 a = PassThroughInterpolate(f);
                Point2 b = PassThroughInterpolate(f + prec);
                float distance = (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
                if (sum == 0 || (this.PixelLength > 0 && distance + sum <= this.PixelLength))
                {
                    DistanceTime dt = new DistanceTime();
                    sum += distance;
                    dt.distance = sum;
                    dt.t = f;
                    dt.point = b;
                    this.distanceTime.Add(dt);
                }
                else
                {
                    break;
                }
            }
            return sum;
        }

        public Point2 BezierInterpolate(List<Point2> Pts, float t)
        {
            int n = Pts.Count;
            if (n == 2)
            {
                return Points[0].Lerp(Points[1], t);
            }
            Point2[] points = new Point2[Pts.Count];

            for (int i = 0; i < n; i++)
            {
                points[i] = Pts[i] + new Point2();
            }

            for (int k = 1; k < n; k++)
            {
                for (int i = 0; i < n - k; i++)
                {
                    points[i] = (1 - t) * points[i] + t * points[i + 1];
                }
            }
            return points[0];
        }

        public Point2 UniformSpeed(List<Point2> points, float target)
        {
            int high = this.distanceTime.Count - 1;
            int low = 0;
            while (low <= high)
            {
                int mid = (high + low) / 2;
                if (mid == high || mid == low)
                {
                    if (mid + 1 >= this.distanceTime.Count)
                    {
                        return this.distanceTime[mid].point;
                    }
                    else
                    {
                        DistanceTime a = this.distanceTime[mid];
                        DistanceTime b = this.distanceTime[mid + 1];
                        return a.point.Lerp(b.point, (target - a.distance) / (b.distance - a.distance));
                    }
                }
                if (this.distanceTime[mid].distance > target)
                {
                    high = mid;
                }
                else
                {
                    low = mid;
                }
            }
            return new Point2();
        }
        
        public float BezierLength(List<Point2> Pts, float prec = 0.01f)
        {
            float sum = 0;
            for (float f = 0; f < 1f; f += prec)
            {
                Point2 a = BezierInterpolate(Pts, f);
                Point2 b = BezierInterpolate(Pts, f + prec);
                float distance = (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
                if (this.PixelLength > 0 && distance + sum <= this.PixelLength)
                {
                    DistanceTime dt = new DistanceTime();
                    sum += distance;
                    dt.distance = sum;
                    dt.t = f;
                    dt.point = b;
                    this.distanceTime.Add(dt);
                }
                else
                {
                    break;
                }
            }
            return sum;
        }

        public float LinearLength(List<Point2> Pts)
        {
            float sum = 0;
            for (int i = 0; i < Pts.Count - 1; i++)
            {
                float distance = Pts[i].DistanceTo(Pts[i + 1]);
                if (this.PixelLength > 0 && distance + sum <= this.PixelLength)
                {
                    sum += distance;
                }
                else
                {
                    break;
                }
            }
            return sum;
        }

        public Point2 LinearInterpolate(List<Point2> Pts, float distance)
        {
            float dsum = 0;
            int n = Pts.Count - 1;
            for (int i = 0; i < n; i++)
            {
                float d = Pts[0].DistanceTo(Pts[1]);
                if (dsum + d > distance)
                {
                    return Pts[i].Lerp(Pts[i + 1], (distance - dsum) / d);
                }
                dsum += d;
            }
            return Pts[n];
        }

        public class Catmull : List<SplineFunction>
        {
            public SliderObject parent = null;

            public Catmull(IList<Point2> Points, SliderObject parent)
            {
                this.parent = parent;
                List<float> Times = new List<float>();
                for (float i = 0; i <= 1; i += 1f / Points.Count)
                {
                    Times.Add(i);
                }
                int n = Points.Count - 1;

                Point2[] b = new Point2[n];
                Point2[] d = new Point2[n];
                Point2[] a = new Point2[n];
                Point2[] c = new Point2[n + 1];
                Point2[] l = new Point2[n + 1];
                Point2[] u = new Point2[n + 1];
                Point2[] z = new Point2[n + 1];
                float[] h = new float[n + 1];

                l[0] = new Point2(1);
                u[0] = new Point2(0);
                z[0] = new Point2(0);
                h[0] = Times[1] - Times[0];

                for (int i = 1; i < n; i++)
                {
                    h[i] = Times[i + 1] - Times[i];
                    l[i] = 2 * (Times[i + 1] - Times[i - 1]) - (h[i - 1] * u[i - 1]);
                    u[i] = h[i] / l[i];
                    a[i] = (3 / h[i]) * (Points[i + 1] - Points[i]) - ((3 / h[i - 1]) * (Points[i] - Points[i - 1]));
                    z[i] = (a[i] - (h[i - 1] * z[i - 1])) / l[i];
                }
                l[n] = new Point2(1);
                z[n] = c[n] = new Point2(0);

                for (int j = n - 1; j >= 0; j--)
                {
                    c[j] = z[j] - (u[j] * c[j + 1]);
                    b[j] = (Points[j + 1] - Points[j]) / h[j] - (h[j] * (c[j + 1] + 2 * c[j])) / 3;
                    d[j] = (c[j + 1] - c[j]) / (3 * h[j]);
                }

                for (int i = 0; i < n; i++)
                {
                    Add(new SplineFunction(Times[i], Points[i], b[i], c[i], d[i]));
                }
            }

            private Point2 Interpolate(float T)
            {
                if (Count == 0) return new Point2();

                Sort();
                SplineFunction it = this.LastOrDefault(sf => sf.T <= T);
                return it.Eval(T);
            }


            public float Length(float prec = 0.01f)
            {
                float sum = 0;
                for (float f = 0; f < 1f; f += prec)
                {
                    Point2 a = Interpolate(f);
                    Point2 b = Interpolate(f + prec);
                    float distance = (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
                    DistanceTime dt = new DistanceTime();
                    sum += distance;
                    dt.distance = sum;
                    dt.t = f;
                    dt.point = b;
                    this.parent.distanceTime.Add(dt);
                }
                return sum;
            }
        }
        public class SplineFunction : IComparable
        {
            internal Point2 _a, _b, _c, _d;
            internal float T { get; set; }

            public SplineFunction(float x)
            {
                T = x;
            }
            public SplineFunction(float x, Point2 a, Point2 b, Point2 c, Point2 d)
            {
                _a = a;
                _b = b;
                _c = c;
                _d = d;
                T = x;
            }
            public int CompareTo(object Obj)
            {
                return Obj == null ? 1 : T.CompareTo(((SplineFunction)Obj).T);
            }

            public Point2 Eval(float x)
            {
                float xix = x - T;
                return _a + _b * xix + _c * (xix * xix) + _d * (xix * xix * xix);
            }
        }
        public Point2 CatmullInterpolate(List<Point2> Pts, float T)
        {
            if (Pts.Count == 2)
            {
                return Pts[0].Lerp(Pts[1], T);
            }
            Catmull spline = new Catmull(Pts, this);
            if (spline.Count == 0) return new Point2();
            spline.Sort();
            SplineFunction it = spline.LastOrDefault(sf => sf.T <= T);
            return it.Eval(T);
        }
    }
}
