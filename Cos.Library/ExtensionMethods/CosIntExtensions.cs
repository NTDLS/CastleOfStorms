using Cos.Library.Mathematics;

namespace Cos.Library.ExtensionMethods
{
    public static class CosIntExtensions
    {
        /// <summary>
        /// Converts the given degrees to radians.
        /// </summary>
        public static float ToRadians(this int value) => CosMath.DegToRad(value);

        /// <summary>
        /// Converts the given radian to degrees.
        /// </summary>
        public static float ToDegrees(this int value) => CosMath.RadToDeg(value);

        /// <summary>
        /// Multiplies the integer by -1, inverting its sign, if the boolean is true.
        /// </summary>
        public static int Invert(this int value, bool shouldInvert) => shouldInvert ? value * -1 : value;

        /// <summary>
        /// Multiplies the integer by -1, inverting its sign.
        /// </summary>
        public static int Invert(this int value) => value * -1;

        /// <summary>
        /// Clips a value to a min/max value.
        /// </summary>
        public static int Clamp(this int value, int minValue, int maxValue)
        {
            if (value > maxValue) return maxValue;
            else if (value < minValue) return minValue;
            else return value;
        }

        /// <summary>
        /// Clips a value to a max value.
        /// </summary>
        public static int Clamp(this int value, int maxValue)
        {
            if (value > maxValue) return maxValue;
            else return value;
        }
    }
}
