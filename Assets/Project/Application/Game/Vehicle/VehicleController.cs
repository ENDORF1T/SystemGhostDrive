using System;
using UnityEngine;

namespace Project.Application.Game.Vehicle
{
    [RequireComponent(typeof(VehicleInput))]
    public class VehicleController : MonoBehaviour
    {
        [field: Header("Suspension"), Space(10)]
        [field: SerializeField] public float SpringForce { get; private set; } = 30000.0f;
        [field: SerializeField] public float SpringDamper { get; private set; } = 200.0f;

        [field: Header("Car Stats"), Space(10)]
        [field: SerializeField] public float MaxSpeed { get; private set; } = 100.0f;
        [field: SerializeField] public float Acceleration { get; private set; } = 30.0f;
        [field: SerializeField] public AnimationCurve AccelerationCurve { get; private set; } = new AnimationCurve(new Keyframe(-0.001f, 1.000f), new Keyframe(0.136f, 0.357f), new Keyframe(1.006f, 0.094f));
        [field: SerializeField] public float MaxTurnAngle { get; private set; } = 30.0f;
        [field: SerializeField] public AnimationCurve TurnCurve { get; private set; } = new AnimationCurve(new Keyframe(0.000f, 1.000f), new Keyframe(1.000f, 0.100f));
        [field: SerializeField] public float BrakeAcceleration { get; private set; } = 5.0f;
        [field: SerializeField] public float RollingResistance { get; private set; } = 2.0f;
        [field: SerializeField] public float DriftFactor { get; private set; } = 0.2f;
        [field: SerializeField] public float FrictionCoefficient { get; private set; } = 1.0f;
        [field: SerializeField] public AnimationCurve SideFrictionCurve { get; private set; } = new AnimationCurve(new Keyframe(0.000f, 1.000f), new Keyframe(1.000f, 1.000f));
        [field: SerializeField] public AnimationCurve ForwardFrictionCurve { get; private set; } = new AnimationCurve(new Keyframe(0.000f, 1.000f), new Keyframe(1.000f, 2.000f));
        [field: SerializeField] public Transform CenterOfMass_air { get; private set; } = null;
        [field: SerializeField] public bool AutoCounterSteer { get; private set; } = true;
        [field: SerializeField] public float DownForce { get; private set; } = 5.0f;

        [field: Header("Visuals"), Space(10)]
        [field: SerializeField] public Transform VehicleBody { get; private set; } = null;
        [field: Range(0, 10), SerializeField] public float ForwardBodyTilt { get; private set; } = 3.0f;
        [field: Range(0, 10), SerializeField] public float SidewaysBodyTilt { get; private set; } = 4.0f;
        [field: SerializeField] public GameObject WheelSkid { get; private set; } = null;
        [field: SerializeField] public GameObject SkidMarkController { get; private set; } = null;
        [field: SerializeField] public float WheelRadius { get; private set; } = 0.3810216f;
        [field: SerializeField] public float SkidmarkWidth { get; private set; } = 0.3286417f;
        [field: SerializeField] public Transform[] HardPoints { get; private set; } = new Transform[4];
        [field: SerializeField] public Transform[] Wheels { get; private set; } = new Transform[4];

        [Header("Other settings")]
        private RaycastHit[] _wheelHits = new RaycastHit[4];
        private bool _tempGroundedProperty;
        private Vector3 _centreOfMass_ground;
        private float _maxSpringDistance;

        private float _rearTrack;
        private float _wheelBase;
        private float _ackermennLeftAngle; 
        private float _ackermennRightAngle;

        private Vector3 _lastVelocity;
        private int _numberOfGroundedWheels;
        private float[] _offsetPrev = new float[4];

        //Skidmarks
        private float[] _forwardSlip = new float[4];
        private float[] _slipCoeff = new float[4];
        private float[] _skidTotal = new float[4];
        private WheelSkid[] _wheelSkids = new WheelSkid[4];

        private Rigidbody _rb;
        private Vehicle _vehicle = null;

        public Vector3 LocalVehicleVelocity { get; private set; }
        public bool VehicleIsGrounded { get; private set; }

        #region Vehicle Events Stuff

