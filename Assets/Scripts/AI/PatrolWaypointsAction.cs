using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Unity.Properties;
using System.Collections.Generic;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "PatrolWaypoints",
    story: "[Agent] patrols between [Waypoints] at [Speed]",
    category: "Action/Navigation",
    id: "patrol_waypoints_action")]
public partial class PatrolWaypointsAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;
    [SerializeReference] public BlackboardVariable<float> Speed;

    private NavMeshAgent navAgent;
    private int currentWaypointIndex = 0;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;
        navAgent = Agent.Value.GetComponent<NavMeshAgent>();
        if (navAgent == null) return Status.Failure;

        var waypoints = Waypoints.Value;
        if (waypoints == null || waypoints.Count == 0) return Status.Failure;

        navAgent.speed = Speed.Value;
        navAgent.SetDestination(waypoints[currentWaypointIndex].transform.position);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (navAgent == null) return Status.Failure;

        // Check if reached current waypoint
        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % Waypoints.Value.Count;
            navAgent.SetDestination(Waypoints.Value[currentWaypointIndex].transform.position);
        }

        return Status.Running; // Patrol runs indefinitely
    }

    protected override void OnEnd() { }
}
