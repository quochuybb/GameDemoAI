using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyDoorInteractor : MonoBehaviour
{
    [Header("Door Interaction")]
    [SerializeField] private float doorDetectDistance;
    [SerializeField] private LayerMask doorLayer;   
    [SerializeField] private float timeToWaitForDoor;         
    [SerializeField] private float distanceToCloseDoor;
    [Range(0f, 1f)]
    [SerializeField] private float closeProbability; 
    [Header("Flag")]
    public bool isPatrolling = true; 



    private NavMeshAgent agent;
    private bool isCrossingLink = false; 
    private bool isWaitingForDoor = false; 
    
    private DoorController currentlyTrackingDoor;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false; 
    }

    private void Update()
    {
        if (!isWaitingForDoor)
        {
            TryOpenDoorWithRaycast();

            if (agent.isOnOffMeshLink && !isCrossingLink)
            {
                StartCoroutine(HandleLinkTraversal());
            }
        }
    }

    private void TryOpenDoorWithRaycast()
    {
        Vector3 moveDirection = agent.desiredVelocity.normalized;

        if (moveDirection.sqrMagnitude < 0.01f) return; 

        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, moveDirection, out RaycastHit hit, doorDetectDistance, doorLayer))
        {
            DoorController door = hit.collider.GetComponent<DoorController>();

            if (door != null)
            {
                if (!door.IsOpen)
                {
                    StartCoroutine(PauseAndOpenDoor(door));
                }
                else if (door != currentlyTrackingDoor)
                {
                    StartCoroutine(CheckAndCloseDoorBehind(door));
                }
            }
        }
    }

    private IEnumerator PauseAndOpenDoor(DoorController door)
    {
        isWaitingForDoor = true;
        agent.isStopped = true;

        door.Interact(this.gameObject); 

        yield return new WaitForSeconds(timeToWaitForDoor);

        agent.isStopped = false; 
        isWaitingForDoor = false;

        StartCoroutine(CheckAndCloseDoorBehind(door));
    }

    private IEnumerator HandleLinkTraversal()
    {
        isCrossingLink = true;
        isWaitingForDoor = true; 

        OffMeshLinkData linkData = agent.currentOffMeshLinkData;
        
        DoorController openedDoor = null;
        Collider[] colliders = Physics.OverlapSphere(linkData.endPos, 3, doorLayer);
        
        foreach (var col in colliders)
        {
            DoorController door = col.GetComponent<DoorController>();
            if (door != null)
            {
                if (!door.IsOpen)
                {
                    door.Interact(this.gameObject); 
                    openedDoor = door;
                }
                else
                {
                    openedDoor = door;
                }
            }
        }

        if (openedDoor != null && !openedDoor.IsOpen)
        {
            yield return new WaitForSeconds(timeToWaitForDoor); 
        }

        Vector3 endPos = linkData.endPos + Vector3.up * agent.baseOffset;

        while (transform.position != endPos)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                endPos, 
                agent.speed * Time.deltaTime
            );
            yield return null;
        }

        agent.CompleteOffMeshLink();
        
        isCrossingLink = false;
        isWaitingForDoor = false;

        if (openedDoor != null)
        {
            StartCoroutine(CheckAndCloseDoorBehind(openedDoor));
        }
    }

    private IEnumerator CheckAndCloseDoorBehind(DoorController door)
    {
        currentlyTrackingDoor = door;

        if (!isPatrolling)
        {
            currentlyTrackingDoor = null;
            yield break;
        }

        if (Random.value > closeProbability)
        {
            currentlyTrackingDoor = null;
            yield break;
        }

        while (door != null && Vector3.Distance(transform.position, door.transform.position) < distanceToCloseDoor)
        {
            yield return null; 
        }

        if (door != null && door.IsOpen)
        {
            isWaitingForDoor = true;
            agent.isStopped = true; 

            door.Interact(this.gameObject); 

            yield return new WaitForSeconds(timeToWaitForDoor); 

            agent.isStopped = false; 
            isWaitingForDoor = false;
        }

        currentlyTrackingDoor = null;
    }
}