using UnityEngine;

/// <summary>
/// Interface cho các skill có thể sử dụng
/// </summary>
public interface ISkill
{
    /// <summary>
    /// Kiểm tra có thể sử dụng skill không
    /// </summary>
    /// <param name="caster">Người cast skill</param>
    bool CanUse(GameObject caster);
    
    /// <summary>
    /// Thực thi skill
    /// </summary>
    /// <param name="caster">Người cast skill</param>
    /// <param name="targetPosition">Vị trí target</param>
    void Execute(GameObject caster, Vector3 targetPosition);
    
    /// <summary>
    /// Thực thi skill với target cụ thể
    /// </summary>
    /// <param name="caster">Người cast skill</param>
    /// <param name="target">Target cụ thể</param>
    void Execute(GameObject caster, GameObject target);
    
    /// <summary>
    /// Lấy thời gian cooldown
    /// </summary>
    float GetCooldown();
    
    /// <summary>
    /// Lấy cost để sử dụng (mana, stamina, etc.)
    /// </summary>
    float GetCost();
    
    /// <summary>
    /// Lấy tên skill
    /// </summary>
    string GetSkillName();
}
