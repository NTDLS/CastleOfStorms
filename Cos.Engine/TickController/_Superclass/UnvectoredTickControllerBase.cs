namespace Cos.Engine.TickController._Superclass
{
    /// <summary>
    /// Tick managers that do not handle sprites or do not use a vector to update their sprites.
    /// Things like Events, etc.
    /// </summary>
    public class UnvectoredTickControllerBase<T> : ITickController<T> where T : class
    {
        public EngineCore Engine { get; private set; }

        public virtual void ExecuteWorldClockTick() { }

        public UnvectoredTickControllerBase(EngineCore engine)
        {
            Engine = engine;
        }
    }
}
