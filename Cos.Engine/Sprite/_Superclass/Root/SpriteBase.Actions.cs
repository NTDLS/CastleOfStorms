namespace Cos.Engine.Sprite._Superclass._Root
{
    public partial class SpriteBase
    {
        public virtual void Explode()
        {
            IsDeadOrExploded = true;
            _isVisible = false;

            OnExplode?.Invoke(this);
        }

        public virtual void HitExplosion()
        {
            _engine.Sprites.Animations.AddRandomHitExplosionAt(this);
        }
    }
}

