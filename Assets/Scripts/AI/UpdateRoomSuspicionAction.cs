using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Update Room Suspicion",
    story: "Update [CurrentRoom] suspicion by [Amount] (or set to [ExactValue])",
    category: "Action/Room",
    id: "update_room_suspicion_action")]
public partial class UpdateRoomSuspicionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> CurrentRoom;
    [SerializeReference] public BlackboardVariable<int> Amount = new BlackboardVariable<int>(1);
    [SerializeReference] public BlackboardVariable<bool> UseExactValue = new BlackboardVariable<bool>(false);
    [SerializeReference] public BlackboardVariable<int> ExactValue = new BlackboardVariable<int>(0);

    protected override Status OnStart()
    {
        if (CurrentRoom.Value == null) return Status.Failure;

        RoomController room = CurrentRoom.Value.GetComponent<RoomController>();
        if (room != null)
        {
            if (UseExactValue.Value)
            {
                room.suspicionLevel = ExactValue.Value;
            }
            else
            {
                room.suspicionLevel += Amount.Value;
            }
            return Status.Success;
        }

        return Status.Failure;
    }
}
