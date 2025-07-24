using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TestUIClick : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("=== TESTING UI CLICK FUNCTIONALITY ===");
        
        // Test each button
        Button[] buttons = Object.FindObjectsOfType<Button>();
        
        foreach (Button button in buttons)
        {
            TestButton(button);
        }
        
        // Test EventSystem raycast
        TestEventSystemRaycast();
        
        Debug.Log("=== UI CLICK TEST COMPLETED ===");
        Debug.Log("If you still can't click UI, try entering Play Mode first.");
    }
    
    static void TestButton(Button button)
    {
        Debug.Log($"Testing button: {button.name}");
        
        // Check if button is active and enabled
        if (!button.gameObject.activeInHierarchy)
        {
            Debug.LogError($"  ✗ Button {button.name} is not active in hierarchy");
            return;
        }
        
        if (!button.enabled)
        {
            Debug.LogError($"  ✗ Button {button.name} component is disabled");
            return;
        }
        
        if (!button.interactable)
        {
            Debug.LogError($"  ✗ Button {button.name} is not interactable");
            return;
        }
        
        // Check target graphic
        if (button.targetGraphic == null)
        {
            Debug.LogWarning($"  ⚠ Button {button.name} has no target graphic");
            
            // Try to set it
            Image image = button.GetComponent<Image>();
            if (image != null)
            {
                button.targetGraphic = image;
                Debug.Log($"    Set target graphic to Image component");
            }
        }
        else
        {
            Debug.Log($"  ✓ Button {button.name} has target graphic: {button.targetGraphic.name}");
        }
        
        // Check if target graphic can receive raycast
        if (button.targetGraphic != null)
        {
            Graphic graphic = button.targetGraphic;
            if (!graphic.raycastTarget)
            {
                Debug.LogWarning($"  ⚠ Target graphic {graphic.name} has raycastTarget disabled");
                graphic.raycastTarget = true;
                Debug.Log($"    Enabled raycastTarget for {graphic.name}");
            }
            else
            {
                Debug.Log($"  ✓ Target graphic can receive raycast");
            }
        }
        
        // Check Canvas and GraphicRaycaster
        Canvas canvas = button.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError($"  ✗ Button {button.name} is not under a Canvas");
            return;
        }
        
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            Debug.LogError($"  ✗ Canvas {canvas.name} has no GraphicRaycaster");
            return;
        }
        
        if (!raycaster.enabled)
        {
            Debug.LogError($"  ✗ GraphicRaycaster on {canvas.name} is disabled");
            raycaster.enabled = true;
            Debug.Log($"    Enabled GraphicRaycaster");
        }
        
        Debug.Log($"  ✓ Button {button.name} should be clickable");
        
        // Add test click listener if not already present
        if (button.onClick.GetPersistentEventCount() == 0)
        {
            button.onClick.AddListener(() => {
                Debug.Log($"Button {button.name} was clicked!");
            });
            Debug.Log($"    Added test click listener");
        }
    }
    
    static void TestEventSystemRaycast()
    {
        Debug.Log("Testing EventSystem raycast...");
        
        EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("  ✗ No EventSystem found");
            return;
        }
        
        if (!eventSystem.enabled)
        {
            Debug.LogError("  ✗ EventSystem is disabled");
            eventSystem.enabled = true;
            Debug.Log("    Enabled EventSystem");
        }
        
        // Check input module
        var inputModule = eventSystem.currentInputModule;
        if (inputModule == null)
        {
            Debug.LogError("  ✗ No input module found");
            return;
        }
        
        Debug.Log($"  ✓ Current input module: {inputModule.GetType().Name}");
        Debug.Log($"  ✓ Input module enabled: {inputModule.enabled}");
        
        // Test raycast at screen center
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = screenCenter;
        
        var results = new System.Collections.Generic.List<RaycastResult>();
        eventSystem.RaycastAll(eventData, results);
        
        Debug.Log($"  Raycast at screen center ({screenCenter}) found {results.Count} hits:");
        for (int i = 0; i < Mathf.Min(results.Count, 5); i++)
        {
            Debug.Log($"    {i}: {results[i].gameObject.name}");
        }
    }
}