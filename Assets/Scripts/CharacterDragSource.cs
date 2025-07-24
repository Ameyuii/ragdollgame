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
            if (source.isDragging)
            {
                source.ForceEndDrag();
            }
        }
        currentlyDragging = null;
        Debug.Log("Reset all drag states");
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
        // Reset drag state if mouse is not pressed and we think we're dragging
        var mouse = UnityEngine.InputSystem.Mouse.current;
        if (isDragging && mouse != null && !mouse.leftButton.isPressed)
        {
            ForceEndDrag();
        }
        
        // Additional safety check - if we're dragging but no preview exists
        if (isDragging && dragPreview == null)
        {
            Debug.LogWarning("Drag state inconsistent - resetting");
            ForceEndDrag();
        }
        
        // Check if we're the current dragging instance but not actually dragging
        if (currentlyDragging == this && !isDragging)
        {
            Debug.LogWarning("Global drag state inconsistent - resetting");
            currentlyDragging = null;
        }
    }
    
    void ForceEndDrag()
    {
        if (isDragging && currentlyDragging == this)
        {
            Debug.Log($"[DRAG DEBUG] Force ending drag operation for {name} (Instance {instanceID})");
            isDragging = false;
            currentlyDragging = null;
            
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
        else if (isDragging)
        {
            Debug.LogWarning($"[DRAG DEBUG] {name} (Instance {instanceID}) is dragging but not current dragging instance!");
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"[DRAG DEBUG] OnBeginDrag called on {name} (Instance {instanceID})");
        Debug.Log($"[DRAG DEBUG] characterPrefab: {(characterPrefab != null ? characterPrefab.name : "null")}");
        Debug.Log($"[DRAG DEBUG] setupMode: {(gameManager != null ? gameManager.setupMode : false)}");
        Debug.Log($"[DRAG DEBUG] currentlyDragging: {(currentlyDragging != null ? currentlyDragging.name : "null")}");
        
        if (characterPrefab == null || !gameManager.setupMode) 
        {
            Debug.Log($"[DRAG DEBUG] Aborting drag - prefab null or not in setup mode");
            return;
        }
        
        // Check if another instance is already dragging
        if (currentlyDragging != null && currentlyDragging != this)
        {
            Debug.Log($"[DRAG DEBUG] Another character is already being dragged: {currentlyDragging.name}");
            return;
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
            // Use MapStateManager if available, otherwise fallback to old system
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
                        gameManager.selectedTeam, characterPrefab);
                    
                    // Notify UI Manager
                    AdvancedUIManager uiManager = FindObjectOfType<AdvancedUIManager>();
                    if (uiManager != null)
                    {
                        uiManager.OnCharacterPlaced(groundPosition);
                    }
                    
                    Debug.Log($"[DRAG DEBUG] Successfully dropped character {instanceID} at {groundPosition}");
                }
                else
                {
                    Debug.Log("[DRAG DEBUG] Invalid drop position - too close to other characters or out of bounds");
                }
            }
            else
            {
                // Fallback to old system
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
        if (isDragging && currentlyDragging == this)
        {
            currentlyDragging = null;
        }
        
        if (dragPreview != null)
        {
            DestroyImmediate(dragPreview);
            dragPreview = null;
        }
        isDragging = false;
    }
    
    void CreateDragPreview()
    {
        if (characterPrefab == null) return;
        
        // Create a simple preview (could be improved with actual character preview)
        dragPreview = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        dragPreview.name = "DragPreview";
        
        // Make it semi-transparent
        Renderer renderer = dragPreview.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material previewMat = new Material(Shader.Find("Standard"));
            previewMat.color = new Color(0.5f, 0.5f, 1f, 0.5f);
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
    }
    
    Vector3 GetMouseWorldPosition()
    {
        if (mainCamera == null) return Vector3.zero;
        
        // Use Mouse.current from Input System
        var mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse == null) return Vector3.zero;
        
        Vector2 mousePos = mouse.position.ReadValue();
        
        // Raycast from camera to ground
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        
        // Try to hit the ground plane (y = 0)
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return hit.point;
        }
        
        // If no hit, project onto y = 0 plane
        float distance = -ray.origin.y / ray.direction.y;
        if (distance > 0)
        {
            return ray.origin + ray.direction * distance;
        }
        
        return Vector3.zero;
    }
    
    bool IsValidDropPosition(Vector3 position)
    {
        // Check if position is within reasonable bounds
        if (position.x < -15 || position.x > 15 || position.z < -10 || position.z > 10)
            return false;
        
        // Check if there's already a character too close
        Collider[] nearby = Physics.OverlapSphere(position, 1.5f);
        foreach (Collider col in nearby)
        {
            if (col.GetComponent<RagdollCharacter>() != null)
            {
                return false; // Too close to another character
            }
        }
        
        return true;
    }
}