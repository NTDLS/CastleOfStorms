using static Cos.Library.CosConstants;

namespace Cos.Engine.Sprite.SupportingClasses.Metadata
{
    internal class SpriteAnimationMetadata
    {
        public SpriteAnimationMetadata() { }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public float Speed { get; set; } = 1f;
        public float MaxThrottle { get; set; } = 0f;
        public float Throttle { get; set; } = 1f;

        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public float FramesPerSecond { get; set; }

        public CosAnimationPlayMode PlayMode { get; set; }
    }
}
