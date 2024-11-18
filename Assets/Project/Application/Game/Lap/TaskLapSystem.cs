namespace Project.Application.Game.Lap
{
    /// <summary>
    /// ����� ��������������� ������ ��������� �������
    /// </summary>
    public class TaskLapSystem : LapSystem
    {
        public override void IncreaseLap()
        {
            base.IncreaseLap();

            if (CurrentLap == 1)
            {
                RepleySystem.RepleySystem.Instance.Recorder.Recording = true; // ������ ������ ������, ���� ��� ������ ���� 
            }
            else if (CurrentLap == 2)
            {
                RepleySystem.RepleySystem.Instance.PlayRecording.StartPlayRecording(); // ��������������� ������, ����� �������� 2-�� �����
            }
        }
    }
}