using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragDropFixer : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("=== FIXING DRAG DROP ISSUES ===");
        
        // Find all character drag sources
        CharacterDragSource[] dragSources = Object.FindObjectsOfType<CharacterDragSource>();
        
        foreach (CharacterDragSource source in dragSources)
        {
            // Ensure each drag source has proper event system setup
            FixDragSource(source);
        }
        
        // Ensure EventSystem exists and is properly configured
        EnsureEventSystem();
        
        Debug.Log("=== DRAG DROP FIXES APPLIED ===");
    }
    
    static void FixDragSource(CharacterDragSource source)
    {
        // Ensure the GameObject has a GraphicRaycaster
        Canvas canvas = source.GetComponentInParent<Canvas>();
        if (canvas != null && canvas.GetComponent<GraphicRaycaster>() == null)
        {
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            Debug.Log($"Added GraphicRaycaster to canvas for {source.name}");
        }
        
        // Ensure the button is properly configured
        Button button = source.GetComponent<Button>();
        if (button != null)
        {
            button.interactable = true;
            
            // Ensure the button has proper transition settings
            if (button.transition == Selectable.Transition.None)
            {
                button.transition = Selectable.Transition.ColorTint;
            }
        }
        
        // Ensure the Image component exists and is raycast target
        Image image = source.GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = true;
        }
        
        // Reset any stuck drag state
        if (source.isDragging)
        {
            Debug.Log($"Resetting stuck drag state for {source.name}");
            // Use reflection to access private ForceEndDrag method
            var method = typeof(CharacterDragSource).GetMethod("ForceEndDrag", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(source, null);
        }
    }
    
    static void EnsureEventSystem()
    {
        EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("Created new EventSystem");
        }
        else
        {
            // Ensure the EventSystem is enabled
            eventSystem.enabled = true;
            
            // Ensure it has a StandaloneInputModule
            if (eventSystem.GetComponent<StandaloneInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<StandaloneInputModule>();
                Debug.Log("Added StandaloneInputModule to existing EventSystem");
            }
        }
    }
}