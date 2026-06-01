using UnityEngine;

public class MonsterVision : MonoBehaviour
{
    [Header("Vision Config")]
    [SerializeField] private float viewDistance = 15f;
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private float eyeHeight = 1.5f;

    public bool CanSeeTarget { get; private set; }
    public Vector3 LastSeenPosition { get; private set; }

    private Transform _target;

    public void SetTarget(Transform target) => _target = target;

    void Update()
    {
        if (_target == null) { CanSeeTarget = false; return; }

        Vector3 agentPos = transform.position;
        Vector3 targetPos = _target.position;

        float distance = Vector3.Distance(agentPos, targetPos);
        if (distance > viewDistance) { CanSeeTarget = false; return; }

        Vector3 directionToTarget = (targetPos - agentPos).normalized;
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        if (angle > viewAngle / 2f) { CanSeeTarget = false; return; }

        Vector3 eyePos = agentPos + Vector3.up * eyeHeight;
        if (CheckLineOfSight(eyePos, targetPos + Vector3.up * 1.8f) ||
            CheckLineOfSight(eyePos, targetPos + Vector3.up * 1.0f) ||
            CheckLineOfSight(eyePos, targetPos + Vector3.up * 0.2f))
        {
            CanSeeTarget = true;
            LastSeenPosition = targetPos;
            return;
        }

        CanSeeTarget = false;
    }

    private bool CheckLineOfSight(Vector3 origin, Vector3 targetPoint)
    {
        Vector3 rayDir = (targetPoint - origin).normalized;
        float rayDist = Vector3.Distance(origin, targetPoint);
        RaycastHit[] hits = Physics.RaycastAll(origin, rayDir, rayDist,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

        bool isBlockedByWall = false;
        bool hitTarget = false;

        foreach (var hit in hits)
        {
            if (hit.transform.IsChildOf(transform)) continue;
            if (hit.transform.IsChildOf(_target)) { hitTarget = true; continue; }
            isBlockedByWall = true;
            break;
        }

        return (hitTarget && !isBlockedByWall) || (!isBlockedByWall);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 agentPos = transform.position;
        Vector3 eyePos = agentPos + Vector3.up * eyeHeight;
        Vector3 forward = transform.forward;

        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.1f);
        Gizmos.DrawWireSphere(agentPos, viewDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(eyePos, eyePos + Quaternion.AngleAxis(-viewAngle / 2f, Vector3.up) * forward * viewDistance);
        Gizmos.DrawLine(eyePos, eyePos + Quaternion.AngleAxis( viewAngle / 2f, Vector3.up) * forward * viewDistance);

        if (_target == null) return;

        Vector3 targetPos = _target.position;
        float distance = Vector3.Distance(agentPos, targetPos);
        float angle = Vector3.Angle(forward, (targetPos - agentPos).normalized);

        if (distance <= viewDistance && angle <= viewAngle / 2f)
        {
            Gizmos.color = CanSeeTarget ? Color.green : Color.red;
            Gizmos.DrawLine(eyePos, targetPos + Vector3.up * 1.8f);
            Gizmos.DrawLine(eyePos, targetPos + Vector3.up * 1.0f);
            Gizmos.DrawLine(eyePos, targetPos + Vector3.up * 0.2f);
        }
    }
}