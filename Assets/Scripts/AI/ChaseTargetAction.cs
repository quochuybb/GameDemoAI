using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "ChaseTarget",
    story: "[Agent] chases [Target] at [Speed]",
    category: "Action/Navigation",
    id: "chase_target_action")]
public partial class ChaseTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Speed;

    private NavMeshAgent navAgent;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) return Status.Failure;
        navAgent = Agent.Value.GetComponent<NavMeshAgent>();
        if (navAgent == null) return Status.Failure;

        navAgent.speed = Speed.Value;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (navAgent == null || Target.Value == null) return Status.Failure;

        navAgent.SetDestination(Target.Value.transform.position);
        
        // Trả về Success ngay lập tức để Behavior Tree kết thúc nhánh này 
        // và vòng lặp On Start sẽ chạy lại từ đầu để kiểm tra tầm nhìn (CheckPlayerVisible) ở frame tiếp theo.
        return Status.Success; 
    }

    protected override void OnEnd() { }
}