using UnityEngine;

namespace Project.Application.Utility.MonoBehaviourCashe
{
    public class MonoBehaviourCashe : MonoBehaviour
    {
        public void Tick_Update() => OnUpdate();
        public void Tick_FixedUpdate() => OnFixedUpdate();
        public void Tick_LateUpdate() => OnLateUpdate();

        public void UnSubscribeEverything()
        {
            UnSubscribeToUpdate();
            UnSubscribeToFixedUpdate();
            UnSubscribeLateToUpdate();
        }

        public void SubscribeToUpdate() => GlobalMonoBehaviourCashe.Updates.Add(this);
        public void UnSubscribeToUpdate() => GlobalMonoBehaviourCashe.Updates.Remove(this);

        public void SubscribeToFixedUpdate() => GlobalMonoBehaviourCashe.FixedUpdates.Add(this);
        public void UnSubscribeToFixedUpdate() => GlobalMonoBehaviourCashe.FixedUpdates.Remove(this);

        public void SubscribeToLateUpdate() => GlobalMonoBehaviourCashe.LateUpdates.Add(this);
        public void UnSubscribeLateToUpdate() => GlobalMonoBehaviourCashe.LateUpdates.Remove(this);

        protected virtual void OnUpdate() { }

        protected virtual void OnFixedUpdate() { }

        protected virtual void OnLateUpdate() { }

        protected virtual void OnDestroy()
        {
            UnSubscribeEverything();
        }
    }
}