using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Demo script đơn giản để test ragdoll functionality
/// Sử dụng Input System để toggle ragdoll và apply force
/// </summary>
public class SimpleRagdollDemo : MonoBehaviour
{
    [Header("Ragdoll Component")]
    [SerializeField] private RagdollPhysicsController ragdollController;
    [SerializeField] private bool autoFindRagdollController = true;
    
    [Header("Test Settings")]
    [SerializeField] private bool debugMode = false;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference toggleRagdollAction;
    [SerializeField] private InputActionReference applyForceAction;
    
    [Header("Force Settings")]
    [SerializeField] private float forceAmount = 500f;
    [SerializeField] private Vector3 forceDirection = Vector3.up;
    
    private void Start()
    {
        // Setup ragdoll controller
        SetupRagdollController();
        
        // Setup input actions
        SetupInputActions();
    }
    
    private void OnEnable()
    {
        // Enable input actions
        if (toggleRagdollAction?.action != null)
            toggleRagdollAction.action.Enable();
            
        if (applyForceAction?.action != null)
            applyForceAction.action.Enable();
    }
    
    private void OnDisable()
    {
        // Disable input actions
        if (toggleRagdollAction?.action != null)
            toggleRagdollAction.action.Disable();
            
        if (applyForceAction?.action != null)
            applyForceAction.action.Disable();
    }
    
    /// <summary>
    /// Setup ragdoll controller reference
    /// </summary>
    private void SetupRagdollController()
    {
        if (ragdollController == null && autoFindRagdollController)
        {
            ragdollController = GetComponent<RagdollPhysicsController>();
            
            if (ragdollController == null)
                ragdollController = GetComponentInChildren<RagdollPhysicsController>();
        }
        
        if (ragdollController == null)
        {
            Debug.LogWarning($"Không tìm thấy RagdollPhysicsController trên {gameObject.name}!");
            return;
        }
        
        if (debugMode)
            Debug.Log($"SimpleRagdollDemo: Found RagdollPhysicsController on {ragdollController.gameObject.name}");
    }
    
    /// <summary>
    /// Setup input action callbacks
    /// </summary>
    private void SetupInputActions()
    {
        // Toggle ragdoll action
        if (toggleRagdollAction?.action != null)
        {
            toggleRagdollAction.action.performed += OnToggleRagdoll;
            
            if (debugMode)
                Debug.Log("SimpleRagdollDemo: Toggle ragdoll action setup complete");
        }
        
        // Apply force action
        if (applyForceAction?.action != null)
        {
            applyForceAction.action.performed += OnApplyForce;
            
            if (debugMode)
                Debug.Log("SimpleRagdollDemo: Apply force action setup complete");
        }
    }
    
    /// <summary>
    /// Callback khi nhận input toggle ragdoll
    /// </summary>
    private void OnToggleRagdoll(InputAction.CallbackContext context)
    {
        if (ragdollController == null) return;
        
        ragdollController.ToggleRagdoll();
        
        if (debugMode)
            Debug.Log($"SimpleRagdollDemo: Toggled ragdoll - Active: {ragdollController.IsRagdollActive}");
    }
    
    /// <summary>
    /// Callback khi nhận input apply force
    /// </summary>
    private void OnApplyForce(InputAction.CallbackContext context)
    {
        if (ragdollController == null || !ragdollController.IsRagdollActive) return;
        
        Vector3 force = forceDirection.normalized * forceAmount;
        ragdollController.ApplyForce(force, transform.position);
        
        if (debugMode)
            Debug.Log($"SimpleRagdollDemo: Applied force {force} at {transform.position}");
    }
    
    /// <summary>
    /// Test method để toggle ragdoll từ code
    /// </summary>
    [ContextMenu("Toggle Ragdoll")]
    public void TestToggleRagdoll()
    {
        if (ragdollController != null)
        {
            ragdollController.ToggleRagdoll();
            Debug.Log($"Manual toggle - Ragdoll Active: {ragdollController.IsRagdollActive}");
        }
    }
    
    /// <summary>
    /// Test method để apply force từ code
    /// </summary>
    [ContextMenu("Apply Random Force")]
    public void TestApplyForce()
    {
        if (ragdollController != null && ragdollController.IsRagdollActive)
        {
            Vector3 randomForce = new Vector3(
                Random.Range(-forceAmount, forceAmount),
                Random.Range(0, forceAmount),
                Random.Range(-forceAmount, forceAmount)
            );
            
            ragdollController.ApplyForce(randomForce, transform.position);
            Debug.Log($"Applied random force: {randomForce}");
        }
    }
}
