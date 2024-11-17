using System.Collections;
using UnityEngine;

namespace Project.Application.Game.Vehicle
{
    public class GearSystem : MonoBehaviour
    {
        [field: SerializeField] public int[] GearSpeeds { get; private set; } = new int[] { 40, 80, 120, 160, 220 };

        public float CarSpeed { get; private set; }
        public int CurrentGear { get; private set; }

        private VehicleAudioSystem _audioSystem = null;
        private Vehicle _vehicle;
        private int _currentGearTemp;

        public int CurrntGearProperty
        {
            get
            {
                return _currentGearTemp;
            }

            set
            {
                _currentGearTemp = value;

                if (_vehicle.Input.Acceleration > 0 && _vehicle.LocalVehicleVelocity.z > 0 && !_audioSystem.GearSound.isPlaying && _vehicle.VehicleIsGrounded)
                {
                    _vehicle.VehicleEvents.OnGearChange.Invoke();
                    _audioSystem.GearSound.Play();
                    StartCoroutine(ShiftingGear());
                }

                _audioSystem.EngineSound.volume = 0.5f;
            }
        }

        private void GearShift()
        {
            for (int i = 0; i < GearSpeeds.Length; i++)
            {
                if (CarSpeed > GearSpeeds[i])
                {
                    CurrentGear = i + 1;
                }
                else break;
            }
            if (CurrntGearProperty != CurrentGear)
            {
                CurrntGearProperty = CurrentGear;
            }

        }

        private IEnumerator ShiftingGear()
        {
            _vehicle.CanAccelerate = false;
            yield return new WaitForSeconds(0.3f);
            _vehicle.CanAccelerate = true;
        }

        private void Update()
        {
            CarSpeed = Mathf.RoundToInt(_vehicle.LocalVehicleVelocity.magnitude * 3.6f); //car speed in Km/hr

            GearShift();
        }

        private void Start()
        {
            _vehicle = GetComponent<Vehicle>();
            _audioSystem = GetComponent<VehicleAudioSystem>();
            CurrentGear = 1;
        }
    }
}
