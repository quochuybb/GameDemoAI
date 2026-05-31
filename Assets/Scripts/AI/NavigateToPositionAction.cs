using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "NavigateToPosition",
    story: "[Agent] navigates to [Position] at [Speed]",
    category: "Action/Navigation",
    id: "navigate_to_position_action")]
public partial class NavigateToPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> Position;
    [SerializeReference] public BlackboardVariable<float> Speed;

    private NavMeshAgent navAgent;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;
        navAgent = Agent.Value.GetComponent<NavMeshAgent>();
        if (navAgent == null) return Status.Failure;

        navAgent.speed = Speed.Value;
        navAgent.SetDestination(Position.Value);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (navAgent == null) return Status.Failure;

        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            return Status.Success; // Đã đến nơi
        }

        return Status.Running;
    }

    protected override void OnEnd() { }
}
