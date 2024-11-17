using Project.Application.Utility.Scripts.Singletons;
using UnityEngine;

namespace Project.Application.Game.RepleySystem
{
    [RequireComponent(typeof(PlayRecording))]
    [RequireComponent(typeof(GhostRecoder))]
    public class RepleySystem : Singleton<RepleySystem>
    {
        public PlayRecording PlayRecording { get; private set; } = null;
        public GhostRecoder Recorder { get; private set; } = null;

        private void Awake()
        {
            PlayRecording = GetComponent<PlayRecording>();
            Recorder = GetComponent<GhostRecoder>();
        }
    }
}