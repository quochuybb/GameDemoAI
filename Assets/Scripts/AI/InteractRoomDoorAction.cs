using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Interact Room Door",
    story: "[Agent] interacts with [CurrentRoom] door",
    category: "Action/Room",
    id: "interact_room_door_action")]
public partial class InteractRoomDoorAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> CurrentRoom;

    protected override Status OnStart()
    {
        if (CurrentRoom.Value == null || Agent.Value == null) return Status.Failure;

        RoomController room = CurrentRoom.Value.GetComponent<RoomController>();
        if (room != null && room.roomDoor != null)
        {
            // Chỉ đóng cửa (nếu cửa đang mở thì interact)
            // Hoặc có thể gọi thẳng hàm đóng cửa nếu có.
            // Script DoorController hiện tại có hàm Interact().
            if (room.roomDoor.IsOpen)
            {
                room.roomDoor.Interact(Agent.Value);
            }
            return Status.Success;
        }

        return Status.Failure;
    }
}
