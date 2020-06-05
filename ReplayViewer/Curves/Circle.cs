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
                if (Math.Abs(area) <= 0.01f)
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
        /// <summary>
        /// finds center of circle from three points on it's edge
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private Vector2 CircleCenter(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            // form is (x^2 + y^2) + Bx + Cy + D = 0
            // see: http://ambrsoft.com/TrigoCalc/Circle3D.htm
            // mirror: https://i.imgur.com/1OACqKx.png
            float w0 = p0.LengthSquared;
            float w1 = p1.LengthSquared;
            float w2 = p2.LengthSquared;
            float A = Det31(p0.X, p0.Y, p1.X, p1.Y, p2.X, p2.Y);
            float B = -Det31(w0, p0.Y, w1, p1.Y, w2, p2.Y);
            float C = Det31(w0, p0.X, w1, p1.X, w2, p2.X);
            float scale = -1 / (2.0f * A);
            return new Vector2(B * scale, C * scale);
        }
        /// <summary>
        /// Special case of 3x3 determinate when the last column is ones
        /// v0 u0 1
        /// v1 u1 1
        /// v2 u2 1
        /// </summary>
        /// <param name="v0">row 1, col 1</param>
        /// <param name="u0">row 1, col 2</param>
        /// <param name="v1">row 2, col 1</param>
        /// <param name="u1">row 2, col 2</param>
        /// <param name="v2">row 3, col 1</param>
        /// <param name="u2">row 3, col 2</param>
        /// <returns></returns>
        private float Det31(float v0, float u0, float v1, float u1, float v2, float u2)
        {
			return v0 * (u1 - u2) - u0 * (v1 - v2) + v1 * u2 - v2 * u1;
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
