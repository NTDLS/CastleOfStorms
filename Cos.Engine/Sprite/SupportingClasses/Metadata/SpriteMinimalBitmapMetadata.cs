namespace Cos.Engine.Sprite.SupportingClasses.Metadata
{
    internal class SpriteMinimalBitmapMetadata
    {
        public SpriteMinimalBitmapMetadata() { }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public float Speed { get; set; } = 1f;
        public float MaxThrottle { get; set; } = 0f;
        public float Throttle { get; set; } = 1f;
    }
}
