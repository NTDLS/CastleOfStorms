namespace Cos.Library.Mathematics
{
    public static class CosMathHelpers
    {
        /// <summary>
        /// Returns a unique value for a given pair of numbers regardless of their order.
        /// </summary>
        /// <param name="k1"></param>
        /// <param name="k2"></param>
        /// <returns></returns>
        public static uint CantorPair(uint k1, uint k2)
        {
            var minK = Math.Min(k1, k2);
            var maxK = Math.Max(k1, k2);
            return ((minK + maxK) * (minK + maxK + 1) / 2) + maxK;
        }
    }
}
