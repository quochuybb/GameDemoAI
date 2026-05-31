using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Check Player Audible",
    story: "[Agent] hears [Target] moving within [HearingRadius] and saves to [OutputPos]",
    category: "Action/Perception",
    id: "check_player_audible_action")]
public partial class CheckPlayerAudibleAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> HearingRadius = new BlackboardVariable<float>(8f);
    [SerializeReference] public BlackboardVariable<Vector3> OutputPos;

    private Vector3 lastTargetPos;
    private bool isInitialized = false;
    private float moveThreshold = 0.05f; // Di chuyển hơn mức này trong 1 frame thì tính là có tiếng động

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) 
        {
            Debug.LogWarning("CheckPlayerAudible: Agent hoặc Target đang bị Null trong Blackboard!");
            return Status.Failure;
        }

        // Nếu mới chạy lần đầu, lưu lại vị trí ban đầu của Player
        if (!isInitialized)
        {
            lastTargetPos = Target.Value.transform.position;
            isInitialized = true;
            return Status.Failure; // Cần đợi frame tiếp theo để so sánh
        }

        float distToTarget = Vector3.Distance(Agent.Value.transform.position, Target.Value.transform.position);
        float targetMovedDist = Vector3.Distance(Target.Value.transform.position, lastTargetPos);

        // Cập nhật lại vị trí cho lần check sau
        lastTargetPos = Target.Value.transform.position;

        // Bỏ qua nếu đứng quá xa
        if (distToTarget > HearingRadius.Value) return Status.Failure;

        // Nếu Player có sự dịch chuyển đáng kể (phát ra tiếng động)
        if (targetMovedDist > moveThreshold)
        {
            // Lưu lại vị trí phát ra tiếng động vào Blackboard
            OutputPos.Value = Target.Value.transform.position;
            return Status.Success;
        }

        return Status.Failure;
    }
}
