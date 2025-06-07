# Hướng Dẫn Sử Dụng Ragdoll System

## Tổng Quan
Hệ thống Ragdoll hoàn chỉnh cho Unity với các tính năng:
- Chuyển đổi mượt mà giữa animation và physics
- Quản lý hiệu suất với pooling và LOD
- Editor tools để tự động setup ragdoll
- Demo scene với UI controls

## Unity Version Requirements
**Tương thích:** Unity 2022.3 LTS và Unity 2023.x+

### Compatibility Notes
- ✅ **Unity 2023.x+**: Fully supported với updated properties
- ✅ **Unity 2022.3 LTS**: Compatible với updated properties
- ❌ **Unity 2021.x và cũ hơn**: Không được hỗ trợ (deprecated properties)

### Recent Updates (Version Compatibility Fix)
- **Fixed deprecated properties** để tương thích với Unity 2023+:
  - `rb.linearDamping` → `rb.drag`
  - `rb.angularDamping` → `rb.angularDrag`
  - `rb.linearVelocity` → `rb.velocity`
- **Verified compatibility** với Unity 2022.3+ và 2023.x
- **No warnings** trong console về deprecated APIs

## Cấu Trúc Scripts

### 1. RagdollSettings.cs
**ScriptableObject** chứa tất cả cấu hình ragdoll
```csharp
// Tạo asset mới
[CreateAssetMenu] -> Ragdoll System -> Ragdoll Settings
```

**Thông số chính:**
- `ragdollPrefabs`: Danh sách prefab từ Assets/Prefabs/
- `defaultMass`: Mass mặc định cho Rigidbody
- `transitionDuration`: Thời gian chuyển đổi
- `maxActiveRagdolls`: Số lượng ragdoll tối đa
- `ragdollLifetime`: Thời gian tự động despawn

### 2. RagdollController.cs
**Script chính** quản lý trạng thái ragdoll

**Public Methods:**
- `EnableRagdoll()`: Kích hoạt ragdoll mode
- `DisableRagdoll()`: Vô hiệu hóa ragdoll
- `ApplyForce(Vector3 force, Vector3 position)`: Áp dụng lực
- `ApplyExplosionForce(float force, Vector3 center, float radius)`: Explosion

**Properties:**
- `IsRagdollActive`: Kiểm tra trạng thái active
- `IsTransitioning`: Kiểm tra đang chuyển đổi
- `Settings`: Truy cập RagdollSettings

**Events:**
- `OnRagdollStateChanged`: Khi thay đổi trạng thái
- `OnRagdollEnabled`: Khi enable ragdoll
- `OnRagdollDisabled`: Khi disable ragdoll

### 3. RagdollManager.cs
**Singleton** quản lý tất cả ragdoll instances

**Chức năng chính:**
- Object pooling cho hiệu suất
- LOD system theo khoảng cách camera
- Automatic cleanup expired ragdolls
- Spawn/Despawn management

**Public Methods:**
- `SpawnRagdoll(GameObject prefab, Vector3 pos, Quaternion rot)`
- `SpawnRandomRagdoll(Vector3 pos, Quaternion rot)`
- `DespawnRagdoll(RagdollController controller)`
- `GetNearestRagdoll(Vector3 position)`
- `GetRagdollsInRadius(Vector3 center, float radius)`

### 4. RagdollBuilder.cs
**Editor Script** tự động setup ragdoll

**Cách sử dụng:**
1. Mở: `Tools -> Ragdoll System -> Ragdoll Builder`
2. Assign Target Character (với Animator)
3. Click "Auto Detect Bones"
4. Configure physics settings
5. Click "Build Custom Ragdoll"

**Tính năng:**
- Auto detect bones từ Humanoid rig
- Tạo Rigidbody và Collider tự động
- Setup Character Joints
- Integration với Unity Ragdoll Wizard
- Validation và cleanup tools

### 5. RagdollUtilities.cs
**Static utilities** cho các chức năng bổ trợ

**Helper Methods:**
- `GetAllRagdollBones(Transform root)`
- `CalculateTotalMass(Transform root)`
- `SetMassDistribution(Transform root, float totalMass)`
- `OptimizeRagdollForDistance(controller, distance, threshold)`
- `ApplyExplosion(Transform root, Vector3 center, float force, float radius)`
- `IsRagdollStable(Transform root, float threshold)`

**Extension Methods:**
- `controller.EnableWithForce(Vector3 force, Vector3 position)`
- `controller.IsStable(float threshold)`
- `controller.GetTotalMass()`
- `controller.BackupCurrentPose()`

### 6. RagdollDemo.cs
**Demo script** minh họa cách sử dụng

## Hướng Dẫn Setup

### Bước 1: Tạo RagdollSettings Asset
1. Right-click trong Project -> Create -> Ragdoll System -> Ragdoll Settings
2. Assign các prefab từ Assets/Prefabs/
3. Configure physics parameters

### Bước 2: Setup Character Ragdoll
**Cách 1: Sử dụng RagdollBuilder (Khuyến nghị)**
1. Mở Tools -> Ragdoll System -> Ragdoll Builder
2. Assign character có Animator với Human rig
3. Click "Auto Detect Bones"
4. Assign RagdollSettings asset
5. Click "Build Custom Ragdoll"

**Cách 2: Manual Setup**
1. Add RagdollController component vào character
2. Add Rigidbody và Collider vào các bones
3. Setup Character Joints
4. Assign RagdollSettings

