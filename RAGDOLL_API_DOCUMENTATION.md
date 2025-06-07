# 📚 RAGDOLL SYSTEM - API DOCUMENTATION

## 📖 Tổng Quan
Tài liệu API chi tiết cho Ragdoll System, bao gồm tất cả public methods, properties, events và usage examples.

---

## 🎯 RagdollController

### Properties

#### `bool IsRagdollActive { get; }`
**Mô tả**: Kiểm tra xem ragdoll có đang active không  
**Return**: `true` nếu ragdoll đang active, `false` nếu không  
**Example**:
```csharp
if (ragdollController.IsRagdollActive)
{
    Debug.Log("Ragdoll is currently active");
}
```

#### `bool IsTransitioning { get; }`
**Mô tả**: Kiểm tra xem ragdoll có đang trong quá trình chuyển đổi không  
**Return**: `true` nếu đang transition, `false` nếu không  
**Example**:
```csharp
if (ragdollController.IsTransitioning)
{
    Debug.Log("Ragdoll is transitioning between states");
}
```

#### `RagdollSettings Settings { get; }`
**Mô tả**: Truy cập settings configuration của ragdoll  
**Return**: Reference tới RagdollSettings ScriptableObject  
**Example**:
```csharp
float mass = ragdollController.Settings.defaultMass;
```

### Methods

#### `void EnableRagdoll()`
**Mô tả**: Kích hoạt ragdoll mode  
**Parameters**: Không có  
**Return**: `void`  
**Example**:
```csharp
ragdollController.EnableRagdoll();
```

#### `void DisableRagdoll()`
**Mô tả**: Vô hiệu hóa ragdoll mode và trở về animation  
**Parameters**: Không có  
**Return**: `void`  
**Example**:
```csharp
ragdollController.DisableRagdoll();
```

#### `void ApplyForce(Vector3 force, Vector3 position, ForceMode forceMode = ForceMode.Impulse)`
**Mô tả**: Áp dụng lực lên ragdoll tại vị trí cụ thể  
**Parameters**:
- `force` (Vector3): Vector lực cần áp dụng
- `position` (Vector3): Vị trí world space để áp dụng lực
- `forceMode` (ForceMode): Kiểu lực (mặc định: Impulse)

**Return**: `void`  
**Example**:
```csharp
Vector3 force = Vector3.forward * 500f;
Vector3 position = transform.position + Vector3.up;
ragdollController.ApplyForce(force, position);
```

#### `void ApplyExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)`
**Mô tả**: Áp dụng lực explosion lên toàn bộ ragdoll  
**Parameters**:
- `explosionForce` (float): Cường độ explosion
- `explosionPosition` (Vector3): Tâm explosion  
- `explosionRadius` (float): Bán kính ảnh hưởng

**Return**: `void`  
**Example**:
```csharp
ragdollController.ApplyExplosionForce(1000f, Vector3.zero, 5f);
```

#### `void ToggleRagdoll()` [ContextMenu]
**Mô tả**: Toggle between ragdoll và animation mode  
**Parameters**: Không có  
**Return**: `void`  
**Usage**: Có thể gọi từ Inspector hoặc code  

#### `void ApplyRandomForce()` [ContextMenu]
**Mô tả**: Áp dụng lực ngẫu nhiên để test  
**Parameters**: Không có  
**Return**: `void`  
**Usage**: Chỉ để testing/debugging  

### Events

#### `System.Action<bool> OnRagdollStateChanged`
**Mô tả**: Event được trigger khi trạng thái ragdoll thay đổi  
**Parameters**: `bool isActive` - trạng thái mới  
**Example**:
```csharp
ragdollController.OnRagdollStateChanged += (isActive) => {
    Debug.Log($"Ragdoll state changed to: {isActive}");
};
```

#### `System.Action OnRagdollEnabled`
**Mô tả**: Event được trigger khi ragdoll được enable  
**Parameters**: Không có  
**Example**:
```csharp
ragdollController.OnRagdollEnabled += () => {
    Debug.Log("Ragdoll has been enabled");
};
```

