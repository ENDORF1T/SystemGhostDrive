using UnityEngine;

namespace Project.Application.Game.Vehicle
{
    /// <summary>
    /// Класс содержащий основные компоненты, свойства, переменные и другое
    /// </summary>
    [RequireComponent(typeof(VehicleController))]
    [RequireComponent(typeof(VehicleInput))]
    public class Vehicle : MonoBehaviour
    {
        public VehicleInput Input { get; private set; }

        public VehicleController Controller { get; private set; }

        public bool CanDrive { get; set; }

        public bool CanAccelerate { get; set; }

        public Vector3 LocalVehicleVelocity => Controller.LocalVehicleVelocity;

        public bool VehicleIsGrounded => Controller.VehicleIsGrounded;

        [field: Header("Events"), Space(10)]
        [field: SerializeField] public VehicleEvents VehicleEvents { get; private set; } = new VehicleEvents();

        protected virtual void OnEnable()
        {
            Input = GetComponent<VehicleInput>();
            Controller = GetComponent<VehicleController>();

            CanDrive = true;
            CanAccelerate = true;
        }
    }
}