using Project.Application.Game.Vehicle;
using Project.Application.Game.Player.HandlerSystems.Input;

namespace Project.Application.Game.Player.Vehicle
{
    public class Player_VehicleInput : VehicleInput
    {
        private Player_VehicleHandlerInputSystem _inputSystem = null;

        protected override void Initialize()
        {
            _inputSystem = GetComponentInChildren<Player_VehicleHandlerInputSystem>();

            _inputSystem.OnChangedInputState += OnChangedInputState;
        }

        private void OnChangedInputState()
        {
            SetInput(_inputSystem.Movement.y, _inputSystem.Movement.x, _inputSystem.IsBrake);
        }

        private void OnDisable()
        {
            if (_inputSystem) _inputSystem.OnChangedInputState -= OnChangedInputState;
        }
    }
}