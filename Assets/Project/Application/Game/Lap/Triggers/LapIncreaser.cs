namespace Project.Application.Game.Lap.Triggers
{
    /// <summary>
    /// ����� ������������ �������� �������� �����
    /// </summary>
    public class LapIncreaser : LapTriggerEnter
    {
        protected override void PlayerOnTriggerEnter()
        {
            LapSystem.Instance.IncreaseLap();
            gameObject.SetActive(false);
        }
    }
}