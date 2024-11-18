using UnityEngine;

namespace Project.Application.Game.RepleySystem
{
    /// <summary>
    /// Класс воспроизведение записи
    /// </summary>
    [RequireComponent(typeof(GhostRecoder))]
    public class PlayRecording : MonoBehaviour
    {
        [SerializeField] private Vehicle.Vehicle _vehiclePrefab = null;

        private Vehicle.Vehicle _currentVehicle = null;
        private GhostRecoder _recoder;

        private bool _isPlay = false;

        /// <summary>
        /// Начать воспроизведение
        /// </summary>
        public void StartPlayRecording()
        {
            _isPlay = true;

            _currentVehicle = Instantiate(_vehiclePrefab);
            RepleyByKeyFrame();
        }

        /// <summary>
        /// Воспроизвести запись по ключевому кадру
        /// </summary>
        private void RepleyByKeyFrame()
        {
            var data = _recoder.GetReplayData();

            if (data != null)
            {
                _currentVehicle.Input.SetInput(data.MovementInput.x, data.MovementInput.x, data.IsBrakeInput);
                _currentVehicle.transform.position = data.Position;
                _currentVehicle.transform.rotation = data.Rotation;
            }
        }

        private void Update()
        {
            if (!_isPlay) return;

            RepleyByKeyFrame();
        }

        private void Awake()
        {
            _recoder = GetComponent<GhostRecoder>();
        }
    }
}