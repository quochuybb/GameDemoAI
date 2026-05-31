using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[Condition(
    name: "CheckState",
    story: "[CurrentState] equals [ExpectedState]",
    category: "Conditions/State",
    id: "check_state_condition")]
public partial class CheckStateCondition : Condition
{
    [SerializeReference] public BlackboardVariable<EnemyState> CurrentState;
    [SerializeReference] public BlackboardVariable<EnemyState> ExpectedState;

    public override bool IsTrue()
    {
        return CurrentState.Value == ExpectedState.Value;
    }
}