namespace Cos.GameEngine.Sprite.SupportingClasses.Metadata
{
    /// <summary>
    /// Contains sprite metadata.
    /// </summary>
    public class InteractiveSpriteMetadata
    {
        public InteractiveSpriteMetadata() { }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public float Speed { get; set; } = 1f;
        public float MaxThrottle { get; set; } = 1.0f;
        public float Throttle { get; set; } = 1.0f;
        public int Hull { get; set; } = 0;
        public int Bounty { get; set; } = 0;
    }
}
