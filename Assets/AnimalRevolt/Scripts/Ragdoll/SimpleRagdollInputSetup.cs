using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Utility script để tự động setup Input Actions cho SimpleRagdollDemo
/// </summary>
[CreateAssetMenu(fileName = "SimpleRagdollInputSetup", menuName = "AnimalRevolt/Ragdoll/Input Setup")]
public class SimpleRagdollInputSetup : ScriptableObject
{
    [Header("Input Action References")]
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference attackAction;
    
    /// <summary>
    /// Thiết lập input actions cho SimpleRagdollDemo component
    /// </summary>
    /// <param name="ragdollDemo">Component SimpleRagdollDemo cần setup</param>
    public void SetupInputActions(SimpleRagdollDemo ragdollDemo)
    {
        if (ragdollDemo == null)
        {
            Debug.LogError("SimpleRagdollDemo component is null!");
            return;
        }
        
        // Sử dụng reflection để set private fields
        var type = typeof(SimpleRagdollDemo);
        
        // Set toggle action (Jump)
        var toggleField = type.GetField("toggleRagdollAction", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (toggleField != null && jumpAction != null)
        {
            toggleField.SetValue(ragdollDemo, jumpAction);
            Debug.Log("Toggle action (Jump) setup thành công!");
        }
        
        // Set force action (Attack)
        var forceField = type.GetField("applyForceAction", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (forceField != null && attackAction != null)
        {
            forceField.SetValue(ragdollDemo, attackAction);
            Debug.Log("Force action (Attack) setup thành công!");
        }
        
        Debug.Log($"Input actions setup completed cho {ragdollDemo.gameObject.name}");
    }
    
    /// <summary>
    /// Tự động tìm và setup cho tất cả SimpleRagdollDemo components trong scene
    /// </summary>
    [ContextMenu("Auto Setup All In Scene")]
    public void AutoSetupAllInScene()
    {
        var ragdollDemos = FindObjectsOfType<SimpleRagdollDemo>();
        
        if (ragdollDemos.Length == 0)
        {
            Debug.LogWarning("Không tìm thấy SimpleRagdollDemo nào trong scene!");
            return;
        }
        
        foreach (var demo in ragdollDemos)
        {
            SetupInputActions(demo);
        }
        
        Debug.Log($"Setup completed cho {ragdollDemos.Length} SimpleRagdollDemo components!");
    }
    
    public InputActionReference JumpAction => jumpAction;
    public InputActionReference AttackAction => attackAction;
}