# ROO CODE - AI COMBAT SYSTEM RESTORATION PROMPT

## URGENT: Khôi phục hệ thống AI Combat bị hỏng

### VẤN ĐỀ HIỆN TẠI
Sau các lần fix trước, hệ thống AI combat đã bị hỏng hoàn toàn:
1. **DetectionMask = 0** → AI không thể phát hiện enemy
2. **Animator Parameters thiếu** → Animation system không hoạt động
3. **AI không di chuyển và tấn công nhau** → Game logic bị break

### CONSOLE ERRORS CHUẨN ĐOÁN
```
[EnemyDetector] DetectionMask: 0, Position: (3.91, -0.13, 0.74)
[EnemyDetector] Found 0 objects in range
⚠️ Animator parameter 'IsMoving' not found
⚠️ Animator parameter 'InCombat' not found
```

### YÊU CẦU ROO CODE THỰC HIỆN

#### 1. FIX DETECTION MASK NGAY LẬP TỨC
```csharp
// File: Assets/AnimalRevolt/Scripts/Combat/EnemyDetector.cs
// PROBLEM: detectionMask đang bị reset về 0
// FIX: Đảm bảo detectionMask được set đúng trong Start() hoặc Awake()

public class EnemyDetector : MonoBehaviour 
{
    [SerializeField] private LayerMask detectionMask = -1; // PHẢI SET DEFAULT = -1 (Everything)
    
    private void Start()
    {
        // THÊM CODE NÀY ĐỂ FORCE SET DETECTION MASK
        if (detectionMask == 0)
        {
            detectionMask = -1; // Everything layer
            Debug.LogWarning($"[EnemyDetector] DetectionMask was 0, setting to Everything for {gameObject.name}");
        }
    }
}
```

#### 2. FIX ANIMATOR PARAMETERS
```csharp
// File: Assets/AnimalRevolt/Scripts/AI/AIMovementController.cs
// PROBLEM: Code đang tìm parameters không tồn tại
// FIX: Chỉ sử dụng parameters thực sự có trong Animator Controller

private void SetAnimatorParameter(string paramName, bool value)
{
    if (animator == null) return;
    
    // THÊM CHECK PARAMETER EXISTS
    if (HasParameter(paramName))
    {
        animator.SetBool(paramName, value);
    }
}

private bool HasParameter(string paramName)
{
    foreach (AnimatorControllerParameter param in animator.parameters)
    {
        if (param.name == paramName)
            return true;
    }
    return false;
}
```

#### 3. KHÔI PHỤC LOGIC AI COMBAT
```csharp
// File: Assets/AnimalRevolt/Scripts/AI/AIMovementController.cs
// THÊM VÀO HandleSeekingState():

private void HandleSeekingState()
{
    if (currentTarget == null)
    {
        ChangeState(AIState.Idle);
        return;
    }

    // FORCE MOVEMENT TOWARD TARGET
    Vector3 direction = (currentTarget.transform.position - transform.position);
    float distance = direction.magnitude;
    
    // DI CHUYỂN ĐẾN TARGET
    if (distance > combatRange)
    {
        direction.y = 0;
        direction.Normalize();
        
        // FORCE NAVMESH MOVEMENT
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(currentTarget.transform.position);
            navMeshAgent.isStopped = false;
        }
        
        Debug.Log($"[AI] {gameObject.name} moving to target {currentTarget.name}, distance: {distance:F2}");
    }
    else
    {
        // VÀO COMBAT KHI ĐỦ GẦN
        combatController?.StartCombat(currentTarget);
        ChangeState(AIState.Combat);
    }
}
```

#### 4. ĐẢM BẢO ENEMY DETECTION HOẠT ĐỘNG
```csharp
// File: Assets/AnimalRevolt/Scripts/Combat/EnemyDetector.cs
// TRONG UpdateDetection():

private void UpdateDetection()
{
    if (teamMember == null) return;
    
    // FORCE DETECTION WITH CORRECT MASK
    Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionMask);
    
    Debug.Log($"[EnemyDetector] Detection check - Mask: {detectionMask.value}, Radius: {detectionRadius}, Found: {colliders.Length}");
    
    foreach (var collider in colliders)
    {
        if (collider.gameObject == gameObject) continue;
        
        TeamMember otherTeam = collider.GetComponent<TeamMember>();
        if (otherTeam != null && teamMember.IsEnemy(otherTeam))
        {
            Debug.Log($"[EnemyDetector] ENEMY FOUND: {collider.name} (Team: {otherTeam.TeamName})");
            AddTarget(collider.gameObject);
        }
    }
}
```

#### 5. CONFIG CHO GAMEOBJECTS
Đảm bảo các NPC có:
- **Layer**: Default hoặc tạo layer riêng cho AI
- **Collider**: Để có thể detect được
- **NavMeshAgent**: Enabled và trên NavMesh
- **Team setup**: TeamMember với team khác nhau

### CÁC BƯỚC THỰC HIỆN THEO THỨ TỰ

1. **FIX DetectionMask trong EnemyDetector.cs** (ưu tiên cao nhất)
2. **FIX Animator parameter checking** (tránh warning spam)  
3. **KHÔI PHỤC logic di chuyển trong HandleSeekingState()**
4. **THÊM debug logs chi tiết** để track được AI behavior
5. **TEST với 2 NPC khác team** để xác nhận họ tìm và tấn công nhau

### EXPECTED BEHAVIOR SAU KHI FIX
- Console log: `[EnemyDetector] ENEMY FOUND: npc test (Team: AI_Team2)`
- Console log: `[AI] Warrok W Kurniawan moving to target npc test, distance: 12.34`
- AI sẽ di chuyển về phía enemy và bắt đầu combat

### LƯU Ý QUAN TRỌNG
- **KHÔNG THAY ĐỔI** team logic đã được refactor
- **GIỮ NGUYÊN** TeamMember system hiện tại
- **CHỈ FIX** detection mask và movement logic
- **THỰC HIỆN** theo đúng thứ tự để tránh conflict

### RULE VÀ SETTING CHO ROO
- Luôn thêm debug logs để kiểm tra được flow
- Kiểm tra null reference trước khi sử dụng component
- Sử dụng Unity coding conventions
- Comment bằng tiếng Việt cho dễ hiểu
- Test từng bước một, không làm nhiều thay đổi cùng lúc
