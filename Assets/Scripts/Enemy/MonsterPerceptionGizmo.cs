using UnityEngine;

public class MonsterPerceptionGizmo : MonoBehaviour
{
    [Header("Config Visuals")]
    public float viewDistance = 15f;
    public float viewAngle = 120f;
    public float eyeHeight = 1.5f;

    [Header("Runtime State (Debug)")]
    public Transform target;
    public bool canSeePlayer = false;

    private void OnDrawGizmosSelected()
    {
        Vector3 agentPos = transform.position;
        Vector3 eyePos = agentPos + Vector3.up * eyeHeight;
        Vector3 forward = transform.forward;

        // 1. Luôn vẽ vòng tròn tầm xa và 2 biên góc nhìn (FOV) để định vị
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.1f);
        Gizmos.DrawWireSphere(agentPos, viewDistance);

        Gizmos.color = Color.yellow;
        Vector3 leftBoundary = Quaternion.AngleAxis(-viewAngle / 2f, Vector3.up) * forward;
        Vector3 rightBoundary = Quaternion.AngleAxis(viewAngle / 2f, Vector3.up) * forward;

        Gizmos.DrawLine(eyePos, eyePos + leftBoundary * viewDistance);
        Gizmos.DrawLine(eyePos, eyePos + rightBoundary * viewDistance);

        // 2. Kiểm tra điều kiện để SNAP tia quét vào Player
        if (target != null)
        {
            Vector3 targetPos = target.position;
            
            // Tính toán nhanh khoảng cách và góc để quyết định xem có vẽ tia quét không
            float distance = Vector3.Distance(agentPos, targetPos);
            Vector3 directionToTarget = (targetPos - agentPos).normalized;
            float angle = Vector3.Angle(forward, directionToTarget);

            // CHỈ vẽ tia nối đến Player khi Player nằm TRONG tầm nhìn và TRONG góc nhìn
            if (distance <= viewDistance && angle <= viewAngle / 2f)
            {
                // Thay đổi màu sắc dựa vào kết quả quét thực tế từ Behavior Graph Action
                Gizmos.color = canSeePlayer ? Color.green : Color.red;
                
                // Vẽ 3 tia snap chính xác vào Đầu, Ngực, Chân của Player
                Gizmos.DrawLine(eyePos, targetPos + Vector3.up * 1.8f);
                Gizmos.DrawLine(eyePos, targetPos + Vector3.up * 1.0f);
                Gizmos.DrawLine(eyePos, targetPos + Vector3.up * 0.2f);
            }
        }
    }
}