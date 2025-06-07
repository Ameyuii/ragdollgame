# 🔗 RAGDOLL SYSTEM - INTEGRATION GUIDE

## 🎯 Mục Tiêu
Hướng dẫn chi tiết cách tích hợp Ragdoll System vào game project hiện có, bao gồm best practices và optimization tips.

---

## 📋 YÊU CẦU TÍCH HỢP

### Unity Project Requirements
- ✅ **Unity Version**: 2022.3 LTS+ hoặc 2023.x+
- ✅ **Render Pipeline**: Universal RP (khuyến nghị) hoặc Built-in
- ✅ **Physics**: 3D Physics enabled
- ✅ **Input System**: New Input System (khuyến nghị)

### Character Requirements
- ✅ **Rig Type**: Humanoid (bắt buộc cho auto-setup)
- ✅ **Animator**: Có Animator component
- ✅ **Hierarchy**: Proper bone hierarchy
- ✅ **Scale**: Uniform scale (khuyến nghị)

---

## 🏗️ KIẾN TRÚC TÍCH HỢP

### 1. Core System Integration

#### A. Scene Architecture
```
GameManager
├── RagdollManager (Singleton)
├── CharacterManager
│   ├── Player Characters
│   │   ├── PlayerController
│   │   ├── Animator
│   │   └── RagdollController ← Tích hợp
│   └── NPC Characters
│       ├── AIController
│       ├── Animator
│       └── RagdollController ← Tích hợp
└── Systems
    ├── CombatSystem
    ├── PhysicsManager
    └── EffectsManager
```

#### B. Script Dependencies
```csharp
// Thứ tự khởi tạo quan trọng
1. RagdollSettings (ScriptableObject)
2. RagdollManager (Scene Singleton)
3. RagdollController (Per Character)
4. Game Systems Integration
```

---

## 🔧 INTEGRATION WORKFLOW

### Step 1: Project Setup (10 phút)

#### Import Ragdoll System
```
1. Copy Scripts folder vào Assets/Scripts/RagdollSystem/
2. Tạo RagdollSettings asset
3. Setup RagdollManager trong main scene
4. Verify no compilation errors
```

#### Project Structure
```
Assets/
├── Scripts/
│   ├── RagdollSystem/           ← Ragdoll scripts
│   │   ├── RagdollController.cs
│   │   ├── RagdollManager.cs
│   │   ├── RagdollSettings.cs
│   │   ├── RagdollUtilities.cs
│   │   └── Editor/
│   │       └── RagdollBuilder.cs
│   ├── Characters/              ← Game characters
│   ├── Combat/                  ← Combat system
│   └── UI/                      ← UI systems
├── Settings/
│   └── RagdollSettings/         ← Settings assets
└── Prefabs/
    └── Characters/              ← Character prefabs
```

### Step 2: Character Integration (5 phút per character)

#### Existing Character Workflow
```csharp
// Trước integration
public class ExistingCharacter : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private CharacterController controller;
    
    public void Die()
    {
        // Old death logic
        animator.SetTrigger("Death");
        agent.enabled = false;
    }
}
```

#### Sau integration
```csharp
// Sau integration  
public class ExistingCharacter : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private CharacterController controller;
    [SerializeField] private RagdollController ragdoll; // ← Thêm
    
    private void Start()
    {
        // Subscribe ragdoll events
        if (ragdoll != null)
        {
            ragdoll.OnRagdollEnabled += OnRagdollActivated;
            ragdoll.OnRagdollDisabled += OnRagdollDeactivated;
        }
    }
    
    public void Die(Vector3 damageDirection, float force)
    {
        // Disable game components
        agent.enabled = false;
        controller.enabled = false;
        animator.enabled = false;
        
        // Enable ragdoll với death force
        ragdoll.EnableRagdoll();
        ragdoll.ApplyForce(damageDirection * force, transform.position);
    }
    
    private void OnRagdollActivated()
    {
        // Handle ragdoll activation
        // Disable AI, UI, etc.
    }
    
    private void OnRagdollDeactivated()
    {
        // Handle ragdoll deactivation
        // Re-enable components if needed
    }
}
```

### Step 3: Combat System Integration

