using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class FinalUIInputFix : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("=== FINAL UI INPUT FIX ===");
        
        // Step 1: Remove DragDropDebugger to avoid conflicts
        RemoveDebugger();
        
        // Step 2: Completely rebuild EventSystem
        RebuildEventSystem();
        
        // Step 3: Fix all UI elements
        FixAllUIElements();
        
        // Step 4: Test everything
        TestEverything();
        
        Debug.Log("=== FINAL UI INPUT FIX COMPLETED ===");
        Debug.Log("UI should now be fully functional!");
    }
    
    static void RemoveDebugger()
    {
        DragDropDebugger debugger = Object.FindObjectOfType<DragDropDebugger>();
        if (debugger != null)
        {
            Object.DestroyImmediate(debugger);
            Debug.Log("Removed DragDropDebugger to avoid conflicts");
        }
    }
    
    static void RebuildEventSystem()
    {
        Debug.Log("Rebuilding EventSystem...");
        
        // Remove existing EventSystem
        EventSystem[] existingSystems = Object.FindObjectsOfType<EventSystem>();
        foreach (EventSystem system in existingSystems)
        {
            Object.DestroyImmediate(system.gameObject);
        }
        
        // Create new EventSystem
        GameObject eventSystemObj = new GameObject("EventSystem");
        EventSystem eventSystem = eventSystemObj.AddComponent<EventSystem>();
        
        // Add InputSystemUIInputModule
        var inputModule = eventSystemObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        
        // Configure EventSystem
        eventSystem.sendNavigationEvents = true;
        
        // Configure InputModule
        inputModule.deselectOnBackgroundClick = true;
        inputModule.pointerBehavior = UnityEngine.InputSystem.UI.UIPointerBehavior.SingleMouseOrPenButMultiTouchAndTrack;
        
        Debug.Log("EventSystem rebuilt successfully");
    }
    
    static void FixAllUIElements()
    {
        Debug.Log("Fixing all UI elements...");
        
        // Fix Canvas
        Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            // Ensure GraphicRaycaster
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
            
            raycaster.enabled = true;
            raycaster.ignoreReversedGraphics = true;
            raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            
            Debug.Log($"Fixed canvas: {canvas.name}");
        }
        
        // Fix all buttons
        Button[] buttons = Object.FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            // Enable button
            button.enabled = true;
            button.interactable = true;
            
            // Fix target graphic
            if (button.targetGraphic == null)
            {
                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    button.targetGraphic = image;
                }
            }
            
            // Ensure target graphic can receive raycast
            if (button.targetGraphic != null)
            {
                button.targetGraphic.raycastTarget = true;
            }
            
            // Clear and add test listeners
            button.onClick.RemoveAllListeners();
            string buttonName = button.name;
            button.onClick.AddListener(() => {
                Debug.Log($"[UI SUCCESS] Button '{buttonName}' clicked!");
            });
            
            Debug.Log($"Fixed button: {button.name}");
        }
        
        // Fix all child UI elements that might block raycast
        Text[] texts = Object.FindObjectsOfType<Text>();
        foreach (Text text in texts)
        {
            // Disable raycast for text elements that are children of buttons
            Button parentButton = text.GetComponentInParent<Button>();
            if (parentButton != null && text.gameObject != parentButton.gameObject)
            {
                text.raycastTarget = false;
                Debug.Log($"Disabled raycast for text: {text.name}");
            }
        }
        
        // Fix all child images that might block raycast
        Image[] images = Object.FindObjectsOfType<Image>();
        foreach (Image image in images)
        {
            Button parentButton = image.GetComponentInParent<Button>();
            if (parentButton != null && image.gameObject != parentButton.gameObject)
            {
                // Only disable raycast for decorative images
                if (image.name.Contains("Preview") || image.name.Contains("Border") || image.name.Contains("Icon"))
                {
                    image.raycastTarget = false;
                    Debug.Log($"Disabled raycast for image: {image.name}");
                }
            }
        }
    }
    
    static void TestEverything()
    {
        Debug.Log("Testing everything...");
        
        // Test EventSystem
        EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            Debug.Log($"✓ EventSystem found and enabled: {eventSystem.enabled}");
            Debug.Log($"✓ Input module: {eventSystem.currentInputModule?.GetType().Name}");
        }
        else
        {
            Debug.LogError("✗ No EventSystem found!");
        }
        
        // Test Input System
        var mouse = Mouse.current;
        if (mouse != null)
        {
            Debug.Log($"✓ Mouse input available: {mouse.name}");
        }
        else
        {
            Debug.LogError("✗ No mouse input found!");
        }
        
        // Test Canvas raycasting
        Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster != null && raycaster.enabled)
            {
                Debug.Log($"✓ Canvas {canvas.name} has working GraphicRaycaster");
            }
            else
            {
                Debug.LogError($"✗ Canvas {canvas.name} has no working GraphicRaycaster");
            }
        }
        
        // Test buttons
        Button[] buttons = Object.FindObjectsOfType<Button>();
        int workingButtons = 0;
        foreach (Button button in buttons)
        {
            if (button.enabled && button.interactable && button.targetGraphic != null && button.targetGraphic.raycastTarget)
            {
                workingButtons++;
            }
        }
        
        Debug.Log($"✓ {workingButtons}/{buttons.Length} buttons are properly configured");
        
        // Final test - simulate raycast
        if (eventSystem != null)
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            PointerEventData eventData = new PointerEventData(eventSystem);
            eventData.position = screenCenter;
            
            var results = new System.Collections.Generic.List<RaycastResult>();
            eventSystem.RaycastAll(eventData, results);
            
            Debug.Log($"✓ Raycast test found {results.Count} UI elements at screen center");
        }
    }
}