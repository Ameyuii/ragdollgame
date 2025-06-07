using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject chứa cấu hình ragdoll system
/// Quản lý danh sách prefab và thông số physics
/// </summary>
[CreateAssetMenu(fileName = "RagdollSettings", menuName = "AnimalRevolt/Ragdoll Settings")]
public class RagdollSettings : ScriptableObject
{
    [Header("Prefab Configuration")]
    [Tooltip("Danh sách prefab ragdoll từ Assets/Prefabs/")]
    public List<GameObject> ragdollPrefabs = new List<GameObject>();
    
    [Header("Physics Settings")]
    [Tooltip("Mass mặc định cho các Rigidbody")]
    [Range(0.1f, 10f)]
    public float defaultMass = 1f;
    
    [Tooltip("Drag mặc định cho các Rigidbody")]
    [Range(0f, 10f)]
    public float defaultDrag = 0.5f;
    
    [Tooltip("Angular drag mặc định")]
    [Range(0f, 10f)]
    public float defaultAngularDrag = 5f;
    
    [Header("Joint Configuration")]
    [Tooltip("Break force cho các joint")]
    public float jointBreakForce = Mathf.Infinity;
    
    [Tooltip("Break torque cho các joint")]
    public float jointBreakTorque = Mathf.Infinity;
    
    [Header("Transition Settings")]
    [Tooltip("Thời gian chuyển đổi từ animation sang ragdoll")]
    [Range(0f, 2f)]
    public float transitionDuration = 0.2f;
    
    [Tooltip("Lực ban đầu khi kích hoạt ragdoll")]
    [Range(0f, 1000f)]
    public float initialForceMultiplier = 100f;
    
    [Header("Performance Optimization")]
    [Tooltip("Số lượng ragdoll tối đa cùng lúc")]
    [Range(1, 50)]
    public int maxActiveRagdolls = 10;
    
    [Tooltip("Thời gian tự động despawn ragdoll (giây)")]
    [Range(5f, 60f)]
    public float ragdollLifetime = 15f;
    
    [Tooltip("Khoảng cách LOD - giảm quality khi xa camera")]
    [Range(10f, 100f)]
    public float lodDistance = 30f;
    
    [Header("Collision Layers")]
    [Tooltip("Layer cho ragdoll bodies")]
    public LayerMask ragdollLayer = 1 << 8;
    
    [Tooltip("Layer mà ragdoll có thể va chạm")]
    public LayerMask collisionMask = ~0;
    
    [Header("Advanced Settings")]
    [Tooltip("Có sử dụng continuous collision detection")]
    public bool useContinuousCollision = true;
    
    [Tooltip("Có freeze rotation Z axis")]
    public bool freezeZRotation = false;
    
    [Tooltip("Minimum velocity để ragdoll hoạt động")]
    [Range(0f, 5f)]
    public float minimumVelocityThreshold = 0.1f;
    
    /// <summary>
    /// Lấy prefab ngẫu nhiên từ danh sách
    /// </summary>
    public GameObject GetRandomPrefab()
    {
        if (ragdollPrefabs == null || ragdollPrefabs.Count == 0)
        {
            Debug.LogWarning("Không có prefab nào trong RagdollSettings!");
            return null;
        }
        
        return ragdollPrefabs[Random.Range(0, ragdollPrefabs.Count)];
    }
    
    /// <summary>
    /// Lấy prefab theo index
    /// </summary>
    public GameObject GetPrefab(int index)
    {
        if (ragdollPrefabs == null || index < 0 || index >= ragdollPrefabs.Count)
        {
            Debug.LogWarning($"Index {index} không hợp lệ trong RagdollSettings!");
            return null;
        }
        
        return ragdollPrefabs[index];
    }
    
    /// <summary>
    /// Validate settings khi save
    /// </summary>
    private void OnValidate()
    {
        // Đảm bảo giá trị hợp lệ
        defaultMass = Mathf.Max(0.1f, defaultMass);
        defaultDrag = Mathf.Max(0f, defaultDrag);
        defaultAngularDrag = Mathf.Max(0f, defaultAngularDrag);
        transitionDuration = Mathf.Max(0f, transitionDuration);
        initialForceMultiplier = Mathf.Max(0f, initialForceMultiplier);
        maxActiveRagdolls = Mathf.Max(1, maxActiveRagdolls);
        ragdollLifetime = Mathf.Max(1f, ragdollLifetime);
        lodDistance = Mathf.Max(1f, lodDistance);
        
        // Loại bỏ null prefab
        if (ragdollPrefabs != null)
        {
            ragdollPrefabs.RemoveAll(prefab => prefab == null);
        }
    }
}