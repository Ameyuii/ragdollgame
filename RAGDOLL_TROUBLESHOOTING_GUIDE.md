# 🔧 RAGDOLL SYSTEM - TROUBLESHOOTING GUIDE

## 🎯 Mục Tiêu
Hướng dẫn giải quyết các vấn đề thường gặp khi sử dụng Ragdoll System, từ setup cơ bản đến performance optimization.

---

## 🚨 COMMON ISSUES & SOLUTIONS

### ❌ Issue 1: Ragdoll Không Hoạt Động

#### Triệu chứng:
- Gọi `EnableRagdoll()` nhưng character không chuyển sang ragdoll mode
- Character vẫn ở animation state
- Không có reaction khi apply force

#### Nguyên nhân & Giải pháp:

**🔍 Check 1: RagdollController Component**
```csharp
// Verify component exists
RagdollController ragdoll = GetComponent<RagdollController>();
if (ragdoll == null)
{
    Debug.LogError("Missing RagdollController component!");
    // Solution: Add RagdollController component
}
```

**🔍 Check 2: RagdollSettings Assignment**
```csharp
// Check settings
if (ragdoll.Settings == null)
{
    Debug.LogError("RagdollSettings not assigned!");
    // Solution: Assign RagdollSettings asset in Inspector
}
```

**🔍 Check 3: Rigidbody Components**
```csharp
// Verify rigidbodies exist
Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
if (rigidbodies.Length == 0)
{
    Debug.LogError("No Rigidbody components found!");
    // Solution: Use RagdollBuilder to setup rigidbodies
}
```

**🔍 Check 4: Animator Conflicts**
```csharp
// Check animator state
Animator animator = GetComponent<Animator>();
if (animator != null && animator.enabled && ragdoll.IsRagdollActive)
{
    Debug.LogWarning("Animator still enabled during ragdoll!");
    // Solution: Ensure animator is disabled when ragdoll active
    animator.enabled = false;
}
```

**✅ Complete Fix:**
```csharp
public void DiagnoseRagdoll()
{
    var ragdoll = GetComponent<RagdollController>();
    
    // Check 1: Component
    if (ragdoll == null)
    {
        Debug.LogError("Add RagdollController component");
        return;
    }
    
    // Check 2: Settings
    if (ragdoll.Settings == null)
    {
        Debug.LogError("Assign RagdollSettings in Inspector");
        return;
    }
    
    // Check 3: Rigidbodies
    var rigidbodies = GetComponentsInChildren<Rigidbody>();
    if (rigidbodies.Length == 0)
    {
        Debug.LogError("Use Tools > Ragdoll Builder to setup physics");
        return;
    }
    
    // Check 4: Humanoid rig
    var animator = GetComponent<Animator>();
    if (animator != null && !animator.isHuman)
    {
        Debug.LogError("Character must have Humanoid rig");
        return;
    }
    
    Debug.Log("Ragdoll setup appears correct!");
}
```

---

### ❌ Issue 2: Physics Không Realistic

#### Triệu chứng:
- Character bay quá xa khi apply force
- Ragdoll quay như con quay
- Movement không tự nhiên
- Character không ngã đúng cách

#### Nguyên nhân & Giải pháp:

**🔍 Problem: Mass Values Quá Thấp**
```csharp
// Check current masses
var rigidbodies = GetComponentsInChildren<Rigidbody>();
foreach (var rb in rigidbodies)
{
    if (rb.mass < 0.5f)
    {
        Debug.LogWarning($"Low mass on {rb.name}: {rb.mass}");
        // Solution: Increase mass
        rb.mass = 1.0f; // Reasonable default
    }
}
```

**🔍 Problem: Drag Values Không Phù Hợp**
```csharp
// Fix drag values
foreach (var rb in rigidbodies)
{
    // Typical good values
    rb.linearDamping = 0.5f;  // Tăng nếu movement quá floaty
    rb.angularDamping = 10f;  // Tăng để giảm spinning
}
```

**🔍 Problem: Joint Limits Quá Loose**
```csharp
// Fix joint constraints
var joints = GetComponentsInChildren<CharacterJoint>();
foreach (var joint in joints)
{
    // Reasonable limits for human joints
    joint.swing1Limit = new SoftJointLimit { limit = 30f };
    joint.swing2Limit = new SoftJointLimit { limit = 30f };
    joint.highTwistLimit = new SoftJointLimit { limit = 15f };
    joint.lowTwistLimit = new SoftJointLimit { limit = -15f };
}
```

