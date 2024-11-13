namespace Cos.Engine.Sprite._Superclass
{
    /// <summary>
    /// The ship base is a ship object that moves, can be hit, explodes and can be the subject of locking weapons.
    /// </summary>
    public class SpriteInteractiveShipBase : SpriteInteractiveBase
    {
        public SpriteInteractiveShipBase(EngineCore engine, string imagePath)
            : base(engine, imagePath)
        {
            _engine = engine;
        }

        public SpriteInteractiveShipBase(EngineCore engine, SharpDX.Direct2D1.Bitmap bitmap)
            : base(engine, bitmap)
        {
            _engine = engine;
        }
    }
}
