using UnityEngine;
using UnityEngine.UI;

public class FixHealthBarsTiny : MonoBehaviour
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
                MakeHealthBarTiny(canvas.gameObject);
                fixedCount++;
            }
        }
        
        Debug.Log($"Made {fixedCount} health bars tiny!");
    }
    
    static void MakeHealthBarTiny(GameObject healthBar)
    {
        // Set extremely small scale factor
        CanvasScaler scaler = healthBar.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.scaleFactor = 0.0001f; // Cực kỳ nhỏ
        }
        
        // Set tiny canvas size
        RectTransform canvasRect = healthBar.GetComponent<RectTransform>();
        if (canvasRect != null)
        {
            canvasRect.sizeDelta = new Vector2(100, 20); // Nhỏ hơn nữa
        }
        
        // Fix health bar background
        Transform bgTransform = healthBar.transform.Find("HealthBarBG");
        if (bgTransform != null)
        {
            RectTransform bgRect = bgTransform.GetComponent<RectTransform>();
            if (bgRect != null)
            {
                bgRect.sizeDelta = new Vector2(80, 8); // Rất nhỏ
                bgRect.anchorMin = new Vector2(0.5f, 0.5f);
                bgRect.anchorMax = new Vector2(0.5f, 0.5f);
                bgRect.anchoredPosition = Vector2.zero;
            }
            
            // Fix background color - make it more transparent
            Image bgImage = bgTransform.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.6f);
            }
            
            // Fix health bar fill
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
                
                // Make fill color brighter to compensate for small size
                Image fillImage = fillTransform.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = new Color(0.2f, 1f, 0.2f, 1f); // Bright green
                }
            }
        }
        
        // Position closer to head
        healthBar.transform.localPosition = Vector3.up * 2.1f;
        
        // Set overall scale to be tiny
        healthBar.transform.localScale = Vector3.one * 0.3f; // Scale down to 30%
        
        Debug.Log($"Made health bar tiny for {healthBar.transform.parent.name}");
    }
}