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
            Throttle = 0;

            CenterInUniverse();
        }

        public string GetLoadoutHelpText()
        {
            string result = $"             Name : {Metadata.Name}\n";
            result += $"             Hull : {Metadata.Hull:n0}\n";
            result += $"            Speed : {Metadata.Speed:n1}\n";
            result += $"         Throttle : {Metadata.MaxThrottle:n1}\n";
            result += $"\n{Metadata.Description}";

            return result;
        }

        /// <summary>
        /// Resets ship state, health etc while keeping the existing class.
        /// </summary>
        public void Reset()
        {
            ReviveDeadOrExploded();

            //TODO: We should reload metadata and reapply it.
        }
    }
}
