using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Performance optimization manager for character system
/// </summary>
public class PerformanceManager : MonoBehaviour
{
    [Header("Performance Settings")]
    public int maxActiveCharacters = 50;
    public int maxUIPreviewInstances = 5;
    public float characterUpdateInterval = 0.1f;
    public bool enablePerformanceMonitoring = true;

    [Header("Pooling")]
    public bool enableObjectPooling = true;
    public int poolPrewarmCount = 10;

    [Header("LOD Settings")]
    public bool enableLODSystem = true;
    public float[] lodDistances = { 10f, 25f, 50f };
    public int maxHighDetailCharacters = 20;

    [Header("Culling")]
    public bool enableFrustumCulling = true;
    public bool enableDistanceCulling = true;
    public float maxRenderDistance = 100f;

    [Header("Update Optimization")]
    public bool enableBatchUpdates = true;
    public int charactersPerFrame = 5;

    // Performance tracking
    private int activeCharacterCount = 0;
    private float lastPerformanceCheck = 0f;
    private float averageFrameTime = 0f;
    private Queue<float> frameTimeHistory = new Queue<float>();

    // Character management
    private List<EnhancedCharacterController> activeCharacters = new List<EnhancedCharacterController>();
    private List<EnhancedCharacterController> culledCharacters = new List<EnhancedCharacterController>();
    private int currentUpdateIndex = 0;

    // Singleton
    private static PerformanceManager _instance;
    public static PerformanceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PerformanceManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("PerformanceManager");
                    _instance = go.AddComponent<PerformanceManager>();
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
            InitializePerformanceManager();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (enableObjectPooling)
        {
            PrewarmCharacterPools();
        }

        if (enableLODSystem)
        {
            SetupLODSystem();
        }

        // Start performance monitoring
        if (enablePerformanceMonitoring)
        {
            InvokeRepeating(nameof(MonitorPerformance), 1f, 1f);
        }

