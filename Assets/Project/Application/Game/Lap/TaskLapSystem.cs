using Project.Application.Game.RepleySystem;
using UnityEngine;

namespace Project.Application.Game.Lap
{
    public class TaskLapSystem : LapSystem
    {
        public override void IncreaseLap()
        {
            base.IncreaseLap();

            if (CurrentLap == 1)
            {
                RepleySystem.RepleySystem.Instance.Recorder.Recording = true;
            }
            else if (CurrentLap == 2)
            {
                RepleySystem.RepleySystem.Instance.PlayRecording.StartPlayRecording();
            }
        }
    }
}