**✅ Realistic Physics Setup:**
```csharp
[ContextMenu("Fix Physics")]
public void FixRagdollPhysics()
{
    // 1. Set realistic masses
    var totalMass = 70f; // Average human weight
    RagdollUtilities.SetMassDistribution(transform, totalMass);
    
    // 2. Adjust drag values
    var rigidbodies = GetComponentsInChildren<Rigidbody>();
    foreach (var rb in rigidbodies)
    {
        rb.linearDamping = 1f;   // Reduce floatiness
        rb.angularDamping = 15f; // Reduce spinning
        
        // Enable continuous collision for important bones
        if (rb.name.Contains("Pelvis") || rb.name.Contains("Spine"))
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }
    
    // 3. Tighten joint limits
    var joints = GetComponentsInChildren<CharacterJoint>();
    foreach (var joint in joints)
    {
        joint.swing1Limit = new SoftJointLimit { limit = 25f };
        joint.swing2Limit = new SoftJointLimit { limit = 25f };
        joint.highTwistLimit = new SoftJointLimit { limit = 10f };
        joint.lowTwistLimit = new SoftJointLimit { limit = -10f };
        
        // Add spring to joints
        joint.swingLimitSpring = new SoftJointLimitSpring { spring = 100f, damper = 5f };
    }
    
    Debug.Log("Physics settings optimized for realism");
}
```

---

### ❌ Issue 3: Performance Problems

#### Triệu chứng:
- Frame rate drops khi có nhiều ragdolls
- Game lag khi enable ragdoll
- Memory usage tăng cao
- Mobile device chạy chậm

#### Nguyên nhân & Giải pháp:

**🔍 Problem: Quá Nhiều Active Ragdolls**
```csharp
// Check active count
int activeCount = RagdollManager.Instance.ActiveRagdollCount;
if (activeCount > 8) // Too many for most games
{
    Debug.LogWarning($"Too many active ragdolls: {activeCount}");
    
    // Solution: Reduce max active
    RagdollManager.Instance.GlobalSettings.maxActiveRagdolls = 5;
}
```

**🔍 Problem: Pooling Disabled**
```csharp
// Enable pooling for better performance
var manager = RagdollManager.Instance;
if (!manager.enablePooling)
{
    Debug.LogWarning("Object pooling disabled!");
    manager.enablePooling = true;
    manager.poolSize = 10;
}
```

**🔍 Problem: LOD System Không Hoạt Động**
```csharp
// Check LOD settings
if (!RagdollManager.Instance.enableLOD)
{
    Debug.LogWarning("LOD system disabled!");
    // Enable LOD
    RagdollManager.Instance.enableLOD = true;
    RagdollManager.Instance.GlobalSettings.lodDistance = 30f;
}
```

**✅ Performance Optimization:**
```csharp
[ContextMenu("Optimize Performance")]
public void OptimizePerformance()
{
    var manager = RagdollManager.Instance;
    var settings = manager.GlobalSettings;
    
    // 1. Limit active ragdolls
    settings.maxActiveRagdolls = Application.isMobilePlatform ? 3 : 8;
    
    // 2. Enable pooling
    manager.enablePooling = true;
    manager.poolSize = settings.maxActiveRagdolls * 2;
    
    // 3. Shorter lifetime
    settings.ragdollLifetime = Application.isMobilePlatform ? 8f : 15f;
    
    // 4. Enable LOD
    manager.enableLOD = true;
    settings.lodDistance = Application.isMobilePlatform ? 20f : 40f;
    
    // 5. Reduce physics quality for distance
    settings.useContinuousCollision = !Application.isMobilePlatform;
    
    Debug.Log("Performance optimized for current platform");
}
```

**📱 Mobile-Specific Optimization:**
```csharp
private void OptimizeForMobile()
{
    if (!Application.isMobilePlatform) return;
    
    var settings = RagdollManager.Instance.GlobalSettings;
    
    // Aggressive limits for mobile
    settings.maxActiveRagdolls = 2;
    settings.ragdollLifetime = 5f;
    settings.lodDistance = 15f;
    settings.useContinuousCollision = false;
    
    // Disable some features on low-end devices
    if (SystemInfo.systemMemorySize < 2048) // < 2GB RAM
    {
        settings.maxActiveRagdolls = 1;
        RagdollManager.Instance.enablePooling = false; // Save memory
    }
}
```

