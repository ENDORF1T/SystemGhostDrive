using UnityEngine;

namespace Project.Application.Utility.Scripts.Singletons
{
    public abstract class SingletonPersistent<T> : Singleton<T>
        where T : MonoBehaviour
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            gameObject.transform.parent = null;
            DontDestroyOnLoad(this);
        }
    }
}