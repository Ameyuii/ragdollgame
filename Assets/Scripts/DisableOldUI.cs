using UnityEngine;

public class DisableOldUI
{
    public static void Execute()
    {
        // Find and disable the old setup panel
        GameObject setupPanel = GameObject.Find("UI Canvas/SetupPanel");
        if (setupPanel != null)
        {
            setupPanel.SetActive(false);
            Debug.Log("Disabled old SetupPanel");
        }
        
        // Also disable the old GameUI Panel to make room for new UI
        GameObject gameUIPanel = GameObject.Find("UI Canvas/GameUI Panel");
        if (gameUIPanel != null)
        {
            // Move it to top right corner
            RectTransform rect = gameUIPanel.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchorMin = new Vector2(0.75f, 0.75f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.offsetMin = new Vector2(-10, -10);
                rect.offsetMax = new Vector2(-10, -10);
            }
            Debug.Log("Moved GameUI Panel to top right");
        }
    }
}