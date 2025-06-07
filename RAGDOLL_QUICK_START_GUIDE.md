# ⚡ RAGDOLL SYSTEM - QUICK START GUIDE

## 🎯 Mục Tiêu
Hướng dẫn thiết lập và sử dụng Ragdoll System trong **5-10 phút** với các bước đơn giản nhất.

---

## 📋 Yêu Cầu Tối Thiểu
- ✅ Unity 2022.3 LTS hoặc Unity 2023.x+
- ✅ Character model với **Humanoid rig**
- ✅ Universal Render Pipeline (URP) - khuyến nghị

---

## 🚀 SETUP NHANH (5 PHÚT)

### Bước 1: Tạo RagdollSettings Asset (30 giây)
```
1. Right-click trong Project Window
2. Create → Ragdoll System → Ragdoll Settings
3. Đặt tên: "MyRagdollSettings"
4. ✅ Done!
```

### Bước 2: Setup Character Ragdoll (2 phút)
```
1. Mở Tools → Ragdoll System → Ragdoll Builder
2. Drag character vào "Target Character"
3. Assign "MyRagdollSettings" vào "Ragdoll Settings"
4. Click "Auto Detect Bones"
5. Click "Build Custom Ragdoll"
6. ✅ Ragdoll Ready!
```

### Bước 3: Setup RagdollManager (1 phút)
```
1. Tạo empty GameObject tên "RagdollManager"
2. Add component: RagdollManager
3. Assign "MyRagdollSettings" vào "Global Settings"
4. ✅ Manager Ready!
```

### Bước 4: Test Ragdoll (30 giây)
```
1. Select character trong scene
2. Trong Inspector, click "Toggle Ragdoll"
3. Hoặc nhấn Play và click "Apply Random Force"
4. ✅ Ragdoll Working!
```

---

## 🎮 SỬ DỤNG CƠ BẢN

### Code Đơn Giản Nhất
```csharp
// Lấy component
RagdollController ragdoll = GetComponent<RagdollController>();

// Enable ragdoll
ragdoll.EnableRagdoll();

// Apply force khi hit
Vector3 hitForce = hitDirection * 500f;
ragdoll.ApplyForce(hitForce, hitPosition);
```

### Keyboard Controls (Demo)
```
Space: Toggle Ragdoll On/Off
F: Apply Random Force  
N: Spawn New Ragdoll
E: Create Explosion
Mouse Click: Apply Force tại cursor
```

---

## 📁 FILE STRUCTURE SAU KHI SETUP

```
Assets/
├── Scripts/
│   ├── RagdollController.cs     ← Đã có
│   ├── RagdollManager.cs        ← Đã có  
│   ├── RagdollSettings.cs       ← Đã có
│   └── RagdollUtilities.cs      ← Đã có
├── Settings/
│   └── MyRagdollSettings.asset  ← Vừa tạo
└── Prefabs/
    └── [Your Character].prefab  ← Character có ragdoll
```

---

## ⚙️ CẤU HÌNH MẶC ĐỊNH

### RagdollSettings Cơ Bản
```
✅ Default Mass: 1.0
✅ Default Drag: 0.5  
✅ Angular Drag: 5.0
✅ Transition Duration: 0.2s
✅ Max Active Ragdolls: 10
✅ Ragdoll Lifetime: 15s
✅ LOD Distance: 30 units
```

### Character Setup Tự Động
```
✅ Rigidbody trên tất cả bones chính
✅ Collider phù hợp cho từng bone part
✅ Character Joints với limits hợp lý
✅ RagdollController component
✅ Physics materials tối ưu
```

---

## 🔧 INTEGRATION VÀO GAME

### 1. Character Death System
```csharp
public void OnCharacterDeath(Vector3 damageDirection, float force)
{
    // Disable game components
    GetComponent<NavMeshAgent>().enabled = false;
    GetComponent<Animator>().enabled = false;
    
    // Enable ragdoll với death force
    var ragdoll = GetComponent<RagdollController>();
    ragdoll.EnableRagdoll();
    ragdoll.ApplyForce(damageDirection * force, transform.position);
}
```

### 2. Explosion Effect  
```csharp
public void CreateExplosion(Vector3 center, float force, float radius)
{
    var ragdolls = RagdollManager.Instance.GetRagdollsInRadius(center, radius);
    
    foreach (var ragdoll in ragdolls)
    {
        ragdoll.EnableRagdoll();
        ragdoll.ApplyExplosionForce(force, center, radius);
    }
}
```

