using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Application.Game.Player.HandlerSystems.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player_VehicleHandlerInputSystem : MonoBehaviour
    {
        public event Action OnChangedInputState;

        public Vector2 Movement { get; private set; }

        public float IsBrake { get; private set; }

        public void OnReceiveMovement(InputAction.CallbackContext ctx)
        {
            Movement = ctx.ReadValue<Vector2>();

            OnChangedInputState?.Invoke();
        }

        public void OnReceiveBrakeStatus(InputAction.CallbackContext ctx)
        {
            IsBrake = ctx.ReadValue<float>();

            OnChangedInputState?.Invoke();
        }
    }
}