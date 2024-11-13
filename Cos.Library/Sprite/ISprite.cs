using Cos.Library.Mathematics;

namespace Cos.Library.Sprite
{
    /// <summary>
    /// All sprites (or their base classes) must inherit this interface.
    /// </summary>
    public interface ISprite
    {
        /// <summary>
        /// The x,y, location of the center of the sprite in the universe.
        /// Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public CosVector Location { get; set; }

        /// <summary>
        /// Number or radians to rotate the sprite Orientation along its center at each call to ApplyMotion().
        /// Negative for counter-clockwise, positive for clockwise.
        /// </summary>
        //public float RotationSpeed { get; set; }

        /// <summary>
        /// The angle in which the sprite is pointing, note that this is NOT the travel angle.
        /// The travel angle is baked into the MovementVector. If you need the movement vector
        /// to follow this direction angle then call RecalculateMovementVector() after modifying
        /// the PointingAngle.
        /// </summary>
        public CosVector Orientation { get; set; }
    }
}