#### `System.Action OnRagdollDisabled`
**Mô tả**: Event được trigger khi ragdoll được disable  
**Parameters**: Không có  
**Example**:
```csharp
ragdollController.OnRagdollDisabled += () => {
    Debug.Log("Ragdoll has been disabled");
};
```

---

## 🎮 RagdollManager

### Static Properties

#### `RagdollManager Instance { get; }`
**Mô tả**: Singleton instance của RagdollManager  
**Return**: RagdollManager instance  
**Example**:
```csharp
var manager = RagdollManager.Instance;
```

### Properties

#### `int ActiveRagdollCount { get; }`
**Mô tả**: Số lượng ragdoll đang active  
**Return**: Số nguyên  
**Example**:
```csharp
int count = RagdollManager.Instance.ActiveRagdollCount;
```

#### `int PooledRagdollCount { get; }`
**Mô tả**: Số lượng ragdoll trong pool  
**Return**: Số nguyên  
**Example**:
```csharp
int pooled = RagdollManager.Instance.PooledRagdollCount;
```

#### `RagdollSettings GlobalSettings { get; }`
**Mô tả**: Global settings cho tất cả ragdolls  
**Return**: RagdollSettings reference  

### Methods

#### `RagdollController SpawnRagdoll(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)`
**Mô tả**: Spawn ragdoll từ prefab cụ thể  
**Parameters**:
- `prefab` (GameObject): Prefab để spawn
- `position` (Vector3): Vị trí spawn
- `rotation` (Quaternion): Rotation khi spawn
- `parent` (Transform): Parent transform (optional)

**Return**: `RagdollController` - Controller của ragdoll vừa spawn  
**Example**:
```csharp
var newRagdoll = RagdollManager.Instance.SpawnRagdoll(
    ragdollPrefab, 
    spawnPosition, 
    Quaternion.identity
);
```

#### `RagdollController SpawnRandomRagdoll(Vector3 position, Quaternion rotation, Transform parent = null)`
**Mô tả**: Spawn ragdoll ngẫu nhiên từ settings  
**Parameters**: Tương tự SpawnRagdoll nhưng không cần prefab  
**Return**: `RagdollController`  
**Example**:
```csharp
var randomRagdoll = RagdollManager.Instance.SpawnRandomRagdoll(
    spawnPosition, 
    Quaternion.identity
);
```

#### `void DespawnRagdoll(RagdollController controller)`
**Mô tả**: Despawn ragdoll về pool hoặc destroy  
**Parameters**:
- `controller` (RagdollController): Controller cần despawn

**Return**: `void`  
**Example**:
```csharp
RagdollManager.Instance.DespawnRagdoll(ragdollController);
```

#### `void RegisterRagdoll(RagdollController controller)`
**Mô tả**: Đăng ký ragdoll với manager  
**Parameters**:
- `controller` (RagdollController): Controller cần đăng ký

**Return**: `void`  
**Note**: Thường được gọi tự động  

#### `void UnregisterRagdoll(RagdollController controller)`
**Mô tả**: Hủy đăng ký ragdoll khỏi manager  
**Parameters**:
- `controller` (RagdollController): Controller cần hủy đăng ký

**Return**: `void`  
**Note**: Thường được gọi tự động  

#### `void DespawnAllRagdolls()`
**Mô tả**: Despawn tất cả ragdolls đang active  
**Parameters**: Không có  
**Return**: `void`  
**Example**:
```csharp
RagdollManager.Instance.DespawnAllRagdolls();
```

#### `RagdollController GetNearestRagdoll(Vector3 position)`
**Mô tả**: Tìm ragdoll gần nhất với position  
**Parameters**:
- `position` (Vector3): Vị trí tham chiếu

**Return**: `RagdollController` hoặc `null`  
**Example**:
```csharp
var nearest = RagdollManager.Instance.GetNearestRagdoll(playerPosition);
```

#### `List<RagdollController> GetRagdollsInRadius(Vector3 center, float radius)`
**Mô tả**: Lấy tất cả ragdolls trong bán kính  
**Parameters**:
- `center` (Vector3): Tâm vòng tròn
- `radius` (float): Bán kính

