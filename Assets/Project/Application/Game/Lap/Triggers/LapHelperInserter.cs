using UnityEngine;

namespace Project.Application.Game.Lap.Triggers
{
    /// <summary>
    /// ����� ��� ��������� LapIncreaser - ��������� ��� ��������� ����� ������
    /// </summary>
    public class LapHelperInserter : LapTriggerEnter
    {
        [SerializeField] private LapIncreaser _lapIncreaser = null;

        protected override void PlayerOnTriggerEnter()
        {
            _lapIncreaser.gameObject.SetActive(true);
        }
    }
}