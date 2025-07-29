using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Simple Character Drag - Simplified replacement for CharacterDragSource
/// Handles drag and drop functionality for character spawning
/// </summary>
public class SimpleCharacterDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("🎯 Character Data")]
    [Tooltip("Character entry from registry")]
    public CharacterRegistry.CharacterEntry characterEntry;
    
    [Tooltip("Reference to unified game manager")]
    public UnifiedGameManager unifiedGameManager;
    
    [Header("🖱️ Drag Settings")]
    [Tooltip("Drag preview prefab (optional)")]
    public GameObject dragPreviewPrefab;
    
    [Tooltip("Scale factor for drag preview")]
    public float previewScale = 0.8f;
    
    [Tooltip("Alpha for drag preview")]
    public float previewAlpha = 0.7f;
    
    [Header("📊 Runtime State")]
    [SerializeField] private bool isDragging = false;
    [SerializeField] private bool canDrag = true;
    
    // Drag preview objects
    private GameObject dragPreview;
    private Canvas dragCanvas;
    private GraphicRaycaster graphicRaycaster;
    
    // Original button state
    private Image originalImage;
    private Color originalColor;
    
    private void Start()
    {
        InitializeDragComponent();
    }
    
    private void InitializeDragComponent()
    {
        // Get original image component
        originalImage = GetComponent<Image>();
        if (originalImage != null)
        {
            originalColor = originalImage.color;
        }
        
        // Find unified game manager if not assigned
        if (unifiedGameManager == null)
        {
            unifiedGameManager = UnifiedGameManager.Instance;
        }
        
        // Validate character entry
        if (characterEntry?.prefab == null)
        {
            Debug.LogWarning($"⚠️ SimpleCharacterDrag: Invalid character entry on {gameObject.name}");
            canDrag = false;
        }
        
        Debug.Log($"🖱️ SimpleCharacterDrag initialized for: {characterEntry?.displayName}");
    }
    
    #region Drag Interface Implementation
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag || characterEntry?.prefab == null)
        {
            Debug.LogWarning("⚠️ Cannot start drag - invalid setup");
            return;
        }
        
        isDragging = true;
        
        // Create drag preview
        CreateDragPreview(eventData);
        
        // Dim original button
        if (originalImage != null)
        {
            originalImage.color = originalColor * 0.5f;
        }
        
        Debug.Log($"🖱️ Started dragging: {characterEntry.displayName}");
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || dragPreview == null) return;
        
        // Update drag preview position
        UpdateDragPreviewPosition(eventData);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        isDragging = false;
        
        // Restore original button appearance
        if (originalImage != null)
        {
            originalImage.color = originalColor;
        }
        
        // Handle drop
        HandleDrop(eventData);
        
        // Cleanup drag preview
        CleanupDragPreview();
        
        Debug.Log($"🖱️ Ended dragging: {characterEntry.displayName}");
    }
    
    #endregion
    
    #region Drag Preview Management
    
    private void CreateDragPreview(PointerEventData eventData)
    {
        // Create drag canvas if needed
        CreateDragCanvas();
        
        if (dragPreviewPrefab != null)
        {
            // Use custom preview prefab
            dragPreview = Instantiate(dragPreviewPrefab, dragCanvas.transform);
        }
        else
        {
            // Create simple preview from character prefab
            CreateSimplePreview();
        }
        
        if (dragPreview != null)
        {
            // Setup preview properties
            dragPreview.transform.localScale = Vector3.one * previewScale;
            
            // Set alpha
            SetPreviewAlpha(previewAlpha);
            
            // Initial position
            UpdateDragPreviewPosition(eventData);
        }
    }
    
    private void CreateDragCanvas()
    {
        if (dragCanvas != null) return;
        
        // Create canvas for drag preview
        GameObject canvasObj = new GameObject("DragCanvas");
        dragCanvas = canvasObj.AddComponent<Canvas>();
        dragCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        dragCanvas.sortingOrder = 1000; // High sorting order to appear on top
        
        // Add GraphicRaycaster for UI interactions
        graphicRaycaster = canvasObj.AddComponent<GraphicRaycaster>();
        
        // Disable raycasting so it doesn't block other UI
        graphicRaycaster.enabled = false;
    }
    
    private void CreateSimplePreview()
    {
        if (characterEntry?.prefab == null) return;
        
        // Create simple image preview
        GameObject previewObj = new GameObject("CharacterPreview");
        previewObj.transform.SetParent(dragCanvas.transform, false);
        
        // Add Image component
        Image previewImage = previewObj.AddComponent<Image>();
        
        // Use character icon if available
        if (characterEntry.icon != null)
        {
            previewImage.sprite = characterEntry.icon;
        }
        else
        {
            // Use a simple colored square
            previewImage.color = GetCharacterColor();
        }
        
        // Set size
        RectTransform rectTransform = previewObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(80, 80);
        
        dragPreview = previewObj;
    }
    
    private Color GetCharacterColor()
    {
        if (characterEntry?.category == null) return Color.gray;
        
        switch (characterEntry.category.ToUpper())
        {
            case "ROBOT": return Color.cyan;
            case "QUAIVAT": return Color.red;
            case "CHIENBINH": return Color.green;
            case "ZOMBIE": return Color.yellow;
            default: return Color.gray;
        }
    }
    
    private void SetPreviewAlpha(float alpha)
    {
        if (dragPreview == null) return;
        
        // Set alpha for all Image components in preview
        Image[] images = dragPreview.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            Color color = img.color;
            color.a = alpha;
            img.color = color;
        }
    }
    
    private void UpdateDragPreviewPosition(PointerEventData eventData)
    {
        if (dragPreview == null || dragCanvas == null) return;
        
        // Convert screen position to canvas position
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );
        
        dragPreview.transform.localPosition = localPoint;
    }
    
    private void CleanupDragPreview()
    {
        if (dragPreview != null)
        {
            DestroyImmediate(dragPreview);
            dragPreview = null;
        }
        
        if (dragCanvas != null)
        {
            DestroyImmediate(dragCanvas.gameObject);
            dragCanvas = null;
            graphicRaycaster = null;
        }
    }
    
    #endregion
    
    #region Drop Handling
    
    private void HandleDrop(PointerEventData eventData)
    {
        // Check if dropped on valid drop zone
        Vector3 worldPosition = GetDropWorldPosition(eventData);
        
        if (IsValidDropPosition(worldPosition))
        {
            SpawnCharacter(worldPosition);
        }
        else
        {
            Debug.Log("⚠️ Invalid drop position");
        }
    }
    
    private Vector3 GetDropWorldPosition(PointerEventData eventData)
    {
        // Convert screen position to world position
        Camera camera = Camera.main;
        if (camera == null)
        {
            camera = FindFirstObjectByType<Camera>();
        }
        
        if (camera != null)
        {
            // Raycast to ground plane
            Ray ray = camera.ScreenPointToRay(eventData.position);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            
            if (groundPlane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }
        }
        
        // Fallback: use screen position converted to world
        Vector3 screenPos = eventData.position;
        screenPos.z = 10f; // Distance from camera
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
    
    private bool IsValidDropPosition(Vector3 worldPosition)
    {
        // Simple validation - check if position is within reasonable bounds
        float maxDistance = 20f;
        return Vector3.Distance(Vector3.zero, worldPosition) <= maxDistance;
    }
    
    private void SpawnCharacter(Vector3 position)
    {
        if (unifiedGameManager == null || characterEntry?.id == null)
        {
            Debug.LogError("❌ Cannot spawn character - missing references");
            return;
        }
        
        // Spawn character using unified game manager
        GameObject spawnedCharacter = unifiedGameManager.SpawnCharacter(characterEntry.id, position);
        
        if (spawnedCharacter != null)
        {
            Debug.Log($"✅ Successfully spawned {characterEntry.displayName} at {position}");
        }
        else
        {
            Debug.LogError($"❌ Failed to spawn {characterEntry.displayName}");
        }
    }
    
    #endregion
    
    #region Public API
    
    /// <summary>
    /// Enable or disable drag functionality
    /// </summary>
    public void SetDragEnabled(bool enabled)
    {
        canDrag = enabled;
    }
    
    /// <summary>
    /// Update character entry (useful for dynamic UI)
    /// </summary>
    public void SetCharacterEntry(CharacterRegistry.CharacterEntry entry)
    {
        characterEntry = entry;
        canDrag = entry?.prefab != null;
    }
    
    #endregion
}
