using Unity.Behavior;

/// <summary>
/// Enum đại diện cho 4 trạng thái chính của Enemy AI Behavior Tree.
/// </summary>
[BlackboardEnum]
public enum EnemyState
{
    /// <summary>
    /// Tuần tra giữa các waypoint. Trạng thái mặc định.
    /// </summary>
    Patrol = 0,

    /// <summary>
    /// Điều tra khi phát hiện dấu hiệu đáng ngờ (SuspicionLevel vượt ngưỡng).
    /// Di chuyển đến TargetPosition.
    /// </summary>
    Investigating = 1,

    /// <summary>
    /// Tìm kiếm player tại vị trí cuối cùng nhìn thấy (LastSeenPosition).
    /// Kích hoạt khi mất tầm nhìn player lúc đang Chasing.
    /// </summary>
    Searching = 2,

    /// <summary>
    /// Đuổi theo player. Kích hoạt khi SuspicionLevel đạt ngưỡng chase
    /// và player trong tầm nhìn.
    /// </summary>
    Chasing = 3
}
