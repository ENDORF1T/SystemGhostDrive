using Project.Application.Utility.Scripts.Singletons;
using UnityEngine;

namespace Project.Application.Game.Player
{
    public class Cursor : SingletonPersistent<Cursor>
    {
        public void SetCursorState(bool ctx)
        {
            UnityEngine.Cursor.lockState = ctx ? CursorLockMode.None : CursorLockMode.Locked;
            UnityEngine.Cursor.visible = ctx;
        }

        private void Awake()
        {
            SetCursorState(false);
        }
    }
}