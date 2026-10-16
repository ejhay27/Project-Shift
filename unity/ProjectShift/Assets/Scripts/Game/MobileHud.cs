using UnityEngine;
using UnityEngine.UI;

namespace ProjectShift.Game
{
    /// <summary>
    /// M1 HUD (UI-001/UI-003): interaction prompt, toast messages, and button
    /// wiring hooks. Built by PrototypeSceneBuilder; styled for landscape
    /// phones + tablets with scalable canvas.
    /// </summary>
    public sealed class MobileHud : MonoBehaviour
    {
        [SerializeField] private Text _promptLabel;
        [SerializeField] private Text _toastLabel;
        [SerializeField] private Button _interactButton;
        [SerializeField] private Button _jumpButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private float _toastSeconds = 2.5f;

        private float _toastUntil;

        public Button InteractButton => _interactButton;
        public Button JumpButton => _jumpButton;
        public Button SaveButton => _saveButton;

        public void SetPrompt(string prompt)
        {
            if (_promptLabel == null) return;
            _promptLabel.text = prompt ?? string.Empty;
            _promptLabel.gameObject.SetActive(!string.IsNullOrEmpty(prompt));
            if (_interactButton != null)
                _interactButton.gameObject.SetActive(!string.IsNullOrEmpty(prompt));
        }

        public void ShowToast(string message)
        {
            if (_toastLabel == null) return;
            _toastLabel.text = message ?? string.Empty;
            _toastLabel.gameObject.SetActive(!string.IsNullOrEmpty(message));
            _toastUntil = Time.time + _toastSeconds;
        }

        private void Update()
        {
            if (_toastLabel != null && _toastLabel.gameObject.activeSelf
                && Time.time >= _toastUntil)
            {
                _toastLabel.gameObject.SetActive(false);
            }
        }
    }
}
