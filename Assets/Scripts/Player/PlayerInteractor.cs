using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("The point from which the ray is cast (usually the Main Camera)")]
    [SerializeField] private Transform interactionPoint; 
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;

    private void Start()
    {
        InputManager.Instance.OnInteractPerformed += TryInteract;
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnInteractPerformed -= TryInteract;
        }
    }

    private void TryInteract()
    {
        Ray ray = new Ray(interactionPoint.position, interactionPoint.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                interactable.Interact(this.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (interactionPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(interactionPoint.position, interactionPoint.forward * interactionDistance);
        }
    }
}