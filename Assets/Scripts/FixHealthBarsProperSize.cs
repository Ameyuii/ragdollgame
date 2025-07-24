using UnityEngine;
using UnityEngine.UI;

public class FixHealthBarsProperSize : MonoBehaviour
{
    public static void Execute()
    {
        // Find all health bars in the scene
        Canvas[] allCanvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        
        int fixedCount = 0;
        
        foreach (Canvas canvas in allCanvases)
        {
            // Check if this is a health bar canvas
            if (canvas.name == "HealthBar" && canvas.renderMode == RenderMode.WorldSpace)
            {
                FixHealthBarSize(canvas.gameObject);
                fixedCount++;
            }
        }
        
        Debug.Log($"Fixed {fixedCount} health bars to proper size!");
    }
    
    static void FixHealthBarSize(GameObject healthBar)
    {
        // Set very small but visible scale
        healthBar.transform.localScale = Vector3.one * 0.005f; // Much smaller
        
        // Set proper canvas scaler
        CanvasScaler scaler = healthBar.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.scaleFactor = 0.01f; // Reasonable size
        }
        
        // Set small canvas size
        RectTransform canvasRect = healthBar.GetComponent<RectTransform>();
        if (canvasRect != null)
        {
            canvasRect.sizeDelta = new Vector2(100, 20);
        }
        
        // Fix health bar background
        Transform bgTransform = healthBar.transform.Find("HealthBarBG");
        if (bgTransform != null)
        {
            RectTransform bgRect = bgTransform.GetComponent<RectTransform>();
            if (bgRect != null)
            {
                bgRect.sizeDelta = new Vector2(80, 10); // Small but visible
                bgRect.anchorMin = new Vector2(0.5f, 0.5f);
                bgRect.anchorMax = new Vector2(0.5f, 0.5f);
                bgRect.anchoredPosition = Vector2.zero;
            }
            
            // Set background color
            Image bgImage = bgTransform.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f); // Dark background
            }
            
            // Fix health bar fill
            Transform fillTransform = bgTransform.Find("HealthBarFill");
            if (fillTransform != null)
            {
                RectTransform fillRect = fillTransform.GetComponent<RectTransform>();
                if (fillRect != null)
                {
                    fillRect.sizeDelta = new Vector2(80, 10);
                    fillRect.anchorMin = new Vector2(0, 0);
                    fillRect.anchorMax = new Vector2(1, 1);
                    fillRect.anchoredPosition = Vector2.zero;
                }
                
                // Set fill color
                Image fillImage = fillTransform.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = new Color(0f, 0.8f, 0f, 1f); // Green but not too bright
                }
            }
        }
        
        // Position properly above head
        Transform parentTransform = healthBar.transform.parent;
        if (parentTransform != null)
        {
            Transform headBone = FindHeadBone(parentTransform);
            if (headBone != null)
            {
                healthBar.transform.position = headBone.position + Vector3.up * 0.2f;
            }
            else
            {
                healthBar.transform.localPosition = Vector3.up * 2.2f;
            }
        }
        
        // Set canvas properties
        Canvas canvas = healthBar.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = false; // Don't override sorting
            canvas.sortingOrder = 0; // Normal sorting order
        }
        
        Debug.Log($"Fixed health bar size for {healthBar.transform.parent.name}");
    }
    
    static Transform FindHeadBone(Transform parent)
    {
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