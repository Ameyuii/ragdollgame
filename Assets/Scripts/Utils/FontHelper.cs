using UnityEngine;
using System.Linq;

/// <summary>
/// Font Helper - Provides safe font loading with fallbacks
/// Handles Unity version compatibility for built-in fonts
/// </summary>
public static class FontHelper
{
    /// <summary>
    /// Get a safe built-in font with fallbacks
    /// </summary>
    public static Font GetSafeBuiltinFont()
    {
        // Try modern Unity built-in font first
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font != null)
        {
            Debug.Log("✅ Using LegacyRuntime.ttf font");
            return font;
        }
        
        // Try legacy Arial font (with exception handling)
        try
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (font != null)
            {
                Debug.Log("✅ Using Arial.ttf font (legacy)");
                return font;
            }
        }
        catch (System.ArgumentException)
        {
            Debug.Log("⚠️ Arial.ttf not available (deprecated)");
        }
        
        // Try other common built-in fonts
        string[] commonFonts = {
            "Arial.ttf",
            "LegacyRuntime.ttf", 
            "Arial Unicode MS.ttf",
            "Helvetica.ttf",
            "Times.ttf"
        };
        
        foreach (string fontName in commonFonts)
        {
            try
            {
                font = Resources.GetBuiltinResource<Font>(fontName);
                if (font != null)
                {
                    Debug.Log($"✅ Using {fontName} font");
                    return font;
                }
            }
            catch (System.ArgumentException)
            {
                // Font not available, continue to next
                continue;
            }
        }
        
        // Last resort: find any available font
        Font[] allFonts = Resources.FindObjectsOfTypeAll<Font>();
        font = allFonts.FirstOrDefault(f => f != null);
        
        if (font != null)
        {
            Debug.Log($"✅ Using fallback font: {font.name}");
            return font;
        }
        
        Debug.LogWarning("⚠️ No fonts available! Text may not render correctly.");
        return null!; // Fix warning CS8603
    }
    
    /// <summary>
    /// List all available fonts for debugging - Safe method without deprecated fonts
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void ListAvailableFonts()
    {
        Debug.Log("=== AVAILABLE FONTS ===");
        
        // Test only the known working font first
        Debug.Log("📋 Testing Safe Built-in Font:");
        try
        {
            Font legacyFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (legacyFont != null)
            {
                Debug.Log("  LegacyRuntime.ttf: ✅ Available");
            }
            else
            {
                Debug.Log("  LegacyRuntime.ttf: ❌ Not Available");
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log($"  LegacyRuntime.ttf: ❌ Failed to load - {ex.Message}");
        }
        
        // List all fonts actually available in project (safer approach)
        Font[] allFonts = Resources.FindObjectsOfTypeAll<Font>();
        Debug.Log($"📋 All Fonts in Project ({allFonts.Length}):");
        
        if (allFonts.Length > 0)
        {
            foreach (Font font in allFonts)
            {
                if (font != null)
                {
                    bool isBuiltIn = font.name.Contains("Legacy") || font.name.Contains("Arial") || 
                                   font.name.Contains("Default") || font.name.Contains("Runtime");
                    string fontType = isBuiltIn ? "[Built-in]" : "[Project]";
                    Debug.Log($"  {font.name} {fontType}");
                }
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No fonts found in project!");
        }
        
        // Test the safe font loading method
        Debug.Log("📋 Testing Safe Font Loading:");
        Font safeFont = GetSafeBuiltinFont();
        if (safeFont != null)
        {
            Debug.Log($"  Safe Font Result: ✅ {safeFont.name}");
        }
        else
        {
            Debug.LogError("  Safe Font Result: ❌ No safe font available!");
        }
    }
}
