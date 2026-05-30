using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorController : MonoBehaviour,IInteractable
{


    [Header("Door Settings")]
    [SerializeField] private bool isOpen = false;

    
    private Animator animator;
    private int animBoolID;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        animBoolID = Animator.StringToHash("IsOpen");
        animator.SetBool(animBoolID, isOpen);
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