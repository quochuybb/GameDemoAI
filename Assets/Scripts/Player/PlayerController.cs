using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("AI Hearing Bridge")]
    [SerializeField] private AudioPingSet playerAudioPingSet; 
    [SerializeField] private float walkAudioRadius = 5f;
    [SerializeField] private float sprintAudioRadius = 12f;
    [SerializeField] private float walkStepInterval = 0.3f;   
    [SerializeField] private float sprintStepInterval = 0.1f; 


    [Header("Camera Setup")]
    public Transform cameraRoot; 
    private float xRotation = 0f;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f; 
    public float gravity = -9.81f; 
    
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 33f; 
    public float staminaRegenRate = 15f;
    [HideInInspector] public float currentStamina;
    private bool isExhausted = false;

    private float velocityY; 
    private CharacterController controller;
    private float footstepTimer = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentStamina = maxStamina;
    }

    void Update()
    {
        if (InputManager.Instance != null && cameraRoot != null)
        {
            Vector2 lookInput = InputManager.Instance.LookInput;
            float mouseX = lookInput.x;
            transform.Rotate(Vector3.up * mouseX);
            float mouseY = lookInput.y;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f); 
            cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        Vector2 inputDir = Vector2.zero;
        if (InputManager.Instance != null) inputDir = InputManager.Instance.MoveInput;

        bool isMoving = inputDir.magnitude > 0.1f;
        bool isSprintPressed = InputManager.Instance != null && InputManager.Instance.RunInput;
        
        if (isExhausted && currentStamina > maxStamina * 0.2f) isExhausted = false;

        bool isSprinting = isMoving && isSprintPressed && !isExhausted;

        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isExhausted = true;
                isSprinting = false;
            }
        }
        else
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                if (currentStamina > maxStamina) currentStamina = maxStamina;
            }
        }

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 move = (transform.right * inputDir.x) + (transform.forward * inputDir.y);

        if (controller.isGrounded && velocityY < 0) velocityY = -2f; 
        velocityY += gravity * Time.deltaTime;

        Vector3 finalVelocity = (move * currentSpeed) + (Vector3.up * velocityY);
        controller.Move(finalVelocity * Time.deltaTime); 

            if (playerAudioPingSet != null)
            {
                if (isMoving)
                {
                    footstepTimer -= Time.deltaTime;
                    if (footstepTimer <= 0f)
                    {
                        playerAudioPingSet.Clear();
                        float currentAudioRadius = isSprinting ? sprintAudioRadius : walkAudioRadius;
                        footstepTimer = isSprinting ? sprintStepInterval : walkStepInterval;
                        playerAudioPingSet.AddPing(transform.position, currentAudioRadius);
                        
                    }
                }
                else
                {
                    playerAudioPingSet.Clear();
                    
                    footstepTimer = isSprintPressed ? sprintStepInterval : walkStepInterval;
                }
            }
    }
    private void OnDrawGizmos()
    {
        if (playerAudioPingSet == null || playerAudioPingSet.Pings == null) return;

        for (int i = 0; i < playerAudioPingSet.Pings.Count; i++)
        {
            AudioPing ping = playerAudioPingSet.Pings[i];

            Gizmos.color = new Color(1.0f, 0.6f, 0.0f, 0.15f);
            Gizmos.DrawSphere(ping.position, ping.radius);

            Gizmos.color = new Color(1.0f, 0.6f, 0.0f, 0.6f);
            Gizmos.DrawWireSphere(ping.position, ping.radius);

            Gizmos.color = Color.orange;
            Gizmos.DrawSphere(ping.position, 0.15f);
        }
    }
}