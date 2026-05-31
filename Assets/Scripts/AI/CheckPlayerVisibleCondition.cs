using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[Condition(
    name: "CheckPlayerVisible",
    story: "[Agent] can see [Target] within [ViewAngle] degrees and [ViewDistance] range",
    category: "Conditions/Perception",
    id: "check_player_visible_condition")]
public partial class CheckPlayerVisibleCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> ViewAngle;
    [SerializeReference] public BlackboardVariable<float> ViewDistance;

    public override bool IsTrue()
    {
        if (Agent.Value == null || Target.Value == null) return false;

        Vector3 directionToTarget = Target.Value.transform.position - Agent.Value.transform.position;
        float distance = directionToTarget.magnitude;

        // Kiểm tra khoảng cách
        if (distance > ViewDistance.Value) return false;

        // Kiểm tra góc nhìn (FOV)
        float angle = Vector3.Angle(Agent.Value.transform.forward, directionToTarget);
        if (angle > ViewAngle.Value / 2f) return false;

        // Kiểm tra line of sight (Raycast)
        Vector3 origin = Agent.Value.transform.position + Vector3.up * 1.5f;
        if (Physics.Raycast(origin, directionToTarget.normalized, out RaycastHit hit, distance))
        {
            if (hit.collider.gameObject == Target.Value)
            {
                return true;
            }
        }

        return false;
    }
}