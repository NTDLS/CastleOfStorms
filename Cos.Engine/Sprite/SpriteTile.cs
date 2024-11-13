using Cos.Engine.Sprite._Superclass._Root;

namespace Cos.Engine.Sprite
{
    /// <summary>
    /// These are just minimal non-collidable, non interactive, generic bitmap sprites.
    /// </summary>
    public class SpriteTile : SpriteBase
    {
        public SpriteTile(EngineCore engine, string imagePath)
            : base(engine)
        {
            SetImageAndLoadMetadata(imagePath);
        }

        public SpriteTile(EngineCore engine, SharpDX.Direct2D1.Bitmap bitmap)
            : base(engine)
        {
            SetImage(bitmap);
        }

        public SpriteTile(EngineCore engine)
            : base(engine)
        {
        }

        private void SetImageAndLoadMetadata(string spriteImagePath)
        {
            SetImage(spriteImagePath);
        }
    }
}
