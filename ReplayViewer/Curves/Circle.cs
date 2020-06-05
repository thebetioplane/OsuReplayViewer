using System;
using Vector2 = OpenTK.Vector2;

namespace ReplayViewer.Curves
{
    public class Circle : Curve
    {
        public Circle() : base(BMAPI.v1.SliderType.PSpline)
        {
        }

        protected override Vector2 Interpolate(float t)
        {
            if (this.Points.Count == 3)
            {
                float area = this.Area(this.Points[0], this.Points[1], this.Points[2]);
                if (Math.Abs(area) <= 0.001f)
                {
                    // this means that the points are very close to (or exactly) collinear and our
                    // circle drawing algorithm will be unstable
                    // so we just treat it like a straight line
                    return Vector2.Lerp(this.Points[0], this.Points[2], t);
                }
                // essentially we are just drawing a circle between two angles
                Vector2 center = this.CircleCenter(this.Points[0], this.Points[1], this.Points[2]);
                float radius = this.Distance(this.Points[0], center);
                // arctangent gives us the angles around the circle that the point is at
                float start = this.Atan2(this.Points[0] - center);
                float end = this.Atan2(this.Points[2] - center);
                float twopi = (float)(2 * Math.PI);
                // determine which direction the circle should be drawn
                // we want it so that the curve passes throught all the points
                // area > 0 is clockwise
                // the reason why this is backwards from normal is because our Y axis points down,
                // so we are also experiencing a vertical flip
                if (area > 0)
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
                // t is now the angle around the circle to draw
                return new Vector2((float)(Math.Cos(t) * radius), (float)(Math.Sin(t) * radius)) + center;
            }
            else
            {
                return Vector2.Zero;
            }
        }

        private Vector2 CircleCenter(Vector2 A, Vector2 B, Vector2 C)
        {
            // finds the point of a circle from three points on it's edges
            float yDelta_a = B.Y - A.Y;
            float xDelta_a = B.X - A.X;
            float yDelta_b = C.Y - B.Y;
            float xDelta_b = C.X - B.X;
            Vector2 center = new Vector2();
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

        private float Area(Vector2 a, Vector2 b, Vector2 c)
        {
            // this is related to the cross product
            // it's also the determinate of a 3x3 matrix as follows
            // x0 x1 x2
            // y0 y1 y2
            // 1  1  1
            // where your 3 vectors are represented by (x0, y0), (x1, y1), (x2, y2)
            return a.X * b.Y - b.X * a.Y + b.X * c.Y - c.X * b.Y + c.X * a.Y - a.X * c.Y;
        }
    }
}