        public bool GroundedProperty
        {
            get
            {
                return _tempGroundedProperty;
            }

            set
            {
                if (value != _tempGroundedProperty)
                {
                    _tempGroundedProperty = value;
                    if (_tempGroundedProperty)
                    {
                        Debug.Log("Grounded");
                        _vehicle.VehicleEvents.OnGrounded.Invoke();
                    }
                    else
                    {
                        Debug.Log("Take off");
                        _vehicle.VehicleEvents.OnTakeOff.Invoke();
                    }
                }


            }
        }
        #endregion

        private void AddAcceleration(float accelerationInput)
        {
            float deltaSpeed = Acceleration * accelerationInput * Time.fixedDeltaTime;
            deltaSpeed = Mathf.Clamp(deltaSpeed, -MaxSpeed, MaxSpeed) * AccelerationCurve.Evaluate(Mathf.Abs(LocalVehicleVelocity.z / MaxSpeed));

            if (accelerationInput > 0 && LocalVehicleVelocity.z < 0 || accelerationInput < 0 && LocalVehicleVelocity.z > 0)
            {
                deltaSpeed = (1 + Mathf.Abs(LocalVehicleVelocity.z / MaxSpeed)) * Acceleration * accelerationInput * Time.fixedDeltaTime;
            }
            if (_vehicle.Input.Brake < 0.1f && LocalVehicleVelocity.z < MaxSpeed)
            {
                _rb.velocity += transform.forward * deltaSpeed;
            }

        }

        private void AddRollingResistance()
        {
            float localSpeed = Vector3.Dot(_rb.velocity, transform.forward);

            float deltaSpeed = RollingResistance * Time.fixedDeltaTime * Mathf.Clamp01(Mathf.Abs(localSpeed));
            deltaSpeed = Mathf.Clamp(deltaSpeed, -MaxSpeed, MaxSpeed);
            if (_vehicle.Input.Acceleration == 0)
            {
                if (localSpeed > 0)
                {
                    _rb.velocity -= transform.forward * deltaSpeed;
                }
                else
                {
                    _rb.velocity += transform.forward * deltaSpeed;
                }
            }

        }

        private void BrakeLogic(float brakeInput)
        {
            float localSpeed = Vector3.Dot(_rb.velocity, transform.forward);

            float deltaSpeed = BrakeAcceleration * brakeInput * Time.fixedDeltaTime * Mathf.Clamp01(Mathf.Abs(localSpeed));
            deltaSpeed = Mathf.Clamp(deltaSpeed, -MaxSpeed, MaxSpeed);
            if (localSpeed > 0)
            {
                _rb.velocity -= transform.forward * deltaSpeed;
            }
            else
            {
                _rb.velocity += transform.forward * deltaSpeed;
            }

        }

