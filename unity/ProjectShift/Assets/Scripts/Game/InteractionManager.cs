using System.Collections.Generic;
using UnityEngine;

namespace ProjectShift.Game
{
    /// <summary>
    /// M1 interaction scanning (UI-003): finds the nearest usable Interactable
    /// in range at a throttled rate, drives the HUD prompt, and dispatches
    /// the Interact action (E key / on-screen button).
    /// </summary>
    public sealed class InteractionManager : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private float _scanInterval = 0.15f;
        [SerializeField] private MobileHud _hud;

        private readonly List<Interactable> _cache = new List<Interactable>();
        private float _nextScan;
        private Interactable _current;

        public Interactable Current => _current;

        private void Update()
        {
            if (_player == null) return;
            if (Time.time >= _nextScan)
            {
                _nextScan = Time.time + _scanInterval;
                Scan();
            }
            if (Input.GetKeyDown(KeyCode.E)) TryInteract();
        }

        private void Scan()
        {
            _cache.Clear();
            // M1 scale: scene-wide scan at 0.15s is fine. M4 replaces this
            // with region-partitioned queries (PERF-002).
            var all = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
            Interactable best = null;
            float bestDist = float.MaxValue;
            foreach (var it in all)
            {
                if (it == null || !it.isActiveAndEnabled) continue;
                float d = Vector3.Distance(_player.position, it.transform.position);
                if (d > it.Radius) continue;
                if (!it.CanInteract(_player.gameObject)) continue;
                if (d < bestDist) { bestDist = d; best = it; }
            }
            _current = best;
            if (_hud != null) _hud.SetPrompt(best != null ? best.Prompt : string.Empty);
        }

        public void TryInteract()
        {
            if (_current != null && _player != null)
                _current.Interact(_player.gameObject);
        }

        public void Bind(Transform player, MobileHud hud)
        {
            _player = player;
            _hud = hud;
        }
    }
}