        // Start batch updates
        if (enableBatchUpdates)
        {
            StartCoroutine(BatchUpdateCharacters());
        }
    }

    void Update()
    {
        if (enablePerformanceMonitoring)
        {
            TrackFrameTime();
        }

        if (enableFrustumCulling || enableDistanceCulling)
        {
            UpdateCulling();
        }
    }

    /// <summary>
    /// Initialize performance manager
    /// </summary>
    private void InitializePerformanceManager()
    {
        // Subscribe to character events
        CharacterEvents.OnCharacterSpawned += OnCharacterSpawned;
        CharacterEvents.OnCharacterDied += OnCharacterDied;

        Debug.Log("[PerformanceManager] Performance manager initialized");
    }

    /// <summary>
    /// Prewarm character pools
    /// </summary>
    private void PrewarmCharacterPools()
    {
        CharacterPool pool = CharacterPool.Instance;

        // Prewarm pools for common character types
        string[] commonCharacters = { "warrior_basic_default_01", "archer_basic_default_01", "mage_basic_default_01" };

        foreach (string characterID in commonCharacters)
        {
            pool.PrewarmPool(characterID, poolPrewarmCount);
        }

        Debug.Log($"[PerformanceManager] Prewarmed {commonCharacters.Length} character pools");
    }

    /// <summary>
    /// Setup LOD system
    /// </summary>
    private void SetupLODSystem()
    {
        // LOD system will be applied to characters as they spawn
        Debug.Log("[PerformanceManager] LOD system enabled");
    }

    /// <summary>
    /// Monitor performance metrics
    /// </summary>
    private void MonitorPerformance()
    {
        // Check frame rate
        float currentFPS = 1f / averageFrameTime;
        
        // Adjust quality based on performance
        if (currentFPS < 30f && activeCharacterCount > maxHighDetailCharacters)
        {
            ReduceQuality();
        }
        else if (currentFPS > 50f && activeCharacterCount < maxActiveCharacters)
        {
            IncreaseQuality();
        }

        // Log performance stats
        if (Time.time - lastPerformanceCheck > 5f)
        {
            LogPerformanceStats();
            lastPerformanceCheck = Time.time;
        }
    }

    /// <summary>
    /// Track frame time for performance monitoring
    /// </summary>
    private void TrackFrameTime()
    {
        float frameTime = Time.unscaledDeltaTime;
        frameTimeHistory.Enqueue(frameTime);

        if (frameTimeHistory.Count > 60) // Keep last 60 frames
        {
            frameTimeHistory.Dequeue();
        }

        // Calculate average
        float total = 0f;
        foreach (float time in frameTimeHistory)
        {
            total += time;
        }
        averageFrameTime = total / frameTimeHistory.Count;
    }

    /// <summary>
    /// Update culling for characters
    /// </summary>
    private void UpdateCulling()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        foreach (EnhancedCharacterController character in activeCharacters)
        {
            if (character == null) continue;

            bool shouldCull = false;

            // Distance culling
            if (enableDistanceCulling)
            {
                float distance = Vector3.Distance(mainCamera.transform.position, character.transform.position);
                if (distance > maxRenderDistance)
                {
                    shouldCull = true;
                }
            }

            // Frustum culling
            if (enableFrustumCulling && !shouldCull)
            {
                Bounds bounds = GetCharacterBounds(character);
                if (!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(mainCamera), bounds))
                {
                    shouldCull = true;
                }
            }

            // Apply culling
            SetCharacterCulled(character, shouldCull);
        }
    }

    /// <summary>
    /// Get character bounds for culling
    /// </summary>
    private Bounds GetCharacterBounds(EnhancedCharacterController character)
    {
        Renderer[] renderers = character.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return new Bounds(character.transform.position, Vector3.one);
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }

    /// <summary>
    /// Set character culled state
    /// </summary>
    private void SetCharacterCulled(EnhancedCharacterController character, bool culled)
    {
        if (culled && !culledCharacters.Contains(character))
        {
            culledCharacters.Add(character);
            activeCharacters.Remove(character);
            
            // Disable renderers
            Renderer[] renderers = character.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
        }
        else if (!culled && culledCharacters.Contains(character))
        {
            culledCharacters.Remove(character);
            activeCharacters.Add(character);
            
            // Enable renderers
            Renderer[] renderers = character.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }
        }
    }

    /// <summary>
    /// Batch update characters to spread load across frames
    /// </summary>
    private IEnumerator BatchUpdateCharacters()
    {
        while (true)
        {
            if (activeCharacters.Count > 0)
            {
                int charactersToUpdate = Mathf.Min(charactersPerFrame, activeCharacters.Count);
                
                for (int i = 0; i < charactersToUpdate; i++)
                {
                    int index = (currentUpdateIndex + i) % activeCharacters.Count;
                    if (index < activeCharacters.Count && activeCharacters[index] != null)
                    {
                        UpdateCharacterLOD(activeCharacters[index]);
                    }
                }

                currentUpdateIndex = (currentUpdateIndex + charactersToUpdate) % activeCharacters.Count;
            }

            yield return new WaitForSeconds(characterUpdateInterval);
        }
    }

    /// <summary>
    /// Update character LOD based on distance
    /// </summary>
    private void UpdateCharacterLOD(EnhancedCharacterController character)
    {
        if (!enableLODSystem || character == null) return;

        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        float distance = Vector3.Distance(mainCamera.transform.position, character.transform.position);
        
        // Determine LOD level
        int lodLevel = 0;
        for (int i = 0; i < lodDistances.Length; i++)
        {
            if (distance > lodDistances[i])
            {
                lodLevel = i + 1;
            }
        }

        // Apply LOD
        ApplyLODToCharacter(character, lodLevel);
    }

    /// <summary>
    /// Apply LOD level to character
    /// </summary>
    private void ApplyLODToCharacter(EnhancedCharacterController character, int lodLevel)
    {
        // Disable/enable components based on LOD level
        Animator animator = character.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = lodLevel < 2; // Disable animation at high distances
        }

        // Reduce update frequency for distant characters
        if (lodLevel >= 2)
        {
            // Reduce AI update frequency
            ImprovedCharacterAI ai = character.GetComponent<ImprovedCharacterAI>();
            if (ai != null)
            {
                ai.enabled = false; // Disable AI for very distant characters
            }
        }
    }

    /// <summary>
    /// Reduce quality to improve performance
    /// </summary>
    private void ReduceQuality()
    {
        maxHighDetailCharacters = Mathf.Max(10, maxHighDetailCharacters - 2);
        Debug.Log($"[PerformanceManager] Reduced quality - Max high detail characters: {maxHighDetailCharacters}");
    }

    /// <summary>
    /// Increase quality when performance allows
    /// </summary>
    private void IncreaseQuality()
    {
        maxHighDetailCharacters = Mathf.Min(30, maxHighDetailCharacters + 1);
        Debug.Log($"[PerformanceManager] Increased quality - Max high detail characters: {maxHighDetailCharacters}");
    }

    /// <summary>
    /// Log performance statistics
    /// </summary>
    private void LogPerformanceStats()
    {
        float fps = 1f / averageFrameTime;
        Debug.Log($"[PerformanceManager] FPS: {fps:F1}, Active Characters: {activeCharacterCount}, Culled: {culledCharacters.Count}");
    }

    /// <summary>
    /// Handle character spawned event
    /// </summary>
    private void OnCharacterSpawned(ICharacter character)
    {
        if (character is EnhancedCharacterController enhanced)
        {
            activeCharacters.Add(enhanced);
            activeCharacterCount++;
        }
    }

    /// <summary>
    /// Handle character died event
    /// </summary>
    private void OnCharacterDied(ICharacter character)
    {
        if (character is EnhancedCharacterController enhanced)
        {
            activeCharacters.Remove(enhanced);
            culledCharacters.Remove(enhanced);
            activeCharacterCount--;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        CharacterEvents.OnCharacterSpawned -= OnCharacterSpawned;
        CharacterEvents.OnCharacterDied -= OnCharacterDied;
    }
}
