using UnityEngine;

public class MonsterHearingGizmo : MonoBehaviour
{
    [Header("Config Visuals")]
    public float hearingRadius = 8f;

    [Header("Runtime State (Debug)")]
    public bool isHearingSomething = false;
    public Vector3 lastHeardPosition;

    private float pulseTimer = 0f;

    private void OnDrawGizmosSelected()
    {
        Vector3 agentPos = transform.position;

        // 1. Vẽ vùng bán kính thính giác (Màu xanh dương / Lam)
        // Nếu đang nghe thấy âm thanh, vòng tròn sẽ đổi màu đậm hơn hoặc nháy nhẹ
        if (isHearingSomething)
        {
            pulseTimer += Time.deltaTime * 5f;
            float pulse = Mathf.Sin(pulseTimer) * 0.1f + 0.9f; // Tạo hiệu ứng mạch đập nhẹ
            Gizmos.color = new Color(0.0f, 0.5f, 1.0f, 0.25f * pulse);
        }
        else
        {
            Gizmos.color = new Color(0.0f, 0.5f, 1.0f, 0.08f);
        }
        
        // Vẽ khối đặc mờ và viền cho vùng thính giác
        Gizmos.DrawSphere(agentPos + Vector3.up * 0.1f, hearingRadius);
        Gizmos.color = new Color(0.0f, 0.7f, 1.0f, 0.5f);
        Gizmos.DrawWireSphere(agentPos + Vector3.up * 0.1f, hearingRadius);

        // 2. Vẽ điểm đánh dấu vị trí âm thanh thu được (Audio Trace Landmark)
        if (isHearingSomething)
        {
            // Vẽ một đường nối từ quái vật tới nguồn âm thanh
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(agentPos + Vector3.up * 1.0f, lastHeardPosition + Vector3.up * 0.5f);

            // Vẽ một khối cầu nhỏ tại vị trí Player phát ra tiếng động
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lastHeardPosition + Vector3.up * 0.5f, 0.4f);
            
            // Vẽ thêm chữ hoặc ký hiệu nếu muốn (Tùy chọn hiển thị)
            Gizmos.DrawLine(lastHeardPosition, lastHeardPosition + Vector3.up * 1.5f);
        }
    }
}