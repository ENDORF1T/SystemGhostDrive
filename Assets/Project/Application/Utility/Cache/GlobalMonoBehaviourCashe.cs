using System.Collections.Generic;
using UnityEngine;

namespace Project.Application.Utility.MonoBehaviourCashe
{
    public static class GlobalMonoBehaviourCashe
    {
        public static List<MonoBehaviourCashe> Updates { get; set; } = new List<MonoBehaviourCashe>();
        public static List<MonoBehaviourCashe> FixedUpdates { get; set; } = new List<MonoBehaviourCashe>();
        public static List<MonoBehaviourCashe> LateUpdates { get; set; } = new List<MonoBehaviourCashe>();

        private class UpdaterMonoBehaviourCashe : MonoBehaviour
        {
            private void Update()
            {
                for (int i = 0; i < Updates.Count; i++)
                {
                    try
                    {
                        Updates[i].Tick_Update();
                    }
                    catch (System.Exception) { }
                }
            }

            private void FixedUpdate()
            {
                for (int i = 0; i < FixedUpdates.Count; i++) {
                    try
                    {
                        FixedUpdates[i].Tick_FixedUpdate();
                    }
                    catch (System.Exception) { }
                }
            }

            private void LateUpdate()
            {
                for (int i = 0; i < LateUpdates.Count; i++) {
                    try
                    {
                        LateUpdates[i].Tick_LateUpdate();
                    }
                    catch (System.Exception) { }
                }
            }
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            GameObject localGameObject = new GameObject("Local_Updater_MonoBehaviour_Cashe");

            localGameObject.AddComponent<UpdaterMonoBehaviourCashe>();

            Object.DontDestroyOnLoad(localGameObject);

            UnityEngine.Application.quitting += () => Object.Destroy(localGameObject);
        }
    }
}