using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CompleteInputSystemFix : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("=== COMPLETE INPUT SYSTEM FIX ===");
        
        // Step 1: Fix EventSystem
        FixEventSystem();
        
        // Step 2: Fix Canvas and GraphicRaycaster
        FixCanvasRaycasting();
        
        // Step 3: Fix Button configurations
        FixButtonConfigurations();
        
        // Step 4: Test input system
        TestInputSystem();
        
        Debug.Log("=== INPUT SYSTEM FIX COMPLETED ===");
        Debug.Log("UI should now be clickable!");
    }
    
    static void FixEventSystem()
    {
        Debug.Log("Fixing EventSystem...");
        
        EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<EventSystem>();
            Debug.Log("Created new EventSystem");
        }
        
        // Remove old input modules
        StandaloneInputModule[] oldModules = eventSystem.GetComponents<StandaloneInputModule>();
        foreach (var module in oldModules)
        {
            Object.DestroyImmediate(module);
            Debug.Log("Removed StandaloneInputModule");
        }
        
        // Ensure we have InputSystemUIInputModule
        var inputModule = eventSystem.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        if (inputModule == null)
        {
            inputModule = eventSystem.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            Debug.Log("Added InputSystemUIInputModule");
        }
        
        // Configure the input module
        inputModule.enabled = true;
        inputModule.deselectOnBackgroundClick = true;
        
        // Enable EventSystem
        eventSystem.enabled = true;
        
        Debug.Log($"EventSystem configured: {eventSystem.name}");
    }
    
    static void FixCanvasRaycasting()
    {
        Debug.Log("Fixing Canvas raycasting...");
        
        Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            // Ensure GraphicRaycaster exists
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = canvas.gameObject.AddComponent<GraphicRaycaster>();
                Debug.Log($"Added GraphicRaycaster to {canvas.name}");
            }
            
            // Configure GraphicRaycaster
            raycaster.enabled = true;
            raycaster.ignoreReversedGraphics = true;
            raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            
            Debug.Log($"Canvas {canvas.name} configured for raycasting");
        }
    }
    
    static void FixButtonConfigurations()
    {
        Debug.Log("Fixing button configurations...");
        
        Button[] buttons = Object.FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            // Enable button
            button.enabled = true;
            button.interactable = true;
            
            // Ensure target graphic is set
            if (button.targetGraphic == null)
            {
                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    button.targetGraphic = image;
                    Debug.Log($"Set target graphic for {button.name}");
                }
            }
            
            // Ensure target graphic can receive raycast
            if (button.targetGraphic != null)
            {
                button.targetGraphic.raycastTarget = true;
            }
            
            // Add test click listener if none exists
            if (button.onClick.GetPersistentEventCount() == 0)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => {
                    Debug.Log($"[UI CLICK] Button {button.name} clicked!");
                });
            }
            
            Debug.Log($"Button {button.name} configured");
        }
    }
    
    static void TestInputSystem()
    {
        Debug.Log("Testing input system...");
        
        // Test mouse input
        var mouse = Mouse.current;
        if (mouse != null)
        {
            Debug.Log($"Mouse device found: {mouse.name}");
            Debug.Log($"Mouse enabled: {mouse.enabled}");
        }
        else
        {
            Debug.LogError("No mouse device found!");
        }
        
        // Test EventSystem
        EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            Debug.Log($"EventSystem active: {eventSystem.enabled}");
            Debug.Log($"Current input module: {eventSystem.currentInputModule?.GetType().Name}");
        }
        
        // Test raycast at screen center
        if (eventSystem != null)
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            PointerEventData eventData = new PointerEventData(eventSystem);
            eventData.position = screenCenter;
            
            var results = new System.Collections.Generic.List<RaycastResult>();
            eventSystem.RaycastAll(eventData, results);
            
            Debug.Log($"Raycast test at screen center found {results.Count} UI elements");
            for (int i = 0; i < Mathf.Min(results.Count, 3); i++)
            {
                Debug.Log($"  {i}: {results[i].gameObject.name}");
            }
        }
    }
}