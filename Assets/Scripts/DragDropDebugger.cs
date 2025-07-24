using UnityEngine;
using UnityEngine.UI;

public class DragDropDebugger : MonoBehaviour
{
    [Header("Debug Info")]
    public Text debugText;
    
    void Start()
    {
        if (debugText == null)
        {
            // Create debug text if not assigned
            GameObject canvas = GameObject.Find("UI Canvas");
            if (canvas != null)
            {
                GameObject debugObj = new GameObject("DebugText");
                debugObj.transform.SetParent(canvas.transform, false);
                
                RectTransform rect = debugObj.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(1, 0.2f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                
                debugText = debugObj.AddComponent<Text>();
                debugText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                debugText.fontSize = 14;
                debugText.color = Color.white;
                debugText.alignment = TextAnchor.UpperLeft;
            }
        }
    }
    
    void Update()
    {
        if (debugText != null)
        {
            string debugInfo = "=== DRAG DROP DEBUG ===\n";
            
            // Check all CharacterDragSource components
            CharacterDragSource[] dragSources = FindObjectsOfType<CharacterDragSource>();
            debugInfo += $"Total Drag Sources: {dragSources.Length}\n";
            
            int draggingCount = 0;
            for (int i = 0; i < dragSources.Length; i++)
            {
                if (dragSources[i].isDragging)
                {
                    draggingCount++;
                    debugInfo += $"Source {i}: DRAGGING ({dragSources[i].name})\n";
                }
            }
            
            debugInfo += $"Currently Dragging: {draggingCount}\n";
            
            // Check mouse state using Input System
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (mouse != null)
            {
                debugInfo += $"Mouse Button 0: {mouse.leftButton.isPressed}\n";
                debugInfo += $"Mouse Position: {mouse.position.ReadValue()}\n";
            }
            else
            {
                debugInfo += $"Mouse: Not found\n";
            }
            
            // Check game manager state
            BattleGameManager gameManager = FindObjectOfType<BattleGameManager>();
            if (gameManager != null)
            {
                debugInfo += $"Setup Mode: {gameManager.setupMode}\n";
                debugInfo += $"Selected Team: {gameManager.selectedTeam}\n";
            }
            
            debugText.text = debugInfo;
        }
    }
    
    [ContextMenu("Force Reset All Drag States")]
    public void ForceResetAllDragStates()
    {
        CharacterDragSource.ResetAllDragStates();
        Debug.Log("Manually reset all drag states");
    }
    
    [ContextMenu("List All Drag Sources")]
    public void ListAllDragSources()
    {
        CharacterDragSource[] dragSources = FindObjectsOfType<CharacterDragSource>();
        Debug.Log($"Found {dragSources.Length} drag sources:");
        
        for (int i = 0; i < dragSources.Length; i++)
        {
            Debug.Log($"  {i}: {dragSources[i].name} - isDragging: {dragSources[i].isDragging}");
        }
    }
}