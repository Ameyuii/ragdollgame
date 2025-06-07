using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script chính quản lý ragdoll physics và chuyển đổi state
/// Chuyển đổi giữa animated và ragdoll mode
/// </summary>
[RequireComponent(typeof(Animator))]
public class RagdollPhysicsController : MonoBehaviour
{    [Header("References")]
    [SerializeField] private RagdollSettings settings;
    [SerializeField] private Animator animator;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    
    // Private variables
    private Rigidbody[] ragdollRigidbodies;
    private Collider[] ragdollColliders;
    private CharacterJoint[] ragdollJoints;
    private bool isRagdollActive = false;
    private bool isTransitioning = false;
    
    // Animation state backup
    private Vector3[] bonePositions;
    private Quaternion[] boneRotations;
    private Transform[] boneTransforms;
    
    // Events
    public System.Action<bool> OnRagdollStateChanged;
    public System.Action OnRagdollEnabled;
    public System.Action OnRagdollDisabled;
    
    // Properties
    public bool IsRagdollActive => isRagdollActive;
    public bool IsTransitioning => isTransitioning;
    public RagdollSettings Settings => settings;
    
    private void Awake()
    {
        // Tự động tìm Animator nếu chưa assign
        if (animator == null)
            animator = GetComponent<Animator>();
            
        // Load default settings nếu chưa có
        if (settings == null)
            LoadDefaultSettings();
            
        InitializeRagdoll();
    }
    
    private void Start()
    {
        // Khởi tạo hoàn thành
        if (debugMode)
            Debug.Log($"RagdollPhysicsController started for {gameObject.name}");
    }
    
    /// <summary>
    /// Khởi tạo ragdoll components
    /// </summary>
    private void InitializeRagdoll()
    {
        // Tìm tất cả Rigidbody trong children
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollJoints = GetComponentsInChildren<CharacterJoint>();
        
        // Lưu trữ bone transforms để backup
        List<Transform> bones = new List<Transform>();
        foreach (var rb in ragdollRigidbodies)
        {
            bones.Add(rb.transform);
        }
        boneTransforms = bones.ToArray();
        bonePositions = new Vector3[boneTransforms.Length];
        boneRotations = new Quaternion[boneTransforms.Length];
        
        // Cấu hình ragdoll theo settings
        ConfigureRagdollPhysics();
        
        // Bắt đầu ở chế độ animation
        DisableRagdoll();
        
        if (debugMode)
            Debug.Log($"RagdollController initialized với {ragdollRigidbodies.Length} rigidbodies");
    }
    
    /// <summary>
    /// Cấu hình physics properties cho ragdoll
    /// </summary>
    private void ConfigureRagdollPhysics()
    {
        if (settings == null) return;
        
        foreach (var rb in ragdollRigidbodies)
        {
            rb.mass = settings.defaultMass;
            rb.linearDamping = settings.defaultDrag;
            rb.angularDamping = settings.defaultAngularDrag;
            rb.collisionDetectionMode = settings.useContinuousCollision ? 
                CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete;
                
            // Freeze Z rotation nếu cần
            if (settings.freezeZRotation)
                rb.freezeRotation = true;
        }
        
        // Cấu hình joints
        foreach (var joint in ragdollJoints)
        {
            joint.breakForce = settings.jointBreakForce;
            joint.breakTorque = settings.jointBreakTorque;
        }
        
        // Cấu hình layers
        foreach (var col in ragdollColliders)
        {
            col.gameObject.layer = (int)Mathf.Log(settings.ragdollLayer.value, 2);
        }
    }
    
    /// <summary>
    /// Kích hoạt ragdoll mode
    /// </summary>
    public void EnableRagdoll()
    {
        if (isRagdollActive || isTransitioning) return;
        
        StartCoroutine(TransitionToRagdoll());
    }
    
    /// <summary>
    /// Vô hiệu hóa ragdoll mode
    /// </summary>
    public void DisableRagdoll()
    {
        if (!isRagdollActive || isTransitioning) return;
        
        StartCoroutine(TransitionToAnimation());
    }
    
    /// <summary>
    /// Áp dụng lực lên ragdoll
    /// </summary>
    public void ApplyForce(Vector3 force, Vector3 position, ForceMode forceMode = ForceMode.Impulse)
    {
        if (!isRagdollActive) return;
        
        // Tìm rigidbody gần nhất với position
        Rigidbody closestRb = GetClosestRigidbody(position);
        if (closestRb != null)
        {
            closestRb.AddForceAtPosition(force * settings.initialForceMultiplier, position, forceMode);
            
            if (debugMode)
                Debug.Log($"Applied force {force} tại position {position}");
        }
    }
    
    /// <summary>
    /// Áp dụng lực lên toàn bộ ragdoll
    /// </summary>
    public void ApplyExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        if (!isRagdollActive) return;
        
        foreach (var rb in ragdollRigidbodies)
        {
            rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
        }
        
