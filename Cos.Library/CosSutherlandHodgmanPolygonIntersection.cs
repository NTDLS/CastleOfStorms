using System.Drawing;

namespace Cos.Library
{
    /// <summary>
    /// To get the exact overlapping area of two rotated rectangles, we need to calculate the intersection
    ///   polygon of the two sets of corners.
    /// 
    /// This is more complex than just finding the overlap of AABBs (SiSeparatingAxisTheorem). We are going to roll a
    /// poor-mans clipping algorithm (Sutherland-Hodgman), to find the intersection polygon of the two shapes.
    ///
    /// This polygon would represent the exact overlapping area.
    /// </summary>
    public class CosSutherlandHodgmanPolygonIntersection
    {
        /// <summary>
        /// The intersection of two rotated rectangles is a convex polygon, not necessarily a rectangle.
        /// So we are going to approximate the intersection as a rectangle.
        /// </summary>
        public static RectangleF GetIntersectionBoundingBox(RectangleF bounds1, float angleRadians1, RectangleF bounds2, float angleRadians2)
        {
            var rectangle1Corners = CosSeparatingAxisTheorem.GetRotatedRectangleCorners(bounds1, angleRadians1).ToArray();
            var rectangle2Corners = CosSeparatingAxisTheorem.GetRotatedRectangleCorners(bounds2, angleRadians2).ToArray();
            return GetIntersectionBoundingBox(rectangle1Corners, rectangle2Corners);
        }

        public static RectangleF GetIntersectionBoundingBox(PointF[] poly1, PointF[] poly2)
        {
            var intersectionPolygon = GetIntersectedPolygon(poly1, poly2);

            if (intersectionPolygon.Count() == 0)
            {
                return RectangleF.Empty; // No intersection
            }

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            // Iterate through the points of the intersection polygon to find the min and max X and Y
            foreach (PointF point in intersectionPolygon)
            {
                if (point.X < minX) minX = point.X;
                if (point.X > maxX) maxX = point.X;
                if (point.Y < minY) minY = point.Y;
                if (point.Y > maxY) maxY = point.Y;
            }

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// This returns the real polygon of the intersection, not some dodgy ass rectangle such as GetIntersectionBoundingBox()
        /// </summary>
        /// <param name="bounds1"></param>
        /// <param name="angleRadians1"></param>
        /// <param name="bounds2"></param>
        /// <param name="angleRadians2"></param>
        /// <returns></returns>
        public static PointF[] GetIntersectedPolygon(RectangleF bounds1, float angleRadians1, RectangleF bounds2, float angleRadians2)
        {
            var rectangle1Corners = CosSeparatingAxisTheorem.GetRotatedRectangleCorners(bounds1, angleRadians1).ToArray();
            var rectangle2Corners = CosSeparatingAxisTheorem.GetRotatedRectangleCorners(bounds2, angleRadians2).ToArray();

            return GetIntersectedPolygon(rectangle1Corners, rectangle2Corners);
        }

        public static PointF? LineIntersection(PointF p1, PointF p2, PointF p3, PointF p4)
        {
            float denominator = (p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y);
            if (denominator == 0) return null;

            float ua = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / denominator;
            float ub = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / denominator;

            if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
            {
                return new PointF(p1.X + ua * (p2.X - p1.X), p1.Y + ua * (p2.Y - p1.Y));
            }
            return null;
        }

        public static PointF[] ClipPolygon(PointF[] polygon, PointF clipStart, PointF clipEnd)
        {
            var outputList = new List<PointF>();
            PointF p1 = polygon[polygon.Count() - 1];

            foreach (PointF p2 in polygon)
            {
                PointF? intersection = LineIntersection(clipStart, clipEnd, p1, p2);
                if (IsInside(p2, clipStart, clipEnd))
                {
                    if (!IsInside(p1, clipStart, clipEnd))
                    {
                        if (intersection != null) outputList.Add(intersection.Value);
                    }
                    outputList.Add(p2);
                }
                else if (IsInside(p1, clipStart, clipEnd))
                {
                    if (intersection != null) outputList.Add(intersection.Value);
                }
                p1 = p2;
            }

            return outputList.ToArray();
        }

        public static bool IsInside(PointF test, PointF clipStart, PointF clipEnd)
        {
            return (clipEnd.X - clipStart.X) * (test.Y - clipStart.Y) > (clipEnd.Y - clipStart.Y) * (test.X - clipStart.X);
        }

        public static PointF[] GetIntersectedPolygon(PointF[] poly1, PointF[] poly2)
        {
            PointF[] outputList = poly1;

            for (int i = 0; i < poly2.Count(); i++)
            {
                PointF clipStart = poly2[i];
                PointF clipEnd = poly2[(i + 1) % poly2.Count()];
                outputList = ClipPolygon(outputList, clipStart, clipEnd);
                if (outputList.Count() == 0) break;
            }

            return outputList;
        }
    }
}
