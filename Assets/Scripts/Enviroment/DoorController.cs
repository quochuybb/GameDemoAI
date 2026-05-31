using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
public class DoorController : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool isDirection =true;

    private Animator animator;
    private int animIsOpenBoolID;
    private int animIs270BoolID;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        animIsOpenBoolID = Animator.StringToHash("IsOpen");
        animIs270BoolID=Animator.StringToHash("IsDirection");

        animator.SetBool(animIs270BoolID,isDirection);
        animator.SetBool(animIs270BoolID, isOpen);
    }

    public bool Interact(GameObject interactor)
    {
        DoorToggle();
        return true;
    }

    private void DoorToggle()
    {
        isOpen = !isOpen;
        animator.SetBool(animIs270BoolID,isDirection);
        animator.SetBool(animIsOpenBoolID, isOpen);
    }
}