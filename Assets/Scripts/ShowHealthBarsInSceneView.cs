using UnityEngine;
using UnityEngine.UI;

public class ShowHealthBarsInSceneView : MonoBehaviour
{
    public static void Execute()
    {
        // Find all RagdollCharacter objects
        RagdollCharacter[] characters = Object.FindObjectsByType<RagdollCharacter>(FindObjectsSortMode.None);
        
        foreach (RagdollCharacter character in characters)
        {
            // Create or update health bar for each character
            CreateVisibleHealthBar(character);
        }
        
        Debug.Log($"Created visible health bars for {characters.Length} characters!");
    }
    
    static void CreateVisibleHealthBar(RagdollCharacter character)
    {
        // Remove existing health bar if any
        Transform existingHealthBar = character.transform.Find("HealthBar");
        if (existingHealthBar != null)
        {
            Object.DestroyImmediate(existingHealthBar.gameObject);
        }
        
        // Create new health bar canvas
        GameObject healthBarPrefab = new GameObject("HealthBar");
        healthBarPrefab.transform.SetParent(character.transform);
        
        // Find head bone for positioning
        Transform headBone = FindHeadBone(character.transform);
        if (headBone != null)
        {
            healthBarPrefab.transform.position = headBone.position + Vector3.up * 0.3f;
        }
        else
        {
            healthBarPrefab.transform.localPosition = Vector3.up * 2.5f;
        }
        
        healthBarPrefab.transform.localScale = Vector3.one * 0.015f; // Ultra tiny
        
        Canvas canvas = healthBarPrefab.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        
        // Make sure it's visible in Scene View
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000; // High sorting order to be visible
        
        UnityEngine.UI.CanvasScaler scaler = healthBarPrefab.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.scaleFactor = 0.000005f; // Ultra tiny
        
        // Set ultra tiny canvas size
        RectTransform canvasRect = healthBarPrefab.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(50, 10);
        
        // Create health bar background
        GameObject healthBarBG = new GameObject("HealthBarBG");
        healthBarBG.transform.SetParent(healthBarPrefab.transform);
        healthBarBG.transform.localPosition = Vector3.zero;
        healthBarBG.transform.localScale = Vector3.one;
        
        UnityEngine.UI.Image bgImage = healthBarBG.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = new Color(0.0f, 0.0f, 0.0f, 0.8f); // Black background
        
        RectTransform bgRect = healthBarBG.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(40, 4); // Ultra tiny
        bgRect.anchorMin = new Vector2(0.5f, 0.5f);
        bgRect.anchorMax = new Vector2(0.5f, 0.5f);
        bgRect.anchoredPosition = Vector2.zero;
        
        // Create health bar fill
        GameObject healthBarFill = new GameObject("HealthBarFill");
        healthBarFill.transform.SetParent(healthBarBG.transform);
        healthBarFill.transform.localPosition = Vector3.zero;
        healthBarFill.transform.localScale = Vector3.one;
        
        UnityEngine.UI.Image fillImage = healthBarFill.AddComponent<UnityEngine.UI.Image>();
        fillImage.color = new Color(0f, 1f, 0f, 1f); // Pure bright green
        
        RectTransform fillRect = healthBarFill.GetComponent<RectTransform>();
        fillRect.sizeDelta = new Vector2(40, 4);
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(1, 1);
        fillRect.anchoredPosition = Vector2.zero;
        
        // Create slider component
        UnityEngine.UI.Slider healthSlider = healthBarBG.AddComponent<UnityEngine.UI.Slider>();
        healthSlider.fillRect = fillRect;
        healthSlider.value = 1f;
        healthSlider.maxValue = 1f;
        
        // Update the character's health bar reference
        var healthBarCanvasField = typeof(RagdollCharacter).GetField("healthBarCanvas", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (healthBarCanvasField != null)
        {
            healthBarCanvasField.SetValue(character, healthBarPrefab);
        }
        
        var healthSliderField = typeof(RagdollCharacter).GetField("healthSlider", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (healthSliderField != null)
        {
            healthSliderField.SetValue(character, healthSlider);
        }
        
        Debug.Log($"Created visible health bar for {character.name}");
    }
    
    static Transform FindHeadBone(Transform parent)
    {
        // Search for head bone in the hierarchy
        Transform[] allChildren = parent.GetComponentsInChildren<Transform>();
        
        foreach (Transform child in allChildren)
        {
            if (child.name.ToLower().Contains("head"))
            {
                return child;
            }
        }
        
        return null;
    }
}