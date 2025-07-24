using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragDropSequenceTest : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("=== DRAG DROP SEQUENCE TEST ===");
        
        // Find all character drag sources and sort them by name
        CharacterDragSource[] allSources = Object.FindObjectsOfType<CharacterDragSource>();
        System.Array.Sort(allSources, (a, b) => a.name.CompareTo(b.name));
        
        Debug.Log($"Found {allSources.Length} drag sources:");
        for (int i = 0; i < allSources.Length; i++)
        {
            CharacterDragSource source = allSources[i];
            Debug.Log($"  {i}: {source.name} - isDragging: {source.isDragging}");
            
            // Check if this source has proper setup
            Button button = source.GetComponent<Button>();
            Image image = source.GetComponent<Image>();
            
            Debug.Log($"      Button: {(button != null ? $"interactable={button.interactable}" : "null")}");
            Debug.Log($"      Image: {(image != null ? $"raycastTarget={image.raycastTarget}" : "null")}");
            Debug.Log($"      CharacterPrefab: {(source.characterPrefab != null ? source.characterPrefab.name : "null")}");
            Debug.Log($"      GameManager: {(source.gameManager != null ? "found" : "null")}");
            
            if (source.gameManager != null)
            {
                Debug.Log($"      SetupMode: {source.gameManager.setupMode}");
                Debug.Log($"      SelectedTeam: {source.gameManager.selectedTeam}");
            }
        }
        
        // Test specific sequence: Character 1 -> Character 1 again
        CharacterDragSource character1 = null;
        foreach (CharacterDragSource source in allSources)
        {
            if (source.name.Contains("CharacterButton_1"))
            {
                character1 = source;
                break;
            }
        }
        
        if (character1 != null)
        {
            Debug.Log($"=== TESTING CHARACTER 1 SEQUENCE ===");
            Debug.Log($"Character 1 found: {character1.name}");
            
            // Simulate first drag attempt
            TestDragSequence(character1, "First drag attempt");
            
            // Wait a frame and test again
            TestDragSequence(character1, "Second drag attempt");
        }
        else
        {
            Debug.LogError("Could not find CharacterButton_1!");
        }
        
        Debug.Log("=== DRAG DROP SEQUENCE TEST COMPLETED ===");
    }
    
    static void TestDragSequence(CharacterDragSource source, string testName)
    {
        Debug.Log($"--- {testName} for {source.name} ---");
        
        // Check pre-conditions
        Debug.Log($"Pre-drag state:");
        Debug.Log($"  isDragging: {source.isDragging}");
        // Get currently dragging through reflection since it's private
        var field = typeof(CharacterDragSource).GetField("currentlyDragging", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        CharacterDragSource currentlyDragging = field?.GetValue(null) as CharacterDragSource;
        Debug.Log($"  currentlyDragging: {(currentlyDragging != null ? currentlyDragging.name : "null")}");
        
        // Check if button is clickable
        Button button = source.GetComponent<Button>();
        if (button != null)
        {
            Debug.Log($"  button.interactable: {button.interactable}");
            Debug.Log($"  button.enabled: {button.enabled}");
        }
        
        // Check if image can receive raycast
        Image image = source.GetComponent<Image>();
        if (image != null)
        {
            Debug.Log($"  image.raycastTarget: {image.raycastTarget}");
            Debug.Log($"  image.enabled: {image.enabled}");
        }
        
        // Check game manager state
        if (source.gameManager != null)
        {
            Debug.Log($"  gameManager.setupMode: {source.gameManager.setupMode}");
        }
        
        // Check if there are any blocking UI elements
        CheckForBlockingElements(source);
    }
    
    static void CheckForBlockingElements(CharacterDragSource source)
    {
        // Get the canvas
        Canvas canvas = source.GetComponentInParent<Canvas>();
        if (canvas == null) return;
        
        // Get the GraphicRaycaster
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null) return;
        
        // Get the center point of the source
        RectTransform rectTransform = source.GetComponent<RectTransform>();
        if (rectTransform == null) return;
        
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
        
        // Create pointer event data
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPoint;
        
        // Raycast
        var results = new System.Collections.Generic.List<RaycastResult>();
        raycaster.Raycast(eventData, results);
        
        Debug.Log($"Raycast results for {source.name} at {screenPoint}:");
        for (int i = 0; i < results.Count; i++)
        {
            RaycastResult result = results[i];
            Debug.Log($"  {i}: {result.gameObject.name} (distance: {result.distance})");
            
            if (i == 0 && result.gameObject != source.gameObject)
            {
                Debug.LogWarning($"  WARNING: First hit is not the source itself! It's {result.gameObject.name}");
            }
        }
    }
}