using Cos.Engine.Sprite;
using Cos.Engine.Sprite._Superclass._Root;
using Cos.Engine.Sprite.Player._Superclass;
using Cos.GameEngine.TickController.VectoredTickController.Collidable;
using Cos.GameEngine.TickController.VectoredTickController.Uncollidable;
using Cos.Library;
using Cos.Library.Mathematics;
using NTDLS.Helpers;
using SharpCompress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Cos.Library.CosConstants;

namespace Cos.Engine.Manager
{
    /// <summary>
    /// Contains the collection of all sprites and their factories. This class stringently controls access to the internal collection
    ///     only allowing insertion and deletions from it to occur within events so that it can be safely assumes that the collection
    ///     can be enumerated in the world clock controllers without fear of collection modification during enumeration.
    /// </summary>
    public class SpriteManager : IDisposable
    {
        public delegate void CollectionAccessor(SpriteBase[] sprites);
        public delegate T CollectionAccessorT<T>(SpriteBase[] sprites);

        private readonly EngineCore _engine;

        private readonly List<SpriteBase> _collection = new();

        #region Sprite Tick Controllerss.

        public AnimationSpriteTickController Animations { get; private set; }
        public InteractiveBitmapSpriteTickController InteractiveBitmaps { get; private set; }
        public MinimalBitmapSpriteTickController GenericBitmaps { get; private set; }
        public DebugSpriteTickController Debugs { get; private set; }
        public EnemySpriteTickController Enemies { get; private set; }
        public ParticleSpriteTickController Particles { get; private set; }
        public TextBlocksSpriteTickController TextBlocks { get; private set; }

        #endregion

        public SpriteManager(EngineCore engine)
        {
            _engine = engine;

            Animations = new AnimationSpriteTickController(_engine, this);
            Debugs = new DebugSpriteTickController(_engine, this);
            Enemies = new EnemySpriteTickController(_engine, this);
            InteractiveBitmaps = new InteractiveBitmapSpriteTickController(_engine, this);
            GenericBitmaps = new MinimalBitmapSpriteTickController(_engine, this);
            Particles = new ParticleSpriteTickController(_engine, this);
            TextBlocks = new TextBlocksSpriteTickController(_engine, this);
        }

        public SpriteBase[] Visible() => _collection.Where(o => o.Visible == true).ToArray();

        public SpriteBase[] All() => _collection.ToArray();

        public List<SpritePlayerBase> AllVisiblePlayers
        {
            get
            {
                var players = VisibleOfType<SpritePlayerBase>().ToList();
                players.Add(_engine.Player.Sprite);
                return players;
            }
        }

        /// <summary>
        /// This is to be used ONLY for the debugger to access the collection. Otherwise, this class managed all access to the internal collection,
        /// </summary>
        /// <param name="collectionAccessor"></param>
        public void DebugOnlyAccess(CollectionAccessor collectionAccessor)
            => collectionAccessor(All());

        public void QueueAllForDeletionOfType<T>() where T : SpriteBase
            => OfType<T>().ForEach(c => c.QueueForDelete());

        public void Dispose()
        {
        }

        public T CreateByType<T>() where T : SpriteBase
        {
            return (T)Activator.CreateInstance(typeof(T), [_engine]).EnsureNotNull();
        }

        public void Add(SpriteBase item)
        {
            if (_engine.IsInitializing == true)
            {
                //When the engine is initializing, we do all kinds of pre-caching.
                //We want to make sure that none of these new classes make it to the sprite collection.
                return;
            }

            if (item == null)
            {
                throw new Exception("NULL sprites cannot be added to the manager.");
            }
            _engine.Events.Add(() => _collection.Add(item));
        }

        public void HardDelete(SpriteBase item)
        {
            item.Cleanup();
            _collection.Remove(item);
        }

        public void HardDeleteAllQueuedDeletions()
        {
            _collection.Where(o => o.IsQueuedForDeletion).ToList().ForEach(p => p.Cleanup());
            _collection.RemoveAll(o => o.IsQueuedForDeletion);

            _engine.Events.CleanupQueuedForDeletion();

            if (_engine.Player.Sprite.IsDeadOrExploded)
            {
                _engine.Player.Sprite.Visible = false;
                _engine.Player.Sprite.ReviveDeadOrExploded();
            }
        }

        /// <summary>
        /// Deletes all the non-background sprite types.
        /// </summary>
        public void QueueDeletionOfActionSprites()
        {
            Enemies.QueueAllForDeletion();
            Animations.QueueAllForDeletion();
        }

        public T[]? GetSpritesByTag<T>(string name) where T : SpriteBase
            => _collection.Where(o => o.SpriteTag == name).ToArray() as T[];

        public T? GetSingleSpriteByTag<T>(string name) where T : SpriteBase
            => _collection.Where(o => o.SpriteTag == name).SingleOrDefault() as T;

        public T? GetSpriteByOwner<T>(uint ownerUID) where T : SpriteBase
            => _collection.Where(o => o.UID == ownerUID).SingleOrDefault() as T;

        public T[] OfType<T>() where T : SpriteBase
            => _collection.OfType<T>().ToArray();

        public T[] VisibleOfType<T>() where T : SpriteBase
            => _collection.OfType<T>().Where(o => o.Visible).ToArray();

        public SpriteBase[] VisibleOfTypes(Type[] types)
        {
            var result = new List<SpriteBase>();
            foreach (var type in types)
            {
                result.AddRange(_collection.Where(o => o.Visible == true && type.IsAssignableFrom(o.GetType())));
            }

            return result.ToArray();
        }

