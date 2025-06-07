# Hệ thống AI Movement và Enemy Seeking - Animal Revolt

## Tổng quan

Hệ thống AI Movement và Enemy Seeking cho game Animal Revolt đã được tạo với các tính năng:

- **NavMeshAgent Integration**: Sử dụng Unity NavMesh cho pathfinding thông minh
- **State Machine**: Quản lý các trạng thái AI (Idle, Seeking, Moving, Combat)
- **Team System Integration**: Tích hợp với hệ thống TeamMember để nhận diện đồng minh/kẻ địch
- **Ragdoll Recovery**: Xử lý chuyển đổi từ ragdoll về active state
- **Performance Optimization**: Hệ thống LOD và culling cho multiple AI agents

## Cấu trúc Files

### Core Scripts

#### 1. AIStateMachine.cs
- **Vị trí**: `Assets/AnimalRevolt/Scripts/AI/AIStateMachine.cs`
- **Chức năng**: Quản lý state machine cho AI
- **States**: Idle, Seeking, Moving, Combat
- **Features**: State transition validation, debug logging

#### 2. AIMovementController.cs
- **Vị trí**: `Assets/AnimalRevolt/Scripts/AI/AIMovementController.cs`
- **Chức năng**: Controller chính cho AI movement
- **Features**:
  - NavMeshAgent integration
  - Enemy seeking logic
  - Ragdoll recovery
  - Performance optimization
  - Team-based targeting

#### 3. AISetupHelper.cs
- **Vị trí**: `Assets/AnimalRevolt/Scripts/AI/AISetupHelper.cs`
- **Chức năng**: Utility script để setup AI system
- **Features**:
  - Auto-setup tất cả components cần thiết
  - Team detection
  - Validation tools

#### 4. AIPerformanceManager.cs
- **Vị trí**: `Assets/AnimalRevolt/Scripts/AI/AIPerformanceManager.cs`
- **Chức năng**: Tối ưu performance cho multiple AI
- **Features**:
  - LOD system
  - Distance-based culling
  - Active AI limiting

### Modified Scripts

#### 5. CombatController.cs (Updated)
- **Vị trí**: `Assets/AnimalRevolt/Scripts/Combat/CombatController.cs`
- **Thay đổi**: Tích hợp NavMeshAgent support
- **Features**: Hybrid movement system (NavMesh + CharacterController)

## Setup Instructions

### 1. Chuẩn bị NavMesh

```csharp
// Đảm bảo scene có NavMesh được bake
// Window -> AI -> Navigation
// Chọn ground objects và mark là "Navigation Static"
// Click "Bake" để tạo NavMesh
```

### 2. Setup AI Character

#### Cách 1: Sử dụng AISetupHelper (Khuyến nghị)

```csharp
// 1. Add AISetupHelper component vào GameObject
// 2. Configure settings trong Inspector:
//    - Team Type (Player, Enemy, AI_Team1, etc.)
//    - Movement speeds
//    - Combat ranges
// 3. Click "Setup AI System" button hoặc enable "Auto Setup On Start"
```

#### Cách 2: Manual Setup

```csharp
// Required Components (theo thứ tự):
// 1. TeamMember
// 2. NavMeshAgent  
// 3. EnemyDetector
// 4. CombatController
// 5. AIMovementController
// 6. AIStateMachine

// Đảm bảo GameObject có Animator và các components cần thiết
```

### 3. Performance Setup

```csharp
// Tạo empty GameObject với AIPerformanceManager
// Hoặc add vào Game Manager existing
GameObject perfManager = new GameObject("AI Performance Manager");
perfManager.AddComponent<AIPerformanceManager>();
```

## Configuration

### AI Movement Settings

```csharp
[Header("Movement Settings")]
public float walkSpeed = 3f;          // Tốc độ đi bộ
public float runSpeed = 6f;           // Tốc độ chạy
public float stoppingDistance = 2f;   // Khoảng cách dừng
public float engageDistance = 8f;     // Khoảng cách engage combat

[Header("AI Behavior")]
public AIBehaviorType behaviorType = AIBehaviorType.Aggressive;
public float seekRadius = 15f;        // Bán kính tìm kiếm enemy
public float patrolRadius = 10f;      // Bán kính patrol
public bool enablePatrol = true;      // Có patrol khi idle không
```

### Team Configuration

```csharp
[Header("Team Settings")]
public TeamType teamType = TeamType.AI_Team1;
public string teamName = "AI_Team1";
public Color teamColor = Color.blue;
```

### Performance Settings

```csharp
[Header("Performance")]
public int maxActiveAI = 20;          // Số lượng AI active tối đa
public float updateDistance = 50f;    // Khoảng cách update AI
public float cullingDistance = 100f;  // Khoảng cách disable AI
public bool enableLOD = true;         // Có sử dụng LOD không
```

## API Reference

### AIMovementController

#### Public Methods

```csharp
// Set destination cho AI
public void SetDestination(Vector3 destination)

// Stop movement
public void StopMovement()

// Properties
public AIState CurrentState { get; }
public TeamMember CurrentTarget { get; }
public bool IsMoving { get; }
public bool CanMove { get; }
```

#### Events

