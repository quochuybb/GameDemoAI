using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Camera Setup")]
    public Transform cameraRoot; 
    private float xRotation = 0f;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f; 
    
    private float velocityY; 

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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
        if (InputManager.Instance != null)
        {
            inputDir = InputManager.Instance.MoveInput;
        }

        Vector3 move = (transform.right * inputDir.x) + (transform.forward * inputDir.y);

        if (controller.isGrounded && velocityY < 0)
        {
            velocityY = -2f; 
        }
        velocityY += gravity * Time.deltaTime;

        Vector3 finalVelocity = (move * moveSpeed) + (Vector3.up * velocityY);
        controller.Move(finalVelocity * Time.deltaTime); 
    }
}