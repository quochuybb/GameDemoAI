using System;
using UnityEngine;
using Unity.Behavior;

[RequireComponent(typeof(BehaviorGraphAgent))]
public class EnemyPerception : MonoBehaviour
{
    [Header("Perception Settings")]
    [SerializeField] private float viewDistance = 15f;
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private float hearingDistance = 8f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Suspicion Settings")]
    [SerializeField] private float suspicionIncreaseRate = 0.5f;
    [SerializeField] private float suspicionDecreaseRate = 0.2f;
    [SerializeField] private float investigateThreshold = 0.3f;
    [SerializeField] private float chaseThreshold = 0.8f;

    private BehaviorGraphAgent behaviorAgent;
    private GameObject player;
    private float currentSuspicion = 0f;

    private void Awake()
    {
        behaviorAgent = GetComponent<BehaviorGraphAgent>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            behaviorAgent.BlackboardReference.SetVariableValue("Player", player);
        }
    }

    private void Update()
    {
        if (player == null) return;

        bool canSeePlayer = CheckVisibility();
        UpdateSuspicion(canSeePlayer);
        UpdateBlackboard(canSeePlayer);
    }

    private bool CheckVisibility()
    {
        Vector3 dirToPlayer = player.transform.position - transform.position;
        float distance = dirToPlayer.magnitude;

        if (distance > viewDistance) return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > viewAngle / 2f) return false;

        Vector3 origin = transform.position + Vector3.up * 1.5f;
        if (Physics.Raycast(origin, dirToPlayer.normalized, out RaycastHit hit, distance, ~obstacleMask))
        {
            return hit.collider.gameObject == player;
        }

        return false;
    }

    private void UpdateSuspicion(bool canSeePlayer)
    {
        if (canSeePlayer)
        {
            currentSuspicion += suspicionIncreaseRate * Time.deltaTime;
        }
        else
        {
            currentSuspicion -= suspicionDecreaseRate * Time.deltaTime;
        }
        currentSuspicion = Mathf.Clamp01(currentSuspicion);
    }

    private void UpdateBlackboard(bool canSeePlayer)
    {
        // Cập nhật SuspicionLevel
        behaviorAgent.BlackboardReference.SetVariableValue("SuspicionLevel", currentSuspicion);

        if (canSeePlayer)
        {
            // Cập nhật vị trí cuối cùng thấy player
            behaviorAgent.BlackboardReference.SetVariableValue("LastSeenPosition", player.transform.position);
            behaviorAgent.BlackboardReference.SetVariableValue("TargetPosition", player.transform.position);
        }

        // Tự động chuyển state dựa trên suspicion
        EnemyState currentState = EnemyState.Patrol;
        behaviorAgent.BlackboardReference.GetVariableValue("CurrentState", out currentState);

        if (canSeePlayer && currentSuspicion >= chaseThreshold)
        {
            behaviorAgent.BlackboardReference.SetVariableValue("CurrentState", EnemyState.Chasing);
        }
        else if (canSeePlayer && currentSuspicion >= investigateThreshold)
        {
            behaviorAgent.BlackboardReference.SetVariableValue("CurrentState", EnemyState.Investigating);
        }
        else if (!canSeePlayer && currentState == EnemyState.Chasing)
        {
            // Mất tầm nhìn khi đang chase → chuyển sang search
            behaviorAgent.BlackboardReference.SetVariableValue("CurrentState", EnemyState.Searching);
        }
        else if (currentSuspicion <= 0f && currentState != EnemyState.Patrol)
        {
            behaviorAgent.BlackboardReference.SetVariableValue("CurrentState", EnemyState.Patrol);
        }
    }
}
