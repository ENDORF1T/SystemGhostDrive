namespace Project.Application.Game.Lap.Triggers
{
    /// <summary>
    /// Класс увеличивание счетчика текущего круга
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