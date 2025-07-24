using UnityEngine;

public class AssignDragDropMaterials : MonoBehaviour
{
    public static void Execute()
    {
        // Load materials
        Material validMaterial = Resources.Load<Material>("Materials/ValidDropMaterial");
        Material invalidMaterial = Resources.Load<Material>("Materials/InvalidDropMaterial");
        
        // Nếu không load được từ Resources, thử load trực tiếp
        if (validMaterial == null)
        {
            validMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ValidDropMaterial.mat");
        }
        if (invalidMaterial == null)
        {
            invalidMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/InvalidDropMaterial.mat");
        }
        
        // Tìm tất cả các CharacterDragDrop components
        CharacterDragDrop[] dragDropComponents = FindObjectsOfType<CharacterDragDrop>();
        
        foreach (CharacterDragDrop dragDrop in dragDropComponents)
        {
            // Gán materials
            dragDrop.validDropMaterial = validMaterial;
            dragDrop.invalidDropMaterial = invalidMaterial;
            
            // Thiết lập ground layer mask (layer 8 = Ground)
            dragDrop.groundLayerMask = 1 << 8;
            
            Debug.Log($"Assigned materials to {dragDrop.gameObject.name}");
        }
        
        Debug.Log($"Assigned drag drop materials to {dragDropComponents.Length} characters");
    }
}