---

### ❌ Issue 4: Transition Không Smooth

#### Triệu chứng:
- Character jump/teleport khi enable/disable ragdoll
- Animation không blend tốt
- Visible pops trong transition
- Ragdoll position không match animation

#### Nguyên nhân & Giải pháp:

**🔍 Problem: Transition Duration Quá Ngắn**
```csharp
// Increase transition time
var settings = ragdollController.Settings;
if (settings.transitionDuration < 0.2f)
{
    settings.transitionDuration = 0.5f; // Smoother transition
}
```

**🔍 Problem: Position Mismatch**
```csharp
// Ensure pose matching before transition
public void SmoothEnableRagdoll()
{
    // 1. Backup current animation pose
    var pose = ragdollController.BackupCurrentPose();
    
    // 2. Enable ragdoll
    ragdollController.EnableRagdoll();
    
    // 3. Match ragdoll to animation pose
    RagdollUtilities.RestorePose(pose);
    
    // 4. Apply physics after a frame
    StartCoroutine(ApplyPhysicsDelayed());
}

private IEnumerator ApplyPhysicsDelayed()
{
    yield return new WaitForFixedUpdate();
    // Now safe to apply forces
}
```

**🔍 Problem: Animation Blend Issues**
```csharp
// Smooth disable with blending
public void SmoothDisableRagdoll()
{
    if (!ragdollController.IsRagdollActive) return;
    
    // 1. Backup ragdoll pose
    var ragdollPose = ragdollController.BackupCurrentPose();
    
    // 2. Disable ragdoll
    ragdollController.DisableRagdoll();
    
    // 3. Blend from ragdoll pose to animation
    StartCoroutine(BlendToAnimation(ragdollPose));
}

private IEnumerator BlendToAnimation(RagdollPose startPose)
{
    float blendTime = 1f;
    float elapsed = 0f;
    
    while (elapsed < blendTime)
    {
        float t = elapsed / blendTime;
        
        // Custom blending logic here
        // Lerp between ragdoll pose and animation
        
        elapsed += Time.deltaTime;
        yield return null;
    }
}
```

---

### ❌ Issue 5: Collision Issues

#### Triệu chứng:
- Ragdoll đi xuyên qua terrain/walls
- Collision không detect đúng
- Character stuck trong geometry
- Weird collision behaviors

#### Nguyên nhân & Giải pháp:

**🔍 Problem: Layer Conflicts**
```csharp
// Check collision layers
private void FixCollisionLayers()
{
    var ragdollLayer = 8; // Dedicated ragdoll layer
    var colliders = GetComponentsInChildren<Collider>();
    
    foreach (var collider in colliders)
    {
        collider.gameObject.layer = ragdollLayer;
    }
    
    // Setup collision matrix in Physics settings
    // Ragdoll layer should collide with:
    // - Default (ground)
    // - Environment 
    // - Props
    // Should NOT collide with:
    // - Player layer
    // - UI layer
    // - Triggers
}
```

**🔍 Problem: Collider Sizes Sai**
```csharp
// Fix collider sizes
[ContextMenu("Fix Collider Sizes")]
public void FixColliderSizes()
{
    var colliders = GetComponentsInChildren<Collider>();
    
    foreach (var collider in colliders)
    {
        if (collider is CapsuleCollider capsule)
        {
            // Ensure reasonable sizes
            capsule.radius = Mathf.Clamp(capsule.radius, 0.05f, 0.3f);
            capsule.height = Mathf.Clamp(capsule.height, 0.1f, 0.8f);
        }
        else if (collider is BoxCollider box)
        {
            // Reasonable box sizes
            var size = box.size;
            size.x = Mathf.Clamp(size.x, 0.05f, 0.5f);
            size.y = Mathf.Clamp(size.y, 0.05f, 0.8f);
            size.z = Mathf.Clamp(size.z, 0.05f, 0.5f);
            box.size = size;
        }
    }
}
```

**🔍 Problem: Collision Detection Mode**
```csharp
// Use appropriate collision detection
private void SetCollisionDetection()
{
    var rigidbodies = GetComponentsInChildren<Rigidbody>();
    
    foreach (var rb in rigidbodies)
    {
        // Important bones need continuous
        if (rb.name.Contains("Pelvis") || rb.name.Contains("Head"))
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        else
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }
    }
}
```

