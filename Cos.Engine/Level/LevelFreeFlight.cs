using Cos.Engine.Core.Types;
using Cos.Engine.Level._Superclass;

namespace Cos.Engine.Level
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// This level is just a peaceful free flight.
    /// </summary>
    internal class LevelFreeFlight : LevelBase
    {
        public LevelFreeFlight(EngineCore engine)
            : base(engine,
                  "Free Flight",
                  "There's nothing in this quadrant or the next that will threaten us.")
        {
            TotalWaves = 5;
        }

        public override void Begin()
        {
            base.Begin();

            AddSingleFireEvent(500, FirstShowPlayerCallback);
        }

        private void FirstShowPlayerCallback(CosDefermentEvent sender, object? refObj)
        {
            _engine.Player.ResetAndShow();
        }
    }
}