#### Health/Damage System
```csharp
public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private RagdollController ragdollController;
    
    private float currentHealth;
    
    public void TakeDamage(float damage, Vector3 hitDirection, Vector3 hitPoint)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die(hitDirection, hitPoint);
        }
        else
        {
            // Reaction to hit
            ApplyHitReaction(hitDirection, hitPoint, damage);
        }
    }
    
    private void Die(Vector3 hitDirection, Vector3 hitPoint)
    {
        // Calculate death force based on damage
        float deathForce = Mathf.Clamp(damage * 10f, 200f, 1000f);
        
        // Disable character components
        DisableCharacterComponents();
        
        // Enable ragdoll với death force
        ragdollController.EnableRagdoll();
        ragdollController.ApplyForce(hitDirection * deathForce, hitPoint);
        
        // Trigger death events
        OnCharacterDeath?.Invoke();
    }
    
    private void ApplyHitReaction(Vector3 hitDirection, Vector3 hitPoint, float damage)
    {
        // Temporary ragdoll for hit reaction
        if (damage > 30f) // Heavy hit
        {
            ragdollController.EnableRagdoll();
            ragdollController.ApplyForce(hitDirection * damage * 5f, hitPoint);
            
            // Return to animation sau 1 giây
            StartCoroutine(ReturnToAnimation(1f));
        }
    }
    
    private IEnumerator ReturnToAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        ragdollController.DisableRagdoll();
    }
}
```

#### Weapon/Combat Integration
```csharp
public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private float weaponDamage = 50f;
    [SerializeField] private float weaponForce = 500f;
    
    public void OnHitTarget(Collider target, Vector3 hitPoint, Vector3 hitDirection)
    {
        // Check if target has health system
        var healthSystem = target.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(weaponDamage, hitDirection, hitPoint);
        }
        
        // Direct ragdoll impact (optional)
        var ragdoll = target.GetComponent<RagdollController>();
        if (ragdoll != null && ragdoll.IsRagdollActive)
        {
            ragdoll.ApplyForce(hitDirection * weaponForce, hitPoint);
        }
    }
}
```

### Step 4: AI System Integration

#### AI Controller Integration
```csharp
public class AIController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private RagdollController ragdoll;
    
    private bool isRagdollActive = false;
    
    private void Start()
    {
        // Subscribe to ragdoll events
        ragdoll.OnRagdollStateChanged += OnRagdollStateChanged;
    }
    
    private void Update()
    {
        // Only update AI nếu không ragdoll
        if (!isRagdollActive)
        {
            UpdateAI();
        }
    }
    
    private void OnRagdollStateChanged(bool ragdollActive)
    {
        isRagdollActive = ragdollActive;
        
        if (ragdollActive)
        {
            // Disable AI components
            agent.enabled = false;
            animator.enabled = false;
            
            // Stop all AI behaviors
            StopAllCoroutines();
        }
        else
        {
            // Re-enable AI nếu character còn sống
            if (GetComponent<HealthSystem>().IsAlive)
            {
                agent.enabled = true;
                animator.enabled = true;
            }
        }
    }
    
    private void UpdateAI()
    {
        // AI logic chỉ chạy khi không ragdoll
        // Pathfinding, behavior trees, etc.
    }
}
```

---

## 🎮 PLAYER CONTROL INTEGRATION

### Player Controller Integration
```csharp
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private RagdollController ragdoll;
    
    private bool canMove = true;
    
    private void Start()
    {
        ragdoll.OnRagdollStateChanged += OnRagdollStateChanged;
    }
    
    private void Update()
    {
        if (canMove)
        {
            HandleMovement();
            HandleInput();
        }
    }
    
    private void OnRagdollStateChanged(bool ragdollActive)
    {
        canMove = !ragdollActive;
        
        if (ragdollActive)
        {
            // Disable player controls
            characterController.enabled = false;
        }
        else
        {
            // Re-enable controls
            characterController.enabled = true;
        }
    }
    
    private void HandleInput()
    {
        // Test ragdoll với debug key
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (ragdoll.IsRagdollActive)
                ragdoll.DisableRagdoll();
            else
                ragdoll.EnableRagdoll();
        }
    }
}
```

---

## 🎨 UI SYSTEM INTEGRATION

### Health Bar Integration
```csharp
public class CharacterUI : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private RagdollController ragdoll;
    
    private void Start()
    {
        ragdoll.OnRagdollStateChanged += OnRagdollStateChanged;
    }
    
    private void OnRagdollStateChanged(bool ragdollActive)
    {
        // Ẩn UI khi ragdoll active (character đã chết)
        uiPanel.SetActive(!ragdollActive);
    }
}
```

