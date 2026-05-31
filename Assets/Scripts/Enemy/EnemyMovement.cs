using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public Transform Target;
    public float UpdateSpeed = 0.1f; // how frequently to recalculate path based on Target transform's position

    [Header("Door Interaction")]
    [SerializeField] private float doorDetectDistance = 2.5f;  // how far ahead to detect doors
    [SerializeField] private LayerMask doorLayer;              // layer of door objects

    private NavMeshAgent Agent;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        StartCoroutine(FollowTarget());
    }

    private void Update()
    {
        //TryOpenDoor();
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateSpeed);

        while (enabled)
        {
            Agent.SetDestination(Target.transform.position);

            yield return Wait;
        }
    }

    private void TryOpenDoor()
    {
        // Use the agent's desired velocity as raycast direction
        // This is the actual direction the agent wants to move, not where it's facing
        Vector3 moveDirection = Agent.desiredVelocity.normalized;

        if (moveDirection.sqrMagnitude < 0.01f) return; // Not moving

        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, moveDirection, out RaycastHit hit, doorDetectDistance, doorLayer))
        {
            DoorController door = hit.collider.GetComponent<DoorController>();

            if (door != null && !door.IsOpen)
            {
                door.Interact(this.gameObject);
            }
        }
    }
}
