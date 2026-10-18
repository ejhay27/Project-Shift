using UnityEngine;

namespace ProjectShift.Game
{
    /// <summary>
    /// Base class for anything the player can interact with (UI-003).
    /// Subclasses override Prompt + Interact (nodes, doors, NPCs, loot...).
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        [SerializeField] private float _radius = 3f;

        public float Radius => _radius;

        public virtual string Prompt => "Interact";

        public virtual bool CanInteract(GameObject user) => true;

        public virtual void Interact(GameObject user) { }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
