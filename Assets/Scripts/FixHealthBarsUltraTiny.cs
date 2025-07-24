using UnityEngine;
using UnityEngine.UI;

public class FixHealthBarsUltraTiny : MonoBehaviour
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
                MakeHealthBarUltraTiny(canvas.gameObject);
                fixedCount++;
            }
        }
        
        Debug.Log($"Made {fixedCount} health bars ultra tiny!");
    }
    
    static void MakeHealthBarUltraTiny(GameObject healthBar)
    {
        // Set extremely tiny scale factor - 20 times smaller
        CanvasScaler scaler = healthBar.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.scaleFactor = 0.000005f; // 20 times smaller than 0.0001f
        }
        
        // Set ultra tiny canvas size
        RectTransform canvasRect = healthBar.GetComponent<RectTransform>();
        if (canvasRect != null)
        {
            canvasRect.sizeDelta = new Vector2(50, 10); // Much smaller
        }
        
        // Fix health bar background
        Transform bgTransform = healthBar.transform.Find("HealthBarBG");
        if (bgTransform != null)
        {
            RectTransform bgRect = bgTransform.GetComponent<RectTransform>();
            if (bgRect != null)
            {
                bgRect.sizeDelta = new Vector2(40, 4); // Ultra tiny
                bgRect.anchorMin = new Vector2(0.5f, 0.5f);
                bgRect.anchorMax = new Vector2(0.5f, 0.5f);
                bgRect.anchoredPosition = Vector2.zero;
            }
            
            // Fix background color - make it more visible
            Image bgImage = bgTransform.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = new Color(0.0f, 0.0f, 0.0f, 0.8f); // Black background
            }
            
            // Fix health bar fill
            Transform fillTransform = bgTransform.Find("HealthBarFill");
            if (fillTransform != null)
            {
                RectTransform fillRect = fillTransform.GetComponent<RectTransform>();
                if (fillRect != null)
                {
                    fillRect.sizeDelta = new Vector2(40, 4);
                    fillRect.anchorMin = new Vector2(0, 0);
                    fillRect.anchorMax = new Vector2(1, 1);
                    fillRect.anchoredPosition = Vector2.zero;
                }
                
                // Make fill color very bright to compensate for tiny size
                Image fillImage = fillTransform.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = new Color(0f, 1f, 0f, 1f); // Pure bright green
                }
            }
        }
        
        // Set overall scale to be ultra tiny - 20 times smaller
        healthBar.transform.localScale = Vector3.one * 0.015f; // 20 times smaller than 0.3f
        
        // Position above head - find the head bone
        Transform parentTransform = healthBar.transform.parent;
        if (parentTransform != null)
        {
            // Try to find head bone
            Transform headBone = FindHeadBone(parentTransform);
            if (headBone != null)
            {
                // Position relative to head bone
                healthBar.transform.position = headBone.position + Vector3.up * 0.3f;
            }
            else
            {
                // Fallback to character position + offset
                healthBar.transform.localPosition = Vector3.up * 2.5f;
            }
        }
        
        Debug.Log($"Made health bar ultra tiny for {healthBar.transform.parent.name}");
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