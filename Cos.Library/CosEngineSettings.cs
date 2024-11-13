using System.Drawing;

namespace Cos.Library
{
    /// <summary>
    /// This contains all of the engine settings.
    /// </summary>
    public class CosEngineSettings
    {
        public int GraphicsAdapterId { get; set; } = 0;
        public int WorldClockThreads { get; set; } = 10;

        public Size Resolution { get; set; }
        public bool PreCacheAllAssets { get; set; } = true;

        public bool PlayMusic { get; set; } = true;

        public float WorldTicksPerSecond { get; set; } = 120; //MillisecondPerEpochs = 1000 / WorldTicksPerSecond

        public float MillisecondPerEpoch => 1000f / WorldTicksPerSecond;

        public int MaxHullHealth { get; set; } = 100000;
        public int MaxShieldHealth { get; set; } = 100000;

        /// <summary>
        /// After the frame has been generated, if it takes less time than the framerate - yield the time instead of rending the next frame too early.
        /// this is really just an effort to keep epoch time reasonably close to frame time.
        /// </summary>
        public bool YieldRemainingFrameTime { get; set; } = false;
        public bool VerticalSync { get; set; } = false;
        public bool AntiAliasing { get; set; } = true;

        /// <summary>
        /// Ensure that the average framerate is within sane limits. This is especially important for vSync since we want to make sure a frame is available for the GPU.
        /// </summary>
        public bool FineTuneFramerate { get; set; } = true;
        public float TargetFrameRate { get; set; } = 70;
    }
}
