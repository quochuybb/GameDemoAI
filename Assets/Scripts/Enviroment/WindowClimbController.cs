using UnityEngine;

public class WindowClimbController : MonoBehaviour, IInteractable
{
    [Header("Teleport Points")]
    [Tooltip("Place these two empty GameObjects on each side of the window")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Settings")]
    [SerializeField] private float sideDetectThreshold = 0.5f;

    public bool Interact(GameObject interactor)
    {
        // Determine which side the player is on
        Vector3 playerPos = interactor.transform.position;

        float distToA = Vector3.Distance(playerPos, pointA.position);
        float distToB = Vector3.Distance(playerPos, pointB.position);

        // Teleport to the opposite side
        Transform targetPoint = (distToA < distToB) ? pointB : pointA;

        // CharacterController must be disabled to teleport
        CharacterController cc = interactor.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
            interactor.transform.position = targetPoint.position;
            interactor.transform.rotation = targetPoint.rotation;
            cc.enabled = true;
        }
        else
        {
            interactor.transform.position = targetPoint.position;
            interactor.transform.rotation = targetPoint.rotation;
        }

        return true;
    }
}
