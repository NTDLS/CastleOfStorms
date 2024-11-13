using System.Drawing;

namespace Cos.Library
{
    /// <summary>
    /// The Separating Axis Theorem (SAT) is a method used in computational geometry to determine if two convex shapes are intersecting (colliding).
    /// It's widely used in computer graphics, physics engines for simulations, and video game development for collision detection.
    /// The theorem can be applied to any pair of convex polygons or convex polyhedra in two or three dimensions, respectively.
    /// 
    /// Here is my poor-mans implementation.
    /// </summary>
    public class CosSeparatingAxisTheorem
    {
        /// <summary>
        /// Returns an approximation of the overlap area for two rotated rectangles.
        /// This is the axis-Aligned Bounding Box (AABB) of the overlap of the AABBs of the rotated rectangles,
        /// not the actual overlap of the rotated rectangles themselves.
        ///
        /// The AABB of the overlap will often be larger than the actual rotated overlap, especially when the overlap is
        /// small and the rectangles are at sharp angles, as is likely the case in your example. This discrepancy is due
        /// to the nature of AABBs, which are not rotation-aware. So, just an FYI hommie - this is expected. :/
        /// 
        /// </summary>
        /// <param name="bounds1"></param>
        /// <param name="angleRadians1"></param>
        /// <param name="bounds2"></param>
        /// <param name="angleRadians2"></param>
        /// <returns></returns>
        public static RectangleF GetOverlapRectangle(RectangleF bounds1, float angleRadians1, RectangleF bounds2, float angleRadians2)
        {
            var aabb1 = GetAxisAlignedBoundingBox(GetRotatedRectangleCorners(bounds1, angleRadians1));
            var aabb2 = GetAxisAlignedBoundingBox(GetRotatedRectangleCorners(bounds2, angleRadians2));

            // Calculate Overlap
            float overlapLeft = Math.Max(aabb1.Left, aabb2.Left);
            float overlapTop = Math.Max(aabb1.Top, aabb2.Top);
            float overlapRight = Math.Min(aabb1.Right, aabb2.Right);
            float overlapBottom = Math.Min(aabb1.Bottom, aabb2.Bottom);

            return new RectangleF(overlapLeft, overlapTop, overlapRight - overlapLeft, overlapBottom - overlapTop);
        }

        /// <summary>
        /// Determines if two (non-axis-aligned) rectangles interest using Separating Axis Theorem (SAT).
        /// This allows us to determine if a rotated rectangle intersects another rotated rectangle.
        /// </summary>
        /// <param name="bounds1"></param>
        /// <param name="angleRadians1"></param>
        /// <param name="bounds2"></param>
        /// <param name="angleRadians2"></param>
        /// <returns></returns>
        public static bool IntersectsRotated(RectangleF bounds1, float angleRadians1, RectangleF bounds2, float angleRadians2)
        {
            var corners1 = GetRotatedRectangleCorners(bounds1, angleRadians1);
            var corners2 = GetRotatedRectangleCorners(bounds2, angleRadians2);

            // For each rectangle, there are 2 axes to test (perpendicular to its 2 unique sides)
            for (int i = 0; i < 2; i++)
            {
                var axisA = Perpendicular(Normalize(Subtract(corners1[(i + 1) % 4], corners1[i])));
                var axisB = Perpendicular(Normalize(Subtract(corners2[(i + 1) % 4], corners2[i])));

                if (!Overlaps(corners1, corners2, axisA) || !Overlaps(corners1, corners2, axisB))
                {
                    return false; // No collision if there's no overlap on at least one axis
                }
            }

            return true; // Overlap on all tested axes, so rectangles intersect
        }

        /// <summary>
        /// Returns the points of a rotated rectangle.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="angleRadians"></param>
        /// <returns></returns>
        public static PointF[] GetRotatedRectangleCorners(RectangleF bounds, float angleRadians)
        {
            var center = new PointF(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
            var corners = new PointF[4];

            // Rectangle corners before rotation
            corners[0] = new PointF(bounds.Left, bounds.Top); // Top-left
            corners[1] = new PointF(bounds.Right, bounds.Top); // Top-right
            corners[2] = new PointF(bounds.Right, bounds.Bottom); // Bottom-right
            corners[3] = new PointF(bounds.Left, bounds.Bottom); // Bottom-left

            for (int i = 0; i < 4; i++)
            {
                // Translate corner to origin
                float x = corners[i].X - center.X;
                float y = corners[i].Y - center.Y;

                // Rotate and translate back
                corners[i] = new PointF(
                    (float)(x * Math.Cos(angleRadians) - y * Math.Sin(angleRadians) + center.X),
                    (float)(x * Math.Sin(angleRadians) + y * Math.Cos(angleRadians) + center.Y)
                );
            }

            return corners;
        }

        /// <summary>
        /// Subtract two points to get the vector.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static PointF Subtract(PointF a, PointF b)
            => new(a.X - b.X, a.Y - b.Y);

        /// <summary>
        /// Normalize a vector.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static PointF Normalize(PointF v)
        {
            float length = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
            return new PointF(v.X / length, v.Y / length);
        }

        /// <summary>
        /// Get a vector that is perpendicular to the given vector.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static PointF Perpendicular(PointF v) => new(-v.Y, v.X);

        /// <summary>
        /// Check if projections of two rectangles on a given axis overlap
        /// </summary>
        /// <param name="cornersA"></param>
        /// <param name="cornersB"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static bool Overlaps(PointF[] cornersA, PointF[] cornersB, PointF axis)
        {
            (float minA, float maxA) = Project(cornersA, axis);
            (float minB, float maxB) = Project(cornersB, axis);
            return minA <= maxB && minB <= maxA; // Check for overlap
        }

        /// <summary>
        /// The Project function projects the corners of a polygon (in this case, a rectangle
        ///   or any shape represented by its corners) onto an axis and returns the minimum and
        ///   maximum values of these projections. This is a key part of implementing the Separating
        ///   Axis Theorem (SAT) for collision detection.
        /// </summary>
        /// <param name="corners"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static (float, float) Project(PointF[] corners, PointF axis)
        {
            float min = Dot(corners[0], axis);
            float max = min;
            for (int i = 1; i < corners.Length; i++)
            {
                float p = Dot(corners[i], axis);
                if (p < min) min = p;
                else if (p > max) max = p;
            }
            return (min, max);
        }

        /// <summary>
        /// The Dot function calculates the dot product of two vectors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float Dot(PointF a, PointF axis)
            => a.X * axis.X + a.Y * axis.Y;

        public static RectangleF GetAxisAlignedBoundingBox(PointF[] corners)
        {
            float minX = corners.Min(c => c.X);
            float maxX = corners.Max(c => c.X);
            float minY = corners.Min(c => c.Y);
            float maxY = corners.Max(c => c.Y);

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
