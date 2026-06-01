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
    private float moveThreshold = 0.02f; 

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) 
        {
            Debug.LogWarning("CheckPlayerAudible: Agent hoặc Target đang bị Null trong Blackboard!");
            return Status.Failure;
        }

        // Lấy Component Gizmo từ Agent để đồng bộ hóa
        var hearingGizmo = Agent.Value.GetComponent<MonsterHearingGizmo>();
        if (hearingGizmo != null)
        {
            hearingGizmo.hearingRadius = HearingRadius.Value;
        }

        // Khởi tạo vị trí ban đầu của Player nếu là frame đầu tiên
        if (!isInitialized)
        {
            lastTargetPos = Target.Value.transform.position;
            isInitialized = true;
            if (hearingGizmo != null) hearingGizmo.isHearingSomething = false;
            return Status.Failure;
        }

        Vector3 agentPos = Agent.Value.transform.position;
        Vector3 targetPos = Target.Value.transform.position;

        float distToTarget = Vector3.Distance(agentPos, targetPos);
        float targetMovedDist = Vector3.Distance(targetPos, lastTargetPos);

        // Lưu lại vị trí hiện tại để so sánh cho frame kế tiếp
        lastTargetPos = targetPos;

        // 1. Kiểm tra nếu Player đứng ngoài bán kính thính giác
        if (distToTarget > HearingRadius.Value)
        {
            if (hearingGizmo != null) hearingGizmo.isHearingSomething = false;
            return Status.Failure;
        }

        // 2. Kiểm tra xem Player có đang di chuyển thực sự để tạo tiếng động không
        if (targetMovedDist > moveThreshold)
        {
            // Lưu lại vị trí phát ra âm thanh vào Blackboard công cộng
            OutputPos.Value = targetPos;

            // Đồng bộ sang hệ thống vẽ Gizmo
            if (hearingGizmo != null)
            {
                hearingGizmo.isHearingSomething = true;
                hearingGizmo.lastHeardPosition = targetPos;
            }

            return Status.Success;
        }

        // Nếu Player đứng im (không tạo tiếng động)
        if (hearingGizmo != null) hearingGizmo.isHearingSomething = false;
        return Status.Failure;
    }
}