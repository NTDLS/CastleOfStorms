using static Cos.Library.CosConstants;

namespace Cos.Engine.Sprite.Metadata
{
    internal class SpriteAnimationMetadata : BaseMetaData
    {
        public SpriteAnimationMetadata() { }

        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public float FramesPerSecond { get; set; }

        public CosAnimationPlayMode PlayMode { get; set; }
    }
}
