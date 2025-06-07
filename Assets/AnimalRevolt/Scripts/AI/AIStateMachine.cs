using UnityEngine;

/// <summary>
/// State machine system cho AI movement và behavior
/// Quản lý các trạng thái: Idle, Seeking, Moving, Combat
/// </summary>
public class AIStateMachine : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    
    // Current state
    private AIState currentState = AIState.Idle;
    private AIState previousState = AIState.Idle;
    
    // Events
    public System.Action<AIState, AIState> OnStateChanged;
    
    // Properties
    public AIState CurrentState => currentState;
    public AIState PreviousState => previousState;
    
    private void Start()
    {
        if (debugMode)
            Debug.Log($"AI StateMachine initialized for {gameObject.name} - Initial state: {currentState}");
    }
    
    /// <summary>
    /// Thay đổi state
    /// </summary>
    public void ChangeState(AIState newState)
    {
        if (newState == currentState) return;
        
        previousState = currentState;
        currentState = newState;
          OnStateChanged?.Invoke(previousState, currentState);
        
        if (debugMode)
        {
            // Debug.Log($"{gameObject.name} AI State: {previousState} -> {currentState}");
        }
    }
    
    /// <summary>
    /// Kiểm tra có thể chuyển state không
    /// </summary>
    public bool CanTransitionTo(AIState targetState)
    {
        // Định nghĩa các transition rules
        switch (currentState)
        {
            case AIState.Idle:
                return targetState == AIState.Seeking || targetState == AIState.Moving;
                
            case AIState.Seeking:
                return targetState == AIState.Moving || targetState == AIState.Combat || 
                       targetState == AIState.Idle;
                
            case AIState.Moving:
                return targetState == AIState.Combat || targetState == AIState.Seeking || 
                       targetState == AIState.Idle;
                
            case AIState.Combat:
                return targetState == AIState.Seeking || targetState == AIState.Idle;
                
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Force change state (bỏ qua transition rules)
    /// </summary>
    public void ForceChangeState(AIState newState)
    {
        previousState = currentState;
        currentState = newState;
        
        OnStateChanged?.Invoke(previousState, currentState);
        
        if (debugMode)
            Debug.Log($"{gameObject.name} AI State FORCED: {previousState} -> {currentState}");
    }
    
    /// <summary>
    /// Lấy state name cho debugging
    /// </summary>
    public string GetStateName(AIState state)
    {
        return state switch
        {
            AIState.Idle => "Idle",
            AIState.Seeking => "Seeking",
            AIState.Moving => "Moving",
            AIState.Combat => "Combat",
            _ => "Unknown"
        };
    }
}

/// <summary>
/// Enum định nghĩa các AI states
/// </summary>
public enum AIState
{
    Idle,       // Đứng yên, không có mục tiêu
    Seeking,    // Tìm kiếm enemy
    Moving,     // Di chuyển đến mục tiêu
    Combat      // Đang trong combat
}