# 🤖 AI System Integration Guide - Animal Revolt

## 📋 Tổng quan

Hệ thống AI đã được tối ưu hóa với các tính năng mới:
- **Unified Setup**: Tự động cấu hình AI character hoàn chỉnh
- **Smart Combat**: AI chiến đấu thông minh với NavMesh integration
- **Performance Optimized**: Quản lý hiệu suất tự động
- **Ragdoll Integration**: Tích hợp hoàn chỉnh với hệ thống ragdoll

## 🚀 Quick Start

### 1. Setup AI Character Tự Động

```csharp
// Thêm AICharacterSetup script vào GameObject
AICharacterSetup setup = character.AddComponent<AICharacterSetup>();

// Cấu hình cơ bản
setup.teamType = TeamType.AI_Team1;
setup.characterName = "AI Warrior";
setup.maxHealth = 100f;

// Setup tự động
setup.SetupAICharacter();
```

### 2. Scene Manager Setup

```csharp
// Tạo AISceneManager trong scene
GameObject manager = new GameObject("AI Scene Manager");
AISceneManager sceneManager = manager.AddComponent<AISceneManager>();

// Cấu hình spawn points
sceneManager.blueTeamSpawnPoints = blueSpawns;
sceneManager.redTeamSpawnPoints = redSpawns;

// Initialize scene
sceneManager.InitializeScene();
```

## 🏗️ Kiến trúc Hệ thống

### Core Components

1. **AICharacterSetup** - Unified setup script
   - Tự động cấu hình tất cả components
   - Validation và error checking
   - Support cho reflection-based configuration

2. **AIMovementController** - Enhanced movement
   - NavMesh integration cải tiến
   - Smart target selection
   - Target prediction
   - Ragdoll recovery

3. **CombatController** - Intelligent combat
   - Multiple behavior types (Aggressive, Defensive, Balanced)
   - NavMesh combat movement
   - Attack prediction và optimization

4. **AISceneManager** - Scene management
   - Character spawning và management
   - Team victory/defeat detection
   - Performance monitoring

### Component Dependencies

```
AICharacterSetup (Main)
├── TeamMember (Required)
├── EnemyDetector (Required)
├── CombatController (Required)
├── AIMovementController (Required)
├── AIStateMachine (Required)
├── NavMeshAgent (Required)
├── Animator (Optional)
└── RagdollControllerUI (Optional)
```

## 🎮 Sử dụng trong Unity Editor

### Setup Individual Character

1. **Tạo GameObject mới hoặc chọn existing character**
2. **Add AICharacterSetup component**
3. **Configure settings trong Inspector:**
   ```
   Character Settings:
   - Team Type: AI_Team1 hoặc AI_Team2
   - Character Name: "AI Warrior"
   - Max Health: 100

   Movement Settings:
   - Walk Speed: 3
   - Run Speed: 6
   - Behavior Type: Aggressive

   Combat Settings:
   - Attack Damage: 25
   - Attack Range: 2
   - Combat Behavior: Aggressive
   ```
4. **Click "Setup AI Character" trong context menu**

### Setup Scene với AISceneManager

1. **Tạo empty GameObject tên "AI Scene Manager"**
2. **Add AISceneManager component**
3. **Configure spawn points:**
   - Tạo empty GameObjects làm spawn points
   - Assign vào Blue Team Spawn Points và Red Team Spawn Points
4. **Add character prefabs vào Character Prefabs array**
5. **Click "Initialize Scene"**

## ⚙️ Configuration Options

### AI Behavior Types

#### Movement Behaviors
- **Aggressive**: Luôn chase enemy, high speed
- **Defensive**: Chỉ tấn công khi bị tấn công
- **Patrol**: Patrol area, tấn công khi detect
- **Guard**: Giữ position, defensive

#### Combat Behaviors
- **Aggressive**: Luôn tiến về phía enemy
- **Defensive**: Giữ khoảng cách, retreat khi cần
- **Balanced**: Kết hợp dựa trên health

### Performance Settings

```csharp
// AI Performance Configuration
AISceneSettings settings = new AISceneSettings()
{
    updateInterval = 0.1f,        // Update frequency
    maxActiveCharacters = 20,     // Max AI characters
    walkSpeed = 3f,              // Default walk speed
    runSpeed = 6f,               // Default run speed
    attackDamage = 25f,          // Default attack damage
    detectionRadius = 10f        // Default detection range
};
```

## 🔧 Advanced Usage

### Custom AI Behaviors

