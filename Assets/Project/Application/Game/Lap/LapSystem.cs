using Project.Application.Utility.Scripts.Singletons;
using UnityEngine;

namespace Project.Application.Game.Lap
{
    public class LapSystem : Singleton<LapSystem>
    {
        [field: SerializeField] public int MaxLap { get; private set; } = 2;

        public int CurrentLap { get; private set; } = 0;

        public virtual void IncreaseLap()
        {
            CurrentLap++;
        }
    }
}