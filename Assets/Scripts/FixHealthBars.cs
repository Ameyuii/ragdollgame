using UnityEngine;
using UnityEngine.UI;

public class FixHealthBars : MonoBehaviour
{
    public static void Execute()
    {
        // Find all RagdollCharacter objects
        RagdollCharacter[] characters = Object.FindObjectsByType<RagdollCharacter>(FindObjectsSortMode.None);
        
        foreach (RagdollCharacter character in characters)
        {
            // Find health bar canvas
            Transform healthBarTransform = character.transform.Find("HealthBar");
            if (healthBarTransform != null)
            {
                FixHealthBar(healthBarTransform.gameObject);
            }
        }
        
        Debug.Log($"Fixed {characters.Length} health bars!");
    }
    
    static void FixHealthBar(GameObject healthBar)
    {
        // Fix canvas scaler
        CanvasScaler scaler = healthBar.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.scaleFactor = 0.001f; // Much smaller scale
        }
        
        // Fix canvas size
        RectTransform canvasRect = healthBar.GetComponent<RectTransform>();
        if (canvasRect != null)
        {
            canvasRect.sizeDelta = new Vector2(200, 40);
        }
        
        // Fix health bar background
        Transform bgTransform = healthBar.transform.Find("HealthBarBG");
        if (bgTransform != null)
        {
            RectTransform bgRect = bgTransform.GetComponent<RectTransform>();
            if (bgRect != null)
            {
                bgRect.sizeDelta = new Vector2(180, 20);
                bgRect.anchorMin = new Vector2(0.5f, 0.5f);
                bgRect.anchorMax = new Vector2(0.5f, 0.5f);
                bgRect.anchoredPosition = Vector2.zero;
            }
            
            // Fix background color
            Image bgImage = bgTransform.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            }
            
            // Fix health bar fill
            Transform fillTransform = bgTransform.Find("HealthBarFill");
            if (fillTransform != null)
            {
                RectTransform fillRect = fillTransform.GetComponent<RectTransform>();
                if (fillRect != null)
                {
                    fillRect.sizeDelta = new Vector2(180, 20);
                    fillRect.anchorMin = new Vector2(0, 0);
                    fillRect.anchorMax = new Vector2(1, 1);
                    fillRect.anchoredPosition = Vector2.zero;
                }
            }
        }
        
        // Adjust position
        healthBar.transform.localPosition = Vector3.up * 2.2f;
        
        Debug.Log($"Fixed health bar for {healthBar.transform.parent.name}");
    }
}