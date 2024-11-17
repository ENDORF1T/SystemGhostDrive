using UnityEngine;

namespace Project.Application.Game.Lap.Triggers
{
    public class LapIncreaser : LapTriggerEnter
    {
        protected override void PlayerOnTriggerEnter()
        {
            LapSystem.Instance.IncreaseLap();
            gameObject.SetActive(false);
        }
    }
}