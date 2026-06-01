using UnityEngine;
using UnityEngine.AI;

public class MonsterHearing : MonoBehaviour
{
    [Header("Hearing Config")]
    [SerializeField] private AudioPingSet audioPingSet;
    [SerializeField] private float hearingRadius = 8f;
    [SerializeField] private NavMeshAgent navAgent;

    public bool HeardSound { get; private set; }
    public Vector3 LastHeardPosition { get; private set; }
    public float CurrentSourceRadius { get; private set; }
    private Vector3 _lastTrackedPosition;

    void Update()
    {
        if (audioPingSet == null) return;
        Vector3 agentPos = transform.position;
        foreach (var ping in audioPingSet.Pings)
        {
            float dist = Vector3.Distance(agentPos, ping.position);
            if (dist <= hearingRadius + ping.radius)
            {
                if (HeardSound && navAgent != null &&
                    Vector3.Distance(ping.position, _lastTrackedPosition) > 0.5f)
                {
                    navAgent.ResetPath();
                }

                _lastTrackedPosition = ping.position; 
                HeardSound = true;
                LastHeardPosition = ping.position;
                CurrentSourceRadius = ping.radius;
                return;
            }
        }
        HeardSound = false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 agentPos = transform.position;

        if (HeardSound)
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

        if (HeardSound)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(agentPos + Vector3.up * 1.0f, LastHeardPosition + Vector3.up * 0.5f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(LastHeardPosition + Vector3.up * 0.5f, 0.3f);

            Gizmos.color = new Color(1.0f, 0.5f, 0.0f, 0.3f);
            Gizmos.DrawWireSphere(LastHeardPosition + Vector3.up * 0.1f, CurrentSourceRadius);
        }
    }
}