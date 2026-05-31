using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Navigate To Pos", 
    category: "Action/Navigation", 
    story: "[agent] navigates to [targetPosition], redirects if stuck for [stuckTime]s",
    id: "navigate_to_pos")]
public partial class NavigateToPosAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> agent;
    [SerializeReference] public BlackboardVariable<Vector3> targetPosition;
    
    [SerializeReference] public BlackboardVariable<float> stuckTime = new BlackboardVariable<float>(2.0f);

    private Vector3 lastPosition;
    private float stuckTimer;

    protected override Status OnStart()
    {
        if (agent == null || agent.Value == null) return Status.Failure;
        
        agent.Value.SetDestination(targetPosition.Value);

        lastPosition = agent.Value.transform.position;
        stuckTimer = 0f;
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (agent == null || agent.Value == null) return Status.Failure;

        if (!agent.Value.pathPending && agent.Value.remainingDistance <= agent.Value.stoppingDistance)
        {
            return Status.Success;
        }

        if (agent.Value.isStopped)
        {
            ResetStuckDetection();
            return Status.Running;
        }

        float distanceMoved = Vector3.Distance(agent.Value.transform.position, lastPosition);
        
        if (distanceMoved > 0.5f) 
        {
            ResetStuckDetection();
        }
        else
        {
            stuckTimer += Time.deltaTime;
            
            if (stuckTimer >= stuckTime.Value)
            {
                Debug.Log("Monster stuck! Redirecting");
                return Status.Failure; 
            }
        }

        return Status.Running;
    }

    private void ResetStuckDetection()
    {
        lastPosition = agent.Value.transform.position;
        stuckTimer = 0f;
    }
}