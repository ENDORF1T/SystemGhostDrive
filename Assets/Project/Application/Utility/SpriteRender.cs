using UnityEngine;

namespace Project.Application.Utility
{
    public static class SpriteRender
    {
        public static void SetColorCertainTime(SpriteRenderer render, Color changeColor, float duration, GameObject callingGameobject)
        {
            if (!render || !callingGameobject) return;

            Color currentColor = render.color;
            render.color = changeColor;

            Timer.FunctionTimer.Create(() => render.color = currentColor, duration, sender: callingGameobject);
        }
    }
}