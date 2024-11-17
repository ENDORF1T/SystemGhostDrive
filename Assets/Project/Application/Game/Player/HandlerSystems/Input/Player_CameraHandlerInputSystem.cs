using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Application.Game.Player.HandlerSystems.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player_CameraHandlerInputSystem : MonoBehaviour
    {
        public event Action OnChangedInputState;

        public Vector2 RotationDelta { get; private set; }

        public void OnReceiveRotationDelta(InputAction.CallbackContext ctx)
        {
            RotationDelta = ctx.ReadValue<Vector2>();

            OnChangedInputState?.Invoke();
        }
    }
}