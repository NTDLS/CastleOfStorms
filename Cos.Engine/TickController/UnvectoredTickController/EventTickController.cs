using Cos.Engine.Core.Types;
using Cos.Engine.TickController._Superclass;
using NTDLS.Semaphore;
using System.Collections.Generic;
using static Cos.Engine.Core.Types.CosDefermentEvent;

namespace Cos.Engine.TickController.UnvectoredTickController
{
    public class EventTickController : UnvectoredTickControllerBase<CosDefermentEvent>
    {
        private readonly PessimisticCriticalResource<List<CosDefermentEvent>> _collection = new();

        /// <summary>
        /// Delegate for the event execution callback.
        /// </summary>
        /// <typeparam name="T">Type of the parameter for the event.</typeparam>
        /// <param name="parameter">An object passed by the user code</param>
        public delegate void SiDefermentSimpleExecuteCallbackT<T>(T parameter);

        public EventTickController(EngineCore engine)
            : base(engine)
        {
        }

        public override void ExecuteWorldClockTick()
        {
            _collection.Use(o =>
            {
                for (int i = 0; i < o.Count; i++)
                {
                    var engineEvent = o[i];
                    if (engineEvent.IsQueuedForDeletion == false)
                    {
                        engineEvent.CheckForTrigger();
                    }
                }
            });
        }

        #region Factories.

        /// <summary>
        /// Creates a new event. This can be a recurring event, single event, synchronous, asynchronous and can be passed parameters.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="parameter">An object that will be passed to the execution callback.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <param name="eventMode">Whether the event is one time or recurring.</param>
        /// <param name="threadModel">Whether the event callback is run synchronous or asynchronous.</param>
        /// <returns></returns>
        public CosDefermentEvent Add(int timeoutMilliseconds, SiDefermentExecuteCallback executionCallback, object? parameter,
            CosDefermentEventMode eventMode = CosDefermentEventMode.OneTime,
            CosDefermentEventThreadModel threadModel = CosDefermentEventThreadModel.Synchronous)
        {
            return _collection.Use(o =>
            {
                var obj = new CosDefermentEvent(timeoutMilliseconds, parameter, executionCallback, eventMode, threadModel);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new event. This can be a recurring event, single event, synchronous or asynchronous.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <param name="eventMode">Whether the event is one time or recurring.</param>
        /// <param name="threadModel">Whether the event callback is run synchronous or asynchronous.</param>
        /// <returns></returns>
        public CosDefermentEvent Add(int timeoutMilliseconds, SiDefermentExecuteCallback executionCallback,
            CosDefermentEventMode eventMode = CosDefermentEventMode.OneTime,
            CosDefermentEventThreadModel threadModel = CosDefermentEventThreadModel.Synchronous)
        {
            return _collection.Use(o =>
            {
                var obj = new CosDefermentEvent(timeoutMilliseconds, null, executionCallback, eventMode, threadModel);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new single threaded event. This can be a recurring event, single event.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <param name="eventMode">Whether the event is one time or recurring.</param>
        /// <returns></returns>
        public CosDefermentEvent Add(int timeoutMilliseconds, SiDefermentExecuteCallback executionCallback,
            CosDefermentEventMode eventMode = CosDefermentEventMode.OneTime)
        {
            return _collection.Use(o =>
            {
                var obj = new CosDefermentEvent(timeoutMilliseconds, null, executionCallback, eventMode);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new single threaded event.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="parameter">An object that will be passed to the execution callback.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <returns></returns>
        public CosDefermentEvent Add(int timeoutMilliseconds, object parameter, SiDefermentExecuteCallback executionCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new CosDefermentEvent(timeoutMilliseconds, parameter, executionCallback);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new single threaded event.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <returns></returns>
        public CosDefermentEvent Add(int timeoutMilliseconds, SiDefermentExecuteCallback executionCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new CosDefermentEvent(timeoutMilliseconds, executionCallback);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new single threaded event and passes a parameter of the given type T.
        /// </summary>
        /// <typeparam name="T">Type of the parameter for the event.</typeparam>
        /// <param name="timeoutMilliseconds"></param>
        /// <param name="parameter">An object passed by the user code</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <returns></returns>
        public CosDefermentEvent Add<T>(int timeoutMilliseconds, T parameter, SiDefermentSimpleExecuteCallbackT<T> executionCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new CosDefermentEvent(timeoutMilliseconds,
                    (CosDefermentEvent sender, object? refObj) =>
                {
                    executionCallback(parameter);
                });
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new single threaded, single-fire event.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <returns></returns>
        public CosDefermentEvent Add(int timeoutMilliseconds, SiDefermentSimpleExecuteCallback executionCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new CosDefermentEvent(timeoutMilliseconds, executionCallback);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Creates a new single threaded, single-fire event.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <returns></returns>
        public CosDefermentEvent Add(SiDefermentSimpleExecuteCallback executionCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new CosDefermentEvent(0, executionCallback);
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Adds an existing even to the collection.
        /// </summary>
        /// <param name="CosDefermentEvent">An existing event to add.</param>
        /// <returns></returns>
        public CosDefermentEvent Add(CosDefermentEvent obj)
        {
            return _collection.Use(o =>
            {
                o.Add(obj);
                return obj;
            });
        }

        /// <summary>
        /// Deletes an event from the collection.
        /// </summary>
        /// <param name="obj"></param>
        public void HardDelete(CosDefermentEvent obj)
        {
            _collection.Use(o =>
            {
                o.Remove(obj);
            });
        }

        /// <summary>
        /// Queues an event for deletion from the collection.
        /// </summary>
        public void CleanupQueuedForDeletion()
        {
            _collection.Use(o =>
            {
                for (int i = 0; i < o.Count; i++)
                {
                    if (o[i].IsQueuedForDeletion)
                    {
                        o.Remove(o[i]);
                    }
                }
            });
        }

        #endregion
    }
}
