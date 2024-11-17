using UnityEngine;

namespace Project.Application.Game.Vehicle
{
    public class TireSmoke : MonoBehaviour
    {
        private ParticleSystem _smoke;

        public void playSmoke()
        {
            _smoke.Play();
        }

        public void stopSmoke()
        {
            _smoke.Stop();
        }

        private void Awake()
        {
            _smoke = GetComponent<ParticleSystem>();
            _smoke.Stop();
        }
    }
}
