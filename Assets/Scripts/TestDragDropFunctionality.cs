using UnityEngine;
using UnityEngine.UI;

public class TestDragDropFunctionality : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("=== TESTING DRAG DROP FUNCTIONALITY ===");
        
        // Find Character Button 1
        CharacterDragSource[] allSources = Object.FindObjectsOfType<CharacterDragSource>();
        CharacterDragSource character1 = null;
        
        foreach (CharacterDragSource source in allSources)
        {
            if (source.name.Contains("CharacterButton_1"))
            {
                character1 = source;
                break;
            }
        }
        
        if (character1 == null)
        {
            Debug.LogError("Could not find CharacterButton_1!");
            return;
        }
        
        Debug.Log($"Testing drag functionality for {character1.name}");
        
        // Test 1: Check if button can be clicked
        Button button = character1.GetComponent<Button>();
        if (button != null && button.interactable)
        {
            Debug.Log("✓ Button is interactable");
        }
        else
        {
            Debug.LogError("✗ Button is not interactable");
        }
        
        // Test 2: Check if image can receive raycast
        Image image = character1.GetComponent<Image>();
        if (image != null && image.raycastTarget)
        {
            Debug.Log("✓ Image can receive raycast");
        }
        else
        {
            Debug.LogError("✗ Image cannot receive raycast");
        }
        
        // Test 3: Check if character prefab is assigned
        if (character1.characterPrefab != null)
        {
            Debug.Log($"✓ Character prefab assigned: {character1.characterPrefab.name}");
        }
        else
        {
            Debug.LogError("✗ Character prefab not assigned");
        }
        
        // Test 4: Check if game manager is assigned and in setup mode
        if (character1.gameManager != null)
        {
            Debug.Log($"✓ Game manager assigned");
            if (character1.gameManager.setupMode)
            {
                Debug.Log("✓ Game is in setup mode");
            }
            else
            {
                Debug.LogWarning("⚠ Game is not in setup mode");
            }
        }
        else
        {
            Debug.LogError("✗ Game manager not assigned");
        }
        
        // Test 5: Check if drag state is clean
        if (!character1.isDragging)
        {
            Debug.Log("✓ Character is not currently dragging");
        }
        else
        {
            Debug.LogWarning("⚠ Character is currently in dragging state");
        }
        
        // Test 6: Check child elements raycast settings
        Text[] childTexts = character1.GetComponentsInChildren<Text>();
        bool allTextsNonRaycast = true;
        foreach (Text text in childTexts)
        {
            if (text.gameObject != character1.gameObject && text.raycastTarget)
            {
                allTextsNonRaycast = false;
                Debug.LogWarning($"⚠ Child text {text.name} still has raycastTarget enabled");
            }
        }
        
        if (allTextsNonRaycast)
        {
            Debug.Log("✓ All child text elements have raycastTarget disabled");
        }
        
        // Test 7: Simulate drag events (without actually dragging)
        Debug.Log("Simulating drag event sequence...");
        
        // Check if OnBeginDrag would succeed
        bool canBeginDrag = (character1.characterPrefab != null && 
                           character1.gameManager != null && 
                           character1.gameManager.setupMode && 
                           !character1.isDragging);
        
        if (canBeginDrag)
        {
            Debug.Log("✓ OnBeginDrag conditions are met");
        }
        else
        {
            Debug.LogError("✗ OnBeginDrag conditions are not met");
        }
        
        Debug.Log("=== DRAG DROP FUNCTIONALITY TEST COMPLETED ===");
        
        // Summary
        Debug.Log("=== TEST SUMMARY ===");
        Debug.Log("If all tests show ✓, then drag and drop should work correctly.");
        Debug.Log("Try dragging CharacterButton_1 to the map now!");
    }
}