using System; // Required for 'Action'
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInput playerInput; 

    public event Action OnInteractPerformed;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool RunInput { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInput = new PlayerInput();

        playerInput.Gameplay.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        playerInput.Gameplay.Move.canceled += ctx => MoveInput = Vector2.zero;

        playerInput.Gameplay.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        playerInput.Gameplay.Look.canceled += ctx => LookInput = Vector2.zero;

        playerInput.Gameplay.Run.performed += ctx => RunInput = true;
        playerInput.Gameplay.Run.canceled += ctx => RunInput = false;

        playerInput.Gameplay.Interact.performed += Interact_Performed;
    }

    private void OnEnable()
    {
        EnableInputs();
    }

    private void OnDisable()
    {
        DisableInputs();
    }

    private void Interact_Performed(InputAction.CallbackContext context)
    {
        OnInteractPerformed?.Invoke();
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.Gameplay.Interact.performed -= Interact_Performed;
        }
    }

    public void EnableInputs()
    {
        if (playerInput != null)
        {
            playerInput.Enable();
        }
    }

    public void DisableInputs()
    {
        if (playerInput != null)
        {
            playerInput.Disable();
        }
    }
}