### Debug UI
```csharp
public class RagdollDebugUI : MonoBehaviour
{
    [SerializeField] private Text statusText;
    [SerializeField] private Button toggleButton;
    [SerializeField] private Slider forceSlider;
    
    private RagdollController targetRagdoll;
    
    private void Start()
    {
        targetRagdoll = FindObjectOfType<RagdollController>();
        
        toggleButton.onClick.AddListener(() => {
            if (targetRagdoll.IsRagdollActive)
                targetRagdoll.DisableRagdoll();
            else
                targetRagdoll.EnableRagdoll();
        });
    }
    
    private void Update()
    {
        if (targetRagdoll != null)
        {
            statusText.text = $"Ragdoll: {(targetRagdoll.IsRagdollActive ? "Active" : "Inactive")}";
            toggleButton.interactable = !targetRagdoll.IsTransitioning;
        }
    }
}
```

---

## 📊 PERFORMANCE OPTIMIZATION

### 1. LOD System Integration
```csharp
public class CharacterLOD : MonoBehaviour
{
    [SerializeField] private RagdollController ragdoll;
    [SerializeField] private float lodDistance = 50f;
    
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
        InvokeRepeating(nameof(UpdateLOD), 1f, 1f); // Check mỗi giây
    }
    
    private void UpdateLOD()
    {
        if (mainCamera == null || ragdoll == null) return;
        
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
        
        // Optimize ragdoll dựa trên distance
        RagdollUtilities.OptimizeRagdollForDistance(ragdoll, distance, lodDistance);
    }
}
```

### 2. Memory Management
```csharp
public class RagdollMemoryManager : MonoBehaviour
{
    [SerializeField] private int maxRagdolls = 10;
    [SerializeField] private float cleanupInterval = 5f;
    
    private void Start()
    {
        InvokeRepeating(nameof(CleanupRagdolls), cleanupInterval, cleanupInterval);
    }
    
    private void CleanupRagdolls()
    {
        if (RagdollManager.Instance.ActiveRagdollCount > maxRagdolls)
        {
            // Force cleanup oldest ragdolls
            var oldestRagdolls = RagdollManager.Instance.GetOldestRagdolls(5);
            foreach (var ragdoll in oldestRagdolls)
            {
                RagdollManager.Instance.DespawnRagdoll(ragdoll);
            }
        }
    }
}
```

### 3. Mobile Optimization
```csharp
public class MobileRagdollOptimizer : MonoBehaviour
{
    private void Start()
    {
        if (Application.isMobilePlatform)
        {
            OptimizeForMobile();
        }
    }
    
    private void OptimizeForMobile()
    {
        var settings = RagdollManager.Instance.GlobalSettings;
        
        // Giảm quality cho mobile
        settings.maxActiveRagdolls = 5;
        settings.ragdollLifetime = 8f;
        settings.lodDistance = 20f;
        settings.useContinuousCollision = false;
        
        // Disable pooling trên mobile cũ
        if (SystemInfo.systemMemorySize < 2048) // < 2GB RAM
        {
            var manager = RagdollManager.Instance;
            manager.enablePooling = false;
        }
    }
}
```

---

## 🎵 AUDIO INTEGRATION

### Sound Effects Integration
```csharp
public class RagdollAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] hitSounds;
    [SerializeField] private AudioClip[] deathSounds;
    [SerializeField] private RagdollController ragdoll;
    
    private void Start()
    {
        ragdoll.OnRagdollEnabled += OnRagdollActivated;
    }
    
    private void OnRagdollActivated()
    {
        // Play death sound
        if (deathSounds.Length > 0)
        {
            var randomSound = deathSounds[Random.Range(0, deathSounds.Length)];
            audioSource.PlayOneShot(randomSound);
        }
    }
    
    public void PlayHitSound()
    {
        if (hitSounds.Length > 0)
        {
            var randomSound = hitSounds[Random.Range(0, hitSounds.Length)];
            audioSource.PlayOneShot(randomSound);
        }
    }
}
```

---

## 🎨 VFX INTEGRATION

### Particle Effects Integration
```csharp
public class RagdollVFXManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem bloodEffect;
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private RagdollController ragdoll;
    
    private void Start()
    {
        ragdoll.OnRagdollEnabled += OnRagdollActivated;
    }
    
    private void OnRagdollActivated()
    {
        // Play death effects
        if (deathEffect != null)
        {
            deathEffect.transform.position = transform.position;
            deathEffect.Play();
        }
    }
    
    public void PlayHitEffect(Vector3 hitPoint)
    {
        if (bloodEffect != null)
        {
            bloodEffect.transform.position = hitPoint;
            bloodEffect.Play();
        }
    }
}
```

---

## 🧪 TESTING INTEGRATION