        private void AddSuspensionForce_2(Vector3 hardPoint, Transform wheel, float MaxSpringDistance, out RaycastHit wheelHit, out bool WheelIsGrounded, out float SuspensionForce, int WheelNum)
        {
            var direction = -transform.up;

            if (Physics.SphereCast(hardPoint + (transform.up * WheelRadius), WheelRadius, direction, out wheelHit, MaxSpringDistance))
            {
                WheelIsGrounded = true;
            }
            else
            {
                WheelIsGrounded = false;
            }

            // suspension spring force
            if (WheelIsGrounded)
            {
                Vector3 springDir = wheelHit.normal;
                float offset = (MaxSpringDistance + 0.1f - wheelHit.distance) / (MaxSpringDistance - WheelRadius - 0.1f);

                float vel = -((offset - _offsetPrev[WheelNum]) / Time.fixedDeltaTime);

                Vector3 wheelWorldVel = _rb.GetPointVelocity(wheelHit.point);
                float WheelVel = Vector3.Dot(springDir, wheelWorldVel);

                _offsetPrev[WheelNum] = offset;
                if (offset < 0.3f)
                {
                    vel = 0;
                }
                else if (vel < 0 && offset > 0.6f && WheelVel < 10)
                {
                    vel *= 10;
                }

                float TotalSpringForce = offset * offset * SpringForce;
                float totalDampingForce = Mathf.Clamp(-(vel * SpringDamper), -0.25f * _rb.mass * Mathf.Abs(WheelVel) / Time.fixedDeltaTime, 0.25f * _rb.mass * Mathf.Abs(WheelVel) / Time.fixedDeltaTime);
                if ((MaxSpringDistance + 0.1f - wheelHit.distance) < 0.1f)
                {
                    totalDampingForce = 0;
                }
                float force = TotalSpringForce + totalDampingForce;
                SuspensionForce = force;

                _rb.AddForceAtPosition(springDir * force, hardPoint);

            }
            else
            {
                SuspensionForce = 0;
            }

        }
        private void AddLateralFriction_2(Vector3 hardPoint, Transform wheel, RaycastHit wheelHit, bool wheelIsGrounded, float factor, float suspensionForce, int wheelNum)
        {
            if (wheelIsGrounded)
            {
                Vector3 SurfaceNormal = wheelHit.normal;

                Vector3 sideVelocity = (wheel.InverseTransformDirection(_rb.GetPointVelocity(hardPoint)).x) * wheel.right;
                Vector3 forwardVelocity = (wheel.InverseTransformDirection(_rb.GetPointVelocity(hardPoint)).z) * wheel.forward;

                _slipCoeff[wheelNum] = sideVelocity.magnitude / (sideVelocity.magnitude + Mathf.Clamp(forwardVelocity.magnitude, 0.1f, forwardVelocity.magnitude));

                Vector3 contactDesiredAccel = -Vector3.ProjectOnPlane(sideVelocity, SurfaceNormal) / Time.fixedDeltaTime;

                Vector3 frictionForce = Vector3.ClampMagnitude(_rb.mass * contactDesiredAccel * SideFrictionCurve.Evaluate(_slipCoeff[wheelNum]), suspensionForce * FrictionCoefficient);
                frictionForce = suspensionForce * FrictionCoefficient * -sideVelocity.normalized * SideFrictionCurve.Evaluate(_slipCoeff[wheelNum]);

                float clampedFrictionForce = Mathf.Min(_rb.mass / 4 * contactDesiredAccel.magnitude, -Physics.gravity.y * _rb.mass);

                frictionForce = Vector3.ClampMagnitude(frictionForce * ForwardFrictionCurve.Evaluate(forwardVelocity.magnitude / MaxSpeed), clampedFrictionForce);
                _rb.AddForceAtPosition(frictionForce * factor, hardPoint);
            }

        }

        private void AckermannSteering(float steerInput)
        {
            float turnRadius = _wheelBase / Mathf.Tan(MaxTurnAngle / Mathf.Rad2Deg);
            if (steerInput > 0) //is turning right
            {
                _ackermennLeftAngle = Mathf.Rad2Deg * Mathf.Atan(_wheelBase / (turnRadius + (_rearTrack / 2))) * steerInput;
                _ackermennRightAngle = Mathf.Rad2Deg * Mathf.Atan(_wheelBase / (turnRadius - (_rearTrack / 2))) * steerInput;
            }
            else if (steerInput < 0) //is turning left
            {
                _ackermennLeftAngle = Mathf.Rad2Deg * Mathf.Atan(_wheelBase / (turnRadius - (_rearTrack / 2))) * steerInput;
                _ackermennRightAngle = Mathf.Rad2Deg * Mathf.Atan(_wheelBase / (turnRadius + (_rearTrack / 2))) * steerInput;
            }
            else
            {
                _ackermennLeftAngle = 0;
                _ackermennRightAngle = 0;
            }

            // auto counter steering
            if (LocalVehicleVelocity.z > 0 && AutoCounterSteer && Mathf.Abs(LocalVehicleVelocity.x) > 1f)
            {
                _ackermennLeftAngle += Vector3.SignedAngle(transform.forward, _rb.velocity + transform.forward, transform.up);
                _ackermennLeftAngle = Mathf.Clamp(_ackermennLeftAngle, -70, 70);
                _ackermennRightAngle += Vector3.SignedAngle(transform.forward, _rb.velocity + transform.forward, transform.up);
                _ackermennRightAngle = Mathf.Clamp(_ackermennRightAngle, -70, 70);
            }

            Wheels[0].localRotation = Quaternion.Euler(0, _ackermennLeftAngle * TurnCurve.Evaluate(LocalVehicleVelocity.z / MaxSpeed), 0);
            Wheels[1].localRotation = Quaternion.Euler(0, _ackermennRightAngle * TurnCurve.Evaluate(LocalVehicleVelocity.z / MaxSpeed), 0);
        }

