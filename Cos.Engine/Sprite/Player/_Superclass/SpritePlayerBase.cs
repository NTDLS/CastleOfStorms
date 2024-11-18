using Cos.Engine.Sprite._Superclass;
using Cos.Library.Mathematics;

namespace Cos.Engine.Sprite.Player._Superclass
{
    /// <summary>
    /// The player base is a sub-class of the ship base. It is only used by the Player and as a model for menu selections.
    /// </summary>
    public class SpritePlayerBase : SpriteInteractiveBase
    {
        public int MaxHullHealth { get; set; }

        public SpritePlayerBase(EngineCore engine, string imagePath)
            : base(engine, imagePath)
        {
            Orientation = new CosVector(0);
            RecalculateMovementVector();

            CenterInUniverse();
        }
    }
}
