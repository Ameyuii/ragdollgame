// BACKUP - PHIÊN BẢN BỊ LỖI (May 28, 2025)
// File này là backup của NPCController.cs sau khi integrate với AnimationRagdollTrigger
// Phiên bản này gây lỗi chức năng hoàn toàn

using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Script quản lý nhân vật với các thuộc tính: máu, team, tốc độ di chuyển, tầm đánh, tốc độ đánh, phạm vi phát hiện
/// Nhân vật khác team có thể tấn công nhau
/// </summary>
public class NPCController_BACKUP_BROKEN : MonoBehaviour
{
    // PHIÊN BẢN BỊ LỖI - KHÔNG SỬ DỤNG
    // File này chỉ để tham khảo những gì đã làm sai
    
    // THÔNG TIN VỀ NHỮNG GÌ ĐÃ LÀM SAI:
    // 1. Thêm AnimationRagdollTrigger dependency làm phức tạp system
    // 2. Tắt navMeshAgent.updatePosition = false gây NPCs không di chuyển được
    // 3. Xóa bỏ các biến quan trọng như isMoving, isTransitioning
    // 4. Làm phức tạp logic di chuyển không cần thiết
    // 5. Dependency vào AnimationRagdollTrigger script không tồn tại hoặc không hoạt động đúng
}
