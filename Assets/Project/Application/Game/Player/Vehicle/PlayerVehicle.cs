using Project.Application.Game.Player.HandlerSystems.Input;

namespace Project.Application.Game.Player.Vehicle
{
    public class PlayerVehicle : Game.Vehicle.Vehicle
    {
        public Player_VehicleHandlerInputSystem HandlerInputSystem { get; private set; } = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            HandlerInputSystem = GetComponentInChildren<Player_VehicleHandlerInputSystem>();
        }
    }
}