```csharp
public class CustomAIBehavior : MonoBehaviour
{
    private AIMovementController aiMovement;
    private CombatController combat;
    
    void Start()
    {
        aiMovement = GetComponent<AIMovementController>();
        combat = GetComponent<CombatController>();
        
        // Custom behavior logic
        aiMovement.OnTargetFound += OnEnemyFound;
        combat.OnAttackStarted += OnAttackStarted;
    }
    
    private void OnEnemyFound(TeamMember enemy)
    {
        // Custom reaction to enemy detection
        Debug.Log($"Enemy found: {enemy.name}");
    }
    
    private void OnAttackStarted(TeamMember target)
    {
        // Custom attack behavior
        Debug.Log($"Attacking: {target.name}");
    }
}
```

### Batch Character Setup

```csharp
// Setup multiple characters cho team
GameObject[] characters = GameObject.FindGameObjectsWithTag("AICharacter");
AICharacterSetup.SetupMultipleCharacters(characters, TeamType.AI_Team1);
```

### Runtime Spawning

```csharp
public class RuntimeSpawner : MonoBehaviour
{
    public AISceneManager sceneManager;
    public GameObject characterPrefab;
    
    public void SpawnTeam()
    {
        // Spawn 5 characters for team 1
        sceneManager.SpawnTeam(TeamType.AI_Team1, 5, new GameObject[] { characterPrefab });
    }
    
    public void SpawnSingleCharacter(Vector3 position)
    {
        // Spawn single character
        AICharacterSetup newChar = sceneManager.SpawnCharacter(characterPrefab, position, TeamType.AI_Team2);
        
        if (newChar != null)
        {
            Debug.Log($"Spawned: {newChar.name}");
        }
    }
}
```

## 🔍 Debugging và Validation

### Built-in Validation

```csharp
// Validate single character setup
AICharacterSetup setup = GetComponent<AICharacterSetup>();
setup.ValidateSetup();

// Validate entire scene
AISceneManager manager = FindObjectOfType<AISceneManager>();
manager.ShowDebugInfo();
```

### Debug Visualization

Enable debug mode trong các components để thấy:
- Gizmos cho detection radius
- Path visualization
- Attack range indicators
- Team indicators
- Health bars

### Common Issues và Solutions

#### 1. NavMesh Issues
```
Problem: AI không di chuyển
Solution: 
- Bake NavMesh cho scene (Window > AI > Navigation)
- Kiểm tra NavMeshAgent trên character
- Đảm bảo spawn points trên NavMesh
```

#### 2. Combat Issues
```
Problem: AI không tấn công
Solution:
- Kiểm tra Team Type khác nhau
- Verify Attack Range settings
- Check EnemyDetector Layer Mask
```

#### 3. Performance Issues
```
Problem: FPS thấp với nhiều AI
Solution:
- Giảm updateInterval
- Giảm maxActiveCharacters
- Enable performance monitoring
```

## 📊 Performance Monitoring

### Built-in Metrics

```csharp
// Get performance stats
AISceneManager manager = FindObjectOfType<AISceneManager>();
string stats = manager.GetPerformanceStats();
Debug.Log(stats);

// Enable performance manager
AIPerformanceManager perf = GetComponent<AIPerformanceManager>();
if (perf != null)
{
    perf.EnableMonitoring(true);
}
```

### Optimization Tips

1. **Update Intervals**: Tăng interval cho AI ít quan trọng
2. **LOD System**: Giảm AI complexity ở khoảng cách xa
3. **Culling**: Disable AI ngoài camera view
4. **Pooling**: Reuse AI objects thay vì destroy/create

## 🎯 Best Practices

### 1. Team Configuration
- Sử dụng consistent naming convention
- Set up proper layer masks cho detection
- Configure team colors để dễ debug

### 2. NavMesh Setup
- Bake NavMesh với appropriate settings
- Test spawn points trước khi build
- Ensure walkable areas đủ lớn

### 3. Performance
- Monitor FPS với nhiều AI characters
- Use profiler để identify bottlenecks
- Test trên target hardware

### 4. Debugging
- Enable debug mode during development
- Use validation methods thường xuyên
- Keep backup của working configurations

## 🚨 Troubleshooting

### Compile Errors
- Ensure tất cả scripts trong đúng namespace
- Check component dependencies
- Verify TeamType enum values

### Runtime Errors
- Use validation methods
- Check console logs cho warnings
- Enable debug mode để trace issues

### Performance Issues
- Monitor với performance manager
- Reduce active AI count nếu cần
- Optimize NavMesh complexity

## 📈 Next Steps

1. **Testing**: Test với different scenarios
2. **Tuning**: Adjust parameters for gameplay
3. **Expansion**: Add custom behaviors nếu cần
4. **Optimization**: Profile và optimize performance

---

**Lưu ý**: Hệ thống này được thiết kế để mở rộng. Bạn có thể customize behaviors, add new AI types, hoặc integrate với other systems tùy theo needs của project.