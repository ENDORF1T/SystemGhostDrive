using UnityEngine.Events;

namespace Project.Application.Game.Vehicle
{
    [System.Serializable]
    public class VehicleEvents
    {
        public UnityEvent OnTakeOff;
        public UnityEvent OnGrounded;
        public UnityEvent OnGearChange;
    }
}