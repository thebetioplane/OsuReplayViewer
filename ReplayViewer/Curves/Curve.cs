using System;
using System.Collections.Generic;
using BMAPI.v1;
using Vector2 = OpenTK.Vector2;

namespace ReplayViewer.Curves
{
    public struct DistanceTime
    {
        public float distance;
        public float t;
        public Vector2 point;
    }
    public abstract class Curve
    {
        public SliderType CurveType { get; set; }
        // list of only the control points relevent to this curve
        protected List<Vector2> Points;
        // points along the curve where segment length, 't' interpolation value, and location is recorded
        protected List<DistanceTime> CurveSnapshots;
        // when the curve should stop being drawn
        // this only applies if this is the last curve in the slider
        public float PixelLength { get; set; }

        public Curve(SliderType sliderType)
        {
            this.CurveType = sliderType;
            this.Points = new List<Vector2>();
            this.CurveSnapshots = new List<DistanceTime>();
            // unless the SliderObject wants the length to be limited, then we have no limit
            this.PixelLength = Single.PositiveInfinity;
        }

        public void AddPoint(Vector2 point)
        {
            this.Points.Add(point);
        }
        public float Length { get; private set; } = -1;

        // takes a number from 0.0 to 1.0 that represents the position on the curve
        // this does not act in a linear manner
        // i.e. from 0.00 to 0.25 is probably not the same distance as 0.25 to 0.50
        protected abstract Vector2 Interpolate(float t);

        private float CalculateLength()
        {
            if (this.Points.Count <= 1)
                return 0.0f;
            float sum = 0;
            this.AddDistanceTime(0.0f, 0.0f, this.Interpolate(0.0f));
            for (int i = 1; i < 200; ++i)
            {
                float t = i / 200.0f;
                Vector2 v = this.Interpolate(t);
                DistanceTime lastDT = this.CurveSnapshots[i - 1];
                float distance = this.Distance(lastDT.point, v);
                sum += distance;
                // take a snapshot of the current position, t value, and distance along the curve
                if (this.PixelLength > 0 && sum <= this.PixelLength)
                {
                    this.AddDistanceTime(sum, t, v);
                }
                else
                {
                    break;
                }
            }
            if (this.PixelLength > 0 && this.PixelLength < sum)
                return this.PixelLength;
            else
                return sum;
        }

        private void AddDistanceTime(float distance, float time, Vector2 point)
        {
            DistanceTime dt = new DistanceTime();
            dt.distance = distance;
            dt.t = time;
            dt.point = point;
            this.CurveSnapshots.Add(dt);
        }

        public void Init()
        {
            this.Length = this.CalculateLength();
        }

        protected Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            // helper method to linearly interpolate between two points
            return (1 - t) * a + t * b;
        }

        protected float Distance(Vector2 a, Vector2 b)
        {
            // helper method to linearly get the distance between two points
            return (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }

        protected float Atan2(Vector2 a)
        {
            // helper method to get the angle between a point and the origin
            return (float)Math.Atan2(a.Y, a.X);
        }

        public Vector2 PositionAtDistance(float d)
        {
            // because interpolation is not linearly correlated to distance along the curve this
            // method exists to get the PositionAtDistance
            //
            // binary search to find the DistanceTime struct that is close enough to our distance that
            // we want to find
            int high = this.CurveSnapshots.Count - 1;
            int low = 0;
            while (low <= high)
            {
                int mid = (high + low) / 2;
                if (mid == high || mid == low)
                {
                    if (mid + 1 >= this.CurveSnapshots.Count)
                    {
                        return this.CurveSnapshots[mid].point;
                    }
                    else
                    {
                        DistanceTime a = this.CurveSnapshots[mid];
                        DistanceTime b = this.CurveSnapshots[mid + 1];
                        // interpolate betweeen the two snapshots
                        return this.Lerp(a.point, b.point, (d - a.distance) / (b.distance - a.distance));
                    }
                }
                if (this.CurveSnapshots[mid].distance > d)
                {
                    high = mid;
                }
                else
                {
                    low = mid;
                }
            }
            return Vector2.Zero;
        }
    }
}
