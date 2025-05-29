using UnityEngine;
using UnityEditor;

/// <summary>
/// Script đơn giản để fix lỗi SerializedObjectNotCreatableException
/// Sử dụng Unity MCP để clean up missing references
/// </summary>
public static class UnityEditorErrorFixer
{
    [MenuItem("Tools/Fix Editor Errors/Clear Selection")]
    public static void ClearSelection()
    {
        Selection.activeObject = null;
        Selection.objects = new Object[0];
        
        // Clear Inspector focus
        EditorGUIUtility.systemCopyBuffer = "";
        
        Debug.Log("✅ Cleared all selections - should fix SerializedObject errors");
    }
    
    [MenuItem("Tools/Fix Editor Errors/Refresh Project")]
    public static void RefreshProject()
    {
        AssetDatabase.Refresh();
        EditorUtility.UnloadUnusedAssetsImmediate();
        System.GC.Collect();
        
        Debug.Log("✅ Refreshed project and cleaned memory");
    }
    
    [MenuItem("Tools/Fix Editor Errors/Force Recompile")]
    public static void ForceRecompile()
    {
        AssetDatabase.Refresh();
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        
        Debug.Log("✅ Forced script recompilation");
    }
    
    [MenuItem("Tools/Fix Editor Errors/Select Main Camera")]
    public static void SelectMainCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Selection.activeGameObject = mainCam.gameObject;
            Debug.Log("✅ Selected Main Camera");
        }
        else
        {
            Debug.LogWarning("⚠️ No Main Camera found");
        }
    }
}
