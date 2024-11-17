using UnityEngine;

namespace Project.Application.Game.RepleySystem
{
    [RequireComponent(typeof(GhostRecoder))]
    public class PlayRecording : MonoBehaviour
    {
        [SerializeField] private Vehicle.Vehicle _vehicle;
        private GhostRecoder _recoder;

        private bool _isPlay = false;

        public void StartPlayRecording()
        {
            _isPlay = true;
            _vehicle.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!_isPlay) return;

            var data = _recoder.GetReplayData();

            if (data != null)
            {
                _vehicle.Input.SetInput(data.MovementInput.x, data.MovementInput.x, data.IsBrakeInput);
                _vehicle.transform.position = data.Position;
                _vehicle.transform.rotation = data.Rotation;
            }
        }

        private void Awake()
        {
            _recoder = GetComponent<GhostRecoder>();
        }
    }
}