        private void TireVisual(bool WheelIsGrounded, Transform wheel, Transform hardPoint, float hitDistance, int tireNum)
        {
            if (WheelIsGrounded)
            {
                if (_offsetPrev[tireNum] > 0.3f)
                {
                    wheel.localPosition = hardPoint.localPosition + (Vector3.up * WheelRadius) - Vector3.up * (hitDistance);
                }
                else
                {
                    wheel.localPosition = Vector3.Lerp(new Vector3(hardPoint.localPosition.x, wheel.localPosition.y, hardPoint.localPosition.z), hardPoint.localPosition + (Vector3.up * WheelRadius) - Vector3.up * (hitDistance), 0.1f);
                }

            }
            else
            {
                wheel.localPosition = Vector3.Lerp(new Vector3(hardPoint.localPosition.x, wheel.localPosition.y, hardPoint.localPosition.z), hardPoint.localPosition + (Vector3.up * WheelRadius) - Vector3.up * _maxSpringDistance, 0.05f);
            }

            Vector3 wheelVelocity = _rb.GetPointVelocity(hardPoint.position);
            float minRotation = (Vector3.Dot(wheelVelocity, wheel.forward) / WheelRadius) * Time.fixedDeltaTime * Mathf.Rad2Deg;
            float maxRotation = (Mathf.Sign(Vector3.Dot(wheelVelocity, wheel.forward)) * MaxSpeed / WheelRadius) * Time.fixedDeltaTime * Mathf.Rad2Deg;
            float wheelRotation = 0;

            if (_vehicle.Input.Brake > 0.1f)
            {
                wheelRotation = 0;
            }
            else if (Mathf.Abs(_vehicle.Input.Acceleration) > 0.1f)
            {
                wheel.GetChild(0).RotateAround(wheel.position, wheel.right, maxRotation / 2);
                wheelRotation = maxRotation;
            }
            else
            {
                wheel.GetChild(0).RotateAround(wheel.position, wheel.right, minRotation);
                wheelRotation = minRotation;
            }
            wheel.GetChild(0).localPosition = Vector3.zero;
            var rot = wheel.GetChild(0).localRotation;
            rot.y = 0;
            rot.z = 0;
            wheel.GetChild(0).localRotation = rot;

            //wheel slip calculation
            _forwardSlip[tireNum] = Mathf.Abs(Mathf.Clamp((wheelRotation - minRotation) / (maxRotation), -1, 1));
            if (WheelIsGrounded)
            {
                _skidTotal[tireNum] = Mathf.MoveTowards(_skidTotal[tireNum], (_forwardSlip[tireNum] + _slipCoeff[tireNum]) / 2, 0.05f);
            }
            else
            {
                _skidTotal[tireNum] = 0;
            }
        }

        private void SetWheelSkidvalues_Start(int wheelNum, Skidmarks skidmarks, float radius)
        {
            _wheelSkids[wheelNum].Skidmarks = skidmarks;
            _wheelSkids[wheelNum].Radius = WheelRadius;
        }

        private void SetWheelSkidvalues_Update(int wheelNum, float skidTotal, Vector3 skidPoint, Vector3 normal)
        {
            _wheelSkids[wheelNum].SkidTotal = skidTotal;
            _wheelSkids[wheelNum].SkidPoint = skidPoint;
            _wheelSkids[wheelNum].Normal = normal;
        }

        private void BodyAnimation()
        {
            Vector3 accel = Vector3.ProjectOnPlane((_rb.velocity - _lastVelocity) / Time.fixedDeltaTime, transform.up);
            accel = transform.InverseTransformDirection(accel);
            _lastVelocity = _rb.velocity;

            VehicleBody.localRotation = Quaternion.Lerp(VehicleBody.localRotation, Quaternion.Euler(Mathf.Clamp(-accel.z / 10, -ForwardBodyTilt, ForwardBodyTilt), 0, Mathf.Clamp(accel.x / 5, -SidewaysBodyTilt, SidewaysBodyTilt)), 0.1f);
        }

        private void GroundedCheckPerWheel(bool wheelIsGrounded)
        {
            if (wheelIsGrounded)
            {
                _numberOfGroundedWheels += 1;
            }
        }

