using Cos.Engine.Core.Types;
using System;
using System.Collections.Generic;
using static Cos.Engine.Core.Types.CosDefermentEvent;
using static Cos.Library.CosConstants;

namespace Cos.Engine.Level._Superclass
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    public class LevelBase
    {
        protected EngineCore _engine;
        protected List<CosDefermentEvent> Events = new();

        public Guid UID { get; private set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrentWave { get; set; } = 0;
        public int TotalWaves { get; set; } = 1;
        public CosLevelState State { get; protected set; } = CosLevelState.NotYetStarted;

        public LevelBase(EngineCore engine, string name, string description)
        {
            _engine = engine;
            Name = name;
            Description = description;
        }

        public virtual void End()
        {
            Events.ForEach(e => e.QueueForDeletion());
            State = CosLevelState.Ended;
        }

        public virtual void Begin()
        {
            State = CosLevelState.Started;
        }

        protected CosDefermentEvent AddRecuringFireEvent(int milliseconds, SiDefermentExecuteCallback executeCallback)
        {
            //Keep track of recurring events to we can delete them when we are done.
            var obj = _engine.Events.Add(milliseconds, executeCallback, null, CosDefermentEventMode.Recurring);
            Events.Add(obj);
            return obj;
        }

        protected CosDefermentEvent AddSingleFireEvent(int milliseconds, SiDefermentExecuteCallback executeCallback)
        {
            return _engine.Events.Add(milliseconds, executeCallback);
        }
    }
}
