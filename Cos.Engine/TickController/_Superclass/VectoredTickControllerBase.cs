using Cos.Engine.Manager;
using Cos.Engine.Sprite._Superclass._Root;
using Cos.Library;
using Cos.Library.Mathematics;
using NTDLS.Helpers;
using System;
using System.Linq;

namespace Cos.Engine.TickController._Superclass
{
    /// <summary>
    /// Tick managers which update their sprites using the supplied 2D vector.
    /// Also contains various factory methods.
    /// </summary>
    public class VectoredTickControllerBase<T> : ITickController<T> where T : SpriteBase
    {
        public EngineCore Engine { get; private set; }
        public SpriteManager SpriteManager { get; private set; }

        public subType[] VisibleOfType<subType>() where subType : T => SpriteManager.VisibleOfType<subType>();
        public T[] Visible() => SpriteManager.VisibleOfType<T>();
        public T[] All() => SpriteManager.OfType<T>();
        public subType[] OfType<subType>() where subType : T => SpriteManager.OfType<subType>();
        public T? ByTag(string name) => SpriteManager.VisibleOfType<T>().FirstOrDefault(o => o.SpriteTag == name);

        public virtual void ExecuteWorldClockTick(float epoch, CosVector displacementVector) { }

        public VectoredTickControllerBase(EngineCore engine, SpriteManager manager)
        {
            Engine = engine;
            SpriteManager = manager;
        }

        public void QueueAllForDeletion() => SpriteManager.QueueAllForDeletionOfType<T>();

        public T CreateSubtypeByName(string typeName)
        {
            var type = CosReflection.GetTypeByName(typeName) ?? throw new ArgumentException($"Type with FullName '{typeName}' not found.");
            object[] param = { Engine };
            var sprite = (T)Activator.CreateInstance(type, param).EnsureNotNull();
            return sprite;
        }

        public void Add(T obj) => SpriteManager.Add(obj);

        public T Add(string bitmapPath, CosVector location)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath).EnsureNotNull();
            obj.Location = location.Clone();
            SpriteManager.Add(obj);
            return obj;
        }

        public T Add(string bitmapPath, float x, float y)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath).EnsureNotNull();
            obj.X = x;
            obj.Y = y;
            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAtCenterScreen()
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
            obj.Location = Engine.Display.CenterOfCurrentScreen;
            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAtCenterScreen(string bitmapPath)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath).EnsureNotNull();
            obj.Location = Engine.Display.CenterOfCurrentScreen;
            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAtCenterUniverse()
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
            obj.X = Engine.Display.TotalCanvasSize.Width / 2;
            obj.Y = Engine.Display.TotalCanvasSize.Height / 2;

            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAtCenterUniverse(string bitmapPath)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath).EnsureNotNull();
            obj.X = Engine.Display.TotalCanvasSize.Width / 2;
            obj.Y = Engine.Display.TotalCanvasSize.Height / 2;

            SpriteManager.Add(obj);
            return obj;
        }

        public T Add(float x, float y)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
            obj.X = x;
            obj.Y = y;
            SpriteManager.Add(obj);
            return obj;
        }

        public T Add(CosVector location)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
            obj.Location = location.Clone();
            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAt(SpriteBase locationOf)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
            obj.Location = locationOf.Location.Clone();
            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAt(CosVector location)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
            obj.Location = location.Clone();
            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAt(SharpDX.Direct2D1.Bitmap bitmap, SpriteBase locationOf)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmap).EnsureNotNull();
            obj.Location = locationOf.Location.Clone();
            SpriteManager.Add(obj);
            return obj;
        }

        public T Add()
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
            SpriteManager.Add(obj);
            return obj;
        }

        public T Add(string bitmapPath)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath).EnsureNotNull();
            SpriteManager.Add(obj);
            return obj;
        }

        public T Add(SharpDX.Direct2D1.Bitmap bitmap)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmap).EnsureNotNull();
            SpriteManager.Add(obj);
            return obj;
        }

        public T Create()
        {
            return (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
        }

        public T Create(string bitmapPath)
        {
            return (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath).EnsureNotNull();
        }
    }
}