### Bước 3: Setup RagdollManager
1. Tạo empty GameObject tên "RagdollManager"
2. Add RagdollManager component
3. Assign Global Settings (RagdollSettings asset)
4. Configure pooling và performance settings

### Bước 4: Test với RagdollDemo
1. Add RagdollDemo component vào scene
2. Assign target RagdollController
3. Test với keyboard controls hoặc UI buttons

## Cách Sử Dụng Trong Code

### Basic Usage
```csharp
// Get controller
RagdollController ragdoll = GetComponent<RagdollController>();

// Enable ragdoll
ragdoll.EnableRagdoll();

// Apply force
Vector3 force = Vector3.forward * 500f;
Vector3 position = transform.position + Vector3.up;
ragdoll.ApplyForce(force, position);

// Check status
if (ragdoll.IsRagdollActive)
{
    Debug.Log("Ragdoll is active");
}
```

### Spawn Management
```csharp
// Spawn từ manager
RagdollController newRagdoll = RagdollManager.Instance.SpawnRandomRagdoll(
    spawnPosition, 
    spawnRotation
);

// Enable và apply force ngay
newRagdoll.EnableWithForce(force, position);

// Despawn khi không cần
RagdollManager.Instance.DespawnRagdoll(newRagdoll);
```

### Events Handling
```csharp
void Start()
{
    ragdoll.OnRagdollEnabled += OnRagdollActivated;
    ragdoll.OnRagdollDisabled += OnRagdollDeactivated;
    ragdoll.OnRagdollStateChanged += OnStateChanged;
}

void OnRagdollActivated()
{
    Debug.Log("Ragdoll activated!");
    // Play sound effects, particles, etc.
}

void OnStateChanged(bool isActive)
{
    // Update UI, disable AI, etc.
}
```

### Advanced Usage
```csharp
// Mass distribution
RagdollUtilities.SetMassDistribution(transform, 50f);

// Check stability
if (ragdoll.IsStable(0.1f))
{
    Debug.Log("Ragdoll has stopped moving");
}

// Explosion effect
Vector3 explosionCenter = transform.position;
RagdollUtilities.ApplyExplosion(transform, explosionCenter, 1000f, 5f);

// Backup/Restore poses
RagdollPose pose = ragdoll.BackupCurrentPose();
// ... later ...
RagdollUtilities.RestorePose(pose);
```

## Performance Optimization

### LOD System
Ragdoll quality tự động giảm khi xa camera:
- Interpolation: Interpolate -> None
- Collision Detection: Continuous -> Discrete
- Disable non-essential colliders

### Object Pooling
```csharp
// Enable pooling trong RagdollManager
enablePooling = true;
poolSize = 10;

// Ragdolls được reuse thay vì destroy/instantiate
```

### Automatic Cleanup
```csharp
// Ragdolls tự động despawn sau ragdollLifetime
// Cleanup chạy mỗi cleanupInterval giây
```

## Keyboard Controls (Demo)
- **Space**: Toggle ragdoll on/off
- **F**: Apply random force
- **N**: Spawn new ragdoll
- **E**: Create explosion
- **A**: Toggle auto demo
- **Mouse Click**: Apply force at cursor

## Tips & Best Practices

### 1. Bone Setup
- Đảm bảo character có Humanoid rig
- Assign đúng bones trong RagdollBuilder
- Core bones quan trọng: Pelvis, Spine, Head

### 2. Physics Tuning
- Bắt đầu với default values trong RagdollSettings
- Tăng dần mass cho realistic behavior
- Adjust joint limits theo animation của character

### 3. Performance
- Sử dụng pooling cho nhiều ragdolls
- Enable LOD để giảm quality khi xa
- Set reasonable ragdollLifetime

### 4. Integration
- Disable AI/navigation khi ragdoll active
- Handle animation blending
- Use events để sync với game systems

## Troubleshooting

### Ragdoll không hoạt động
- Kiểm tra có RagdollController component
- Đảm bảo có Rigidbody trên bones
- Verify RagdollSettings được assign

### Physics không realistic
- Adjust mass distribution
- Check joint constraints
- Tune drag và angular drag values

### Performance issues
- Enable pooling
- Reduce maxActiveRagdolls
- Increase LOD distance
- Shorter ragdollLifetime

### Transition không smooth
- Tăng transitionDuration
- Backup/restore animation poses
- Use proper blend trees

## Ví Dụ Integration

### Character Death System
```csharp
public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private RagdollController ragdoll;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    
    public void Die(Vector3 damageDirection, float force)
    {
        // Disable game components
        agent.enabled = false;
        animator.enabled = false;
        
        // Enable ragdoll
        ragdoll.EnableRagdoll();
        
        // Apply death force
        Vector3 position = transform.position + Vector3.up;
        ragdoll.ApplyForce(damageDirection * force, position);
    }
}
```

### Explosion Effect
```csharp
public class ExplosionEffect : MonoBehaviour
{
    public void Explode(Vector3 center, float force, float radius)
    {
        var ragdolls = RagdollManager.Instance.GetRagdollsInRadius(center, radius);
        
        foreach (var ragdoll in ragdolls)
        {
            if (!ragdoll.IsRagdollActive)
                ragdoll.EnableRagdoll();
                
            ragdoll.ApplyExplosionForce(force, center, radius);
        }
    }
}
```

Hệ thống Ragdoll này cung cấp solution hoàn chỉnh, dễ sử dụng và có thể tùy chỉnh cho mọi dự án Unity!