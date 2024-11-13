namespace Cos.Library
{
    public static class CosSequenceGenerator
    {
        private static uint _nextSequentialId = 0;
        /// <summary>
        /// Used to give all loaded sprites a unique ID. Very handy for debugging.
        /// </summary>
        public static uint Next() => Interlocked.Increment(ref _nextSequentialId);
    }
}
