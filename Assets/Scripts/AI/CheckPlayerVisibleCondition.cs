using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Check Player Visible",
    story: "[Agent] can see [Target] within [ViewAngle] degrees and [ViewDistance] range",
    category: "Action/Perception",
    id: "check_player_visible_action")]
public partial class CheckPlayerVisibleAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> ViewAngle = new BlackboardVariable<float>(120f);
    [SerializeReference] public BlackboardVariable<float> ViewDistance = new BlackboardVariable<float>(15f);

    protected override Status OnStart()
    {
        if (Agent.Value == null || Target.Value == null) 
        {
            Debug.LogWarning("CheckPlayerVisible: Agent hoặc Target đang bị Null trong Blackboard!");
            return Status.Failure;
        }

        Vector3 agentPos = Agent.Value.transform.position;
        Vector3 targetPos = Target.Value.transform.position;

        // Tính khoảng cách
        float distance = Vector3.Distance(agentPos, targetPos);
        if (distance > ViewDistance.Value) return Status.Failure;

        // Tính hướng và góc nhìn
        Vector3 directionToTarget = (targetPos - agentPos).normalized;
        float angle = Vector3.Angle(Agent.Value.transform.forward, directionToTarget);
        if (angle > ViewAngle.Value / 2f) return Status.Failure;

        // Mắt quái vật đặt ở độ cao 1.5m
        Vector3 eyePos = agentPos + Vector3.up * 1.5f;

        // 3 Điểm đích trên cơ thể Player: Đầu (1.8m), Ngực (1.0m), Chân (0.2m)
        Vector3 headPos = targetPos + Vector3.up * 1.8f;
        Vector3 chestPos = targetPos + Vector3.up * 1.0f;
        Vector3 footPos = targetPos + Vector3.up * 0.2f;

        // Bắn 3 tia: Chỉ cần 1 trong 3 tia nhìn thấy Player là thành công
        if (CheckLineOfSight(eyePos, headPos) || 
            CheckLineOfSight(eyePos, chestPos) || 
            CheckLineOfSight(eyePos, footPos))
        {
            return Status.Success;
        }

        return Status.Failure;
    }

    private bool CheckLineOfSight(Vector3 origin, Vector3 targetPoint)
    {
        Vector3 rayDir = (targetPoint - origin).normalized;
        float rayDist = Vector3.Distance(origin, targetPoint);

        RaycastHit[] hits = Physics.RaycastAll(origin, rayDir, rayDist, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        
        bool isBlockedByWall = false;
        bool hitPlayer = false;

        foreach (var hit in hits)
        {
            // Bỏ qua nếu tia đụng trúng chính con quái vật
            if (hit.transform.IsChildOf(Agent.Value.transform)) continue;

            // Nếu đụng trúng Player (hoặc các component con của Player)
            if (hit.transform.IsChildOf(Target.Value.transform))
            {
                hitPlayer = true;
                continue;
            }

            // Nếu đụng trúng vật khác (tường, bàn...) -> Có vật cản
            isBlockedByWall = true;
            break; 
        }

        // Trả về true nếu tia chạm Player và không có vật cản, HOẶC không có gì chắn cả
        return (hitPlayer && !isBlockedByWall) || (!isBlockedByWall);
    }
}