using UnityEngine;

namespace Project.Application.Game.Vehicle
{
    public class VehicleAudioSystem : MonoBehaviour
    {
        [field: SerializeField] public AudioSource EngineSound { get; private set; }
        [field: SerializeField] public AudioSource GearSound { get; private set; }

        [Range(0, 1), SerializeField] private float minPitch;
        [Range(1, 3), SerializeField] private float maxPitch;

        private GearSystem _gearSystem;
        private Vehicle _vehicle;

        private void SoundManager()
        {
            float speed = _gearSystem.CarSpeed;

            float enginePitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(speed) / _gearSystem.GearSpeeds[Mathf.Clamp(_gearSystem.CurrentGear, 0, 4)]);
            if (_vehicle.VehicleIsGrounded)
            {
                EngineSound.pitch = Mathf.MoveTowards(EngineSound.pitch, enginePitch, 0.02f);
            }

            if (Mathf.Abs(_vehicle.Input.Acceleration) > 0.1f)
            {
                EngineSound.volume = Mathf.MoveTowards(EngineSound.volume, 1, 0.01f);
            }
            else
            {
                EngineSound.volume = Mathf.MoveTowards(EngineSound.volume, 0.5f, 0.01f);
            }
        }

        private void FixedUpdate()
        {
            SoundManager();
        }

        private void Start()
        {
            _gearSystem = GetComponent<GearSystem>();
            _vehicle = GetComponent<Vehicle>();
        }
    }
}