### Integration Test Script
```csharp
public class RagdollIntegrationTest : MonoBehaviour
{
    [SerializeField] private RagdollController testRagdoll;
    
    [ContextMenu("Test Integration")]
    public void TestIntegration()
    {
        StartCoroutine(RunIntegrationTests());
    }
    
    private IEnumerator RunIntegrationTests()
    {
        Debug.Log("Starting Ragdoll Integration Tests...");
        
        // Test 1: Enable/Disable
        testRagdoll.EnableRagdoll();
        yield return new WaitForSeconds(1f);
        Assert.IsTrue(testRagdoll.IsRagdollActive, "Enable test failed");
        
        testRagdoll.DisableRagdoll();
        yield return new WaitForSeconds(1f);
        Assert.IsFalse(testRagdoll.IsRagdollActive, "Disable test failed");
        
        // Test 2: Force Application
        testRagdoll.EnableRagdoll();
        testRagdoll.ApplyForce(Vector3.forward * 500f, transform.position);
        yield return new WaitForSeconds(2f);
        
        // Test 3: Manager Integration
        var spawnedRagdoll = RagdollManager.Instance.SpawnRandomRagdoll(
            transform.position + Vector3.right * 3f,
            Quaternion.identity
        );
        Assert.IsNotNull(spawnedRagdoll, "Spawn test failed");
        
        yield return new WaitForSeconds(2f);
        RagdollManager.Instance.DespawnRagdoll(spawnedRagdoll);
        
        Debug.Log("Integration Tests Completed Successfully!");
    }
}
```

---

## 📋 INTEGRATION CHECKLIST

### Pre-Integration
- [ ] Unity version compatibility verified
- [ ] Project backup created
- [ ] Character requirements met
- [ ] Dependencies identified

### Core Integration
- [ ] Scripts imported và compiled
- [ ] RagdollSettings asset created
- [ ] RagdollManager setup trong scene
- [ ] Character prefabs updated

### System Integration
- [ ] Health/Combat system integrated
- [ ] AI system updated
- [ ] Player controls adapted
- [ ] UI system connected

### Testing
- [ ] Basic functionality tested
- [ ] Performance benchmarked
- [ ] Mobile compatibility checked
- [ ] Build test completed

### Optimization
- [ ] LOD system configured
- [ ] Memory management implemented
- [ ] Audio/VFX integrated
- [ ] Platform-specific optimizations applied

---

## 🚨 COMMON INTEGRATION ISSUES

### Issue 1: Animation/Ragdoll Conflicts
**Problem**: Animator và Ragdoll fight for control  
**Solution**:
```csharp
// Disable Animator trước khi enable ragdoll
animator.enabled = false;
ragdoll.EnableRagdoll();
```

### Issue 2: Physics Layer Conflicts
**Problem**: Ragdoll collides với player hoặc environment không mong muốn  
**Solution**:
```csharp
// Setup riêng physics layers cho ragdolls
// Physics Settings → Layer Collision Matrix
```

### Issue 3: Performance Drops
**Problem**: Quá nhiều ragdolls active cùng lúc  
**Solution**:
```csharp
// Implement strict limits
settings.maxActiveRagdolls = 5; // Cho mobile
settings.ragdollLifetime = 10f; // Auto cleanup
```

### Issue 4: Memory Leaks
**Problem**: Ragdolls không được cleanup  
**Solution**:
```csharp
// Enable pooling và auto cleanup
manager.enablePooling = true;
// Set reasonable lifetime
settings.ragdollLifetime = 15f;
```

---

## 📚 BEST PRACTICES

### 1. Performance
- ✅ **Luôn enable object pooling** cho production
- ✅ **Set reasonable limits** cho active ragdolls
- ✅ **Use LOD system** cho distance-based optimization
- ✅ **Profile regularly** để detect performance issues

### 2. Design
- ✅ **Consistent integration pattern** across all characters
- ✅ **Event-driven architecture** cho loose coupling
- ✅ **Graceful degradation** cho low-end devices
- ✅ **Clear separation** giữa ragdoll và game logic

### 3. Maintenance
- ✅ **Document integration points** trong code
- ✅ **Version control** ragdoll settings assets
- ✅ **Regular testing** sau Unity updates
- ✅ **Monitor performance** trong production builds

---

**🎉 Integration Complete!**

Ragdoll System giờ đã được tích hợp đầy đủ vào game project của bạn với performance tối ưu và maintainability cao.

---

**📝 Cập nhật lần cuối**: 6/5/2025  
**🔖 Version**: 1.0  
**🔗 Integration Guide**: Complete