namespace Cos.Library
{
    public class CosRandom
    {
        public static Random Generator = new();

        public static float NextFloat() => (float)Generator.NextDouble();

        /// <summary>
        /// Returns the given value with a given variance added or subtracted (at random).
        /// </summary>
        /// <param name="value"></param>
        /// <param name="variancePercentWholeNumber"></param>
        /// <returns></returns>
        public static float Variance(float value, float variancePercentDecimal)
            => value + RandomSign(value * variancePercentDecimal);

        /// <summary>
        /// 50/50 chance to return a positive/negative of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float RandomSign(float value)
            => FlipCoin() ? value : -value;

        public static T OneOf<T>(T one, T two)
            => OneOf([one, two]);

        public static T OneOf<T>(T one, T two, T three)
            => OneOf([one, two, three]);

        public static T OneOf<T>(T one, T two, T three, T four)
            => OneOf([one, two, three, four]);

        public static T OneOf<T>(T[] values)
            => values[Between(0, values.Length - 1)];

        public static bool ChanceIn(int chanceIn, int outOf)
            => Generator.Next(1, outOf + 1) <= chanceIn;

        public static bool PercentChance(int percentageWholeNumber)
            => ((float)Generator.NextDouble() * 100) <= percentageWholeNumber;

        public static bool FlipCoin() => (Generator.Next(2) == 0);

        /// <summary>
        /// Returns either -1 or 1.
        /// </summary>
        /// <returns></returns>
        public static float PositiveOrNegative() => (Generator.Next(2) == 0 ? 1 : -1);

        public static float Between(float minValue, float maxValue)
            => minValue + (maxValue - minValue) * (float)Generator.NextDouble();

        public static int Between(int minValue, int maxValue)
            => Generator.Next(minValue, maxValue + 1);
    }
}