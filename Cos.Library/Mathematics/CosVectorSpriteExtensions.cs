using Cos.Library.ExtensionMethods;
using Cos.Library.Sprite;
using System.Runtime.CompilerServices;

namespace Cos.Library.Mathematics
{
    public static class CosVectorSpriteExtensions
    {
        /// <summary>
        /// Calculates the angle of one sprite location to another sprite location in signed degrees.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 1-180 to -1-180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInSignedDegrees(this ISprite fromSprite, ISprite toSprite)
        {
            var angle = fromSprite.Location.AngleToInUnsignedDegrees(toSprite.Location);
            if (angle > 180)
            {
                angle -= 180;
                angle = 180 - angle;
                angle *= -1;
            }

            return angle;
        }

        /// <summary>
        /// Calculates the angle of one sprite location to another sprite location in unsigned degrees.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedDegrees(this ISprite fromSprite, ISprite toSprite)
            => fromSprite.Location.AngleToInUnsignedDegrees(toSprite.Location);

        /// <summary>
        /// Calculates the angle of one sprite location to another sprite location in signed radians.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0 though +π and -π though 0.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInSignedRadians(this ISprite fromSprite, ISprite toSprite)
            => fromSprite.Location.AngleToInSignedRadians(toSprite.Location);

        /// <summary>
        /// Calculates the angle of one sprite location to another sprite location in unsigned radians.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0 though 2*π.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedRadians(this ISprite fromSprite, ISprite toSprite)
            => fromSprite.Location.AngleToInUnsignedRadians(toSprite.Location);

        /// <summary>
        /// Calculates the angle of one sprite location to sprite location from 1-180 to -1-180.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="toLocation">The point to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-180 to -180-0.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInSignedDegrees(this ISprite fromSprite, CosVector toLocation)
        {
            var angle = fromSprite.Location.AngleToInUnsignedDegrees(toLocation);
            if (angle > 180)
            {
                angle -= 180;
                angle = 180 - angle;
                angle *= -1;
            }

            return -angle;
        }

        /// <summary>
        /// Calculates the angle of one location to a sprite location.
        /// </summary>
        /// <param name="fromLocation">The object from which the calculation is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedDegrees(this CosVector fromLocation, ISprite toSprite)
            => fromLocation.AngleToInUnsignedDegrees(toSprite.Location);

        /// <summary>
        /// Calculates the angle of one sprite location to another location from 0 - 360.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="toLocation">The point to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedDegrees(this ISprite fromSprite, CosVector toLocation)
            => fromSprite.Location.AngleToInUnsignedDegrees(toLocation);

        ///
        /// <summary>
        /// Returns true if the sprite is pointing AT another sprite, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="atSprite">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing away from the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointingAway(this ISprite fromSprite, ISprite atSprite, float toleranceDegrees)
        {
            var deltaAngle = Math.Abs(fromSprite.HeadingAngleToInUnsignedDegrees(atSprite));
            return deltaAngle < 180 + toleranceDegrees && deltaAngle > 180 - toleranceDegrees;
        }

        /// <summary>
        /// Returns true if the sprite is pointing AWAY another sprite, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="atSprite">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <param name="maxDistance"></param>
        /// <returns>True if the object is pointing away from the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointingAway(this ISprite fromSprite, ISprite atSprite, float toleranceDegrees, float maxDistance)
            => fromSprite.IsPointingAway(atSprite, toleranceDegrees) && fromSprite.DistanceTo(atSprite) <= maxDistance;

        /// <summary>
        /// Returns true if the sprite is pointing AT another sprite, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="atSprite">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing at the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointingAt(this ISprite fromSprite, ISprite atSprite, float toleranceDegrees)
        {
            var deltaAngle = Math.Abs(fromSprite.HeadingAngleToInSignedDegrees(atSprite));
            return deltaAngle < toleranceDegrees || deltaAngle > 360 - toleranceDegrees;
        }