**Return**: `List<RagdollController>`  
**Example**:
```csharp
var ragdollsInRange = RagdollManager.Instance.GetRagdollsInRadius(
    explosionCenter, 
    explosionRadius
);
```

### Events

#### `System.Action<RagdollController> OnRagdollSpawned`
**Mô tả**: Event khi ragdoll được spawn  
**Parameters**: `RagdollController` - ragdoll vừa spawn  

#### `System.Action<RagdollController> OnRagdollDespawned`
**Mô tả**: Event khi ragdoll được despawn  
**Parameters**: `RagdollController` - ragdoll vừa despawn  

#### `System.Action<int> OnActiveRagdollCountChanged`
**Mô tả**: Event khi số lượng active ragdoll thay đổi  
**Parameters**: `int` - số lượng mới  

---

## ⚙️ RagdollSettings

### Properties

#### `List<GameObject> ragdollPrefabs`
**Mô tả**: Danh sách prefab ragdoll  
**Type**: `List<GameObject>`  
**Usage**: Assign trong Inspector  

#### `float defaultMass`
**Mô tả**: Mass mặc định cho Rigidbody  
**Type**: `float`  
**Range**: 0.1f - 10f  
**Default**: 1f  

#### `float defaultDrag`
**Mô tả**: Linear damping mặc định  
**Type**: `float`  
**Range**: 0f - 10f  
**Default**: 0.5f  

#### `float defaultAngularDrag`
**Mô tả**: Angular damping mặc định  
**Type**: `float`  
**Range**: 0f - 10f  
**Default**: 5f  

#### `float transitionDuration`
**Mô tả**: Thời gian chuyển đổi giữa states  
**Type**: `float`  
**Range**: 0f - 2f  
**Default**: 0.2f  

#### `int maxActiveRagdolls`
**Mô tả**: Số lượng ragdoll tối đa cùng lúc  
**Type**: `int`  
**Range**: 1 - 50  
**Default**: 10  

#### `float ragdollLifetime`
**Mô tả**: Thời gian tự động despawn (giây)  
**Type**: `float`  
**Range**: 5f - 60f  
**Default**: 15f  

### Methods

#### `GameObject GetRandomPrefab()`
**Mô tả**: Lấy prefab ngẫu nhiên từ danh sách  
**Return**: `GameObject` hoặc `null`  
**Example**:
```csharp
GameObject randomPrefab = settings.GetRandomPrefab();
```

#### `GameObject GetPrefab(int index)`
**Mô tả**: Lấy prefab theo index  
**Parameters**:
- `index` (int): Chỉ số trong danh sách

**Return**: `GameObject` hoặc `null`  
**Example**:
```csharp
GameObject specificPrefab = settings.GetPrefab(0);
```

---

## 🛠️ RagdollUtilities

### Static Methods

#### `List<Rigidbody> GetAllRagdollBones(Transform root)`
**Mô tả**: Tìm tất cả Rigidbody trong hierarchy  
**Parameters**:
- `root` (Transform): Root transform

**Return**: `List<Rigidbody>`  
**Example**:
```csharp
var bones = RagdollUtilities.GetAllRagdollBones(ragdollRoot);
```

#### `float CalculateTotalMass(Transform root)`
**Mô tả**: Tính tổng mass của ragdoll  
**Parameters**:
- `root` (Transform): Root transform

**Return**: `float` - tổng mass  
**Example**:
```csharp
float totalMass = RagdollUtilities.CalculateTotalMass(ragdollRoot);
```

#### `void SetMassDistribution(Transform root, float totalMass, Dictionary<string, float> distribution = null)`
**Mô tả**: Phân bố mass cho các bones  
**Parameters**:
- `root` (Transform): Root transform
- `totalMass` (float): Tổng mass cần phân bố
- `distribution` (Dictionary): Custom distribution (optional)

**Example**:
```csharp
RagdollUtilities.SetMassDistribution(ragdollRoot, 70f);
```

#### `bool IsRagdollStable(Transform root, float threshold = 0.1f)`
**Mô tả**: Kiểm tra ragdoll đã dừng chuyển động chưa  
**Parameters**:
- `root` (Transform): Root transform
- `threshold` (float): Ngưỡng velocity

