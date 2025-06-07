using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton quáº£n lÃ½ táº¥t cáº£ ragdoll instances
/// Spawn/Despawn ragdoll tá»« prefab vÃ  tá»‘i Æ°u hiá»‡u suáº¥t
/// </summary>
public class RagdollManager : MonoBehaviour
{
    // Singleton instance
    private static RagdollManager instance;
    public static RagdollManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RagdollManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("RagdollManager");
                    instance = go.AddComponent<RagdollManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }
    
    [Header("Configuration")]
    [SerializeField] private RagdollSettings globalSettings;
    [SerializeField] private bool enablePooling = true;
    [SerializeField] private bool enableLOD = true;
    [SerializeField] private bool debugMode = false;
    
    [Header("Performance")]
    [SerializeField] private int poolSize = 10;
    [SerializeField] private float cleanupInterval = 5f;
    [SerializeField] private Camera mainCamera;
    
    // Quáº£n lÃ½ ragdoll instances
    private List<RagdollPhysicsController> activeRagdolls = new List<RagdollPhysicsController>();
    private Queue<GameObject> ragdollPool = new Queue<GameObject>();
    private Dictionary<GameObject, float> ragdollSpawnTimes = new Dictionary<GameObject, float>();
      // Events
    public System.Action<RagdollPhysicsController> OnRagdollSpawned;
    public System.Action<RagdollPhysicsController> OnRagdollDespawned;
    public System.Action<int> OnActiveRagdollCountChanged;
    
    // Properties
    public int ActiveRagdollCount => activeRagdolls.Count;
    public int PooledRagdollCount => ragdollPool.Count;
    public RagdollSettings GlobalSettings => globalSettings;
    
    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Tá»± Ä‘á»™ng tÃ¬m main camera náº¿u chÆ°a assign
        if (mainCamera == null)
            mainCamera = Camera.main;
            
        // Báº¯t Ä‘áº§u cleanup routine
        if (cleanupInterval > 0)
            InvokeRepeating(nameof(CleanupExpiredRagdolls), cleanupInterval, cleanupInterval);
    }
    
    /// <summary>
    /// Khá»Ÿi táº¡o manager
    /// </summary>
    private void Initialize()
    {
        // Load global settings náº¿u chÆ°a cÃ³
        if (globalSettings == null)
            LoadGlobalSettings();
            
        // Khá»Ÿi táº¡o pool náº¿u Ä‘Æ°á»£c báº­t
        if (enablePooling)
            InitializePool();
            
        if (debugMode)
            Debug.Log("RagdollManager initialized");
    }
    
    /// <summary>
    /// Khá»Ÿi táº¡o object pool
    /// </summary>
    private void InitializePool()
    {
        if (globalSettings == null)
        {
            Debug.LogWarning("GlobalSettings chÆ°a Ä‘Æ°á»£c set, khÃ´ng thá»ƒ khá»Ÿi táº¡o pool!");
            return;
        }
        
        if (globalSettings.ragdollPrefabs == null || globalSettings.ragdollPrefabs.Count == 0)
        {
            Debug.LogWarning("KhÃ´ng cÃ³ prefab trong GlobalSettings Ä‘á»ƒ khá»Ÿi táº¡o pool!");
            Debug.LogWarning($"Vui lÃ²ng thÃªm prefabs vÃ o RagdollSettings: {globalSettings.name}");
            return;
        }
        
        // Táº¡o pool vá»›i prefab Ä‘áº§u tiÃªn
        GameObject prefab = globalSettings.ragdollPrefabs[0];
        for (int i = 0; i < poolSize; i++)
        {
            GameObject pooledObj = Instantiate(prefab);
            pooledObj.SetActive(false);
            pooledObj.transform.SetParent(transform);
            ragdollPool.Enqueue(pooledObj);
        }
        
        if (debugMode)
            Debug.Log($"Ragdoll pool initialized vá»›i {poolSize} objects");
    }
    
    /// <summary>
    /// Spawn ragdoll tá»« prefab
    /// </summary>
    public RagdollPhysicsController SpawnRagdoll(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        // Validation Ä‘áº§u vÃ o
        if (prefab == null)
        {
            Debug.LogError("Cannot spawn ragdoll: prefab is null!");
            return null;
        }
        
        // Kiá»ƒm tra giá»›i háº¡n active ragdolls
        if (globalSettings != null && activeRagdolls.Count >= globalSettings.maxActiveRagdolls)
        {
            if (debugMode)
                Debug.Log($"Reached max ragdolls ({globalSettings.maxActiveRagdolls}), despawning oldest...");
            DespawnOldestRagdoll();
        }
        
        GameObject ragdollObj = null;
        
        // Sá»­ dá»¥ng pool náº¿u Ä‘Æ°á»£c báº­t
        if (enablePooling && ragdollPool.Count > 0)
        {
            ragdollObj = ragdollPool.Dequeue();
            ragdollObj.transform.position = position;
            ragdollObj.transform.rotation = rotation;
            ragdollObj.transform.SetParent(parent);
            ragdollObj.SetActive(true);
        }
        else
        {
            // Táº¡o má»›i náº¿u khÃ´ng cÃ³ pool hoáº·c pool trá»‘ng
            ragdollObj = Instantiate(prefab, position, rotation, parent);
        }
          // Láº¥y RagdollPhysicsController
        RagdollPhysicsController controller = ragdollObj.GetComponent<RagdollPhysicsController>();
        if (controller == null)
        {
            Debug.LogError($"Prefab {prefab.name} khÃ´ng cÃ³ RagdollPhysicsController component!");
            Debug.LogError($"Vui lÃ²ng thÃªm RagdollPhysicsController component vÃ o prefab {prefab.name}");
            
            if (enablePooling)
            {
                ragdollObj.SetActive(false);
                ragdollPool.Enqueue(ragdollObj);
            }
            else
            {
                Destroy(ragdollObj);
            }
            return null;
        }
        
        // ÄÄƒng kÃ½ ragdoll
        RegisterRagdoll(controller);
        
        // LÆ°u thá»i gian spawn
        ragdollSpawnTimes[ragdollObj] = Time.time;
        
        // Trigger events
        OnRagdollSpawned?.Invoke(controller);
        OnActiveRagdollCountChanged?.Invoke(activeRagdolls.Count);
        
        if (debugMode)
            Debug.Log($"Spawned ragdoll: {ragdollObj.name} at {position}");
            
        return controller;
    }
    
    /// <summary>
    /// Spawn ragdoll ngáº«u nhiÃªn
    /// </summary>
    public RagdollPhysicsController SpawnRandomRagdoll(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (globalSettings == null)
        {
            Debug.LogError("KhÃ´ng cÃ³ GlobalSettings Ä‘á»ƒ spawn ragdoll!");
            Debug.LogError("Vui lÃ²ng assign RagdollSettings vÃ o RagdollManager hoáº·c táº¡o file DefaultRagdollSettings trong Resources");
            return null;
        }
        
        GameObject prefab = globalSettings.GetRandomPrefab();
        if (prefab == null)
        {
            Debug.LogError("KhÃ´ng cÃ³ prefab há»£p lá»‡ Ä‘á»ƒ spawn!");
            Debug.LogError($"Kiá»ƒm tra danh sÃ¡ch prefabs trong RagdollSettings: {globalSettings.name}");
            return null;
        }
        
        return SpawnRagdoll(prefab, position, rotation, parent);
    }
    
    /// <summary>
    /// Despawn ragdoll
    /// </summary>
    public void DespawnRagdoll(RagdollPhysicsController controller)
    {
        if (controller == null) return;
        
        GameObject ragdollObj = controller.gameObject;
        
        // Há»§y Ä‘Äƒng kÃ½
        UnregisterRagdoll(controller);
        
        // XÃ³a thá»i gian spawn
        ragdollSpawnTimes.Remove(ragdollObj);
        
        // Tráº£ vá» pool hoáº·c destroy
        if (enablePooling)
        {
            ragdollObj.SetActive(false);
            ragdollObj.transform.SetParent(transform);
            ragdollPool.Enqueue(ragdollObj);
        }
        else
        {
            Destroy(ragdollObj);
        }
        
        // Trigger events
        OnRagdollDespawned?.Invoke(controller);
        OnActiveRagdollCountChanged?.Invoke(activeRagdolls.Count);
        
        if (debugMode)
            Debug.Log($"Despawned ragdoll: {ragdollObj.name}");
    }
    
    /// <summary>
    /// Despawn ragdoll cÅ© nháº¥t
    /// </summary>
    private void DespawnOldestRagdoll()
    {
        if (activeRagdolls.Count == 0) return;
        
        RagdollPhysicsController oldest = activeRagdolls[0];
        float oldestTime = float.MaxValue;
        
        // TÃ¬m ragdoll cÅ© nháº¥t
        foreach (var controller in activeRagdolls)
        {
            if (ragdollSpawnTimes.TryGetValue(controller.gameObject, out float spawnTime))
            {
                if (spawnTime < oldestTime)
                {
                    oldestTime = spawnTime;
                    oldest = controller;
                }
            }
        }
        
        DespawnRagdoll(oldest);
    }
    
    /// <summary>
    /// ÄÄƒng kÃ½ ragdoll vá»›i manager
    /// </summary>
    public void RegisterRagdoll(RagdollPhysicsController controller)
    {
        if (controller != null && !activeRagdolls.Contains(controller))
        {
            activeRagdolls.Add(controller);
            
            // Apply global settings náº¿u cÃ³
            if (globalSettings != null && controller.Settings == null)
            {
                // CÃ³ thá»ƒ assign settings thÃ´ng qua reflection hoáº·c public method
            }
        }
    }
    
    /// <summary>
    /// Há»§y Ä‘Äƒng kÃ½ ragdoll
    /// </summary>
    public void UnregisterRagdoll(RagdollPhysicsController controller)
    {
        if (controller != null)
        {
            activeRagdolls.Remove(controller);
        }
    }
    
    /// <summary>
    /// Cleanup cÃ¡c ragdoll Ä‘Ã£ háº¿t háº¡n
    /// </summary>
    private void CleanupExpiredRagdolls()
    {
        if (globalSettings == null) return;
        
        float currentTime = Time.time;
        List<RagdollPhysicsController> toRemove = new List<RagdollPhysicsController>();
        
        foreach (var controller in activeRagdolls)
        {
            if (ragdollSpawnTimes.TryGetValue(controller.gameObject, out float spawnTime))
            {
                if (currentTime - spawnTime > globalSettings.ragdollLifetime)
                {
                    toRemove.Add(controller);
                }
            }
        }
        
        foreach (var controller in toRemove)
        {
            DespawnRagdoll(controller);
        }
        
        if (debugMode && toRemove.Count > 0)
            Debug.Log($"Cleaned up {toRemove.Count} expired ragdolls");
    }
    
    /// <summary>
    /// Update LOD cho táº¥t cáº£ ragdolls
    /// </summary>
    private void Update()
    {
        if (enableLOD && mainCamera != null)
            UpdateRagdollLOD();
    }
    
    /// <summary>
    /// Cáº­p nháº­t LOD dá»±a trÃªn khoáº£ng cÃ¡ch camera
    /// </summary>
    private void UpdateRagdollLOD()
    {
        if (globalSettings == null) return;
        
        Vector3 cameraPos = mainCamera.transform.position;
        
        foreach (var controller in activeRagdolls)
        {
            if (controller == null) continue;
            
            float distance = Vector3.Distance(cameraPos, controller.transform.position);
            bool isNearCamera = distance <= globalSettings.lodDistance;
            
            // Äiá»u chá»‰nh quality dá»±a trÃªn khoáº£ng cÃ¡ch
            // CÃ³ thá»ƒ disable cÃ¡c component khÃ´ng cáº§n thiáº¿t khi xa
            var rigidBodies = controller.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rigidBodies)
            {
                if (rb != null)
                {
                    rb.interpolation = isNearCamera ? 
                        RigidbodyInterpolation.Interpolate : 
                        RigidbodyInterpolation.None;
                }
            }
        }
    }
    
    /// <summary>
    /// Load global settings
    /// </summary>
    private void LoadGlobalSettings()
    {
        globalSettings = Resources.Load<RagdollSettings>("DefaultRagdollSettings");
        if (globalSettings == null)
        {
            if (debugMode)
            {
                Debug.LogWarning("KhÃ´ng tÃ¬m tháº¥y DefaultRagdollSettings trong Resources");
                Debug.LogWarning("Táº¡o file RagdollSettings vÃ  Ä‘áº·t vÃ o thÆ° má»¥c Resources vá»›i tÃªn 'DefaultRagdollSettings'");
            }
            
            // TÃ¬m báº¥t ká»³ RagdollSettings nÃ o trong project
            RagdollSettings[] allSettings = Resources.FindObjectsOfTypeAll<RagdollSettings>();
            if (allSettings.Length > 0)
            {
                globalSettings = allSettings[0];
                if (debugMode)
                    Debug.Log($"Sá»­ dá»¥ng RagdollSettings thay tháº¿: {globalSettings.name}");
            }
        }
        else if (debugMode)
        {
            Debug.Log($"Loaded RagdollSettings: {globalSettings.name}");
        }
    }
    
    /// <summary>
    /// Despawn táº¥t cáº£ ragdolls
    /// </summary>
    public void DespawnAllRagdolls()
    {
        while (activeRagdolls.Count > 0)
        {
            DespawnRagdoll(activeRagdolls[0]);
        }
        
        if (debugMode)
            Debug.Log("Despawned all ragdolls");
    }
    
    /// <summary>
    /// Láº¥y ragdoll gáº§n nháº¥t vá»›i position
    /// </summary>
    public RagdollPhysicsController GetNearestRagdoll(Vector3 position)
    {
        RagdollPhysicsController nearest = null;
        float nearestDistance = float.MaxValue;
        
        foreach (var controller in activeRagdolls)
        {
            if (controller == null) continue;
            
            float distance = Vector3.Distance(position, controller.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = controller;
            }
        }
        
        return nearest;
    }
    
    /// <summary>
    /// Láº¥y táº¥t cáº£ ragdolls trong bÃ¡n kÃ­nh
    /// </summary>
    public List<RagdollPhysicsController> GetRagdollsInRadius(Vector3 center, float radius)
    {
        List<RagdollPhysicsController> result = new List<RagdollPhysicsController>();
        
        foreach (var controller in activeRagdolls)
        {
            if (controller == null) continue;
            
            float distance = Vector3.Distance(center, controller.transform.position);
            if (distance <= radius)
            {
                result.Add(controller);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Debug info
    /// </summary>
    private void OnGUI()
    {
        if (!debugMode) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label($"Active Ragdolls: {activeRagdolls.Count}");
        GUILayout.Label($"Pooled Ragdolls: {ragdollPool.Count}");
        
        if (globalSettings != null)
        {
            GUILayout.Label($"Max Ragdolls: {globalSettings.maxActiveRagdolls}");
            GUILayout.Label($"Lifetime: {globalSettings.ragdollLifetime}s");
        }
        
        if (GUILayout.Button("Spawn Random Ragdoll"))
        {
            Vector3 spawnPos = mainCamera != null ? 
                mainCamera.transform.position + mainCamera.transform.forward * 5f :
                Vector3.zero;
            SpawnRandomRagdoll(spawnPos, Quaternion.identity);
        }
        
        if (GUILayout.Button("Despawn All"))
        {
            DespawnAllRagdolls();
        }
        
        GUILayout.EndArea();
    }
}
