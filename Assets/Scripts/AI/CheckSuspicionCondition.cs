using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[Condition(
    name: "CheckSuspicion",
    story: "[SuspicionLevel] is greater than [Threshold]",
    category: "Conditions/Perception",
    id: "check_suspicion_condition")]
public partial class CheckSuspicionCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> SuspicionLevel;
    [SerializeReference] public BlackboardVariable<float> Threshold;

    public override bool IsTrue()
    {
        return SuspicionLevel.Value > Threshold.Value;
    }
}