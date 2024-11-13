namespace Cos.GameEngine.Sprite.SupportingClasses.Metadata
{
    /// <summary>
    /// Contains sprite metadata.
    /// </summary>
    public class WeaponMetadata
    {
        public WeaponMetadata() { }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// If the sprite has an image, these are the paths to the bitmaps (be default, they are used at random)..
        /// </summary>
        public string[] SpritePaths { get; set; } = new string[0];

        public string? SoundPath { get; set; }
        public float SoundVolume { get; set; } = 1.0f;

        /// <summary>
        /// The variance in degrees that the loaded munition will use for an initial heading angle.
        /// </summary>
        public float AngleVarianceDegrees { get; set; } = 0;
        /// <summary>
        /// The variance expressed in decimal percentage that determines the loaded munitions initial velocity.
        /// </summary>
        public float SpeedVariancePercent { get; set; } = 0;
        /// <summary>
        /// The distance from the total canvas that the munition will be allowed to travel before it is deleted.
        /// </summary>
        //public float MunitionSceneDistanceLimit;
        public float Speed { get; set; } = 25;

        public int FireDelayMilliseconds { get; set; } = 100;
        public int Damage { get; set; } = 1;

        public bool ExplodesOnImpact { get; set; } = false;
    }
}
