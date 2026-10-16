using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectShift.Game
{
    /// <summary>
    /// M1 virtual joystick (UI-002). uGUI-based, works with touch and mouse
    /// (Editor). Supports fixed and floating origin + runtime scale adjust
    /// (spec §2: adjustable control positions/sizes).
    /// </summary>
    public sealed class TouchJoystick : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("References")]
        [SerializeField] private RectTransform _base;
        [SerializeField] private RectTransform _knob;

        [Header("Config")]
        [SerializeField] private float _radius = 90f;
        [SerializeField] private bool _floatingOrigin;
        [SerializeField] private float _deadZone = 0.15f;

        private Vector2 _originAnchored;
        private int _activePointerId = -1;

        /// <summary>Output vector, magnitude 0..1 (y-up = forward).</summary>
        public Vector2 Output { get; private set; }

        /// <summary>Diameter scale for accessibility (0.75..1.5 typical).</summary>
        public void SetScale(float scale)
        {
            scale = Mathf.Clamp(scale, 0.5f, 2f);
            if (_base != null) _base.localScale = Vector3.one * scale;
        }

        private void Awake()
        {
            if (_base == null) _base = GetComponent<RectTransform>();
            _originAnchored = _base.anchoredPosition;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_activePointerId != -1) return;
            _activePointerId = eventData.pointerId;
            if (_floatingOrigin && RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _base.parent as RectTransform, eventData.position,
                    eventData.pressEventCamera, out var local))
            {
                _base.anchoredPosition = local;
            }
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId) return;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _base, eventData.position, eventData.pressEventCamera, out var local))
                return;

            Vector2 delta = local / _radius;
            if (delta.magnitude > 1f) delta.Normalize();
            if (delta.magnitude < _deadZone) delta = Vector2.zero;
            Output = delta;
            if (_knob != null) _knob.anchoredPosition = delta * _radius;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId) return;
            _activePointerId = -1;
            Output = Vector2.zero;
            if (_knob != null) _knob.anchoredPosition = Vector2.zero;
            if (_floatingOrigin) _base.anchoredPosition = _originAnchored;
        }
    }
}
