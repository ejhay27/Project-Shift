using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectShift.Game
{
    /// <summary>
    /// M1 third-person camera (UI-002): follow + swipe orbit + pinch zoom.
    /// Right-side touch orbit; ignores touches that begin on UI controls so
    /// joystick/buttons never fight the camera (spec §2 gesture rules).
    /// </summary>
    public sealed class CameraRig : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _headOffset = new Vector3(0f, 1.6f, 0f);

        [Header("Orbit")]
        [SerializeField] private float _distance = 5.5f;
        [SerializeField] private float _minDistance = 2.5f;
        [SerializeField] private float _maxDistance = 10f;
        [SerializeField] private float _pitch = 18f;
        [SerializeField] private float _minPitch = -10f;
        [SerializeField] private float _maxPitch = 60f;
        [SerializeField] private float _touchSensitivity = 0.35f;
        [SerializeField] private float _mouseSensitivity = 4f;

        [Header("Collision")]
        [SerializeField] private bool _collide = true;
        [SerializeField] private LayerMask _collisionMask = ~0;
        [SerializeField] private float _collisionRadius = 0.3f;

        private float _yaw;
        private int _orbitTouchId = -1;
        private Vector2 _lastOrbitPos;
        private float _lastPinchDist;

        public float Yaw => _yaw;

        private void LateUpdate()
        {
            if (_target == null) return;
            HandleTouch();
            HandleMouse();
            ApplyCamera();
        }

        public void SetTarget(Transform target) => _target = target;

        private bool TouchBeganOnUi(Vector2 screenPos)
        {
            if (EventSystem.current == null) return false;
            var data = new PointerEventData(EventSystem.current) { position = screenPos };
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            return results.Count > 0;
        }

        private void HandleTouch()
        {
            if (Input.touchCount == 1)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)
                {
                    // Left 40% of screen is the joystick zone; UI touches ignored.
                    if (t.position.x > Screen.width * 0.4f && !TouchBeganOnUi(t.position))
                    {
                        _orbitTouchId = t.fingerId;
                        _lastOrbitPos = t.position;
                    }
                }
                else if (t.fingerId == _orbitTouchId)
                {
                    if (t.phase == TouchPhase.Moved)
                    {
                        Vector2 d = t.deltaPosition;
                        _yaw += d.x * _touchSensitivity;
                        _pitch = Mathf.Clamp(_pitch - d.y * _touchSensitivity,
                            _minPitch, _maxPitch);
                    }
                    else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                    {
                        _orbitTouchId = -1;
                    }
                }
                _lastPinchDist = 0f;
            }
            else if (Input.touchCount == 2)
            {
                _orbitTouchId = -1;
                Touch a = Input.GetTouch(0);
                Touch b = Input.GetTouch(1);
                float dist = Vector2.Distance(a.position, b.position);
                if (_lastPinchDist > 0f)
                {
                    float delta = dist - _lastPinchDist;
                    _distance = Mathf.Clamp(_distance - delta * 0.01f,
                        _minDistance, _maxDistance);
                }
                _lastPinchDist = dist;
            }
            else
            {
                _orbitTouchId = -1;
                _lastPinchDist = 0f;
            }
        }

        private void HandleMouse()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(1))
            {
                _yaw += Input.GetAxis("Mouse X") * _mouseSensitivity;
                _pitch = Mathf.Clamp(_pitch - Input.GetAxis("Mouse Y") * _mouseSensitivity,
                    _minPitch, _maxPitch);
            }
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(wheel) > 0.001f)
                _distance = Mathf.Clamp(_distance - wheel * 4f, _minDistance, _maxDistance);
#endif
        }

        private void ApplyCamera()
        {
            Vector3 head = _target.position + _headOffset;
            Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);
            float dist = _distance;

            if (_collide)
            {
                Vector3 dir = rot * Vector3.back;
                if (Physics.SphereCast(head, _collisionRadius, dir,
                        out var hit, _distance, _collisionMask,
                        QueryTriggerInteraction.Ignore))
                {
                    dist = Mathf.Max(_minDistance, hit.distance - 0.2f);
                }
            }

            transform.position = head + rot * Vector3.back * dist;
            transform.rotation = rot;
        }
    }
}