        /// <summary>
        /// Returns true if the sprite is pointing AT another sprite, taking into account the tolerance in degrees and max distance.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="atSprite">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees">The angle in degrees to consider the object to pointing at the other.</param>
        /// <param name="maxDistance">The distance in which the object to pointing at the other.</param>
        /// <returns>True if the object is pointing at the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointingAt(this ISprite fromSprite, ISprite atSprite, float toleranceDegrees, float maxDistance)
        {
            var deltaAngle = Math.Abs(fromSprite.HeadingAngleToInUnsignedDegrees(atSprite));
            if (deltaAngle < toleranceDegrees || deltaAngle > 360 - toleranceDegrees)
            {
                return fromSprite.DistanceTo(atSprite) <= maxDistance;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the sprite is pointing AT another sprite, taking into account the tolerance in degrees and min/max distance.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="atSprite">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees">The angle in degrees to consider the object to pointing at the other.</param>
        /// /// <param name="minDistance">The distance in which the object to pointing at the other.</param>
        /// <param name="maxDistance">The distance in which the object to pointing at the other.</param>
        /// <returns>True if the object is pointing at the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointingAt(this ISprite fromSprite, ISprite atSprite, float toleranceDegrees, float minDistance, float maxDistance)
        {
            var deltaAngle = Math.Abs(fromSprite.HeadingAngleToInUnsignedDegrees(atSprite));
            if (deltaAngle < toleranceDegrees || deltaAngle > 360 - toleranceDegrees)
            {
                return fromSprite.DistanceTo(atSprite).IsBetween(minDistance, maxDistance);
            }

            return false;
        }

        /// <summary>
        /// Returns the angle which would be required to rotate a sprite to be pointing at another sprite.
        /// positive figures indicate right (starboard) side and negative indicate left-hand (port) side of the object.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 180--180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HeadingAngleToInSignedDegrees(this ISprite fromSprite, ISprite toSprite)
            => fromSprite.HeadingAngleToInSignedDegrees(toSprite.Location);

        /// <summary>
        /// Returns the angle which would be required to rotate a sprite to to be pointing at a given location.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="toLocation">The location to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 180--180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HeadingAngleToInSignedDegrees(this ISprite fromSprite, CosVector toLocation)
            => CosMath.RadToDeg(HeadingAngleToInSignedRadians(fromSprite, toLocation)).NormalizeDegrees();

        /// <summary>
        /// Returns the angle which would be required to rotate a sprite to be pointing at another sprite.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HeadingAngleToInUnsignedDegrees(this ISprite fromSprite, ISprite toSprite)
            => fromSprite.HeadingAngleToInUnsignedDegrees(toSprite.Location);

        /// <summary>
        /// Returns the angle which would be required to rotate a sprite to be pointing at a given location.
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="toLocation">The location to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HeadingAngleToInUnsignedDegrees(this ISprite fromSprite, CosVector toLocation)
            => CosMath.RadToDeg(HeadingAngleToInSignedRadians(fromSprite, toLocation)).DenormalizeDegrees();

        /// <summary>
        /// Calculate the angle (in signed radians [+π,-π] ) from one sprite current orientation to another position.
        /// This is the core of all other variants of this function.
        /// </summary>
        /// <param name="fromSprite"></param>
        /// <param name="toLocation"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HeadingAngleToInSignedRadians(this ISprite fromSprite, CosVector toLocation)
        {
            // Subtracts the fromSprite's current location from toLocation, resulting
            // in a vector that points from the sprite to the target location.
            //Normalizes vector to remove the influence of distance on the angle calculation, focusing purely on the direction.
            var toTarget = (toLocation - fromSprite.Location).Normalize();

            //  Calculates the dot product of the sprite's current orientation vector and the normalized target vector.
            //  The dot product is a scalar that reflects the magnitude of the projection of one vector onto another and is calculated as:
            //The result, dot, is used to determine the cosine of the angle between the two vectors because when both vectors are
            //unit vectors(normalized), the dot product equals the cosine of the angle between them.
            var dot = fromSprite.Orientation.Dot(toTarget);

            //Determinant (Pseudo Cross Product in 2D) calculates the determinant of a 2x2 matrix formed by the components of the
            //  fromSprite.Orientation and toTarget vectors. This value is equivalent to the z-component of the cross product of
            //  the two vectors if they were extended into 3D by setting their z-components to 0.
            //The determinant measures the area of the parallelogram formed by the two vectors and its sign indicates the
            //  direction of the smallest rotation from the first vector to the second. A positive sign indicates a counter-clockwise
            //  rotation, and a negative sign indicates a clockwise rotation.*/
            var determinant = fromSprite.Orientation.X * toTarget.Y - fromSprite.Orientation.Y * toTarget.X;

            //Angle Calculation: Uses the Math.Atan2 function, which returns the angle in radians formed by the radius of a circle
            //  and the x-axis, to compute the angle between the orientation of the sprite and the direction to the target.
            //  The use of Math.Atan2(det, dot) effectively calculates this angle based on the sine (det) and cosine (dot) of
            //  the angle, which can range from -π to π (-180 degrees to 180 degrees).
            //This angle indicates how much fromSprite needs to rotate, and in which direction, to directly face toLocation.

            return (float)Math.Atan2(determinant, dot);
        }

        /// <summary>
        /// Returns the distance from one sprite to another
        /// </summary>
        /// <param name="fromSprite">The object from which the calculation is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated distance from one object to the other.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this ISprite fromSprite, ISprite toSprite)
            => fromSprite.Location.DistanceTo(toSprite.Location);
    }
}
