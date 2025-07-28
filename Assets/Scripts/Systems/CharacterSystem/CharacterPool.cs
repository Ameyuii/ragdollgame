using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Object pooling system for characters to improve performance
/// </summary>
public class CharacterPool : MonoBehaviour
{
    [Header("Pool Settings")]
    public int defaultPoolSize = 10;
    public int maxPoolSize = 50;
    public bool enablePooling = true;
    public bool debugMode = false;

    [Header("Preload Settings")]
    public bool preloadOnStart = true;
    public string[] preloadCharacterIDs = { "warrior_basic_default_01", "archer_basic_default_01" };

    // Pool storage
    private Dictionary<string, Queue<GameObject>> pooledCharacters;
    private Dictionary<string, GameObject> prefabReferences;
    private Dictionary<string, int> poolSizes;
    private Dictionary<string, Transform> poolParents;

    // Singleton instance
    private static CharacterPool _instance;
    public static CharacterPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CharacterPool>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("CharacterPool");
                    _instance = go.AddComponent<CharacterPool>();
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
            InitializePool();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (preloadOnStart)
        {
            StartCoroutine(PreloadCharacters());
        }
    }

    /// <summary>
    /// Initialize the pooling system
    /// </summary>
    private void InitializePool()
    {
        pooledCharacters = new Dictionary<string, Queue<GameObject>>();
        prefabReferences = new Dictionary<string, GameObject>();
        poolSizes = new Dictionary<string, int>();
        poolParents = new Dictionary<string, Transform>();

        if (debugMode)
        {
            Debug.Log("[CharacterPool] Pool system initialized");
        }
    }

    /// <summary>
    /// Preload common characters
    /// </summary>
    private IEnumerator PreloadCharacters()
    {
        foreach (string characterID in preloadCharacterIDs)
        {
            PrewarmPool(characterID, defaultPoolSize);
            yield return null; // Spread across frames
        }

        if (debugMode)
        {
            Debug.Log($"[CharacterPool] Preloaded {preloadCharacterIDs.Length} character types");
        }
    }

    /// <summary>
    /// Get character from pool or create new one
    /// </summary>
    public GameObject GetCharacter(string characterID, string variantID = "default")
    {
        if (!enablePooling)
        {
            return CreateNewCharacterInstance(characterID, variantID);
        }

        string poolKey = GetPoolKey(characterID, variantID);

        // Check if we have pooled instances
        if (pooledCharacters.TryGetValue(poolKey, out Queue<GameObject> pool) && pool.Count > 0)
        {
            GameObject pooledCharacter = pool.Dequeue();
            pooledCharacter.SetActive(true);

            if (debugMode)
            {
                Debug.Log($"[CharacterPool] Retrieved {poolKey} from pool. Remaining: {pool.Count}");
            }

            return pooledCharacter;
        }

        // Create new instance if pool is empty
        return CreateNewCharacterInstance(characterID, variantID);
    }

    /// <summary>
    /// Return character to pool
    /// </summary>
    public void ReturnCharacter(GameObject character, string characterID, string variantID = "default")
    {
        if (!enablePooling || character == null)
        {
            if (character != null)
            {
                Destroy(character);
            }
            return;
        }

        string poolKey = GetPoolKey(characterID, variantID);

        // Reset character state
        ResetCharacterForPool(character);

        // Deactivate and move to pool parent
        character.SetActive(false);
        character.transform.SetParent(GetPoolParent(poolKey));

        // Add to pool if not at max capacity
        if (!pooledCharacters.ContainsKey(poolKey))
        {
            pooledCharacters[poolKey] = new Queue<GameObject>();
        }

        if (pooledCharacters[poolKey].Count < maxPoolSize)
        {
            pooledCharacters[poolKey].Enqueue(character);

            if (debugMode)
            {
                Debug.Log($"[CharacterPool] Returned {poolKey} to pool. Total: {pooledCharacters[poolKey].Count}");
            }
        }
        else
        {
            // Pool is full, destroy the character
            Destroy(character);
            if (debugMode)
            {
                Debug.Log($"[CharacterPool] Pool full for {poolKey}, destroyed character");
            }
        }
    }

    /// <summary>
    /// Prewarm pool for specific character
    /// </summary>
    public void PrewarmPool(string characterID, int count, string variantID = "default")
    {
        string poolKey = GetPoolKey(characterID, variantID);

        if (!pooledCharacters.ContainsKey(poolKey))
        {
            pooledCharacters[poolKey] = new Queue<GameObject>();
        }

        for (int i = 0; i < count; i++)
        {
            GameObject character = CreateNewCharacterInstance(characterID, variantID);
            if (character != null)
            {
                ReturnCharacter(character, characterID, variantID);
            }
        }

        if (debugMode)
        {
            Debug.Log($"[CharacterPool] Prewarmed {count} instances of {poolKey}");
        }
    }

    /// <summary>
    /// Create new character instance
    /// </summary>
    private GameObject CreateNewCharacterInstance(string characterID, string variantID)
    {
        // Get character definition from database
        CharacterDefinition definition = GameDatabase.Instance?.GetCharacter(characterID);
        if (definition == null)
        {
            Debug.LogError($"[CharacterPool] Character definition not found: {characterID}");
            return null;
        }

        // Get prefab for variant
        GameObject prefab = definition.GetPrefab(variantID);
        if (prefab == null)
        {
            Debug.LogError($"[CharacterPool] No prefab found for {characterID} variant {variantID}");
            return null;
        }

        // Instantiate character
        GameObject instance = Instantiate(prefab);

        // Ensure it has enhanced controller
        EnhancedCharacterController controller = instance.GetComponent<EnhancedCharacterController>();
        if (controller == null)
        {
            controller = instance.AddComponent<EnhancedCharacterController>();
        }

        // Initialize with character data
        controller.Initialize(definition, variantID, 1); // Default team 1

        if (debugMode)
        {
            Debug.Log($"[CharacterPool] Created new instance of {characterID} ({variantID})");
        }

        return instance;
    }

    /// <summary>
    /// Reset character state for pooling
    /// </summary>
    private void ResetCharacterForPool(GameObject character)
    {
        // Reset position and rotation
        character.transform.position = Vector3.zero;
        character.transform.rotation = Quaternion.identity;

        // Reset character controller
        EnhancedCharacterController controller = character.GetComponent<EnhancedCharacterController>();
        if (controller != null)
        {
            controller.ResetCharacter();
        }

        // Reset legacy controller if present
        RagdollCharacter ragdoll = character.GetComponent<RagdollCharacter>();
        if (ragdoll != null)
        {
            ragdoll.ResetCharacter();
        }

        // Reset rigidbody
        Rigidbody rb = character.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Get pool key for character and variant
    /// </summary>
    private string GetPoolKey(string characterID, string variantID)
    {
        return $"{characterID}_{variantID}";
    }

    /// <summary>
    /// Get or create pool parent for organization
    /// </summary>
    private Transform GetPoolParent(string poolKey)
    {
        if (!poolParents.TryGetValue(poolKey, out Transform parent))
        {
            GameObject parentGO = new GameObject($"Pool_{poolKey}");
            parentGO.transform.SetParent(transform);
            parent = parentGO.transform;
            poolParents[poolKey] = parent;
        }
        return parent;
    }

    /// <summary>
    /// Clear all pools
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in pooledCharacters.Values)
        {
            while (pool.Count > 0)
            {
                GameObject character = pool.Dequeue();
                if (character != null)
                {
                    Destroy(character);
                }
            }
        }

        pooledCharacters.Clear();
        prefabReferences.Clear();
        poolSizes.Clear();

        if (debugMode)
        {
            Debug.Log("[CharacterPool] All pools cleared");
        }
    }

    /// <summary>
    /// Get pool statistics
    /// </summary>
    public void LogPoolStatistics()
    {
        Debug.Log("=== Character Pool Statistics ===");
        foreach (var kvp in pooledCharacters)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value.Count} pooled instances");
        }
    }

    void OnDestroy()
    {
        ClearAllPools();
    }
}
