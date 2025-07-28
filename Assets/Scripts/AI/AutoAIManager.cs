using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AutoAIManager : MonoBehaviour
{
    [Header("AI Settings")]
    public bool enableAutoAI = true;
    public float detectionRange = 15f;
    public float attackRange = 2.5f;
    public float attackCooldown = 1.5f;
    public float aiUpdateInterval = 0.5f;
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    private BattleGameManager battleManager;
    private List<GameObject> managedCharacters = new List<GameObject>();
    
    void Start()
    {
        battleManager = FindAnyObjectByType<BattleGameManager>();
        if (battleManager == null)
        {
            Debug.LogError("AutoAIManager: Không tìm thấy BattleGameManager!");
            enabled = false;
            return;
        }
        
        // Tự động setup AI cho tất cả character hiện có
        SetupExistingCharacters();
        
        // Kiểm tra và setup AI cho character mới mỗi giây
        InvokeRepeating(nameof(CheckForNewCharacters), 1f, 1f);
        
        Debug.Log("AutoAIManager initialized - AI sẽ tự động kích hoạt khi battle bắt đầu");
    }
    
    void SetupExistingCharacters()
    {
        RagdollCharacter[] allCharacters = FindObjectsByType<RagdollCharacter>(FindObjectsSortMode.None);
        foreach (RagdollCharacter character in allCharacters)
        {
            SetupCharacterAI(character.gameObject);
        }
    }
    
    void CheckForNewCharacters()
    {
        RagdollCharacter[] allCharacters = FindObjectsByType<RagdollCharacter>(FindObjectsSortMode.None);
        foreach (RagdollCharacter character in allCharacters)
        {
            if (!managedCharacters.Contains(character.gameObject))
            {
                SetupCharacterAI(character.gameObject);
            }
        }
    }
    
    void SetupCharacterAI(GameObject character)
    {
        if (character == null) return;
        
        // Kiểm tra xem đã có AI component chưa
        SimpleCharacterAI existingAI = character.GetComponent<SimpleCharacterAI>();
        if (existingAI != null)
        {
            // Cập nhật settings
            existingAI.detectionRange = detectionRange;
            existingAI.attackRange = attackRange;
            existingAI.attackCooldown = attackCooldown;
            
            if (!managedCharacters.Contains(character))
                managedCharacters.Add(character);
            return;
        }
        
        // Kiểm tra NavMeshAgent
        NavMeshAgent agent = character.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = character.AddComponent<NavMeshAgent>();
            
            // Cấu hình NavMeshAgent
            agent.speed = 3f;
            agent.acceleration = 8f;
            agent.angularSpeed = 120f;
            agent.stoppingDistance = attackRange * 0.8f;
            agent.radius = 0.5f;
            agent.height = 2f;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }
        
        // Thêm AI component
        SimpleCharacterAI ai = character.AddComponent<SimpleCharacterAI>();
        ai.detectionRange = detectionRange;
        ai.attackRange = attackRange;
        ai.attackCooldown = attackCooldown;
        
        // Disable AI ban đầu (sẽ enable khi battle bắt đầu)
        ai.enabled = false;
        
        managedCharacters.Add(character);
        
        if (showDebugInfo)
            Debug.Log($"Setup AI cho character: {character.name}");
    }
    
    public void EnableAllAI()
    {
        if (!enableAutoAI) return;
        
        int enabledCount = 0;
        
        // Enable AI cho tất cả character
        SimpleCharacterAI[] allAI = FindObjectsByType<SimpleCharacterAI>(FindObjectsSortMode.None);
        foreach (SimpleCharacterAI ai in allAI)
        {
            if (ai != null)
            {
                ai.enabled = true;
                enabledCount++;
            }
        }
        
        // Đảm bảo NavMeshAgent được enable
        NavMeshAgent[] allAgents = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);
        foreach (NavMeshAgent agent in allAgents)
        {
            if (agent != null)
            {
                agent.enabled = true;
            }
        }
        
        if (showDebugInfo)
            Debug.Log($"Enabled AI cho {enabledCount} characters - Battle bắt đầu!");
    }
    
    public void DisableAllAI()
    {
        int disabledCount = 0;
        
        // Disable AI cho tất cả character
        SimpleCharacterAI[] allAI = FindObjectsByType<SimpleCharacterAI>(FindObjectsSortMode.None);
        foreach (SimpleCharacterAI ai in allAI)
        {
            if (ai != null)
            {
                ai.enabled = false;
                disabledCount++;
            }
        }
        
        if (showDebugInfo)
            Debug.Log($"Disabled AI cho {disabledCount} characters");
    }
    
    void Update()
    {
        // Tự động enable AI khi battle bắt đầu
        if (battleManager != null && battleManager.gameStarted)
        {
            // Kiểm tra xem có AI nào chưa được enable không
            SimpleCharacterAI[] allAI = FindObjectsByType<SimpleCharacterAI>(FindObjectsSortMode.None);
            bool hasDisabledAI = false;
            
            foreach (SimpleCharacterAI ai in allAI)
            {
                if (ai != null && !ai.enabled)
                {
                    hasDisabledAI = true;
                    break;
                }
            }
            
            if (hasDisabledAI)
            {
                EnableAllAI();
            }
        }
        else if (battleManager != null && !battleManager.gameStarted)
        {
            // Disable AI khi không trong battle
            SimpleCharacterAI[] allAI = FindObjectsByType<SimpleCharacterAI>(FindObjectsSortMode.None);
            foreach (SimpleCharacterAI ai in allAI)
            {
                if (ai != null && ai.enabled)
                {
                    ai.enabled = false;
                }
            }
        }
    }
    
    // Method để force setup AI cho character cụ thể
    public void ForceSetupCharacterAI(GameObject character)
    {
        SetupCharacterAI(character);
    }
    
    // Method để kiểm tra AI status
    public void CheckAIStatus()
    {
        SimpleCharacterAI[] allAI = FindObjectsByType<SimpleCharacterAI>(FindObjectsSortMode.None);
        int enabledCount = 0;
        int totalCount = allAI.Length;
        
        foreach (SimpleCharacterAI ai in allAI)
        {
            if (ai != null && ai.enabled)
                enabledCount++;
        }
        
        Debug.Log($"AI Status: {enabledCount}/{totalCount} AI components enabled");
        
        // Kiểm tra NavMeshAgent
        NavMeshAgent[] allAgents = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);
        int enabledAgents = 0;
        foreach (NavMeshAgent agent in allAgents)
        {
            if (agent != null && agent.enabled)
                enabledAgents++;
        }
        
        Debug.Log($"NavMesh Status: {enabledAgents}/{allAgents.Length} NavMeshAgents enabled");
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugInfo) return;
        
        // Vẽ detection range cho tất cả character có AI
        SimpleCharacterAI[] allAI = FindObjectsByType<SimpleCharacterAI>(FindObjectsSortMode.None);
        foreach (SimpleCharacterAI ai in allAI)
        {
            if (ai != null && ai.enabled)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(ai.transform.position, detectionRange);
                
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(ai.transform.position, attackRange);
            }
        }
    }
}