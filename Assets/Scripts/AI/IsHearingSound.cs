using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[Condition(
    name: "Is Hearing Sound",
    story: "[Agent] hears something and saves to [OutputPos]",
    category: "Conditions/Perception",
    id: "is_hearing_sound_condition")]
public partial class IsHearingSoundCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> OutputPos;

    public override bool IsTrue()
    {
        if (Agent.Value == null) return false;

        var checker = Agent.Value.GetComponent<MonsterHearing>();
        if (checker == null) return false;

        if (checker.HeardSound)
        {
            OutputPos.Value = checker.LastHeardPosition;
            Debug.Log("Can hear player");
            return true;
        }

        return false;
    }
}