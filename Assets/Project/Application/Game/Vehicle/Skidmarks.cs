using UnityEngine;
using UnityEngine.Rendering;

namespace Project.Application.Game.Vehicle
{
	public class Skidmarks : MonoBehaviour
	{
		[field: SerializeField] public Material SkidmarksMaterial { get; private set; }
		public float SkidmarkWidth { get; set; } = 0.5f;

		private const int MAX_SKID_MARKS = 2048;
		private const float OFFSET = 0.02f;
		private const float MIN_DISTANCE = 0.25f;
		private const float MIN_DISTANCE_SQUARE = MIN_DISTANCE * MIN_DISTANCE;
		private const float MAX_OPACITY = 1.0f;

		private class SkidMarkSection
		{
			public Vector3 Pos = Vector3.zero;
			public Vector3 Normal = Vector3.zero;
			public Vector4 Tangent = Vector4.zero;
			public Vector3 Posl = Vector3.zero;
			public Vector3 Posr = Vector3.zero;
			public Color32 Colour;
			public int LastIndex;
		};

		private int _markIndex;
		private SkidMarkSection[] _skidmarks;
		private Mesh _marksMesh;
		private MeshRenderer _mr;
		private MeshFilter _mf;

		private Vector3[] _vertices;
		private Vector3[] _normals;
		private Vector4[] _tangents;
		private Color32[] _colors;
        private Vector2[] _uvs;
		private int[] _triangles;

		private bool _meshUpdated;
		private bool _haveSetBounds;
		private Color32 _blackColor = Color.black;

		public int AddSkidMark(Vector3 pos, Vector3 normal, float opacity, int lastIndex)
		{
			if (opacity > 1) opacity = 1.0f;
			else if (opacity < 0) return -1;

			_blackColor.a = (byte)(opacity * 255);
			return AddSkidMark(pos, normal, _blackColor, lastIndex);
		}

		public int AddSkidMark(Vector3 pos, Vector3 normal, Color32 colour, int lastIndex)
		{
			if (colour.a == 0) return -1;

			SkidMarkSection lastSection = null;
			Vector3 distAndDirection = Vector3.zero;
			Vector3 newPos = pos + normal * OFFSET;
			if (lastIndex != -1)
			{
				lastSection = _skidmarks[lastIndex];
				distAndDirection = newPos - lastSection.Pos;
				if (distAndDirection.sqrMagnitude < MIN_DISTANCE_SQUARE)
				{
					return lastIndex;
				}

				if (distAndDirection.sqrMagnitude > MIN_DISTANCE_SQUARE * 10)
				{
					lastIndex = -1;
					lastSection = null;
				}
			}

			colour.a = (byte)(colour.a * MAX_OPACITY);

			SkidMarkSection curSection = _skidmarks[_markIndex];

			curSection.Pos = newPos;
			curSection.Normal = normal;
			curSection.Colour = colour;
			curSection.LastIndex = lastIndex;

			if (lastSection != null)
			{
				Vector3 xDirection = Vector3.Cross(distAndDirection, normal).normalized;
				curSection.Posl = curSection.Pos + xDirection * SkidmarkWidth * 0.5f;
				curSection.Posr = curSection.Pos - xDirection * SkidmarkWidth * 0.5f;
				curSection.Tangent = new Vector4(xDirection.x, xDirection.y, xDirection.z, 1);

				if (lastSection.LastIndex == -1)
				{
					lastSection.Tangent = curSection.Tangent;
					lastSection.Posl = curSection.Pos + xDirection * SkidmarkWidth * 0.5f;
					lastSection.Posr = curSection.Pos - xDirection * SkidmarkWidth * 0.5f;
				}
			}

			UpdateSkidmarksMesh();

			int curIndex = _markIndex;
			_markIndex = ++_markIndex % MAX_SKID_MARKS;

			return curIndex;
		}

