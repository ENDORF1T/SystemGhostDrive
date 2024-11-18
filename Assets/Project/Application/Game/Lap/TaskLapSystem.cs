namespace Project.Application.Game.Lap
{
    /// <summary>
    /// Класс реализовывающий логику тестового задания
    /// </summary>
    public class TaskLapSystem : LapSystem
    {
        public override void IncreaseLap()
        {
            base.IncreaseLap();

            if (CurrentLap == 1)
            {
                RepleySystem.RepleySystem.Instance.Recorder.Recording = true; // Начало записи данных, если это первый круг 
            }
            else if (CurrentLap == 2)
            {
                RepleySystem.RepleySystem.Instance.PlayRecording.StartPlayRecording(); // Воспроизведение записи, когда достигли 2-го круга
            }
        }
    }
}