### 3. Spawning System
```csharp
public void SpawnEnemy(Vector3 position)
{
    var newRagdoll = RagdollManager.Instance.SpawnRandomRagdoll(
        position, 
        Quaternion.identity
    );
    
    // Enemies bắt đầu ở animation mode
    // Sẽ chuyển sang ragdoll khi chết
}
```

---

## 🎨 UI INTEGRATION

### Basic UI Setup
```csharp
public class RagdollUI : MonoBehaviour
{
    [SerializeField] private Button ragdollButton;
    [SerializeField] private RagdollController target;
    
    void Start()
    {
        ragdollButton.onClick.AddListener(() => {
            if (target.IsRagdollActive)
                target.DisableRagdoll();
            else
                target.EnableRagdoll();
        });
    }
}
```

---

## 🐛 TROUBLESHOOTING NHANH

### ❌ Ragdoll không hoạt động
**Giải pháp**:
```
1. Check character có Humanoid rig không
2. Verify RagdollSettings đã assign
3. Đảm bảo có Rigidbody trên bones
```

### ❌ Physics không realistic
**Giải pháp**:
```
1. Adjust mass values (thử 0.5-2.0)
2. Increase angular drag (thử 10-15)
3. Check joint limits không quá tight
```

### ❌ Performance lag
**Giải pháp**:
```
1. Giảm maxActiveRagdolls xuống 5-8
2. Enable pooling trong RagdollManager
3. Giảm ragdollLifetime xuống 10s
```

### ❌ Transition không smooth
**Giải pháp**:
```
1. Tăng transitionDuration lên 0.5s
2. Check animator có blend trees không
3. Disable Animator trước khi enable ragdoll
```

---

## 📊 PERFORMANCE TIPS

### Tối Ưu Cơ Bản
```csharp
// Enable pooling
RagdollManager.Instance.enablePooling = true;

// Set reasonable limits
settings.maxActiveRagdolls = 8;
settings.ragdollLifetime = 12f;

// Use LOD
settings.lodDistance = 25f;
```

### Mobile Optimization
```csharp
// Giảm physics quality
settings.useContinuousCollision = false;

// Shorter lifetime
settings.ragdollLifetime = 8f;

// Fewer active ragdolls
settings.maxActiveRagdolls = 5;
```

---

## 🧪 TESTING

### Scene Testing
```
1. Mở Assets/Scenes/SimpleRagdollDemo.unity
2. Nhấn Play
3. Test với keyboard controls
4. Verify all functions work
```

### Build Testing
```
1. Add scene vào Build Settings
2. Build and run
3. Test performance trên target device
4. Adjust settings nếu cần
```

---

## 📚 NEXT STEPS

### Nâng Cao
1. 📖 Đọc [`RAGDOLL_SYSTEM_GUIDE.md`](RAGDOLL_SYSTEM_GUIDE.md) - Documentation đầy đủ
2. 📖 Đọc [`RAGDOLL_API_DOCUMENTATION.md`](RAGDOLL_API_DOCUMENTATION.md) - API chi tiết
3. 🎯 Tùy chỉnh physics parameters trong RagdollSettings
4. 🔧 Custom force calculations cho game mechanics
5. 🎨 Integrate với VFX và sound effects

### Advanced Features
1. **Custom Physics Materials** - Bouncy, friction effects
2. **Joint Breaking** - Dismemberment system  
3. **Pose Blending** - Smooth animation transitions
4. **LOD Customization** - Distance-based quality
5. **Event System** - Game state reactions

---

## ✅ CHECKLIST HOÀN THÀNH

- [ ] RagdollSettings asset được tạo
- [ ] Character có RagdollController component
- [ ] RagdollManager được setup trong scene
- [ ] Test basic enable/disable ragdoll
- [ ] Test apply force functions
- [ ] Verify performance acceptable
- [ ] Integration với game mechanics
- [ ] UI controls working (nếu có)

---

**🎉 Chúc mừng! Bạn đã setup thành công Ragdoll System!**

**⏱️ Thời gian**: ~5-10 phút  
**🎯 Kết quả**: Ragdoll system hoạt động đầy đủ  
**📈 Next**: Customize theo game requirements  

---

**📝 Cập nhật lần cuối**: 6/5/2025  
**🔖 Version**: 1.0  
**⚡ Quick Start Guide**: Complete