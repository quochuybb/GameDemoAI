using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Change Agent Speed",
    story: "Change [Agent] speed to [Speed]",
    category: "Action/Navigation",
    id: "change_agent_speed_action")]
public partial class ChangeAgentSpeedAction : Action
{
    [SerializeReference] public BlackboardVariable<NavMeshAgent> Agent;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(6f);

    protected override Status OnStart()
    {
        if (Agent == null || Agent.Value == null) return Status.Failure;

        // Thay đổi tốc độ của NavMeshAgent
        Agent.Value.speed = Speed.Value;
        
        // Trả về Success ngay lập tức để chuyển sang hành động tiếp theo (ví dụ Navigate To Pos)
        return Status.Success;
    }
}
