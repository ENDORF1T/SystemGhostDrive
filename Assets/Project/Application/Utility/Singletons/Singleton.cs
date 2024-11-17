using UnityEngine;

namespace Project.Application.Utility.Scripts.Singletons
{
    public abstract class Singleton<T> : MonoBehaviour
        where T : MonoBehaviour
    {
        public static T Instance { get; protected set; }

        protected virtual void OnEnable()
        {
            try
            {
                if (!Instance) Instance = this as T;
                else if (gameObject) Destroy(gameObject);
            }
            catch (System.Exception) { Destroy(gameObject); }
        }
    }
}