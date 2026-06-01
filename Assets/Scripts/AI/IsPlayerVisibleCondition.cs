using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[Condition(
    name: "Is Player Visible",
    story: "[Agent] can see [Target]",
    category: "Conditions/Perception",
    id: "is_player_visible_condition")]
public partial class IsPlayerVisibleCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    public override bool IsTrue()
    {
        if (Agent.Value == null || Target.Value == null) return false;

        var vision = Agent.Value.GetComponent<MonsterVision>();
        if (vision == null) return false;

        vision.SetTarget(Target.Value.transform);
        if (vision.CanSeeTarget) Debug.Log("Can see player");
        return vision.CanSeeTarget;
    }
}