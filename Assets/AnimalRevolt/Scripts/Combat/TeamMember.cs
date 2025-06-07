using UnityEngine;

/// <summary>
/// Component định danh team cho character
/// Xác định ai là đồng minh và ai là kẻ địch
/// Enhanced version with public setters to avoid reflection usage
/// </summary>
public class TeamMember : MonoBehaviour
{
    [Header("Team Configuration")]
    [SerializeField] private TeamType teamType = TeamType.Player;
    [SerializeField] private string teamName = "DefaultTeam";
    [SerializeField] private Color teamColor = Color.blue;
    [SerializeField] private bool debugMode = true;
    
    [Header("Combat Settings")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool isAlive = true;
    
    // Events
    public System.Action<TeamMember> OnDeath;
    public System.Action<float> OnHealthChanged;
    public System.Action<TeamMember> OnTeamChanged;
    
    // Properties (Read-only)
    public TeamType TeamType => teamType;
    public string TeamName => teamName;
    public Color TeamColor => teamColor;
    public float Health => health;
    public float MaxHealth => maxHealth;
    public bool IsAlive => isAlive;
    public float HealthPercent => maxHealth > 0 ? health / maxHealth : 0f;
    public int TeamID => (int)teamType; // Thêm TeamID để dễ so sánh
    
    // Public setters để tránh phải dùng reflection
    public void SetTeamType(TeamType newTeamType)
    {
        teamType = newTeamType;
        UpdateTeamColor();
    }
    
    public void SetTeamName(string newTeamName)
    {
        teamName = newTeamName;
    }
    
    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;
    }
    
    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = Mathf.Max(1f, newMaxHealth);
        health = Mathf.Min(health, maxHealth); // Đảm bảo health không vượt quá max
    }
    
    public void SetDebugMode(bool debug)
    {
        debugMode = debug;
    }
    
    /// <summary>
    /// Initialize team với tất cả parameters cần thiết
    /// </summary>
    public void InitializeTeam(TeamType type, string name = "", float healthAmount = 100f)
    {
        teamType = type;
        teamName = string.IsNullOrEmpty(name) ? GetDefaultTeamName(type) : name;
        maxHealth = Mathf.Max(1f, healthAmount);
        health = maxHealth;
        isAlive = true;
        
        UpdateTeamColor();
        
        if (debugMode)
            Debug.Log($"🎯 {gameObject.name} initialized - Team: {teamName} ({teamType}), Health: {health}");
    }
    
    /// <summary>
    /// Cập nhật màu team dựa trên TeamType
    /// </summary>
    private void UpdateTeamColor()
    {
        teamColor = teamType switch
        {
            TeamType.Player => Color.green,
            TeamType.Enemy => Color.red,
            TeamType.AI_Team1 => Color.blue,
            TeamType.AI_Team2 => Color.red,
            TeamType.AI_Team3 => Color.yellow,
            TeamType.Neutral => Color.gray,
            _ => Color.white
        };
    }
    
    /// <summary>
    /// Lấy tên team mặc định dựa trên TeamType
    /// </summary>
    private string GetDefaultTeamName(TeamType type)
    {
        return type switch
        {
            TeamType.Player => "Player Team",
            TeamType.Enemy => "Enemy Team",
            TeamType.AI_Team1 => "Blue Team",
            TeamType.AI_Team2 => "Red Team",
            TeamType.AI_Team3 => "Yellow Team",
            TeamType.Neutral => "Neutral",
            _ => "Unknown Team"
        };
    }
    
    private void Start()
    {
        // 🔥 ENHANCED DEBUG: Log initial state
        Debug.Log($"[TeamMember.Start] Initializing {gameObject.name}");
        Debug.Log($"[TeamMember.Start] Initial TeamType: {teamType}, TeamName: '{teamName}'");
        
        // 🔥 PRIORITY 1 FIX: Auto-assign different teams based on GameObject name
        if (gameObject.name.Contains("Warrok"))
        {
            teamType = TeamType.AI_Team1;
            teamName = "Blue Team";
            Debug.Log($"🎯 {gameObject.name} auto-assigned to AI_Team1 (Blue Team)");
        }
        else if (gameObject.name.Contains("npc"))
        {
            teamType = TeamType.AI_Team2;
            teamName = "Red Team";
            Debug.Log($"🎯 {gameObject.name} auto-assigned to AI_Team2 (Red Team)");
        }
        else
        {
            Debug.Log($"⚠️ {gameObject.name} NO AUTO-ASSIGNMENT (doesn't contain 'Warrok' or 'npc')");
        }
        
        // Update team color after assignment
        UpdateTeamColor();
        
        // Validate health
        health = Mathf.Clamp(health, 0f, maxHealth);
        
        // 🔥 ENHANCED DEBUG: Final state
        Debug.Log($"[TeamMember.Start] FINAL STATE for {gameObject.name}:");
        Debug.Log($"  - TeamType: {teamType}");
        Debug.Log($"  - TeamName: '{teamName}'");
        Debug.Log($"  - TeamColor: {teamColor}");
        Debug.Log($"  - Health: {health}/{maxHealth}");
        Debug.Log($"  - IsAlive: {isAlive}");
    }
    
    /// <summary>
    /// Kiểm tra có cùng team không
    /// </summary>
    public bool IsSameTeam(TeamMember other)
    {
        if (other == null) return false;
        return teamType == other.teamType && teamName == other.teamName;
    }
    
    /// <summary>
    /// Kiểm tra có phải kẻ địch không
    /// </summary>
    public bool IsEnemy(TeamMember other)
    {
        if (other == null)
        {
            Debug.Log($"[TeamMember.IsEnemy] {gameObject.name}: other is NULL, returning false");
            return false;
        }
        
        bool sameTeam = IsSameTeam(other);
        bool result = !sameTeam && other.IsAlive;
        
        // 🔥 ENHANCED DEBUG: Always log detailed team comparison
        Debug.Log($"[TeamMember.IsEnemy] {gameObject.name} vs {other.gameObject.name}:");
        Debug.Log($"  - My Team: Type={teamType}, Name='{teamName}'");
        Debug.Log($"  - Their Team: Type={other.teamType}, Name='{other.teamName}'");
        Debug.Log($"  - SameTeam={sameTeam}, TheirAlive={other.IsAlive}, FinalResult={result}");
        
        return result;
    }
    
    /// <summary>
    /// Thay đổi team
    /// </summary>
    public void ChangeTeam(TeamType newTeam, string newTeamName = "")
    {
        teamType = newTeam;
        if (!string.IsNullOrEmpty(newTeamName))
            teamName = newTeamName;
            
        OnTeamChanged?.Invoke(this);
        
        if (debugMode)
            Debug.Log($"{gameObject.name} changed to team: {teamName} ({teamType})");
    }
    
    /// <summary>
    /// Nhận sát thương
    /// </summary>
    public void TakeDamage(float damage, TeamMember attacker = null)
    {
        if (!isAlive) return;
        
        health -= damage;
        health = Mathf.Max(0f, health);
        
        OnHealthChanged?.Invoke(health);
        
        if (debugMode && attacker != null)
            Debug.Log($"{gameObject.name} took {damage} damage from {attacker.gameObject.name}. Health: {health}/{maxHealth}");
        
        // Check death
        if (health <= 0f)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Hồi máu
    /// </summary>
    public void Heal(float amount)
    {
        if (!isAlive) return;
        
        health += amount;
        health = Mathf.Min(maxHealth, health);
        
        OnHealthChanged?.Invoke(health);
        
        if (debugMode)
            Debug.Log($"{gameObject.name} healed {amount}. Health: {health}/{maxHealth}");
    }
    
    /// <summary>
    /// Chết
    /// </summary>
    private void Die()
    {
        isAlive = false;
        OnDeath?.Invoke(this);
        
        if (debugMode)
            Debug.Log($"{gameObject.name} died!");
        
        // Kích hoạt ragdoll
        RagdollPhysicsController ragdoll = GetComponent<RagdollPhysicsController>();
        if (ragdoll != null)
        {
            ragdoll.EnableRagdoll();
        }
    }
    
    /// <summary>
    /// Hồi sinh
    /// </summary>
    public void Revive(float healthAmount = 0f)
    {
        isAlive = true;
        health = healthAmount > 0f ? healthAmount : maxHealth;
        
        OnHealthChanged?.Invoke(health);
        
        // Tắt ragdoll
        RagdollPhysicsController ragdoll = GetComponent<RagdollPhysicsController>();
        if (ragdoll != null)
        {
            ragdoll.DisableRagdoll();
        }
        
        if (debugMode)
            Debug.Log($"{gameObject.name} revived with {health} health!");
    }
    
    /// <summary>
    /// Tạo visual team indicator (glow effect, outline) dựa trên teamColor
    /// ⚠️ DISABLED - Visual indicators đã được vô hiệu hóa
    /// </summary>
    public void CreateTeamVisualIndicator()
    {
        // Visual indicators đã được vô hiệu hóa theo yêu cầu
        // Giữ nguyên hệ thống quản lý team nhưng không hiển thị visual indicator
        if (debugMode)
            Debug.Log($"🚫 Visual team indicator disabled for {gameObject.name} - Team: {teamName} ({teamType})");
        
        return;
    }
    
    /// <summary>
    /// Tạo glow effect cho team indicator
    /// </summary>
    private void CreateGlowEffect(Transform indicator)
    {
        // Safely check for existing glow sphere
        GameObject glowSphere = null;
        
        // Try to find existing TeamGlow child by name instead of index
        Transform existingGlow = indicator.Find("TeamGlow");
        if (existingGlow != null)
        {
            glowSphere = existingGlow.gameObject;
        }
        
        // Create new glow sphere if not found
        if (glowSphere == null)
        {
            glowSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            glowSphere.name = "TeamGlow";
            glowSphere.transform.SetParent(indicator);
            glowSphere.transform.localPosition = Vector3.zero;
            glowSphere.transform.localScale = Vector3.one * 0.3f;
            
            // Remove collider để không ảnh hưởng physics
            Collider glowCollider = glowSphere.GetComponent<Collider>();
            if (glowCollider != null)
            {
                if (Application.isPlaying)
                    Destroy(glowCollider);
                else
                    DestroyImmediate(glowCollider);
            }
        }
        
        // Set material color
        Renderer glowRenderer = glowSphere.GetComponent<Renderer>();
        if (glowRenderer != null)
        {
            Material glowMaterial = new Material(Shader.Find("Standard"));
            glowMaterial.color = teamColor;
            glowMaterial.SetFloat("_Metallic", 0f);
            glowMaterial.SetFloat("_Glossiness", 0.8f);
            glowMaterial.EnableKeyword("_EMISSION");
            glowMaterial.SetColor("_EmissionColor", teamColor * 0.5f);
            glowRenderer.material = glowMaterial;
            
            if (debugMode)
                Debug.Log($"✨ Updated glow effect for {gameObject.name} - Color: {teamColor}");
        }
    }
    
    /// <summary>
    /// Xóa team visual indicator - Enhanced version với better error handling
    /// </summary>
    public void RemoveTeamVisualIndicator()
    {
        try
        {
            Transform indicator = transform.Find("TeamIndicator");
            if (indicator != null)
            {
                if (debugMode)
                    Debug.Log($"🗑️ Removing team visual indicator from {gameObject.name}");
                
                if (Application.isPlaying)
                    Destroy(indicator.gameObject);
                else
                    DestroyImmediate(indicator.gameObject);
            }
            else if (debugMode)
            {
                Debug.Log($"✅ No team visual indicator found on {gameObject.name} - already clean");
            }
        }
        catch (System.Exception e)
        {
            if (debugMode)
                Debug.LogWarning($"⚠️ Error removing team indicator from {gameObject.name}: {e.Message}");
        }
    }
    
    /// <summary>
    /// Vẽ team indicator trong Scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!debugMode) return;
        
        // Vẽ sphere màu team
        Gizmos.color = teamColor;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.5f);
        
        // Vẽ team name
        #if UNITY_EDITOR
        UnityEditor.Handles.color = teamColor;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2.8f, $"{teamName}\n({teamType})");
        #endif
        
        // Vẽ health bar
        if (Application.isPlaying && isAlive)
        {
            Vector3 barPos = transform.position + Vector3.up * 2.5f;
            Vector3 barSize = new Vector3(1f, 0.1f, 0.1f);
            
            // Background
            Gizmos.color = Color.red;
            Gizmos.DrawCube(barPos, barSize);
            
            // Health
            Gizmos.color = Color.green;
            Vector3 healthSize = barSize;
            healthSize.x *= HealthPercent;
            Gizmos.DrawCube(barPos, healthSize);
        }
    }
}

/// <summary>
/// Enum định nghĩa các loại team
/// </summary>
public enum TeamType
{
    Player = 0,
    Enemy = 1,
    Neutral = 2,
    AI_Team1 = 3,
    AI_Team2 = 4,
    AI_Team3 = 5
}
