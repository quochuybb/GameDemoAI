using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Check Room Suspicion",
    story: "Is [CurrentRoom] suspicion [Operator] [Value]?",
    category: "Condition/Room",
    id: "check_room_suspicion_condition")]
public partial class CheckRoomSuspicionCondition : Condition
{
    public enum CompareOperator
    {
        Equal,
        GreaterThanOrEqual,
        LessThanOrEqual
    }

    [SerializeReference] public BlackboardVariable<GameObject> CurrentRoom;
    [SerializeReference] public BlackboardVariable<CompareOperator> Operator = new BlackboardVariable<CompareOperator>(CompareOperator.Equal);
    [SerializeReference] public BlackboardVariable<int> Value = new BlackboardVariable<int>(1);

    public override bool IsTrue()
    {
        if (CurrentRoom.Value == null) return false;

        RoomController room = CurrentRoom.Value.GetComponent<RoomController>();
        if (room == null) return false;

        switch (Operator.Value)
        {
            case CompareOperator.Equal:
                return room.suspicionLevel == Value.Value;
            case CompareOperator.GreaterThanOrEqual:
                return room.suspicionLevel >= Value.Value;
            case CompareOperator.LessThanOrEqual:
                return room.suspicionLevel <= Value.Value;
            default:
                return false;
        }
    }
}
