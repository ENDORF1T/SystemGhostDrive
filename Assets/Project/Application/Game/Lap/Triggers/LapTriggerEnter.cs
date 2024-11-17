using UnityEngine;

namespace Project.Application.Game.Lap.Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class LapTriggerEnter : MonoBehaviour
    {
        protected virtual void PlayerOnTriggerEnter() { }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Player.Player>(out var component))
            {
                PlayerOnTriggerEnter();
            }
        }

        private void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }
    }
}