```csharp
// Khi tìm thấy target
public System.Action<TeamMember> OnTargetFound;

// Khi mất target  
public System.Action<TeamMember> OnTargetLost;

// Khi set destination
public System.Action<Vector3> OnDestinationSet;

// Khi đến destination
public System.Action OnDestinationReached;
```

### AIStateMachine

#### Public Methods

```csharp
// Thay đổi state
public void ChangeState(AIState newState)

// Force change state (bỏ qua validation)
public void ForceChangeState(AIState newState)

// Kiểm tra có thể transition không
public bool CanTransitionTo(AIState targetState)
```

### AISetupHelper

#### Context Menu Commands

```csharp
[ContextMenu("Setup AI System")]      // Setup complete AI system
[ContextMenu("Remove AI System")]     // Remove tất cả AI components  
[ContextMenu("Validate AI Setup")]    // Kiểm tra setup
[ContextMenu("Auto Detect Team")]     // Tự động detect team từ tên
```

## State Machine Logic

### State Transitions

```
Idle ←→ Seeking ←→ Moving
  ↓       ↓         ↓
  ↓    Combat ←-----┘
  ↓       ↑
  └-------┘
```

### State Behaviors

#### Idle State
- Đứng yên tại chỗ
- Patrol nếu enable
- Chuyển sang Seeking khi phát hiện enemy

#### Seeking State  
- Tìm kiếm enemy gần nhất
- Chuyển sang Moving để tiếp cận
- Chuyển sang Combat khi đủ gần

#### Moving State
- Di chuyển đến destination bằng NavMesh
- Update destination khi target di chuyển
- Chuyển sang Combat khi đến gần enemy

#### Combat State
- Để CombatController handle movement
- Disable NavMesh movement
- Monitor khoảng cách để disengage

## Ragdoll Integration

### Automatic Recovery

```csharp
// Hệ thống tự động detect khi character recover từ ragdoll
// Disable NavMeshAgent khi ragdoll active
// Re-enable và warp position khi ragdoll disabled
// Reset AI state về Idle

private IEnumerator RecoverFromRagdoll()
{
    yield return new WaitForSeconds(0.2f);
    navAgent.enabled = true;
    navAgent.Warp(transform.position);
    stateMachine.ForceChangeState(AIState.Idle);
}
```

## Performance Optimization

### LOD System

```csharp
// High LOD (0-15m): Full update rate (0.1s)
// Medium LOD (15-30m): Reduced rate (0.2s) 
// Low LOD (30-50m): Low rate (0.5s)
// Disabled (50m+): No updates
```

### Distance Culling

```csharp
// Update Distance (50m): AI được update logic
// Culling Distance (100m): AI được disable hoàn toàn
// Active AI Limit (20): Tối đa 20 AI active cùng lúc
```

## Debugging

### Debug Modes

Tất cả scripts đều có `debugMode` flag để enable logging:

```csharp
[SerializeField] private bool debugMode = true;
```

### Visual Debugging

Scripts có OnDrawGizmosSelected để visualize:
- Detection ranges
- Movement paths  
- Current targets
- LOD distances

### Console Commands

```csharp
// Validate AI setup
aiSetupHelper.ValidateSetup();

// Get performance stats
var stats = AIPerformanceManager.Instance.GetPerformanceStats();
Debug.Log($"Active AI: {stats.activeAI}/{stats.totalAI}");
```

## Best Practices

### 1. NavMesh Setup
- Bake NavMesh với resolution phù hợp
- Đảm bảo tất cả walkable surfaces được mark
- Test pathfinding trước khi deploy

### 2. Team Configuration
- Sử dụng consistent naming convention
- Set team colors khác nhau cho easy debugging
- Test team detection logic

### 3. Performance
- Limit số lượng AI active dựa trên target platform
- Sử dụng LOD system cho scenes lớn
- Monitor performance stats trong gameplay

### 4. Integration
- Test ragdoll recovery thoroughly
- Đảm bảo combat system integration
- Verify event subscriptions/unsubscriptions

## Troubleshooting

### Common Issues

#### AI không di chuyển
- Kiểm tra NavMeshAgent enabled
- Verify NavMesh baked properly
- Check destination có valid không

#### AI không tìm enemy
- Verify EnemyDetector settings
- Check team configuration
- Ensure detection layers correct

#### Performance issues  
- Reduce maxActiveAI
- Increase update intervals
- Enable culling distances

#### Ragdoll không recover
- Check RagdollControllerUI integration
- Verify TeamMember IsAlive state
- Debug ragdoll state changes

## Future Enhancements

### Planned Features
- Formation movement for groups
- Advanced pathfinding (A* custom)
- Behavior trees integration
- Machine learning behaviors
- Dynamic difficulty adjustment

### Extension Points
- Custom AI behaviors via inheritance
- Pluggable decision systems  
- External AI system integration
- Analytics and telemetry

---

## Liên hệ

Nếu có vấn đề hoặc cần hỗ trợ, vui lòng:
1. Check documentation này trước
2. Test với debug mode enabled
3. Review console logs
4. Kiểm tra component setup

Hệ thống được thiết kế để dễ extend và maintain. Tất cả scripts follow Unity best practices và có comprehensive error handling.