        private void FixedUpdate()
        {
            LocalVehicleVelocity = transform.InverseTransformDirection(_rb.velocity);

            AckermannSteering(_vehicle.Input.Steer);

            float suspensionForce = 0;
            for (int i = 0; i < Wheels.Length; i++)
            {
                bool wheelIsGrounded = false;

                AddSuspensionForce_2(HardPoints[i].position, Wheels[i], _maxSpringDistance, out _wheelHits[i], out wheelIsGrounded, out suspensionForce, i);

                GroundedCheckPerWheel(wheelIsGrounded);

                TireVisual(wheelIsGrounded, Wheels[i], HardPoints[i], _wheelHits[i].distance, i);
                SetWheelSkidvalues_Update(i, _skidTotal[i], _wheelHits[i].point, _wheelHits[i].normal);

            }

            VehicleIsGrounded = (_numberOfGroundedWheels > 1);

            if (VehicleIsGrounded)
            {
                AddAcceleration(_vehicle.Input.Acceleration);
                AddRollingResistance();
                BrakeLogic(_vehicle.Input.Brake);
                BodyAnimation();

                //AutoBalence
                if (_rb.centerOfMass != _centreOfMass_ground)
                {
                    _rb.centerOfMass = _centreOfMass_ground;
                }

                // angular drag
                _rb.angularDrag = 1;

                //downforce
                _rb.AddForce(-transform.up * DownForce * _rb.mass);
            }
            else
            {
                if (_rb.centerOfMass != CenterOfMass_air.localPosition)
                {
                    _rb.centerOfMass = CenterOfMass_air.localPosition;
                }

                // angular drag
                _rb.angularDrag = 0.1f;
            }

            //friction
            for (int i = 0; i < Wheels.Length; i++)
            {
                if (i < 2)
                {
                    AddLateralFriction_2(HardPoints[i].position, Wheels[i], _wheelHits[i], VehicleIsGrounded, 1, suspensionForce, i);
                }
                else
                {
                    if (_vehicle.Input.Brake > 0.1f)
                    {
                        AddLateralFriction_2(HardPoints[i].position, Wheels[i], _wheelHits[i], VehicleIsGrounded, DriftFactor, suspensionForce, i);
                    }
                    else
                    {
                        AddLateralFriction_2(HardPoints[i].position, Wheels[i], _wheelHits[i], VehicleIsGrounded, 1, suspensionForce, i);
                    }

                }
            }


            _numberOfGroundedWheels = 0; //reset grounded int


            //grounded property for event
            if (GroundedProperty != VehicleIsGrounded)
            {
                GroundedProperty = VehicleIsGrounded;
            }

        }

        private void Awake()
        {
            GameObject SkidMarkController_Self = Instantiate(SkidMarkController);
            SkidMarkController_Self.GetComponent<Skidmarks>().SkidmarkWidth = SkidmarkWidth;

            _vehicle = GetComponent<Vehicle>();
            _rb = GetComponent<Rigidbody>();

            _lastVelocity = Vector3.zero;


            for (int i = 0; i < Wheels.Length; i++)
            {
                HardPoints[i].localPosition = new Vector3(Wheels[i].localPosition.x, 0, Wheels[i].localPosition.z);

                _wheelSkids[i] = Instantiate(WheelSkid, Wheels[i].GetChild(0)).GetComponent<WheelSkid>();
                SetWheelSkidvalues_Start(i, SkidMarkController_Self.GetComponent<Skidmarks>(), WheelRadius);
            }
            _maxSpringDistance = Mathf.Abs(Wheels[0].localPosition.y - HardPoints[0].localPosition.y) + 0.1f + WheelRadius;

            _wheelBase = Vector3.Distance(Wheels[0].position, Wheels[2].position);
            _rearTrack = Vector3.Distance(Wheels[0].position, Wheels[1].position);
        }


        private void Start()
        {
            _centreOfMass_ground = (HardPoints[0].localPosition + HardPoints[1].localPosition + HardPoints[2].localPosition + HardPoints[3].localPosition) / 4;

            _rb.centerOfMass = _centreOfMass_ground;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < Wheels.Length; i++)
            {
                Gizmos.DrawLine(HardPoints[i].position + (transform.up * WheelRadius), Wheels[i].position);
                Gizmos.DrawWireSphere(Wheels[i].position, WheelRadius);
                Gizmos.DrawSphere(HardPoints[i].position + (transform.up * WheelRadius), 0.05f);
            }
        }
    }
}