using UnityEngine;

namespace Project.Application.Game.Vehicle
{
    /// <summary>
    /// Класс позволяющий управлять автомобилем
    /// </summary>
    public class VehicleInput : MonoBehaviour
    {
        public float Steer { get; private set; } 
        public float Acceleration { get; private set; } 
        public float Brake { get; private set; }

        private Vehicle _vehicle = null;

        /// <summary>
        /// Ввод данных для управления
        /// </summary>
        /// <param name="acceleration">Движение вперед и назад</param>
        /// <param name="steer">Поворот вправо или влево</param>
        /// <param name="brake">Использование тормоза</param>
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

        /// <summary>
        /// Метод для инициализации данных в Awake
        /// </summary>
        protected virtual void Initialize() { } 

        private void Awake()
        {
            _vehicle = GetComponent<Vehicle>();

            Initialize();
        }
    } 
}
