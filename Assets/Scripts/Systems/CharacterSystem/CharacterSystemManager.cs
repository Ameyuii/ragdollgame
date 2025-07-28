using UnityEngine;

/// <summary>
/// Main manager for the new character system
/// </summary>
public class CharacterSystemManager : MonoBehaviour
{
    [Header("System Settings")]
    public bool enableNewSystem = false;
    public bool debugMode = true;

    [Header("Database References")]
    public CharacterDatabase characterDatabase;
    public GameDatabase gameDatabase;

    private static CharacterSystemManager _instance;
    public static CharacterSystemManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CharacterSystemManager>();
                
                if (_instance == null)
                {
                    GameObject go = new GameObject("CharacterSystemManager");
                    _instance = go.AddComponent<CharacterSystemManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSystem();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initialize the character system
    /// </summary>
    private void InitializeSystem()
    {
        if (debugMode)
        {
            Debug.Log("[CharacterSystem] Initializing Character Management System...");
        }

        // Load databases if not assigned
        if (gameDatabase == null)
        {
            gameDatabase = Resources.Load<GameDatabase>("GameDatabase");
        }

        if (characterDatabase == null && gameDatabase != null)
        {
            characterDatabase = gameDatabase.characterDatabase;
        }

        if (debugMode)
        {
            Debug.Log($"[CharacterSystem] System initialized. New system enabled: {enableNewSystem}");
            if (characterDatabase != null)
            {
                Debug.Log($"[CharacterSystem] Character database loaded with {characterDatabase.characters.Count} characters");
            }
        }
    }

    /// <summary>
    /// Check if new system should be used
    /// </summary>
    public bool ShouldUseNewSystem()
    {
        return enableNewSystem && characterDatabase != null;
    }

    /// <summary>
    /// Spawn character using new system with object pooling
    /// </summary>
    public GameObject SpawnCharacter(string characterID, string variantID, int teamID, Vector3 position)
    {
        if (!ShouldUseNewSystem())
        {
            if (debugMode) Debug.Log("[CharacterSystem] New system disabled, falling back to legacy system");
            return null;
        }

        CharacterDefinition definition = characterDatabase.GetCharacter(characterID);
        if (definition == null)
        {
            Debug.LogError($"[CharacterSystem] Character not found: {characterID}");
            return null;
        }

        // Get character from pool
        GameObject instance = CharacterPool.Instance.GetCharacter(characterID, variantID);
        if (instance == null)
        {
            Debug.LogError($"[CharacterSystem] Failed to get character from pool: {characterID}, variant: {variantID}");
            return null;
        }

        // Set position
        instance.transform.position = position;
        instance.transform.rotation = Quaternion.identity;

        // Setup enhanced controller
        EnhancedCharacterController controller = instance.GetComponent<EnhancedCharacterController>();
        if (controller == null)
        {
            controller = instance.AddComponent<EnhancedCharacterController>();
        }

        // Initialize with character data
        controller.Initialize(definition, variantID, teamID);

        if (debugMode)
        {
            Debug.Log($"[CharacterSystem] Spawned character: {characterID} ({variantID}) for team {teamID}");
        }

        // Trigger event
        CharacterEvents.TriggerCharacterSpawned(controller);

        return instance;
    }

    /// <summary>
    /// Return character to pool when destroyed
    /// </summary>
    public void ReturnCharacter(GameObject character)
    {
        if (character == null) return;

        EnhancedCharacterController controller = character.GetComponent<EnhancedCharacterController>();
        if (controller != null)
        {
            CharacterPool.Instance.ReturnCharacter(character, controller.CharacterID, controller.VariantID);
        }
        else
        {
            // Fallback - destroy if we can't identify the character
            Destroy(character);
        }
    }

    /// <summary>
    /// Get character definition by ID
    /// </summary>
    public CharacterDefinition GetCharacterDefinition(string characterID)
    {
        return characterDatabase?.GetCharacter(characterID);
    }

    /// <summary>
    /// Get team configuration by ID
    /// </summary>
    public TeamConfiguration GetTeamConfiguration(int teamID)
    {
        return characterDatabase?.GetTeam(teamID);
    }
}