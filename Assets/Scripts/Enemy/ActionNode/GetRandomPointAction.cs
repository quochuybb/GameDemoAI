using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Get Random Point Around Agent", 
    category: "Action/Navigation", 
    story: "Find random point around [agentObject] within [radius] and save to [outputTarget]",
    id: "get_random_point_around_agent")]
public partial class GetRandomPointAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> agentObject;
    [SerializeReference] public BlackboardVariable<float> radius;
    [SerializeReference] public BlackboardVariable<Vector3> outputTarget;

    protected override Status OnStart()
    {
        if (agentObject == null || agentObject.Value == null)
        {
            return Status.Failure;
        }

        Vector3 origin = agentObject.Value.transform.position;
        Vector3 foundPosition = origin;
        bool foundValidPoint = false;

        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius.Value;
            Vector3 candidatePoint = origin + randomDirection;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidatePoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                foundPosition = hit.position;
                foundValidPoint = true;
                break;
            }
        }

        if (foundValidPoint)
        {
            outputTarget.Value = foundPosition;
            return Status.Success;
        }
        return Status.Failure;
    }
}