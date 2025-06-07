using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AnimalRevolt.Utilities
{
    /// <summary>
    /// Input System Fixer - Tự động sửa cài đặt Input System để tránh conflicts
    /// </summary>
    public class InputSystemFixer : MonoBehaviour
    {
        [Header("Auto Fix Settings")]
        [SerializeField] private bool autoFixOnStart = true;
        [SerializeField] private bool showDebugLogs = true;
        
        private void Start()
        {
            if (autoFixOnStart)
            {
                FixInputSystemSettings();
            }
        }
        
        /// <summary>
        /// Fix Input System settings to use Legacy Input
        /// </summary>
        [ContextMenu("Fix Input System Settings")]
        public void FixInputSystemSettings()
        {
            #if UNITY_EDITOR
            try
            {
                // Set active input handling to Legacy Input Manager
                var playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
                if (playerSettings.Length > 0)
                {
                    // Use reflection to set the activeInputHandling property
                    var property = typeof(PlayerSettings).GetProperty("activeInputHandling");
                    if (property != null)
                    {
                        // 0 = Input Manager (Legacy), 1 = Input System Package, 2 = Both
                        property.SetValue(null, 0);
                        
                        if (showDebugLogs)
                        {
                            Debug.Log("[InputSystemFixer] Successfully set Input System to Legacy Input Manager");
                            Debug.Log("[InputSystemFixer] Unity may need to restart to apply changes");
                        }
                        
                        // Mark project settings as dirty
                        EditorUtility.SetDirty(playerSettings[0]);
                        AssetDatabase.SaveAssets();
                    }
                    else
                    {
                        if (showDebugLogs)
                        {
                            Debug.LogWarning("[InputSystemFixer] Could not find activeInputHandling property");
                            Debug.LogWarning("[InputSystemFixer] This might be due to Unity version compatibility");
                        }
                    }
                }
                else
                {
                    if (showDebugLogs)
                    {
                        Debug.LogWarning("[InputSystemFixer] Could not find PlayerSettings");
                    }
                }
            }
            catch (System.Exception e)
            {
                if (showDebugLogs)
                {
                    Debug.LogError($"[InputSystemFixer] Error fixing Input System settings: {e.Message}");
                    Debug.LogError("[InputSystemFixer] Stack trace: " + e.StackTrace);
                    Debug.LogError("[InputSystemFixer] Please try manual fix using Edit > Project Settings > XR Plug-in Management > Input System Package");
                }
            }
            #else
            if (showDebugLogs)
            {
                Debug.Log("[InputSystemFixer] Input System fix is only available in Editor mode");
            }
            #endif
        }
        
        /// <summary>
        /// Check current Input System settings
        /// </summary>
        [ContextMenu("Check Input System Settings")]
        public void CheckInputSystemSettings()
        {
            #if UNITY_EDITOR
            try
            {
                var playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
                if (playerSettings.Length > 0)
                {
                    var property = typeof(PlayerSettings).GetProperty("activeInputHandling");
                    if (property != null)
                    {
                        int currentValue = (int)property.GetValue(null);
                        string inputMode = currentValue == 0 ? "Legacy Input Manager" :
                                         currentValue == 1 ? "Input System Package" :
                                         currentValue == 2 ? "Both" : "Unknown";
                        
                        Debug.Log($"[InputSystemFixer] Current Input System mode: {inputMode} ({currentValue})");
                        
                        if (currentValue != 0)
                        {
                            Debug.LogWarning("[InputSystemFixer] Input System is not set to Legacy mode - this may cause conflicts");
                            Debug.LogWarning("[InputSystemFixer] Consider running FixInputSystemSettings() to resolve issues");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("[InputSystemFixer] Could not access activeInputHandling property");
                    }
                }
                else
                {
                    Debug.LogWarning("[InputSystemFixer] Could not find PlayerSettings");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[InputSystemFixer] Error checking Input System settings: {e.Message}");
                Debug.LogError("[InputSystemFixer] This may indicate Unity version compatibility issues");
            }
            #else
            Debug.Log("[InputSystemFixer] Input System check is only available in Editor mode");
            #endif
        }
        
        /// <summary>
        /// Manual instruction for fixing Input System
        /// </summary>
        [ContextMenu("Show Manual Fix Instructions")]
        public void ShowManualFixInstructions()
        {
            string instructions = @"
=== MANUAL FIX FOR INPUT SYSTEM ===

1. Go to Edit → Project Settings
2. Click on 'XR Plug-in Management' → 'Input System Package' (or search for 'Input')
3. Find 'Active Input Handling' setting
4. Change from 'Input System Package (New)' to 'Input Manager (Old)'
5. Unity will ask to restart - click 'Yes'

This will fix the Input System conflicts and errors.

======================================
            ";
            
            Debug.Log(instructions);
        }
    }
}

#if UNITY_EDITOR
/// <summary>
/// Editor script để tự động fix Input System khi project load
/// </summary>
[InitializeOnLoad]
public static class InputSystemAutoFixer
{
    static InputSystemAutoFixer()
    {
        // Automatically fix Input System when Unity starts
        EditorApplication.delayCall += AutoFixInputSystem;
    }
    
    private static void AutoFixInputSystem()
    {
        try
        {
            var property = typeof(PlayerSettings).GetProperty("activeInputHandling");
            if (property != null)
            {
                int currentValue = (int)property.GetValue(null);
                
                // If using Input System Package (1) or Both (2), switch to Legacy (0)
                if (currentValue != 0)
                {
                    property.SetValue(null, 0);
                    Debug.Log("[InputSystemAutoFixer] Automatically switched to Legacy Input Manager to fix conflicts");
                    Debug.Log("[InputSystemAutoFixer] Unity restart may be required for changes to take effect");
                    
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    // Already using Legacy Input Manager
                    Debug.Log("[InputSystemAutoFixer] Input System already set to Legacy Input Manager");
                }
            }
            else
            {
                if (Application.isEditor)
                {
                    Debug.Log("[InputSystemAutoFixer] ℹ️ activeInputHandling property không khả dụng trong Unity version này");
                    Debug.Log("[InputSystemAutoFixer] 🔧 Có thể fix manual qua Edit → Project Settings → XR Plug-in Management");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[InputSystemAutoFixer] Could not auto-fix Input System: {e.Message}");
            Debug.LogWarning("[InputSystemAutoFixer] Manual fix may be required via Project Settings");
        }
    }
}
#endif