---

### ❌ Issue 6: Memory Leaks

#### Triệu chứng:
- Memory usage tăng dần theo thời gian
- Game crash sau khi chơi lâu
- Too many objects trong Profiler
- Ragdolls không được cleanup

#### Nguyên nhân & Giải pháp:

**🔍 Problem: Event Subscriptions Không Unsubscribe**
```csharp
public class ProperEventHandling : MonoBehaviour
{
    private RagdollController ragdoll;
    
    private void Start()
    {
        ragdoll = GetComponent<RagdollController>();
        
        // Subscribe to events
        ragdoll.OnRagdollEnabled += HandleRagdollEnabled;
        ragdoll.OnRagdollDisabled += HandleRagdollDisabled;
    }
    
    private void OnDestroy()
    {
        // CRITICAL: Always unsubscribe
        if (ragdoll != null)
        {
            ragdoll.OnRagdollEnabled -= HandleRagdollEnabled;
            ragdoll.OnRagdollDisabled -= HandleRagdollDisabled;
        }
    }
    
    private void HandleRagdollEnabled() { /* Logic */ }
    private void HandleRagdollDisabled() { /* Logic */ }
}
```

**🔍 Problem: Ragdolls Không Auto Cleanup**
```csharp
// Enable automatic cleanup
private void EnsureAutoCleanup()
{
    var manager = RagdollManager.Instance;
    
    // Check cleanup settings
    if (manager.cleanupInterval <= 0)
    {
        manager.cleanupInterval = 5f; // Cleanup every 5 seconds
    }
    
    var settings = manager.GlobalSettings;
    if (settings.ragdollLifetime <= 0)
    {
        settings.ragdollLifetime = 15f; // Auto despawn after 15 seconds
    }
}
```

**🔍 Problem: Pool Không Được Manage Đúng**
```csharp
// Proper pool management
public class RagdollPoolManager : MonoBehaviour
{
    private void Update()
    {
        // Monitor pool health
        var manager = RagdollManager.Instance;
        
        if (manager.PooledRagdollCount > manager.poolSize * 2)
        {
            Debug.LogWarning("Pool size growing too large!");
            // Force cleanup excess pooled objects
            manager.CleanupExcessPooledObjects();
        }
    }
}
```

---

## 🛠️ DEBUGGING TOOLS

### Ragdoll Debug Inspector
```csharp
[System.Serializable]
public class RagdollDebugInfo
{
    public bool isActive;
    public bool isTransitioning;
    public int rigidbodyCount;
    public int colliderCount;
    public int jointCount;
    public float totalMass;
    public bool hasSettings;
    public bool hasAnimator;
}

public class RagdollDebugger : MonoBehaviour
{
    [SerializeField] private RagdollDebugInfo debugInfo;
    
    [ContextMenu("Update Debug Info")]
    public void UpdateDebugInfo()
    {
        var ragdoll = GetComponent<RagdollController>();
        var animator = GetComponent<Animator>();
        
        debugInfo.isActive = ragdoll?.IsRagdollActive ?? false;
        debugInfo.isTransitioning = ragdoll?.IsTransitioning ?? false;
        debugInfo.rigidbodyCount = GetComponentsInChildren<Rigidbody>().Length;
        debugInfo.colliderCount = GetComponentsInChildren<Collider>().Length;
        debugInfo.jointCount = GetComponentsInChildren<CharacterJoint>().Length;
        debugInfo.totalMass = ragdoll?.GetTotalMass() ?? 0f;
        debugInfo.hasSettings = ragdoll?.Settings != null;
        debugInfo.hasAnimator = animator != null && animator.isHuman;
    }
}
```

### Performance Monitor
```csharp
public class RagdollPerformanceMonitor : MonoBehaviour
{
    [SerializeField] private bool showGUI = true;
    [SerializeField] private float updateInterval = 1f;
    
    private int activeRagdolls;
    private int pooledRagdolls;
    private float memoryUsage;
    
    private void Start()
    {
        InvokeRepeating(nameof(UpdateStats), 0f, updateInterval);
    }
    
    private void UpdateStats()
    {
        if (RagdollManager.Instance != null)
        {
            activeRagdolls = RagdollManager.Instance.ActiveRagdollCount;
            pooledRagdolls = RagdollManager.Instance.PooledRagdollCount;
        }
        
        memoryUsage = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory(0) / 1024f / 1024f; // MB
    }
    
    private void OnGUI()
    {
        if (!showGUI) return;
        
        GUI.Box(new Rect(10, 10, 200, 100), "Ragdoll Performance");
        GUI.Label(new Rect(15, 30, 190, 20), $"Active: {activeRagdolls}");
        GUI.Label(new Rect(15, 50, 190, 20), $"Pooled: {pooledRagdolls}");
        GUI.Label(new Rect(15, 70, 190, 20), $"Memory: {memoryUsage:F1} MB");
    }
}
```