**Return**: `bool`  
**Example**:
```csharp
if (RagdollUtilities.IsRagdollStable(ragdollRoot))
{
    Debug.Log("Ragdoll has stopped moving");
}
```

#### `void ApplyExplosion(Transform root, Vector3 center, float force, float radius)`
**Mô tả**: Áp dụng explosion force lên ragdoll  
**Parameters**:
- `root` (Transform): Root ragdoll
- `center` (Vector3): Tâm explosion
- `force` (float): Cường độ lực
- `radius` (float): Bán kính ảnh hưởng

**Example**:
```csharp
RagdollUtilities.ApplyExplosion(ragdollRoot, explosionCenter, 1000f, 5f);
```

#### `RagdollPose BackupPose(Transform root)`
**Mô tả**: Backup pose hiện tại của ragdoll  
**Parameters**:
- `root` (Transform): Root ragdoll

**Return**: `RagdollPose`  
**Example**:
```csharp
RagdollPose pose = RagdollUtilities.BackupPose(ragdollRoot);
```

#### `void RestorePose(RagdollPose pose)`
**Mô tả**: Restore pose từ backup  
**Parameters**:
- `pose` (RagdollPose): Pose đã backup

**Example**:
```csharp
RagdollUtilities.RestorePose(savedPose);
```

---

## 🎯 Extension Methods

### RagdollController Extensions

#### `void EnableWithForce(Vector3 force, Vector3 position)`
**Mô tả**: Enable ragdoll và apply force ngay lập tức  
**Parameters**:
- `force` (Vector3): Lực cần áp dụng
- `position` (Vector3): Vị trí áp dụng lực

**Example**:
```csharp
ragdollController.EnableWithForce(Vector3.forward * 500f, hitPosition);
```

#### `bool IsStable(float threshold = 0.1f)`
**Mô tả**: Kiểm tra ragdoll có stable không  
**Parameters**:
- `threshold` (float): Ngưỡng velocity

**Return**: `bool`  
**Example**:
```csharp
if (ragdollController.IsStable())
{
    Debug.Log("Ragdoll is stable");
}
```

#### `float GetTotalMass()`
**Mô tả**: Lấy tổng mass của ragdoll  
**Return**: `float`  
**Example**:
```csharp
float mass = ragdollController.GetTotalMass();
```

#### `RagdollPose BackupCurrentPose()`
**Mô tả**: Backup pose hiện tại  
**Return**: `RagdollPose`  
**Example**:
```csharp
RagdollPose currentPose = ragdollController.BackupCurrentPose();
```

---

## 📊 Data Structures

### RagdollPose
```csharp
[System.Serializable]
public class RagdollPose
{
    public Transform[] boneTransforms;  // Danh sách bone transforms
    public Vector3[] positions;         // Vị trí của từng bone
    public Quaternion[] rotations;      // Rotation của từng bone
    public float timestamp;             // Thời gian backup
}
```

---

## 🚨 Error Handling

### Common Exceptions

#### Missing Components
```csharp
// RagdollController yêu cầu Animator
[RequireComponent(typeof(Animator))]
```

#### Null Reference Protection
```csharp
if (targetRagdoll == null)
{
    Debug.LogWarning("Target ragdoll is null!");
    return;
}
```

#### Settings Validation
```csharp
if (settings == null)
{
    LoadDefaultSettings(); // Tự động load default
}
```

---

## 🔧 Best Practices

### Performance
- Sử dụng object pooling cho nhiều ragdolls
- Enable LOD system để giảm quality khi xa camera
- Set reasonable `ragdollLifetime` để tự động cleanup

### Physics Tuning
- Bắt đầu với default values trong RagdollSettings
- Tăng dần mass cho realistic behavior
- Adjust joint limits theo animation của character

### Integration
- Disable AI/navigation khi ragdoll active
- Handle animation blending smooth
- Sử dụng events để sync với game systems

---

**📝 Cập nhật lần cuối**: 6/5/2025  
**🔖 Version**: 1.0  
**👨‍💻 Tác giả**: Ragdoll System Team