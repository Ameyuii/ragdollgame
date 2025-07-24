using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class FixInputSystem : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("=== FIXING INPUT SYSTEM ===");
        
        // Check EventSystem
        EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("No EventSystem found! Creating one...");
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystem = eventSystemObj.AddComponent<EventSystem>();
        }
        
        Debug.Log($"EventSystem found: {eventSystem.name}");
        Debug.Log($"EventSystem enabled: {eventSystem.enabled}");
        
        // Check input modules
        var inputModules = eventSystem.GetComponents<BaseInputModule>();
        Debug.Log($"Found {inputModules.Length} input modules:");
        
        foreach (var module in inputModules)
        {
            Debug.Log($"  - {module.GetType().Name}: enabled={module.enabled}");
        }
        
        // Remove conflicting modules
        var standaloneModules = eventSystem.GetComponents<StandaloneInputModule>();
        foreach (var module in standaloneModules)
        {
            Debug.Log($"Removing StandaloneInputModule: {module.name}");
            Object.DestroyImmediate(module);
        }
        
        // Ensure we have InputSystemUIInputModule
        var inputSystemModule = eventSystem.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        if (inputSystemModule == null)
        {
            Debug.Log("Adding InputSystemUIInputModule...");
            inputSystemModule = eventSystem.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }
        
        inputSystemModule.enabled = true;
        Debug.Log($"InputSystemUIInputModule enabled: {inputSystemModule.enabled}");
        
        // Check Canvas and GraphicRaycaster
        Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
        Debug.Log($"Found {canvases.Length} canvases:");
        
        foreach (Canvas canvas in canvases)
        {
            Debug.Log($"  Canvas: {canvas.name}");
            
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                Debug.Log($"    Adding GraphicRaycaster to {canvas.name}");
                raycaster = canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
            
            raycaster.enabled = true;
            Debug.Log($"    GraphicRaycaster enabled: {raycaster.enabled}");
        }
        
        // Test button interactivity
        Button[] buttons = Object.FindObjectsOfType<Button>();
        Debug.Log($"Found {buttons.Length} buttons:");
        
        foreach (Button button in buttons)
        {
            Debug.Log($"  Button: {button.name} - interactable: {button.interactable}");
            
            // Ensure button is interactable
            button.interactable = true;
            
            // Check if button has proper target graphic
            if (button.targetGraphic == null)
            {
                Image image = button.GetComponent<Image>();
                if (image != null)
                {
                    button.targetGraphic = image;
                    Debug.Log($"    Set targetGraphic for {button.name}");
                }
            }
        }
        
        // Test mouse input
        Debug.Log("Testing mouse input...");
        var mouse = Mouse.current;
        if (mouse != null)
        {
            Debug.Log($"Mouse found: {mouse.name}");
            Debug.Log($"Mouse position: {mouse.position.ReadValue()}");
            Debug.Log($"Left button pressed: {mouse.leftButton.isPressed}");
        }
        else
        {
            Debug.LogError("No mouse input device found!");
        }
        
        // Force refresh EventSystem
        eventSystem.enabled = false;
        eventSystem.enabled = true;
        
        Debug.Log("=== INPUT SYSTEM FIX COMPLETED ===");
        Debug.Log("Try clicking UI elements now!");
    }
}