using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterDragSource : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag Settings")]
    public GameObject characterPrefab;
    public BattleGameManager gameManager;
    
    private GameObject dragPreview;
    private Canvas canvas;
    private Camera mainCamera;
    public bool isDragging = false;
    
    // Static manager for global drag state
    private static CharacterDragSource currentlyDragging = null;
    private static int nextInstanceID = 0;
    private int instanceID;
    
    void Awake()
    {
        instanceID = nextInstanceID++;
    }
    
    // Static method to reset all drag states
    public static void ResetAllDragStates()
    {
        CharacterDragSource[] allSources = FindObjectsOfType<CharacterDragSource>();
        foreach (CharacterDragSource source in allSources)
        {
            if (source != null && source.isDragging)
            {
                source.ForceEndDrag();
            }
        }
        currentlyDragging = null;
        Debug.Log("Reset all drag states");
    }
    
    // Static method to check if any drag is in progress
    public static bool IsAnyDragInProgress()
    {
        return currentlyDragging != null;
    }
    
    // Static method to get current dragging source
    public static CharacterDragSource GetCurrentDraggingSource()
    {
        return currentlyDragging;
    }
    
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;
        
        if (gameManager == null)
            gameManager = FindObjectOfType<BattleGameManager>();
    }
    
    void Update()
    {
        // Only check mouse state if we're actually dragging
        if (isDragging && currentlyDragging == this)
        {
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (mouse != null && !mouse.leftButton.isPressed)
            {
                Debug.Log($"[DRAG DEBUG] Mouse released detected, force ending drag for {name}");
                ForceEndDrag();
            }
            
            // Additional safety check - if we're dragging but no preview exists
            if (dragPreview == null)
            {
                Debug.LogWarning("Drag state inconsistent - resetting");
                ForceEndDrag();
            }
        }
        
        // Clean up inconsistent global state
        if (currentlyDragging == this && !isDragging)
        {
            Debug.LogWarning("Global drag state inconsistent - resetting");
            currentlyDragging = null;
        }
    }
    
    public void ForceEndDrag()
    {
        Debug.Log($"[DRAG DEBUG] Force ending drag operation for {name} (Instance {instanceID})");
        
        // Always reset our own state
        isDragging = false;
        
        // Only clear global state if we're the current dragging instance
        if (currentlyDragging == this)
        {
            currentlyDragging = null;
        }
        
        // Ensure button remains interactable
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = true;
        }
        
        if (dragPreview != null)
        {
            DestroyImmediate(dragPreview);
            dragPreview = null;
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"[DRAG DEBUG] OnBeginDrag called on {name} (Instance {instanceID})");
        Debug.Log($"[DRAG DEBUG] characterPrefab: {(characterPrefab != null ? characterPrefab.name : "null")}");
        Debug.Log($"[DRAG DEBUG] setupMode: {(gameManager != null ? gameManager.setupMode : false)}");
        Debug.Log($"[DRAG DEBUG] currentlyDragging: {(currentlyDragging != null ? currentlyDragging.name : "null")}");
        
        if (characterPrefab == null || gameManager == null || !gameManager.setupMode) 
        {
            Debug.Log($"[DRAG DEBUG] Aborting drag - prefab null, gameManager null, or not in setup mode");
            return;
        }
        
        // Check if another instance is already dragging
        if (currentlyDragging != null && currentlyDragging != this)
        {
            Debug.Log($"[DRAG DEBUG] Another character is already being dragged: {currentlyDragging.name}");
            // Force end the other drag operation
            currentlyDragging.ForceEndDrag();
        }
        
        // Ensure button remains interactable
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = true;
        }
        
        isDragging = true;
        currentlyDragging = this;
        
        // Create drag preview
        CreateDragPreview();
        
        Debug.Log($"[DRAG DEBUG] Successfully started dragging {characterPrefab.name} (Instance {instanceID})");
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (dragPreview == null || !isDragging) 
        {
            if (isDragging && dragPreview == null)
            {
                Debug.LogWarning($"[DRAG DEBUG] {name} (Instance {instanceID}) is dragging but has no preview!");
            }
            return;
        }
        
        // Update preview position to follow mouse in world space
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        if (mouseWorldPos != Vector3.zero)
        {
            dragPreview.transform.position = mouseWorldPos;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"[DRAG DEBUG] OnEndDrag called on {name} (Instance {instanceID})");
        Debug.Log($"[DRAG DEBUG] isDragging: {isDragging}, currentlyDragging: {(currentlyDragging != null ? currentlyDragging.name : "null")}");
        
        if (!isDragging || currentlyDragging != this) 
        {
            Debug.Log($"[DRAG DEBUG] Aborting end drag - not dragging or not current");
            return;
        }
        
        isDragging = false;
        currentlyDragging = null;
        
        // Ensure button remains interactable
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = true;
        }
        
        // Get drop position
        Vector3 dropPosition = GetMouseWorldPosition();
        Debug.Log($"[DRAG DEBUG] Drop position: {dropPosition}");
        
        if (dropPosition != Vector3.zero && IsValidDropPosition(dropPosition))
        {
            // Team selection được quản lý trực tiếp bởi BattleGameManager
            Debug.Log($"[DRAG DEBUG] Using team from BattleGameManager: {gameManager.GetSelectedTeam()}");

            MapStateManager mapManager = MapStateManager.Instance;
            if (mapManager != null)
            {
                // Get ground position
                Vector3 groundPosition = mapManager.GetGroundPosition(dropPosition);
                
                if (mapManager.IsValidPosition(groundPosition))
                {
                    // Add through MapStateManager
                    string instanceID = mapManager.AddCharacterInstance(
                        characterPrefab.name, groundPosition, 
                        gameManager.GetSelectedTeam(), characterPrefab);
                    
                    Debug.Log($"[DRAG DEBUG] Successfully dropped character {instanceID} at {groundPosition} for team {gameManager.GetSelectedTeam()}");
                }
                else
                {
                    Debug.Log("[DRAG DEBUG] Invalid drop position - too close to other characters or out of bounds");
                }
            }
            else
            {
                // Fallback to original system
                gameManager.SpawnCharacterAtPosition(dropPosition);
                Debug.Log($"[DRAG DEBUG] Dropped character at {dropPosition} (fallback)");
            }
        }
        else
        {
            Debug.Log("[DRAG DEBUG] Invalid drop position");
        }
        
        // Clean up preview
        if (dragPreview != null)
        {
            DestroyImmediate(dragPreview);
            dragPreview = null;
        }
        
        Debug.Log($"[DRAG DEBUG] End drag completed for {name} (Instance {instanceID})");
    }
    
    void OnDisable()
    {
        // Clean up if disabled while dragging
        ForceEndDrag();
    }
    
    void OnDestroy()
    {
        // Clean up if destroyed while dragging
        ForceEndDrag();
    }
    
    void CreateDragPreview()
    {
        if (characterPrefab == null) return;
        
        // Clean up any existing preview first
        if (dragPreview != null)
        {
            DestroyImmediate(dragPreview);
            dragPreview = null;
        }
        
        // Create a simple preview (could be improved with actual character preview)
        dragPreview = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        dragPreview.name = $"DragPreview_{characterPrefab.name}_{instanceID}";
        
        // Make it semi-transparent
        Renderer renderer = dragPreview.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material previewMat = new Material(Shader.Find("Standard"));
            
            // Set team color
            Color teamColor = (gameManager != null && gameManager.GetSelectedTeam() == 1) ? Color.blue : Color.red;
            previewMat.color = new Color(teamColor.r, teamColor.g, teamColor.b, 0.6f);
            
            previewMat.SetFloat("_Mode", 3); // Transparent mode
            previewMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            previewMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            previewMat.SetInt("_ZWrite", 0);
            previewMat.DisableKeyword("_ALPHATEST_ON");
            previewMat.EnableKeyword("_ALPHABLEND_ON");
            previewMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            previewMat.renderQueue = 3000;
            
            renderer.material = previewMat;
        }
        
        // Remove collider to avoid interference
        Collider col = dragPreview.GetComponent<Collider>();
        if (col != null)
            DestroyImmediate(col);
        
        // Scale it down a bit
        dragPreview.transform.localScale = Vector3.one * 0.8f;
        
        Debug.Log($"[DRAG DEBUG] Created drag preview: {dragPreview.name}");
    }
    
    Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) return Vector3.zero;
        
        // Use Mouse.current from Input System
        var mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse == null) return Vector3.zero;
        
        Vector2 mousePos = mouse.position.ReadValue();
        
        // Raycast from camera to any surface
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        
        // Try to hit any surface with extended range
        if (Physics.Raycast(ray, out hit, 1000f))
        {
            return hit.point;
        }
        
        // If no hit, project onto y = 0 plane (extended range)
        if (ray.direction.y != 0)
        {
            float distance = -ray.origin.y / ray.direction.y;
            if (distance > 0 && distance < 1000f)
            {
                return ray.origin + ray.direction * distance;
            }
        }
        
        // Fallback: project at a reasonable distance
        return ray.origin + ray.direction * 50f;
    }
    
    bool IsValidDropPosition(Vector3 position)
    {
        // Removed all restrictions - allow spawning anywhere on map
        // No position bounds check
        // No distance check between characters
        // Allow unlimited spawning
        return true;
    }
}