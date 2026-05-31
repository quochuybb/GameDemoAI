using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "SetEnemyState",
    story: "Set [CurrentState] to [NewState]",
    category: "Action/Blackboard",
    id: "set_enemy_state_action")]
public partial class SetEnemyStateAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyState> CurrentState;
    [SerializeReference] public BlackboardVariable<EnemyState> NewState;

    protected override Status OnStart()
    {
        CurrentState.Value = NewState.Value;
        return Status.Success;
    }

    protected override Status OnUpdate() => Status.Success;
    protected override void OnEnd() { }
}
