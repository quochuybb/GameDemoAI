using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{


    [Header("Door Interaction")]
    [SerializeField] private float doorDetectDistance = 2.5f;
    [SerializeField] private LayerMask doorLayer;   
    [Tooltip("How long the monster waits for the door animation to finish before moving")]
    [SerializeField] private float timeToWaitForDoor = 1.2f;         

    private NavMeshAgent Agent;
    private bool isCrossingLink = false; 
    private bool isWaitingForDoor = false; 

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Agent.autoTraverseOffMeshLink = false; 
    }


    private void Update()
    {
        if (!isWaitingForDoor)
        {
            TryOpenDoorWithRaycast();

            if (Agent.isOnOffMeshLink && !isCrossingLink)
            {
                StartCoroutine(HandleLinkTraversal());
            }
        }
    }


    private void TryOpenDoorWithRaycast()
    {
        Vector3 moveDirection = Agent.desiredVelocity.normalized;

        if (moveDirection.sqrMagnitude < 0.01f) return; 

        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, moveDirection, out RaycastHit hit, doorDetectDistance, doorLayer))
        {
            DoorController door = hit.collider.GetComponent<DoorController>();

            if (door != null && !door.IsOpen)
            {
                StartCoroutine(PauseAndOpenDoor(door.gameObject));
            }
        }
    }


    private IEnumerator PauseAndOpenDoor(GameObject doorObj)
    {
        isWaitingForDoor = true;
        Agent.isStopped = true;

        OpenDoorObject(doorObj);

        yield return new WaitForSeconds(timeToWaitForDoor);

        Agent.isStopped = false; 
        isWaitingForDoor = false;
    }

    private IEnumerator HandleLinkTraversal()
    {
        isCrossingLink = true;
        isWaitingForDoor = true; 

        OffMeshLinkData linkData = Agent.currentOffMeshLinkData;
        
        bool hadToOpenDoor = false;
        Collider[] colliders = Physics.OverlapSphere(linkData.endPos, 3, doorLayer);
        
        foreach (var col in colliders)
        {
            DoorController door = col.GetComponent<DoorController>();
            if (door != null && !door.IsOpen)
            {
                OpenDoorObject(col.gameObject);
                hadToOpenDoor = true;
            }
        }

        if (hadToOpenDoor)
        {
            yield return new WaitForSeconds(timeToWaitForDoor); 
        }

        Vector3 endPos = linkData.endPos + Vector3.up * Agent.baseOffset;

        while (transform.position != endPos)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                endPos, 
                Agent.speed * Time.deltaTime
            );
            yield return null;
        }

        Agent.CompleteOffMeshLink();
        
        isCrossingLink = false;
        isWaitingForDoor = false;
    }

    private void OpenDoorObject(GameObject doorObj)
    {
        DoorController door = doorObj.GetComponent<DoorController>();

        if (door != null && !door.IsOpen)
        {
            door.Interact(this.gameObject);
        }
    }
}