        if (debugMode)
            Debug.Log($"Applied explosion force {explosionForce} at {explosionPosition}");
    }
    
    /// <summary>
    /// Chuyển đổi sang ragdoll mode
    /// </summary>
    private IEnumerator TransitionToRagdoll()
    {
        isTransitioning = true;
        
        // Backup animation state
        BackupAnimationState();
        
        // Disable animator
        if (animator != null)
            animator.enabled = false;
        
        // Enable ragdoll physics
        SetRagdollState(true);
        
        // Đợi transition duration
        if (settings != null && settings.transitionDuration > 0)
            yield return new WaitForSeconds(settings.transitionDuration);
        
        isRagdollActive = true;
        isTransitioning = false;
        
        // Trigger events
        OnRagdollStateChanged?.Invoke(true);
        OnRagdollEnabled?.Invoke();
        
        if (debugMode)
            Debug.Log("Ragdoll enabled");
    }
    
    /// <summary>
    /// Chuyển đổi sang animation mode
    /// </summary>
    private IEnumerator TransitionToAnimation()
    {
        isTransitioning = true;
        
        // Disable ragdoll physics
        SetRagdollState(false);
        
        // Restore animation state nếu có backup
        if (bonePositions != null && boneRotations != null)
            RestoreAnimationState();
        
        // Enable animator
        if (animator != null)
            animator.enabled = true;
        
        // Đợi transition duration
        if (settings != null && settings.transitionDuration > 0)
            yield return new WaitForSeconds(settings.transitionDuration);
        
        isRagdollActive = false;
        isTransitioning = false;
        
        // Trigger events
        OnRagdollStateChanged?.Invoke(false);
        OnRagdollDisabled?.Invoke();
        
        if (debugMode)
            Debug.Log("Ragdoll disabled");
    }
    
    /// <summary>
    /// Set trạng thái ragdoll components
    /// </summary>
    private void SetRagdollState(bool enabled)
    {
        foreach (var rb in ragdollRigidbodies)
        {
            rb.isKinematic = !enabled;
            rb.detectCollisions = enabled;
        }
        
        foreach (var col in ragdollColliders)
        {
            col.enabled = enabled;
        }
    }
    
    /// <summary>
    /// Backup animation state
    /// </summary>
    private void BackupAnimationState()
    {
        for (int i = 0; i < boneTransforms.Length; i++)
        {
            bonePositions[i] = boneTransforms[i].localPosition;
            boneRotations[i] = boneTransforms[i].localRotation;
        }
    }
    
    /// <summary>
    /// Restore animation state
    /// </summary>
    private void RestoreAnimationState()
    {
        for (int i = 0; i < boneTransforms.Length; i++)
        {
            boneTransforms[i].localPosition = bonePositions[i];
            boneTransforms[i].localRotation = boneRotations[i];
        }
    }
    
    /// <summary>
    /// Tìm rigidbody gần nhất với position
    /// </summary>
    private Rigidbody GetClosestRigidbody(Vector3 position)
    {
        Rigidbody closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (var rb in ragdollRigidbodies)
        {
            float distance = Vector3.Distance(rb.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = rb;
            }
        }
        
        return closest;
    }
    
    /// <summary>
    /// Load default settings nếu chưa có
    /// </summary>
    private void LoadDefaultSettings()
    {
        settings = Resources.Load<RagdollSettings>("DefaultRagdollSettings");
        if (settings == null && debugMode)
            Debug.LogWarning("Không tìm thấy DefaultRagdollSettings trong Resources folder");
    }
    
    /// <summary>
    /// Toggle ragdoll state (UI Button)
    /// </summary>
    [ContextMenu("Toggle Ragdoll")]
    public void ToggleRagdoll()
    {
        if (isRagdollActive)
            DisableRagdoll();
        else
            EnableRagdoll();
    }
    
    /// <summary>
    /// Test áp dụng lực ngẫu nhiên (UI Button)
    /// </summary>
    [ContextMenu("Apply Random Force")]
    public void ApplyRandomForce()
    {
        // Bật ragdoll trước nếu chưa bật
        if (!isRagdollActive)
        {
            EnableRagdoll();
            // Đợi một chút để ragdoll được kích hoạt
            StartCoroutine(DelayedApplyForce());
            return;
        }
        
        Vector3 randomForce = new Vector3(
            Random.Range(-500f, 500f),
            Random.Range(0f, 500f),
            Random.Range(-500f, 500f)
        );
        
        ApplyForce(randomForce, transform.position + Vector3.up);
        
        if (debugMode)
            Debug.Log($"Applied random force: {randomForce}");
    }
    
    /// <summary>
    /// Áp dụng lực sau delay
    /// </summary>
    private IEnumerator DelayedApplyForce()
    {
        yield return new WaitForSeconds(0.5f);
        ApplyRandomForce();
    }
    
    /// <summary>
    /// Lấy tổng mass của ragdoll
    /// </summary>
    public float GetTotalMass()
    {
        float totalMass = 0f;
        foreach (var rb in ragdollRigidbodies)
        {
            totalMass += rb.mass;
        }
        return totalMass;
    }
    
    /// <summary>
    /// Kiểm tra ragdoll có ổn định không
    /// </summary>
    public bool IsStable()
    {
        if (!isRagdollActive) return true;
        
        foreach (var rb in ragdollRigidbodies)
        {
            if (rb.linearVelocity.magnitude > 0.1f || rb.angularVelocity.magnitude > 0.1f)
                return false;
        }
        return true;
    }
}