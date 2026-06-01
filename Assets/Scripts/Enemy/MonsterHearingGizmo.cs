using UnityEngine;

public class MonsterHearingGizmo : MonoBehaviour
{
    [Header("Config Visuals")]
    public float hearingRadius = 8f;

    [Header("Runtime State (Debug)")]
    public bool isHearingSomething = false;
    public Vector3 lastHeardPosition;
    public float currentSourceRadius = 0f;

    private void OnDrawGizmosSelected()
    {
        Vector3 agentPos = transform.position;

        // 1. Vẽ vòng tròn thính giác của Quái vật (Xanh dương)
        if (isHearingSomething)
        {
            Gizmos.color = new Color(0.0f, 0.5f, 1.0f, 0.2f);
        }
        else
        {
            Gizmos.color = new Color(0.0f, 0.5f, 1.0f, 0.05f);
        }
        Gizmos.DrawSphere(agentPos + Vector3.up * 0.1f, hearingRadius);
        Gizmos.color = new Color(0.0f, 0.7f, 1.0f, 0.4f);
        Gizmos.DrawWireSphere(agentPos + Vector3.up * 0.1f, hearingRadius);

        // 2. Khi nghe thấy, vẽ thêm vòng tròn sóng âm phát ra từ nguồn (Màu cam cảnh báo)
        if (isHearingSomething)
        {
            // Đường kết nối
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(agentPos + Vector3.up * 1.0f, lastHeardPosition + Vector3.up * 0.5f);

            // Vị trí tâm âm thanh
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(lastHeardPosition + Vector3.up * 0.5f, 0.3f);

            // Vẽ lan tỏa bán kính tiếng động của Player lúc đó (Vòng tròn màu cam)
            Gizmos.color = new Color(1.0f, 0.5f, 0.0f, 0.3f);
            Gizmos.DrawWireSphere(lastHeardPosition + Vector3.up * 0.1f, currentSourceRadius);
        }
    }
}