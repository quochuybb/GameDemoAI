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
    [SerializeReference] public BlackboardVariable<AudioPingSet> audioPingSet;

    protected override Status OnStart()
    {
        if (Agent.Value == null) return Status.Failure;

            if (audioPingSet == null)
            {
                Debug.LogError("CheckPlayerAudible: Không tìm thấy file 'PlayerAudioPingSet' trong thư mục Assets/Resources!");
                return Status.Failure;
            }


        var hearingGizmo = Agent.Value.GetComponent<MonsterHearingGizmo>();
        if (hearingGizmo != null)
        {
            hearingGizmo.hearingRadius = HearingRadius.Value;
        }

        Vector3 agentPos = Agent.Value.transform.position;

        // Duyệt qua tất cả tiếng động đang có trong màn chơi
        foreach (var ping in audioPingSet.Value.Pings)
        {
            float distToPing = Vector3.Distance(agentPos, ping.position);

            // Kiểm tra giao thoa: Nếu khoảng cách giữa quái vật và nguồn âm 
            // nhỏ hơn (Bán kính thính giác quái vật + Bán kính âm thanh phát ra)
            if (distToPing <= (HearingRadius.Value + ping.radius))
            {
                // Lưu vị trí nghe thấy vào Blackboard để AI đi tới điều tra
                OutputPos.Value = ping.position;

                // Cập nhật Gizmo thành công
                if (hearingGizmo != null)
                {
                    hearingGizmo.isHearingSomething = true;
                    hearingGizmo.lastHeardPosition = ping.position;
                    // Đẩy luôn bán kính âm thanh sang cho Gizmo hiển thị
                    hearingGizmo.currentSourceRadius = ping.radius; 
                }

                return Status.Success;
            }
        }

        // Không nghe thấy gì
        if (hearingGizmo != null) hearingGizmo.isHearingSomething = false;
        return Status.Failure;
    }
}