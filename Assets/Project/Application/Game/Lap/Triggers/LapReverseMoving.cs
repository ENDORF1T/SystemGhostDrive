using UnityEngine;

namespace Project.Application.Game.Lap.Triggers
{
    public class LapReverseMoving : LapTriggerEnter
    {
        [SerializeField] private LapIncreaser _lapIncreaser = null;

        protected override void PlayerOnTriggerEnter()
        {
            _lapIncreaser.gameObject.SetActive(false);
        }
    }
}