		private void UpdateSkidmarksMesh()
		{
			SkidMarkSection curr = _skidmarks[_markIndex];

			if (curr.LastIndex == -1) return;

			SkidMarkSection last = _skidmarks[curr.LastIndex];
			_vertices[_markIndex * 4 + 0] = last.Posl;
			_vertices[_markIndex * 4 + 1] = last.Posr;
			_vertices[_markIndex * 4 + 2] = curr.Posl;
			_vertices[_markIndex * 4 + 3] = curr.Posr;

			_normals[_markIndex * 4 + 0] = last.Normal;
			_normals[_markIndex * 4 + 1] = last.Normal;
			_normals[_markIndex * 4 + 2] = curr.Normal;
			_normals[_markIndex * 4 + 3] = curr.Normal;

			_tangents[_markIndex * 4 + 0] = last.Tangent;
			_tangents[_markIndex * 4 + 1] = last.Tangent;
			_tangents[_markIndex * 4 + 2] = curr.Tangent;
			_tangents[_markIndex * 4 + 3] = curr.Tangent;

			_colors[_markIndex * 4 + 0] = last.Colour;
			_colors[_markIndex * 4 + 1] = last.Colour;
			_colors[_markIndex * 4 + 2] = curr.Colour;
			_colors[_markIndex * 4 + 3] = curr.Colour;

			_uvs[_markIndex * 4 + 0] = new Vector2(0, 0);
			_uvs[_markIndex * 4 + 1] = new Vector2(1, 0);
			_uvs[_markIndex * 4 + 2] = new Vector2(0, 1);
			_uvs[_markIndex * 4 + 3] = new Vector2(1, 1);

			_triangles[_markIndex * 6 + 0] = _markIndex * 4 + 0;
			_triangles[_markIndex * 6 + 2] = _markIndex * 4 + 1;
			_triangles[_markIndex * 6 + 1] = _markIndex * 4 + 2;

			_triangles[_markIndex * 6 + 3] = _markIndex * 4 + 2;
			_triangles[_markIndex * 6 + 5] = _markIndex * 4 + 1;
			_triangles[_markIndex * 6 + 4] = _markIndex * 4 + 3;

			_meshUpdated = true;
		}

        private void LateUpdate()
        {
            if (!_meshUpdated) return;
            _meshUpdated = false;

            _marksMesh.vertices = _vertices;
            _marksMesh.normals = _normals;
            _marksMesh.tangents = _tangents;
            _marksMesh.triangles = _triangles;
            _marksMesh.colors32 = _colors;
            _marksMesh.uv = _uvs;

            if (!_haveSetBounds)
            {
                _marksMesh.bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(10000, 10000, 10000));
                _haveSetBounds = true;
            }

            _mf.sharedMesh = _marksMesh;
        }

        private void Start()
        {
            _skidmarks = new SkidMarkSection[MAX_SKID_MARKS];

            for (int i = 0; i < MAX_SKID_MARKS; i++)
            {
                _skidmarks[i] = new SkidMarkSection();
            }

            _mf = GetComponent<MeshFilter>();
            _mr = GetComponent<MeshRenderer>();

            if (_mr == null)
            {
                _mr = gameObject.AddComponent<MeshRenderer>();
            }

            _marksMesh = new Mesh();
            _marksMesh.MarkDynamic();

            if (_mf == null)
            {
                _mf = gameObject.AddComponent<MeshFilter>();
            }
            _mf.sharedMesh = _marksMesh;

            _vertices = new Vector3[MAX_SKID_MARKS * 4];
            _normals = new Vector3[MAX_SKID_MARKS * 4];
            _tangents = new Vector4[MAX_SKID_MARKS * 4];
            _colors = new Color32[MAX_SKID_MARKS * 4];
            _uvs = new Vector2[MAX_SKID_MARKS * 4];
            _triangles = new int[MAX_SKID_MARKS * 6];

            _mr.shadowCastingMode = ShadowCastingMode.Off;
            _mr.receiveShadows = false;
            _mr.material = SkidmarksMaterial;
            _mr.lightProbeUsage = LightProbeUsage.Off;
        }

        private void Awake()
        {
            if (transform.position != Vector3.zero)
            {
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
            }
        }
    }
}