        public void QueueAllForDeletionByTag(string name)
        {
            foreach (var sprite in _collection)
            {
                if (sprite.SpriteTag == name)
                {
                    sprite.QueueForDelete();
                }
            }
        }

        public void QueueAllForDeletionByOwner(uint ownerUID)
        {
            foreach (var sprite in _collection)
            {
                if (sprite.OwnerUID == ownerUID)
                {
                    sprite.QueueForDelete();
                }
            }
        }

        public SpriteBase[] Intersections(SpriteBase with)
        {
            var objects = new List<SpriteBase>();

            foreach (var obj in _collection.Where(o => o.Visible == true))
            {
                if (obj != with)
                {
                    if (obj.IntersectsAABB(with.Location, new CosVector(with.Size.Width, with.Size.Height)))
                    {
                        objects.Add(obj);
                    }
                }
            }
            return objects.ToArray();
        }

        public SpriteBase[] Intersections(float x, float y, float width, float height)
            => Intersections(new CosVector(x, y), new CosVector(width, height));

        public SpriteBase[] Intersections(CosVector location, CosVector size)
        {
            var objects = new List<SpriteBase>();

            foreach (var obj in _collection.Where(o => o.Visible == true))
            {
                if (obj.IntersectsAABB(location, size))
                {
                    objects.Add(obj);
                }
            }
            return objects.ToArray();
        }

        public SpriteBase[] RenderLocationIntersectionsEvenInvisible(CosVector location, CosVector size)
        {
            var objects = new List<SpriteBase>();

            foreach (var obj in _collection)
            {
                if (obj.RenderLocationIntersectsAABB(location, size))
                {
                    objects.Add(obj);
                }
            }
            return objects.ToArray();
        }

        public SpriteBase[] RenderLocationIntersections(CosVector location, CosVector size)
        {
            var objects = new List<SpriteBase>();

            foreach (var obj in _collection.Where(o => o.Visible == true))
            {
                if (obj.RenderLocationIntersectsAABB(location, size))
                {
                    objects.Add(obj);
                }
            }
            return objects.ToArray();
        }

        public SpritePlayerBase AddPlayer(SpritePlayerBase sprite)
        {
            Add(sprite);
            return sprite;
        }

        /// <summary>
        /// Will render the current game state to a single bitmap. If a lock cannot be acquired
        /// for drawing then the previous frame will be returned.
        /// </summary>
        /// <returns></returns>
        public void Render(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            foreach (var sprite in _collection.Where(o => o.Visible == true).OrderBy(o => o.Z))
            {
                if (sprite.IsWithinCurrentScaledScreenBounds)
                {
                    sprite.Render(renderTarget);
                }
            }

            _engine.Player.Sprite?.Render(renderTarget);
        }

        public void CreateFragmentsOf(SpriteBase sprite)
        {
            var image = sprite.GetImage();
            if (image == null)
            {
                return;
            }

            var fragmentImages = _engine.Rendering.GenerateIrregularFragments(image);

            foreach (var fragmentImage in fragmentImages)
            {
                var fragment = _engine.Sprites.GenericBitmaps.AddAt(fragmentImage, sprite);
                fragment.CleanupMode = CosParticleCleanupMode.DistanceOffScreen;
                fragment.FadeToBlackReductionAmount = CosRandom.Between(0.001f, 0.01f); //TODO: Can we implement this?
                fragment.RotationSpeed = CosRandom.FlipCoin() ? CosRandom.Between(-0.05f, -0.02f) : CosRandom.Between(0.02f, 0.05f);
                fragment.VectorType = CosParticleVectorType.Default;

                fragment.Orientation.Degrees = CosRandom.Between(0.0f, 359.0f);
                fragment.Speed = CosRandom.Between(1, 3.5f);
                fragment.Throttle = 1;
                fragment.RecalculateMovementVector();
            }
        }

        public void HydrateCache(SpriteTextBlock loadingHeader, SpriteTextBlock loadingDetail)
        {
            float statusIndex = 0;
            loadingHeader.SetTextAndCenterX("Hydrating sprite cache...");

            var assembly = Assembly.GetExecutingAssembly();
            var baseType = typeof(SpriteBase);
            var derivedTypes = new List<Type>();

            var allTypes = assembly.GetTypes();

            foreach (var type in allTypes)
            {
                loadingDetail.SetTextAndCenterX($"{statusIndex++ / allTypes.Length * 100.0:n0}%");

                if (baseType.IsAssignableFrom(type) && type != baseType)
                {
                    // Check if the constructor parameter is of type EngineCore
                    var constructor = type.GetConstructor([typeof(EngineCore)]);
                    if (constructor != null)
                    {
                        derivedTypes.Add(type);
                    }
                }
            }

            statusIndex = 0;
            loadingHeader.SetTextAndCenterX("Hydrating animation cache...");

            /*
            // Create instances of derived types
            foreach (var type in derivedTypes)
            {
                //Creating the instance of the sprite loads and caches the metadata and images.
                dynamic instance = Activator.CreateInstance(type, _engine).EnsureNotNull();

                loadingDetail.SetTextAndCenterX($"{statusIndex++ / derivedTypes.Count * 100.0:n0}%");
                instance.QueueForDelete();
            }

            //Pre-cache animations:
            //Animations do not have their own classes, so we need to look for them in the assets and load them.
            var animations = _engine.Assets.Entries.Select(o => o.Value.Key.EnsureNotNull())
                .Where(o => o != null && o.StartsWith(@"sprites/animation/", StringComparison.CurrentCultureIgnoreCase)
                && o.EndsWith(@".png", StringComparison.CurrentCultureIgnoreCase)).ToList();

            animations.ForEach(o => Animations.Add(o).QueueForDelete());
            */
        }
    }
}
