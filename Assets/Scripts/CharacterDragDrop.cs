using UnityEngine;

public class CharacterDragDrop : MonoBehaviour
{
    [Header("Drag & Drop Settings")]
    public LayerMask groundLayerMask = -1; // Layer mask cho mặt đất
    public float raycastDistance = 100f; // Khoảng cách raycast
    public float groundOffset = 0.1f; // Khoảng cách offset từ mặt đất
    
    [Header("Visual Feedback")]
    public Material? validDropMaterial; // Material khi có thể thả
    public Material? invalidDropMaterial; // Material khi không thể thả
    
    private Camera? mainCamera;
    private bool isDragging = false;
    private Vector3 dragOffset;
    private Renderer[]? characterRenderers;
    private Material[]? originalMaterials;
    private Collider? characterCollider;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindObjectOfType<Camera>();
            
        characterCollider = GetComponent<Collider>();
        
        // Lưu trữ các renderer và material gốc
        characterRenderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[characterRenderers.Length];
        for (int i = 0; i < characterRenderers.Length; i++)
        {
            originalMaterials[i] = characterRenderers[i].material;
        }
    }
    
    void OnMouseDown()
    {
        if (mainCamera == null) return;
        
        isDragging = true;
        
        // Tính toán offset giữa vị trí chuột và vị trí nhân vật
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        dragOffset = transform.position - mouseWorldPos;
        
        // Tắt collider để tránh raycast hit chính nó
        if (characterCollider != null)
            characterCollider.enabled = false;
    }
    
    void OnMouseDrag()
    {
        if (!isDragging || mainCamera == null) return;
        
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 targetPosition = mouseWorldPos + dragOffset;
        
        // Raycast xuống để tìm mặt đất
        Vector3 groundPosition = FindGroundPosition(targetPosition);
        
        if (groundPosition != Vector3.zero)
        {
            // Đặt nhân vật trên mặt đất
            transform.position = groundPosition + Vector3.up * groundOffset;
            SetMaterialFeedback(true);
        }
        else
        {
            // Không tìm thấy mặt đất, giữ nguyên vị trí cũ hoặc di chuyển theo chuột
            transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            SetMaterialFeedback(false);
        }
    }
    
    void OnMouseUp()
    {
        if (!isDragging) return;
        
        isDragging = false;
        
        // Kiểm tra vị trí cuối cùng
        Vector3 finalGroundPosition = FindGroundPosition(transform.position);
        if (finalGroundPosition != Vector3.zero)
        {
            transform.position = finalGroundPosition + Vector3.up * groundOffset;
        }
        
        // Khôi phục collider
        if (characterCollider != null)
            characterCollider.enabled = true;
            
        // Khôi phục material gốc
        RestoreOriginalMaterials();
    }
    
    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mouseScreenPos);
    }
    
    Vector3 FindGroundPosition(Vector3 position)
    {
        // Raycast từ trên xuống để tìm mặt đất
        Vector3 rayStart = new Vector3(position.x, position.y + 50f, position.z);
        RaycastHit hit;
        
        if (Physics.Raycast(rayStart, Vector3.down, out hit, raycastDistance, groundLayerMask))
        {
            return hit.point;
        }
        
        // Nếu không tìm thấy từ trên xuống, thử raycast từ vị trí hiện tại xuống
        if (Physics.Raycast(position, Vector3.down, out hit, raycastDistance, groundLayerMask))
        {
            return hit.point;
        }
        
        return Vector3.zero; // Không tìm thấy mặt đất
    }
    
    void SetMaterialFeedback(bool isValidDrop)
    {
        if (validDropMaterial == null || invalidDropMaterial == null) return;
        
        Material feedbackMaterial = isValidDrop ? validDropMaterial : invalidDropMaterial;
        
        foreach (Renderer renderer in characterRenderers)
        {
            renderer.material = feedbackMaterial;
        }
    }
    
    void RestoreOriginalMaterials()
    {
        for (int i = 0; i < characterRenderers.Length; i++)
        {
            if (characterRenderers[i] != null && originalMaterials[i] != null)
            {
                characterRenderers[i].material = originalMaterials[i];
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Vẽ gizmo để debug raycast
        Gizmos.color = Color.red;
        Vector3 rayStart = transform.position + Vector3.up * 50f;
        Gizmos.DrawLine(rayStart, rayStart + Vector3.down * raycastDistance);
    }
}