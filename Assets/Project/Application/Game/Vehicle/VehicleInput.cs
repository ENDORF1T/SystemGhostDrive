using UnityEngine;

namespace Project.Application.Game.Vehicle
{
    public class VehicleInput : MonoBehaviour
    {
        public float Steer { get; private set; } 
        public float Acceleration { get; private set; } 
        public float Brake { get; private set; }

        private Vehicle _vehicle = null;

        public void SetInput(float acceleration, float steer, float brake)
        {
            if (!_vehicle) return; 

            if (_vehicle.CanDrive && _vehicle.CanAccelerate)
            {
                Acceleration = acceleration;
                Steer = steer;
                Brake = brake;
            }
            else if (_vehicle.CanDrive && !_vehicle.CanAccelerate)
            {
                Acceleration = 0;
                Steer = steer;
                Brake = brake;
            }
            else
            {
                Acceleration = 0;
                Steer = 0;
                Brake = 1;
            }
        }

        protected virtual void Initialize() { } 

        private void Update()
        {
            //SetInput(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), Input.GetAxis("Jump"));
            //SetInput(0.5f, Input.GetAxis("Horizontal"), Input.GetAxis("Jump"));
        }

        private void Awake()
        {
            _vehicle = GetComponent<Vehicle>();

            Initialize();
        }
    } 
}
