using Cos.Library.Mathematics;

namespace Cos.Library.ExtensionMethods
{
    public static class CosFloatExtensions
    {
        /// <summary>
        /// Converts the given degrees to radians.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToRadians(this float value) => CosMath.DegToRad(value);

        /// <summary>
        /// Converts the given radian to degrees.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToDegrees(this float value) => CosMath.RadToDeg(value);

        /// <summary>
        /// Multiplies the float by -1, inverting its sign, if the boolean is true.
        /// </summary>
        public static float Invert(this float value, bool shouldInvert) => shouldInvert ? value * -1 : value;

        /// <summary>
        /// Multiplies the float by -1, inverting its sign.
        /// </summary>
        public static float Invert(this float value) => value * -1;

        /// <summary>
        /// Returns whether the value is near to zero.
        /// </summary>
        public static bool IsNearZero(this float value)
            => CosMath.IsNearZero(value);

        /// <summary>
        /// Returns whether the value is near to zero.
        /// </summary>
        public static bool IsNearZero(this float? value)
            => value == null ? true : CosMath.IsNearZero((float)value);

        /// <summary>
        /// Degrees 0-360 -> 0 to 180 (right) and 0 to -180 (left).
        /// </summary>
        public static float NormalizeDegrees(this float value)
        {
            return (value + 180) % 360 - 180;
        }

        /// <summary>
        /// Degrees 0-Infinite -> 0 to 360
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float DenormalizeDegrees(this float value)
        {
            return ((dynamic)value + 360) % 360;
        }

        public static bool IsNotBetween(this float value, float minValue, float maxValue)
        {
            return !value.IsBetween(minValue, maxValue);
        }

        public static bool IsNotBetween(this float? value, float minValue, float maxValue)
        {
            return !value.IsBetween(minValue, maxValue);
        }

        public static bool IsBetween(this float value, float minValue, float maxValue)
        {
            if (minValue > maxValue)
            {
                return value >= maxValue && value <= minValue;
            }
            return value >= minValue && value <= maxValue;
        }

        public static bool IsBetween(this float? value, float minValue, float maxValue)
        {
            if (minValue > maxValue)
            {
                return value >= maxValue && value <= minValue;
            }
            return value >= minValue && value <= maxValue;
        }

        /// <summary>
        /// Clips a value to a min/max value.
        /// </summary>
        public static float Clamp(this float value, float minValue, float maxValue)
        {
            if (value > maxValue) return maxValue;
            else if (value < minValue) return minValue;
            else return value;
        }

        /// <summary>
        /// Clips a value to a max value.
        /// </summary>
        public static float Clamp(this float value, float maxValue)
        {
            if (value > maxValue) return maxValue;
            else return value;
        }

        /// <summary>
        /// Take a value divides it by two and makes it negative if it over a given threshold
        /// </summary>
        public static float SplitToSigned(this float value, float halfwayPoint)
        {
            value /= 2.0f;

            if (value > halfwayPoint)
            {
                value *= -1;
            }

            return value;
        }
    }
}
