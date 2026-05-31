using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "LookAround",
    story: "[Agent] looks around for [Duration] seconds",
    category: "Action/Awareness",
    id: "look_around_action")]
public partial class LookAroundAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<float> Duration;

    private float elapsedTime;
    private float rotationSpeed = 90f; // degrees per second

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;
        elapsedTime = 0f;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        elapsedTime += Time.deltaTime;
        Agent.Value.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        if (elapsedTime >= Duration.Value)
        {
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd() { }
}