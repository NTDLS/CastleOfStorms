using System.Drawing;

namespace Cos.Library
{
    /// <summary>
    /// This contains all of the engine settings.
    /// </summary>
    public class CosEngineSettings
    {
        public int GraphicsAdapterId { get; set; } = 0;
        public int MunitionTraversalThreads { get; set; } = CosUtility.LesserOf(Environment.ProcessorCount * 2, 16);
        public int WorldClockThreads { get; set; } = 10;
        public bool EnableSpriteInterrogation { get; set; } = false;
        public bool HighlightNaturalBounds { get; set; } = false;
        public bool HighlightAllSprites { get; set; } = false;
        public bool HighlightCollisions { get; set; } = false;

        public Size Resolution { get; set; }

        public bool MultithreadedWorldClock { get; set; } = true;
        public bool PreCacheAllAssets { get; set; } = true;
        public bool FullScreen { get; set; }

        public bool PlayMusic { get; set; } = true;

        public bool EnableSpeedScaleFactoring { get; set; } = true;

        public float WorldTicksPerSecond { get; set; } = 120; //MillisecondPerEpochs = 1000 / WorldTicksPerSecond

        public float MillisecondPerEpoch => 1000f / WorldTicksPerSecond;

        public float EnemyVelocityRampUp { get; set; } = 0.0375f;
        public float EnemyVelocityRampDown { get; set; } = 0.0075f;

        public float PlayerVelocityRampUp { get; set; } = 0.0375f;
        public float PlayerVelocityRampDown { get; set; } = 0.0075f;

        public int MaxHullHealth { get; set; } = 100000;
        public int MaxShieldHealth { get; set; } = 100000;

        public float MaxPlayerBoostAmount { get; set; } = 10000;
        public float MaxPlayerRotationSpeedDegrees { get; set; } = 1.40f;

        public int InitialFrameStarCount { get; set; } = 100;
        public int DeltaFrameTargetStarCount { get; set; } = 200;

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
        public float MunitionSceneDistanceLimit { get; set; } = 2500; //The distance from the scene that a munition can travel before it is cleaned up.

        /// <summary>
        /// How much larger than the screen (NaturalScreenSize) that we will make the canvas so we can zoom-out. (2 = 2x larger than screen.).
        /// </summary>
        public float OverdrawScale { get; set; } = 1.5f;
    }
}
