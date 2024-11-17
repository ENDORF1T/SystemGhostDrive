using UnityEngine;

namespace Project.Application.Game.Vehicle
{
	public class WheelSkid : MonoBehaviour
	{
		[field: SerializeField] public TireSmoke Smoke { get; private set; }

		public float Radius { get; set; }
		public float SkidTotal { get; set; }
		public Vector3 SkidPoint { get; set; }
		public Vector3 Normal { get; set; }
		public Skidmarks Skidmarks { get; set; }

		private const float MAX_SKID_INTENSITY = 1.0f;
		private int _lastSkid = -1;
		private AudioSource _skidSound;

		public void SkidLogic()
		{
			float intensity = Mathf.Clamp01(SkidTotal / MAX_SKID_INTENSITY);


			if (SkidTotal > 0)
			{
				_lastSkid = Skidmarks.AddSkidMark(SkidPoint, Normal, intensity, _lastSkid);
				if (Smoke && intensity > 0.4f)
				{
					Smoke.playSmoke();
					_skidSound.mute = false;
				}
				else if (Smoke)
				{
					Smoke.stopSmoke();
					_skidSound.mute = true;
				}
				_skidSound.volume = intensity / 3;
			}
			else
			{
				_skidSound.mute = true;
				_lastSkid = -1;
				if (Smoke)
				{
					Smoke.stopSmoke();
				}
			}
		}

        private void FixedUpdate()
        {
            SkidLogic();
        }

        private void Start()
        {
            Smoke.transform.localPosition = Vector3.up * Radius;
            _skidSound = GetComponent<AudioSource>();
            _skidSound.mute = true;
        }
    }
}
