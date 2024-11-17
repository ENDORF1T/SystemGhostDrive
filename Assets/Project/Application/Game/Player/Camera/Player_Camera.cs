using Project.Application.Game.Player.HandlerSystems.Input;
using UnityEngine;

namespace Project.Application.Game.Player.Camera
{
    [RequireComponent(typeof(Player_CameraHandlerInputSystem))]
    public class Player_Camera : MonoBehaviour
    {
        [SerializeField] private float _sensivity = 10.0f;
        [SerializeField] private Transform _targetRotation = null;
        private Player_CameraHandlerInputSystem _input = null;

        private void OnChangedInputState()
        {
            if (_targetRotation == null || _input == null) return;

            _targetRotation.Rotate(0.0f, _input.RotationDelta.x * Time.deltaTime * _sensivity, 0.0f);
        }

        private void OnDisable()
        {
            if (_input) _input.OnChangedInputState -= OnChangedInputState;
        }

        private void Awake()
        {
            _input = GetComponent<Player_CameraHandlerInputSystem>();

            _input.OnChangedInputState += OnChangedInputState;
        }
    }
}