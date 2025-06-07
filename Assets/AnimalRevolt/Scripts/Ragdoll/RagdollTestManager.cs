using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Manager Ä‘á»ƒ test ragdoll vá»›i cÃ¡c character tá»« prefab
/// Spawn character vÃ  test ragdoll functionality
/// </summary>
public class RagdollTestManager : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private List<GameObject> testCharacterPrefabs = new List<GameObject>();
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool debugMode = true;
    
    [Header("Spawn Settings")]
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private int maxCharacters = 5;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference spawnCharacterAction;
    [SerializeField] private InputActionReference clearAllAction;
    [SerializeField] private InputActionReference toggleAllRagdollAction;
    
    // Private variables
    private List<GameObject> spawnedCharacters = new List<GameObject>();
    private Camera mainCamera;
    private int currentPrefabIndex = 0;
    
    // Public properties for Editor access
    public InputActionReference SpawnCharacterAction
    {
        get => spawnCharacterAction;
        set => spawnCharacterAction = value;
    }
    
    public InputActionReference ClearAllAction
    {
        get => clearAllAction;
        set => clearAllAction = value;
    }
    
    public InputActionReference ToggleAllRagdollAction
    {
        get => toggleAllRagdollAction;
        set => toggleAllRagdollAction = value;
    }
    
    private void OnEnable()
    {
        // Enable input actions
        if (spawnCharacterAction != null)
            spawnCharacterAction.action.Enable();
        if (clearAllAction != null)
            clearAllAction.action.Enable();
        if (toggleAllRagdollAction != null)
            toggleAllRagdollAction.action.Enable();
    }
    
    private void OnDisable()
    {
        // Disable input actions
        if (spawnCharacterAction != null)
            spawnCharacterAction.action.Disable();
        if (clearAllAction != null)
            clearAllAction.action.Disable();
        if (toggleAllRagdollAction != null)
            toggleAllRagdollAction.action.Disable();
    }
    
    private void Start()
    {
        // TÃ¬m main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();
            
        // Setup spawn point náº¿u chÆ°a cÃ³
        if (spawnPoint == null)
        {
            GameObject spawnObj = new GameObject("SpawnPoint");
            spawnObj.transform.position = Vector3.up * 2f;
            spawnPoint = spawnObj.transform;
        }
        
        // Load prefabs from Assets/Prefabs if empty
        if (testCharacterPrefabs.Count == 0)
        {
            LoadDefaultPrefabs();
        }
        
        if (debugMode)
        {
            Debug.Log($"RagdollTestManager initialized vá»›i {testCharacterPrefabs.Count} prefabs");
            Debug.Log("Controls: T = Spawn Character, C = Clear All, R = Toggle All Ragdoll");
        }
    }
    
    private void Update()
    {
        HandleInput();
    }
    
    /// <summary>
    /// Xá»­ lÃ½ input
    /// </summary>
    private void HandleInput()
    {
        // Spawn character
        if (spawnCharacterAction != null && spawnCharacterAction.action.WasPressedThisFrame())
        {
            SpawnCharacter();
        }
        
        // Clear all characters
        if (clearAllAction != null && clearAllAction.action.WasPressedThisFrame())
        {
            ClearAllCharacters();
        }
        
        // Toggle all ragdoll
        if (toggleAllRagdollAction != null && toggleAllRagdollAction.action.WasPressedThisFrame())
        {
            ToggleAllRagdoll();
        }
    }
    
    /// <summary>
    /// Spawn character tá»« prefab
    /// </summary>
    [ContextMenu("Spawn Character")]
    public void SpawnCharacter()
    {
        if (testCharacterPrefabs.Count == 0)
        {
            Debug.LogWarning("KhÃ´ng cÃ³ prefab nÃ o Ä‘á»ƒ spawn!");
            return;
        }
        
        if (spawnedCharacters.Count >= maxCharacters)
        {
            Debug.LogWarning($"ÄÃ£ Ä‘áº¡t giá»›i háº¡n {maxCharacters} characters!");
            return;
        }
        
        // Láº¥y prefab hiá»‡n táº¡i
        GameObject prefab = testCharacterPrefabs[currentPrefabIndex];
        
        // TÃ­nh position spawn
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        // Spawn character
        GameObject character = Instantiate(prefab, spawnPosition, Quaternion.identity);
        character.name = $"{prefab.name}_Test_{spawnedCharacters.Count + 1}";
        
        // Setup ragdoll components náº¿u chÆ°a cÃ³
        SetupRagdollForCharacter(character);
        
        // ThÃªm vÃ o danh sÃ¡ch
        spawnedCharacters.Add(character);
        
        // Chuyá»ƒn sang prefab tiáº¿p theo
        currentPrefabIndex = (currentPrefabIndex + 1) % testCharacterPrefabs.Count;
        
        if (debugMode)
        {
            Debug.Log($"Spawned {character.name} táº¡i {spawnPosition}");
        }
    }
    
    /// <summary>
    /// Setup ragdoll cho character náº¿u chÆ°a cÃ³
    /// </summary>
    private void SetupRagdollForCharacter(GameObject character)
    {        // Kiá»ƒm tra Ä‘Ã£ cÃ³ RagdollPhysicsController chÆ°a
        RagdollPhysicsController ragdollController = character.GetComponent<RagdollPhysicsController>();
        if (ragdollController == null)
        {
            ragdollController = character.AddComponent<RagdollPhysicsController>();
        }
        
        // Kiá»ƒm tra Ä‘Ã£ cÃ³ SimpleRagdollDemo chÆ°a
        SimpleRagdollDemo simpleDemo = character.GetComponent<SimpleRagdollDemo>();
        if (simpleDemo == null)
        {
            simpleDemo = character.AddComponent<SimpleRagdollDemo>();
        }
        
        // Kiá»ƒm tra cÃ³ Animator khÃ´ng
        Animator animator = character.GetComponent<Animator>();
        if (animator == null)
        {
            animator = character.AddComponent<Animator>();
        }
        
        if (debugMode)
        {
            Debug.Log($"Setup ragdoll components cho {character.name}");
        }
    }
    
    /// <summary>
    /// Láº¥y position spawn ngáº«u nhiÃªn
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 basePosition = spawnPoint.position + spawnOffset;
        
        // Random position trong radius
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 randomPosition = new Vector3(randomCircle.x, 0, randomCircle.y);
        
        return basePosition + randomPosition;
    }
    
    /// <summary>
    /// Clear all spawned characters
    /// </summary>
    [ContextMenu("Clear All Characters")]
    public void ClearAllCharacters()
    {
        foreach (var character in spawnedCharacters)
        {
            if (character != null)
            {
                DestroyImmediate(character);
            }
        }
        
        spawnedCharacters.Clear();
        
        if (debugMode)
        {
            Debug.Log("Cleared all test characters");
        }
    }
    
    /// <summary>
    /// Toggle ragdoll cho táº¥t cáº£ characters
    /// </summary>
    [ContextMenu("Toggle All Ragdoll")]
    public void ToggleAllRagdoll()
    {
        int ragdollCount = 0;
        foreach (var character in spawnedCharacters)
        {
            if (character != null)
            {
                RagdollPhysicsController ragdollController = character.GetComponent<RagdollPhysicsController>();
                if (ragdollController != null)
                {
                    ragdollController.ToggleRagdoll();
                    ragdollCount++;
                }
            }
        }
        
        if (debugMode)
        {
            Debug.Log($"Toggled ragdoll cho {ragdollCount} characters");
        }
    }
    
    /// <summary>
    /// Apply explosion force to all characters
    /// </summary>
    [ContextMenu("Apply Explosion Force")]
    public void ApplyExplosionForce()
    {
        Vector3 explosionCenter = spawnPoint.position;
        float explosionForce = 1000f;
        float explosionRadius = 10f;
        
        foreach (var character in spawnedCharacters)
        {
            if (character != null)
            {
                RagdollPhysicsController ragdollController = character.GetComponent<RagdollPhysicsController>();
                if (ragdollController != null)
                {
                    if (!ragdollController.IsRagdollActive)
                        ragdollController.EnableRagdoll();
                        
                    ragdollController.ApplyExplosionForce(explosionForce, explosionCenter, explosionRadius);
                }
            }
        }
        
        if (debugMode)
        {
            Debug.Log($"Applied explosion force táº¡i {explosionCenter}");
        }
    }
    
    /// <summary>
    /// Load default prefabs tá»« Assets/Prefabs
    /// </summary>
    private void LoadDefaultPrefabs()
    {
        // ThÃªm manual prefab references sáº½ Ä‘Æ°á»£c assign trong Inspector
        if (debugMode)
        {
            Debug.Log("Please assign test character prefabs in Inspector");
            Debug.Log("Recommended: Warrok vÃ  Military characters tá»« Assets/Prefabs");
        }
    }
    
    /// <summary>
    /// UI hiá»ƒn thá»‹ controls
    /// </summary>
    private void OnGUI()
    {
        if (!debugMode) return;
        
        GUILayout.BeginArea(new Rect(Screen.width - 320, 10, 300, 200));
        GUILayout.Label("=== RAGDOLL TEST MANAGER ===", GUI.skin.box);
        GUILayout.Label($"Characters Spawned: {spawnedCharacters.Count}/{maxCharacters}");
        GUILayout.Label($"Current Prefab: {(testCharacterPrefabs.Count > 0 ? testCharacterPrefabs[currentPrefabIndex].name : "None")}");
        
        GUILayout.Space(10);
        GUILayout.Label("Controls:");
        GUILayout.Label("T - Spawn Character");
        GUILayout.Label("C - Clear All");
        GUILayout.Label("R - Toggle All Ragdoll");
        GUILayout.Label("Right Click â†’ Context Menu");
        
        GUILayout.EndArea();
    }
}
