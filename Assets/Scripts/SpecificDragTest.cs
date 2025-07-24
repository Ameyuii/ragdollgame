using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SpecificDragTest : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("=== SPECIFIC DRAG TEST FOR CHARACTER 1 ISSUE ===");
        
        // Find Character Button 1 specifically
        CharacterDragSource[] allSources = Object.FindObjectsOfType<CharacterDragSource>();
        CharacterDragSource character1Source = null;
        
        foreach (CharacterDragSource source in allSources)
        {
            if (source.name.Contains("CharacterButton_1"))
            {
                character1Source = source;
                break;
            }
        }
        
        if (character1Source == null)
        {
            Debug.LogError("Could not find CharacterButton_1!");
            return;
        }
        
        Debug.Log($"Found Character Button 1: {character1Source.name}");
        Debug.Log($"  - isDragging: {character1Source.isDragging}");
        Debug.Log($"  - GameObject active: {character1Source.gameObject.activeInHierarchy}");
        Debug.Log($"  - Component enabled: {character1Source.enabled}");
        
        // Check button component
        Button button = character1Source.GetComponent<Button>();
        if (button != null)
        {
            Debug.Log($"  - Button interactable: {button.interactable}");
            Debug.Log($"  - Button enabled: {button.enabled}");
        }
        
        // Check Image component
        Image image = character1Source.GetComponent<Image>();
        if (image != null)
        {
            Debug.Log($"  - Image raycastTarget: {image.raycastTarget}");
            Debug.Log($"  - Image enabled: {image.enabled}");
        }
        
        // Check RectTransform
        RectTransform rectTransform = character1Source.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Debug.Log($"  - RectTransform position: {rectTransform.anchoredPosition}");
            Debug.Log($"  - RectTransform size: {rectTransform.sizeDelta}");
        }
        
        // Check if there are any overlapping UI elements
        CheckForOverlappingElements(character1Source);
        
        // Force reset this specific drag source
        if (character1Source.isDragging)
        {
            Debug.Log("Force resetting Character Button 1 drag state");
            var method = typeof(CharacterDragSource).GetMethod("ForceEndDrag", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(character1Source, null);
        }
        
        Debug.Log("=== SPECIFIC DRAG TEST COMPLETED ===");
    }
    
    static void CheckForOverlappingElements(CharacterDragSource targetSource)
    {
        RectTransform targetRect = targetSource.GetComponent<RectTransform>();
        if (targetRect == null) return;
        
        // Get all UI elements in the same parent
        Transform parent = targetSource.transform.parent;
        if (parent == null) return;
        
        Debug.Log($"Checking for overlapping elements with {targetSource.name}:");
        
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child == targetSource.transform) continue;
            
            RectTransform childRect = child.GetComponent<RectTransform>();
            if (childRect == null) continue;
            
            // Check if rects overlap
            Rect targetBounds = GetWorldRect(targetRect);
            Rect childBounds = GetWorldRect(childRect);
            
            if (targetBounds.Overlaps(childBounds))
            {
                Debug.LogWarning($"  - OVERLAP DETECTED with {child.name}");
                Debug.LogWarning($"    Target bounds: {targetBounds}");
                Debug.LogWarning($"    Child bounds: {childBounds}");
            }
        }
    }
    
    static Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        
        float xMin = corners[0].x;
        float xMax = corners[2].x;
        float yMin = corners[0].y;
        float yMax = corners[2].y;
        
        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }
}