---

## 📋 DIAGNOSTIC CHECKLIST

### ✅ Basic Setup Verification
```
□ RagdollController component present
□ RagdollSettings asset assigned  
□ Character has Humanoid rig
□ Rigidbody components on bones
□ Collider components properly sized
□ Character Joint components configured
□ No compilation errors
```

### ✅ Physics Verification
```
□ Mass values reasonable (0.5-5.0)
□ Drag values appropriate (0.5-2.0)
□ Angular drag sufficient (5.0-20.0)
□ Joint limits not too restrictive
□ Collision detection mode appropriate
□ Layer collision matrix configured
```

### ✅ Performance Verification
```
□ Object pooling enabled
□ Max active ragdolls < 10
□ Ragdoll lifetime reasonable (10-20s)
□ LOD system enabled
□ Cleanup interval set (5s)
□ Mobile optimizations applied
```

### ✅ Integration Verification
```
□ Event subscriptions properly managed
□ Animator disabled during ragdoll
□ AI/Player controls adapted
□ UI system updated
□ Audio/VFX integrated
□ Memory leaks checked
```

---

## 🆘 EMERGENCY FIXES

### Quick Performance Fix
```csharp
[ContextMenu("Emergency Performance Fix")]
public void EmergencyPerformanceFix()
{
    // Nuclear option - despawn all ragdolls
    RagdollManager.Instance.DespawnAllRagdolls();
    
    // Aggressive limits
    var settings = RagdollManager.Instance.GlobalSettings;
    settings.maxActiveRagdolls = 3;
    settings.ragdollLifetime = 5f;
    
    // Force garbage collection
    System.GC.Collect();
    
    Debug.Log("Emergency performance fix applied!");
}
```

### Reset To Defaults
```csharp
[ContextMenu("Reset All Settings")]
public void ResetToDefaults()
{
    var settings = RagdollManager.Instance.GlobalSettings;
    
    // Safe default values
    settings.defaultMass = 1f;
    settings.defaultDrag = 0.5f;
    settings.defaultAngularDrag = 5f;
    settings.transitionDuration = 0.2f;
    settings.maxActiveRagdolls = 10;
    settings.ragdollLifetime = 15f;
    settings.lodDistance = 30f;
    settings.useContinuousCollision = true;
    
    Debug.Log("Settings reset to safe defaults");
}
```

---

## 📞 SUPPORT RESOURCES

### Debug Commands
```
Console Commands trong Development Build:
- /ragdoll status    - Show system status
- /ragdoll cleanup   - Force cleanup all
- /ragdoll reset     - Reset to defaults
- /ragdoll optimize  - Apply optimizations
```

### Log Analysis
```
Common Error Patterns:
- "NullReferenceException" in RagdollController → Check component setup
- "MissingReferenceException" → Check prefab integrity  
- "Physics.Rigidbody" errors → Check mass/drag values
- Memory warnings → Enable pooling, reduce limits
```

### Unity Console Filters
```
Filter Tags to Monitor:
- [Ragdoll] - System messages
- [Performance] - Performance warnings
- [Memory] - Memory usage alerts
- [Physics] - Physics-related issues
```

---

**🎯 Kết Luận**

Hầu hết các issues với Ragdoll System đều có thể giải quyết bằng:
1. **Proper setup** - Đảm bảo all components được setup đúng
2. **Reasonable values** - Sử dụng physics values hợp lý
3. **Performance limits** - Set appropriate limits cho target platform
4. **Memory management** - Proper cleanup và pooling

**📞 Nếu vẫn gặp issues**, check lại documentation hoặc run diagnostic tools để identify root cause.

---

**📝 Cập nhật lần cuối**: 6/5/2025  
**🔖 Version**: 1.0  
**🔧 Troubleshooting Guide**: Complete