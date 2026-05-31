using UnityEngine;
using Unity.Behavior;

[RequireComponent(typeof(BoxCollider))]
public class RoomController : MonoBehaviour
{
    [Header("Room Settings")]
    public DoorController roomDoor;
    public int suspicionLevel = 1;

    private void Awake()
    {
        // Đảm bảo BoxCollider luôn là dạng Trigger
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Khi Quái vật bước vào vùng Trigger của phòng này
        if (other.CompareTag("Enemy"))
        {
            var agent = other.GetComponent<BehaviorGraphAgent>();
            if (agent != null)
            {
                // Nạp chính căn phòng này vào bộ não của con AI
                agent.BlackboardReference.SetVariableValue("CurrentRoom", this.gameObject);
            }
        }
    }
}
