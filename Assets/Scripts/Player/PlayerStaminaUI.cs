using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Kéo nhân vật Player vào đây")]
    public PlayerController playerController;
    
    [Tooltip("Kéo thanh Slider UI vào đây")]
    public Slider staminaSlider;

    private void Start()
    {
        // Tự động tìm Player nếu bạn quên không kéo thả vào ô
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        // Tự động tìm Slider nếu bạn gắn code này thẳng vào cục Slider
        if (staminaSlider == null)
        {
            staminaSlider = GetComponent<Slider>();
        }

        // Cài đặt độ dài tối đa của thanh dựa trên max stamina của Player
        if (staminaSlider != null && playerController != null)
        {
            staminaSlider.maxValue = playerController.maxStamina;
            staminaSlider.value = playerController.currentStamina;
        }
    }

    private void Update()
    {
        if (staminaSlider != null && playerController != null)
        {
            // Cập nhật giá trị thanh UI liên tục mỗi khung hình
            staminaSlider.value = playerController.currentStamina;
        }
    }
}
