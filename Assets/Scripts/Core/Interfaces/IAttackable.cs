using UnityEngine;

/// <summary>
/// Interface cho các đối tượng có thể nhận sát thương
/// </summary>
public interface IAttackable
{
    /// <summary>
    /// Nhận sát thương từ attacker
    /// </summary>
    /// <param name="damage">Lượng sát thương</param>
    /// <param name="attacker">Kẻ tấn công</param>
    void TakeDamage(float damage, GameObject attacker);
    
    /// <summary>
    /// Kiểm tra có còn sống không
    /// </summary>
    bool IsAlive();
    
    /// <summary>
    /// Lấy health hiện tại
    /// </summary>
    float GetCurrentHealth();
    
    /// <summary>
    /// Lấy health tối đa
    /// </summary>
    float GetMaxHealth();
    
    /// <summary>
    /// Lấy team ID
    /// </summary>
    int GetTeam();
}
