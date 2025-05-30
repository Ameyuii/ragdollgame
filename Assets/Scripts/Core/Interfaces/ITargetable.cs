using UnityEngine;

/// <summary>
/// Interface cho các đối tượng có thể được target
/// </summary>
public interface ITargetable
{
    /// <summary>
    /// Lấy Transform của target
    /// </summary>
    Transform GetTransform();
    
    /// <summary>
    /// Lấy vị trí của target
    /// </summary>
    Vector3 GetPosition();
    
    /// <summary>
    /// Kiểm tra có phải là target hợp lệ không
    /// </summary>
    /// <param name="requester">Đối tượng yêu cầu target</param>
    bool IsValidTarget(GameObject requester);
    
    /// <summary>
    /// Lấy team ID
    /// </summary>
    int GetTeam();
      /// <summary>
    /// Kiểm tra có thể target được không (không chết, không invisible, etc.)
    /// </summary>
    bool CanBeTargeted();
    
    /// <summary>
    /// Nhận damage từ attacker
    /// </summary>
    void TakeDamage(float damage, GameObject? attacker = null);
}
