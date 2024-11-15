using Cos.Library.Mathematics;
using Cos.Library.Sprite;

namespace Cos.Engine.Sprite._Superclass._Root
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class SpriteBase : ISprite
    {
        /// <summary>
        /// Sets the movement vector in the direction of the sprite taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVector() => MovementVector = MakeMovementVector();

        /// <summary>
        /// Sets the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVector(float angleInRadians) => MovementVector = MakeMovementVector(angleInRadians);

        /// <summary>
        /// Sets the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVector(CosVector angle) => MovementVector = MakeMovementVector(angle);

        /// <summary>
        /// Returns the movement vector in the direction of the sprite taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public CosVector MakeMovementVector() => Orientation * Speed;

        /// <summary>
        /// Returns the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public CosVector MakeMovementVector(float angleInRadians) => new CosVector(angleInRadians) * Speed;

        /// <summary>
        /// Returns the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public CosVector MakeMovementVector(CosVector angle) => angle.Normalize() * Speed;
    }
}
