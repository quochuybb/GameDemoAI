using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private bool isOpen = false;

    private Animator animator;
    private int animBoolID;

    // Save original transform to prevent animation curves from moving the door
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        animBoolID = Animator.StringToHash("IsOpen");

        // Store the original position/rotation before any animation plays
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;

        animator.SetBool(animBoolID, isOpen);
    }

    private void LateUpdate()
    {
        // Force the door back to its original position every frame
        // This prevents animation curves from teleporting the door
        transform.localPosition = originalLocalPosition;
    }

    public bool Interact(GameObject interactor)
    {
        DoorToggle(interactor);
        return true;
    }

    private void DoorToggle(GameObject interactor)
    {
        isOpen = !isOpen;
        animator.SetBool(animBoolID, isOpen);
    }
}