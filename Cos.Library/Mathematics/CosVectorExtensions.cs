using System.Runtime.CompilerServices;

namespace Cos.Library.Mathematics
{
    public static class CosVectorExtensions
    {

        /// <summary>
        /// Rotate a point around another point by a certain angle.
        /// </summary>
        /// <param name="pointToRotate"></param>
        /// <param name="centerPoint"></param>
        /// <param name="angleRadians"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CosVector RotateAroundPoint(this CosVector pointToRotate, CosVector centerPoint, float angleRadians)
        {
            var cosTheta = (float)Math.Cos(angleRadians);
            var sinTheta = (float)Math.Sin(angleRadians);
            var x = cosTheta * (pointToRotate.X - centerPoint.X) - sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X;
            var y = sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y;
            return new CosVector(x, y);
        }

        /// <summary>
        /// Reflect a vector off a surface.Useful for light reflections, bouncing effects, etc.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CosVector Reflect(this CosVector vector, CosVector normal)
        {
            var dotProduct = vector.Dot(normal);
            return new CosVector(vector.X - 2 * dotProduct * normal.X, vector.Y - 2 * dotProduct * normal.Y);
        }

        /// <summary>
        /// Returns a new vector which has been rotated by the given radians.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="radians"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static CosVector RotatedBy(this CosVector vector, float radians)
        {
            float cosTheta = (float)Math.Cos(radians);
            float sinTheta = (float)Math.Sin(radians);

            return new CosVector(
                vector.X * cosTheta - vector.Y * sinTheta,
                vector.X * sinTheta + vector.Y * cosTheta
            );
        }

        /// <summary>
        /// Calculate the angle between two points in unsigned degrees.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedDegrees(this CosVector from, CosVector to)
        {
            var radians = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
            return (CosMath.RadToDeg(radians) + 360.0f) % 360.0f;
        }

        /// <summary>
        /// Calculate the angle between two points in signed degrees.
        /// </summary>
        /// <param name="from">The object from which the calculation is based.</param>
        /// <param name="to">The point to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 1-180 to -1-180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInSignedDegrees(this CosVector from, CosVector to)
        {
            var angle = from.AngleToInUnsignedDegrees(to);
            if (angle > 180)
            {
                angle -= 180;
                angle = 180 - angle;
                angle *= -1;
            }

            return -angle;
        }

        /// <summary>
        /// Calculate the angle between two points in signed radians.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInSignedRadians(this CosVector from, CosVector to)
            => (float)Math.Atan2(to.Y - from.Y, to.X - from.X);

        /// <summary>
        /// Calculate the angle between two points in unsigned radians.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedRadians(this CosVector from, CosVector to)
        {
            var angle = (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
            if (angle < 0)
            {
                angle += 2 * (float)Math.PI; // Convert negative angles to positive by adding 2π
            }
            return angle;
        }
    }
}
