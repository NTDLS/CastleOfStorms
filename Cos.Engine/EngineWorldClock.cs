using Cos.Engine.Core.Types;
using Cos.Engine.Manager;
using Cos.Engine.TickController._Superclass;
using Cos.Library;
using Cos.Library.Mathematics;
using Cos.Rendering;
using NTDLS.DelegateThreadPooling;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Cos.Engine
{
    /// <summary>
    /// The world clock. Moves all objects forward in time, renders all objects and keeps the frame-counter in check.
    /// </summary>
    internal class EngineWorldClock : IDisposable
    {
        private readonly EngineCore _engine;
        private bool _shutdown = false;
        private bool _isPaused = false;
        private readonly Thread _graphicsThread;
        private readonly TrackableQueue<TickControllerMethod>? _worldClockSubPool;

        private readonly DelegateThreadPool _worldClockThreadPool;

        private struct TickControllerMethod
        {
            public object Controller;
            public MethodInfo Method;

            public TickControllerMethod(object controller, MethodInfo method)
            {
                Controller = controller;
                Method = method;
            }
        }

        private readonly List<TickControllerMethod> _vectoredTickControllers = new();
        private readonly List<TickControllerMethod> _unvectoredTickControllers = new();

        public EngineWorldClock(EngineCore engine)
        {
            _engine = engine;
            _worldClockThreadPool = new(engine.Settings.WorldClockThreads);

            engine.OnShutdown += (sender) =>
            {
                _worldClockThreadPool.Stop();
            };

            _graphicsThread = new Thread(GraphicsThreadProc);

            if (_engine.Settings.MultithreadedWorldClock)
            {
                //Create a collection of threads so we can wait on the ones that we start.
                _worldClockSubPool ??= _worldClockThreadPool.CreateChildQueue<TickControllerMethod>();
            }

            #region Cache vectored and unvectored tick controller methods.

            var properties = typeof(SpriteManager).GetProperties();

            foreach (var property in properties)
            {
                if (CosReflection.IsAssignableToGenericType(property.PropertyType, typeof(VectoredTickControllerBase<>))
                    || CosReflection.IsAssignableToGenericType(property.PropertyType, typeof(VectoredCollidableTickControllerBase<>)))
                {
                    var method = property.PropertyType.GetMethod("ExecuteWorldClockTick")
                        ?? throw new Exception("VectoredTickController must contain ExecuteWorldClockTick");

                    var instance = property.GetValue(_engine.Sprites)
                        ?? throw new Exception($"Sprite manager must contain property [{property.Name}] and it must not bu NULL.");

                    _vectoredTickControllers.Add(new TickControllerMethod(instance, method));

                }
                else if (CosReflection.IsAssignableToGenericType(property.PropertyType, typeof(UnvectoredTickControllerBase<>)))
                {
                    var method = property.PropertyType.GetMethod("ExecuteWorldClockTick")
                        ?? throw new Exception("VectoredTickController must contain ExecuteWorldClockTick");

                    var instance = property.GetValue(_engine.Sprites)
                        ?? throw new Exception($"Sprite manager must contain property [{property.Name}] and it must not bu NULL.");

                    _unvectoredTickControllers.Add(new TickControllerMethod(instance, method));
                }
            }

            #endregion
        }

        #region Start / Stop / Pause.

        public void Start()
        {
            _shutdown = false;
            _graphicsThread.Start();

            _engine.Events.Add(10, UpdateStatusText, CosDefermentEvent.CosDefermentEventMode.Recurring);
        }

        public void Dispose()
        {
            _shutdown = true;
            _graphicsThread.Join();
        }

        public bool IsPaused() => _isPaused;

        public void TogglePause()
        {
            _isPaused = !_isPaused;

            _engine.Sprites.TextBlocks.PausedText.X = _engine.Display.NaturalScreenSize.Width / 2 - _engine.Sprites.TextBlocks.PausedText.Size.Width / 2;
            _engine.Sprites.TextBlocks.PausedText.Y = _engine.Display.NaturalScreenSize.Height / 2 - _engine.Sprites.TextBlocks.PausedText.Size.Height / 2;
            _engine.Sprites.TextBlocks.PausedText.Visible = _isPaused;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }

        #endregion

        private void GraphicsThreadProc()
        {
            var framePerSecondLimit = _engine.Settings.VerticalSync ?
                CosRenderingUtility.GetScreenRefreshRate(_engine.Display.Screen, _engine.Settings.GraphicsAdapterId)
                : _engine.Settings.TargetFrameRate;

            float targetTimePerFrameMicroseconds = 1000000.0f / framePerSecondLimit;
            float elapsedEpochMilliseconds = 100;

            while (_shutdown == false)
            {
                var epoch = (float)(elapsedEpochMilliseconds / _engine.Settings.MillisecondPerEpoch);

                if (!_isPaused) ExecuteWorldClockTick(epoch);

                _engine.RenderEverything();

                if (_engine.Settings.VerticalSync == false)
                {
                    var elapsedFrameTime = _engine.Display.FrameCounter.ElapsedMicroseconds;

                    // Enforce the framerate by figuring out how long it took to render the frame,
                    //  then spin for the difference between how long we wanted it to take.
                    while (_engine.Display.FrameCounter.ElapsedMicroseconds - elapsedFrameTime < targetTimePerFrameMicroseconds - elapsedFrameTime)
                    {
                        if (_engine.Settings.YieldRemainingFrameTime) Thread.Yield();
                    }
                }

                if (_isPaused) Thread.Yield();

                elapsedEpochMilliseconds = _engine.Display.FrameCounter.ElapsedMilliseconds;

                _engine.Display.FrameCounter.Calculate();
            }
        }

        private CosVector ExecuteWorldClockTick(float epoch)
        {
            _engine.Settings.MultithreadedWorldClock = false;

            //This is where we execute the world clock for each type of object.
            //Note that this function does employ threads but I DO NOT believe it is necessary for performance.
            //
            //The idea is that sprites are created in Events.ExecuteWorldClockTick(), input is snapshotted,
            //  the player is moved (so we have a displacement vector) and then we should be free to do all
            //  other operations in parallel with the exception of deleting sprites that are queued for deletion -
            //  which is why we do that after the threads have completed.

            _engine.Events.ExecuteWorldClockTick();

            _engine.Input.Snapshot();

            var displacementVector = _engine.Player.ExecuteWorldClockTick(epoch);

            //Enqueue each vectored tick controller for a thread.
            var vectoredParameters = new object[] { epoch, displacementVector };
            if (_worldClockSubPool != null)
            {
                foreach (var vectored in _vectoredTickControllers)
                {
                    _worldClockSubPool.Enqueue(vectored,
                        (TickControllerMethod p) => p.Method.Invoke(p.Controller, vectoredParameters));
                }

                //Wait on all enqueued threads to complete.
                if (!CosUtility.TryAndIgnore(_worldClockSubPool.WaitForCompletion))
                {
                    return displacementVector; //This is kind of an exception, it likely means that the engine is shutting down - so just return.
                }
            }
            else
            {
                foreach (var vectored in _vectoredTickControllers)
                {
                    vectored.Method.Invoke(vectored.Controller, vectoredParameters);
                }
            }

            //After all vectored tick controllers have executed, run the unvectored tick controllers.
            if (_worldClockSubPool != null)
            {
                foreach (var unvectored in _unvectoredTickControllers)
                {
                    _worldClockSubPool.Enqueue(unvectored,
                        (TickControllerMethod p) => p.Method.Invoke(p.Controller, null));
                }

                //Wait on all enqueued threads to complete.
                if (!CosUtility.TryAndIgnore(_worldClockSubPool.WaitForCompletion))
                {
                    return displacementVector; //This is kind of an exception, it likely means that the engine is shutting down - so just return.
                }
            }
            else
            {
                foreach (var vectored in _unvectoredTickControllers)
                {
                    vectored.Method.Invoke(vectored.Controller, null);
                }
            }

            _engine.Sprites.HardDeleteAllQueuedDeletions();

            return displacementVector;
        }

        private void UpdateStatusText(CosDefermentEvent sender, object? refObj)
        {
            var player = _engine.Player.Sprite;

            string playerStatsText =
                 $"      Hull: {player.HullHealth:n0} (Shields: {player.ShieldHealth:n0}) | Bounty: ${player.Metadata.Bounty}\r\n";

            //playerStatsText += $"{_engine.Display.FrameCounter.AverageFrameRate:n2}fps";

            _engine.Sprites.TextBlocks.PlayerStatsText.Text = playerStatsText;

            //_engine.Sprites.DebugText.Text = "Anything we need to know about?";
        }
    }
}
