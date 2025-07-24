using UnityEngine;
using UnityEngine.UI;

public class MakeHealthBarsVisibleInScene : MonoBehaviour
{
    public static void Execute()
    {
        // Find all health bars in the scene
        Canvas[] allCanvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        
        int updatedCount = 0;
        
        foreach (Canvas canvas in allCanvases)
        {
            // Check if this is a health bar canvas
            if (canvas.name == "HealthBar" && canvas.renderMode == RenderMode.WorldSpace)
            {
                MakeHealthBarVisibleInScene(canvas.gameObject);
                updatedCount++;
            }
        }
        
        Debug.Log($"Made {updatedCount} health bars visible in Scene View!");
    }
    
    static void MakeHealthBarVisibleInScene(GameObject healthBar)
    {
        // Temporarily increase scale for Scene View visibility
        healthBar.transform.localScale = Vector3.one * 0.1f; // Larger for Scene View
        
        // Update canvas scaler for better visibility
        CanvasScaler scaler = healthBar.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.scaleFactor = 0.001f; // Larger for Scene View
        }
        
        // Update canvas size
        RectTransform canvasRect = healthBar.GetComponent<RectTransform>();
        if (canvasRect != null)
        {
            canvasRect.sizeDelta = new Vector2(100, 20); // Larger for Scene View
        }
        
        // Update health bar background
        Transform bgTransform = healthBar.transform.Find("HealthBarBG");
        if (bgTransform != null)
        {
            RectTransform bgRect = bgTransform.GetComponent<RectTransform>();
            if (bgRect != null)
            {
                bgRect.sizeDelta = new Vector2(80, 8); // Larger for Scene View
                bgRect.anchorMin = new Vector2(0.5f, 0.5f);
                bgRect.anchorMax = new Vector2(0.5f, 0.5f);
                bgRect.anchoredPosition = Vector2.zero;
            }
            
            // Make background more visible
            Image bgImage = bgTransform.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f); // More opaque
            }
            
            // Update health bar fill
            Transform fillTransform = bgTransform.Find("HealthBarFill");
            if (fillTransform != null)
            {
                RectTransform fillRect = fillTransform.GetComponent<RectTransform>();
                if (fillRect != null)
                {
                    fillRect.sizeDelta = new Vector2(80, 8);
                    fillRect.anchorMin = new Vector2(0, 0);
                    fillRect.anchorMax = new Vector2(1, 1);
                    fillRect.anchoredPosition = Vector2.zero;
                }
                
                // Make fill more visible
                Image fillImage = fillTransform.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = new Color(0f, 1f, 0f, 1f); // Bright green
                }
            }
        }
        
        // Position above head
        Transform parentTransform = healthBar.transform.parent;
        if (parentTransform != null)
        {
            Transform headBone = FindHeadBone(parentTransform);
            if (headBone != null)
            {
                healthBar.transform.position = headBone.position + Vector3.up * 0.5f; // Higher for visibility
            }
            else
            {
                healthBar.transform.localPosition = Vector3.up * 2.8f; // Higher for visibility
            }
        }
        
        // Make sure canvas is set up for Scene View visibility
        Canvas canvas = healthBar.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1000;
        }
        
        Debug.Log($"Made health bar visible in Scene View for {healthBar.transform.parent.name}");
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