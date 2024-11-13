using Cos.Engine.Sprite.Player;
using Cos.Engine.Sprite.Player._Superclass;
using Cos.Engine.TickController._Superclass;
using Cos.GameEngine.Persistent;
using Cos.Library;
using Cos.Library.Mathematics;
using System;
using System.Diagnostics;
using static Cos.Library.CosConstants;

namespace Cos.Engine.TickController.PlayerSpriteTickController
{
    /// <summary>
    /// This is the controller for the single local player.
    /// </summary>
    public class PlayerSpriteTickController : PlayerSpriteTickControllerBase<SpritePlayerBase>
    {
        private readonly EngineCore _engine;
        private readonly Stopwatch _inputDelay = new();

        public PlayerStats Stats { get; set; } = new(); //This should be saved.
        public SpritePlayerBase Sprite { get; set; }

        public PlayerSpriteTickController(EngineCore engine)
            : base(engine)
        {
            //This is where the player is created.
            Sprite = new SpriteDebugPlayer(engine) { Visible = false };
            engine.Sprites.Add(Sprite);
            _engine = engine;
            _inputDelay.Restart();
        }

        public void InstantiatePlayerClass(Type playerClassType)
        {
            //Remove the player from the sprite collection.
            Sprite.QueueForDelete();
            Sprite.Cleanup();

            Sprite = CosReflection.CreateInstanceFromType<SpritePlayerBase>(playerClassType, new object[] { _engine });
            Sprite.Visible = false;
            _engine.Sprites.Add(Sprite); //Add the player back to the sprite collection.
        }

        /// <summary>
        /// Moves the player taking into account any inputs and returns a X,Y describing the amount and direction of movement.
        /// </summary>
        /// <returns></returns>
        public override CosVector ExecuteWorldClockTick(float epoch)
        {
            if (Sprite.Visible)
            {
                if (Engine.Input.IsKeyPressed(CosPlayerKey.SpeedBoost))
                {
                }
            }

            /*
            Sprite.MovementVector = (Sprite.MakeMovementVector() * _forwardVelocity) //Forward / Reverse
                + (Sprite.MakeMovementVector(Sprite.Orientation.RadiansSigned + 90.ToRadians()) * _lateralVelocity);  //Lateral strafing.
            */
            var displacementVector = Sprite.MovementVector * epoch;
            /*
            //Scroll the background.
            Engine.Display.RenderWindowPosition += displacementVector;

            //Move the player in the direction of the background. This keeps the player visually in place, which is in the center screen.
            Sprite.Location += displacementVector;
            */
            return displacementVector;
        }

        public void ResetAndShow()
        {
            Sprite.Reset();

            Engine.Sprites.TextBlocks.PlayerStatsText.Visible = true;
            Sprite.Visible = true;
        }

        public void Show()
        {
            Engine.Sprites.TextBlocks.PlayerStatsText.Visible = true;
            Sprite.Visible = true;
        }

        public void Hide()
        {
            Engine.Sprites.TextBlocks.PlayerStatsText.Visible = false;
            Sprite.Visible = false;
        }
    }
}
