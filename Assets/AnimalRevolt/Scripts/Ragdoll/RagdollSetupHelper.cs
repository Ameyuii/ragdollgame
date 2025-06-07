using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Helper script để setup ragdoll cho character có sẵn
/// Tự động thêm ragdoll components và configure
/// </summary>
public class RagdollSetupHelper : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool debugMode = true;
    
    [Header("Ragdoll Configuration")]
    [SerializeField] private float defaultMass = 1f;
    [SerializeField] private float defaultDrag = 0.5f;
    [SerializeField] private float defaultAngularDrag = 5f;
    [SerializeField] private LayerMask ragdollLayer = 1 << 8;
    
    [Header("Components")]
    [SerializeField] private RagdollPhysicsController ragdollController;
    [SerializeField] private SimpleRagdollDemo simpleDemo;
    [SerializeField] private Animator animator;
    
    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupRagdoll();
        }
    }
    
    /// <summary>
    /// Setup ragdoll cho character này
    /// </summary>
    [ContextMenu("Setup Ragdoll")]
    public void SetupRagdoll()
    {
        if (debugMode)
            Debug.Log($"Setting up ragdoll for {gameObject.name}");
            
        // 1. Setup Animator
        SetupAnimator();
        
        // 2. Setup ragdoll physics components
        SetupRagdollPhysics();
        
        // 3. Setup RagdollPhysicsController
        SetupRagdollController();
        
        // 4. Setup SimpleRagdollDemo
        SetupSimpleDemo();
        
        if (debugMode)
            Debug.Log($"Ragdoll setup completed for {gameObject.name}");
    }
    
    /// <summary>
    /// Setup Animator component
    /// </summary>
    private void SetupAnimator()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
            if (debugMode)
                Debug.Log("Added Animator component");
        }
    }
    
    /// <summary>
    /// Setup ragdoll physics components tự động
    /// </summary>
    private void SetupRagdollPhysics()
    {
        // Tìm tất cả child transforms có thể là bones
        Transform[] allTransforms = GetComponentsInChildren<Transform>();
        List<Transform> boneTransforms = new List<Transform>();
        
        // Filter bone transforms (thường có tên chứa bone, spine, arm, leg, etc.)
        foreach (Transform t in allTransforms)
        {
            if (IsBoneTransform(t))
            {
                boneTransforms.Add(t);
            }
        }
        
        if (debugMode)
            Debug.Log($"Found {boneTransforms.Count} potential bone transforms");
            
        // Add Rigidbody và Collider cho main bones
        foreach (Transform bone in boneTransforms)
        {
            SetupBonePhysics(bone);
        }
        
        // Setup joints sau khi có đủ rigidbodies
        SetupJoints(boneTransforms);
    }
    
    /// <summary>
    /// Kiểm tra transform có phải bone không
    /// </summary>
    private bool IsBoneTransform(Transform t)
    {
        string name = t.name.ToLower();
        
        // Danh sách keywords cho bones
        string[] boneKeywords = {
            "spine", "chest", "neck", "head",
            "shoulder", "arm", "elbow", "hand", "finger",
            "hip", "thigh", "leg", "knee", "foot", "toe",
            "pelvis", "clavicle"
        };
        
        foreach (string keyword in boneKeywords)
        {
            if (name.Contains(keyword))
                return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Setup physics cho một bone
    /// </summary>
    private void SetupBonePhysics(Transform bone)
    {
        // Add Rigidbody nếu chưa có
        Rigidbody rb = bone.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = bone.gameObject.AddComponent<Rigidbody>();
            rb.mass = defaultMass;
            rb.linearDamping = defaultDrag;
            rb.angularDamping = defaultAngularDrag;
            rb.isKinematic = true; // Bắt đầu ở kinematic mode
        }
        
        // Add Collider nếu chưa có
        Collider col = bone.GetComponent<Collider>();
        if (col == null)
        {
            // Tạo collider phù hợp dựa vào tên bone
            col = CreateColliderForBone(bone);
        }
        
        // Set layer
        bone.gameObject.layer = (int)Mathf.Log(ragdollLayer.value, 2);
        
        // Disable collider ban đầu
        col.enabled = false;
        
        if (debugMode)
            Debug.Log($"Setup physics for bone: {bone.name}");
    }
    
    /// <summary>
    /// Tạo collider phù hợp cho bone
    /// </summary>
    private Collider CreateColliderForBone(Transform bone)
    {
        string name = bone.name.ToLower();
        
        // Head bones -> Sphere
        if (name.Contains("head"))
        {
            SphereCollider sphere = bone.gameObject.AddComponent<SphereCollider>();
            sphere.radius = 0.1f;
            return sphere;
        }
        
        // Spine, limbs -> Capsule
        if (name.Contains("spine") || name.Contains("arm") || name.Contains("leg") || 
            name.Contains("thigh") || name.Contains("forearm"))
        {
            CapsuleCollider capsule = bone.gameObject.AddComponent<CapsuleCollider>();
            capsule.radius = 0.05f;
            capsule.height = 0.2f;
            return capsule;
        }
        
        // Default -> Box
        BoxCollider box = bone.gameObject.AddComponent<BoxCollider>();
        box.size = Vector3.one * 0.1f;
        return box;
    }
    
    /// <summary>
    /// Setup joints giữa các bones
    /// </summary>
    private void SetupJoints(List<Transform> bones)
    {
        foreach (Transform bone in bones)
        {
            // Tìm parent bone
            Transform parentBone = FindParentBone(bone, bones);
            if (parentBone != null)
            {
                // Add CharacterJoint
                CharacterJoint joint = bone.GetComponent<CharacterJoint>();
                if (joint == null)
                {
                    joint = bone.gameObject.AddComponent<CharacterJoint>();
                    
                    // Connect to parent
                    Rigidbody parentRb = parentBone.GetComponent<Rigidbody>();
                    if (parentRb != null)
                    {
                        joint.connectedBody = parentRb;
                        joint.breakForce = Mathf.Infinity;
                        joint.breakTorque = Mathf.Infinity;
                    }
                    
                    if (debugMode)
                        Debug.Log($"Added joint: {bone.name} -> {parentBone.name}");
                }
            }
        }
    }
    
    /// <summary>
    /// Tìm parent bone trong hierarchy
    /// </summary>
    private Transform FindParentBone(Transform bone, List<Transform> allBones)
    {
        Transform current = bone.parent;
        while (current != null)
        {
            if (allBones.Contains(current))
                return current;
            current = current.parent;
        }
        return null;
    }
    
    /// <summary>
    /// Setup RagdollPhysicsController component
    /// </summary>
    private void SetupRagdollController()
    {
        ragdollController = GetComponent<RagdollPhysicsController>();
        if (ragdollController == null)
        {
            ragdollController = gameObject.AddComponent<RagdollPhysicsController>();
            if (debugMode)
                Debug.Log("Added RagdollPhysicsController component");
        }
    }
    
    /// <summary>
    /// Setup SimpleRagdollDemo component
    /// </summary>
    private void SetupSimpleDemo()
    {
        simpleDemo = GetComponent<SimpleRagdollDemo>();
        if (simpleDemo == null)
        {
            simpleDemo = gameObject.AddComponent<SimpleRagdollDemo>();
            if (debugMode)
                Debug.Log("Added SimpleRagdollDemo component");
        }
    }
    
    /// <summary>
    /// Test ragdoll functions
    /// </summary>
    [ContextMenu("Test Ragdoll Toggle")]
    public void TestRagdollToggle()
    {
        if (ragdollController != null)
        {
            ragdollController.ToggleRagdoll();
            if (debugMode)
                Debug.Log($"Toggled ragdoll for {gameObject.name}");
        }
        else
        {
            Debug.LogWarning("No RagdollController found! Run Setup Ragdoll first.");
        }
    }
    
    /// <summary>
    /// Test apply force
    /// </summary>
    [ContextMenu("Test Apply Force")]
    public void TestApplyForce()
    {
        if (ragdollController != null)
        {
            if (!ragdollController.IsRagdollActive)
                ragdollController.EnableRagdoll();
                
            Vector3 force = Vector3.forward * 500f;
            Vector3 position = transform.position + Vector3.up;
            ragdollController.ApplyForce(force, position);
            
            if (debugMode)
                Debug.Log($"Applied test force to {gameObject.name}");
        }
        else
        {
            Debug.LogWarning("No RagdollController found! Run Setup Ragdoll first.");
        }
    }
    
    /// <summary>
    /// Reset ragdoll
    /// </summary>
    [ContextMenu("Reset Ragdoll")]
    public void ResetRagdoll()
    {
        if (ragdollController != null)
        {
            ragdollController.DisableRagdoll();
            if (debugMode)
                Debug.Log($"Reset ragdoll for {gameObject.name}");
        }
    }
}
