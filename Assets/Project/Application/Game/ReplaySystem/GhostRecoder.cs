using Project.Application.Game.Player.Vehicle;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Application.Game.RepleySystem
{
    public class GhostRecoder : MonoBehaviour
    {
        [field: SerializeField] public PlayerVehicle Player { get; set; } = null;
        [field: SerializeField] public bool Recording { get; set; } = false;

        private Queue<RepleyData> _data = new Queue<RepleyData>();

        public RepleyData GetReplayData()
        {
            return _data.Dequeue();
        }

        private void Update()
        {
            // Запись данных, когда Recording = true
            if (Recording) _data.Enqueue(new RepleyData(Player.gameObject.transform.position, Player.gameObject.transform.rotation, Player.HandlerInputSystem.Movement, Player.HandlerInputSystem.IsBrake));
        }
    }
}