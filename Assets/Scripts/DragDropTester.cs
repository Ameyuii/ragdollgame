using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DragDropTester : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("=== DRAG DROP TEST STARTED ===");
        
        // Find all character drag sources
        CharacterDragSource[] dragSources = Object.FindObjectsOfType<CharacterDragSource>();
        Debug.Log($"Found {dragSources.Length} drag sources");
        
        // Test each drag source
        for (int i = 0; i < dragSources.Length; i++)
        {
            CharacterDragSource source = dragSources[i];
            Debug.Log($"Testing drag source {i}: {source.name}");
            Debug.Log($"  - isDragging: {source.isDragging}");
            Debug.Log($"  - characterPrefab: {(source.characterPrefab != null ? source.characterPrefab.name : "null")}");
            Debug.Log($"  - gameManager: {(source.gameManager != null ? "found" : "null")}");
            
            // Check if button is interactable
            Button button = source.GetComponent<Button>();
            if (button != null)
            {
                Debug.Log($"  - button interactable: {button.interactable}");
            }
            else
            {
                Debug.Log($"  - no button component found");
            }
        }
        
        // Check game manager state
        BattleGameManager gameManager = Object.FindObjectOfType<BattleGameManager>();
        if (gameManager != null)
        {
            Debug.Log($"Game Manager State:");
            Debug.Log($"  - setupMode: {gameManager.setupMode}");
            Debug.Log($"  - selectedTeam: {gameManager.selectedTeam}");
            Debug.Log($"  - gameStarted: {gameManager.gameStarted}");
        }
        else
        {
            Debug.Log("No BattleGameManager found");
        }
        
        // Force reset all drag states
        CharacterDragSource.ResetAllDragStates();
        Debug.Log("Force reset all drag states");
        
        Debug.Log("=== DRAG DROP TEST COMPLETED ===");
    }
}