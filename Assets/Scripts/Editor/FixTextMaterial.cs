using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FixTextMaterial
{
    [MenuItem("Tools/Fix Text Material")]
    public static void FixAllTextMaterials()
    {
        Debug.Log("🔧 Bắt đầu sửa Material cho tất cả Text components...");
        
        // Tìm tất cả Text components trong scene
        Text[] allTexts = Object.FindObjectsOfType<Text>();
        Debug.Log($"🔍 Tìm thấy {allTexts.Length} Text components");
        
        int fixedCount = 0;
        
        foreach (Text textComp in allTexts)
        {
            if (textComp.material == null || textComp.material.name == "None (Material)")
            {
                // Gán Default UI Material
                textComp.material = Canvas.GetDefaultCanvasMaterial();
                
                Debug.Log($"✅ Đã sửa Material cho Text trong {textComp.gameObject.name}");
                EditorUtility.SetDirty(textComp);
                fixedCount++;
            }
        }
        
        Debug.Log($"🎉 Hoàn thành! Đã sửa {fixedCount} Text components");
        
        // Lưu scene
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
    
    [MenuItem("Tools/Fix TeamDropdown Text Only")]
    public static void FixTeamDropdownTextOnly()
    {
        Debug.Log("🔧 Bắt đầu sửa Material cho Text trong TeamDropdownPanel...");
        
        // Tìm TeamDropdownPanel
        GameObject dropdownPanel = GameObject.Find("TeamDropdownPanel");
        if (dropdownPanel == null)
        {
            Debug.LogError("❌ Không tìm thấy TeamDropdownPanel!");
            return;
        }
        
        // Lấy tất cả Text components trong panel
        Text[] textComponents = dropdownPanel.GetComponentsInChildren<Text>();
        Debug.Log($"🔍 Tìm thấy {textComponents.Length} Text components trong TeamDropdownPanel");
        
        foreach (Text textComp in textComponents)
        {
            Debug.Log($"🔧 Đang sửa Text trong {textComp.transform.parent.name}:");
            Debug.Log($"   - Text: '{textComp.text}'");
            Debug.Log($"   - Material hiện tại: {(textComp.material != null ? textComp.material.name : "NULL")}");
            
            // Gán Default UI Material
            textComp.material = Canvas.GetDefaultCanvasMaterial();
            
            // Đảm bảo các thuộc tính khác cũng đúng
            if (textComp.font == null)
            {
                textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            
            textComp.color = Color.white;
            textComp.fontSize = 16;
            textComp.alignment = TextAnchor.MiddleCenter;
            
            Debug.Log($"   ✅ Đã sửa Material: {textComp.material.name}");
            EditorUtility.SetDirty(textComp);
        }
        
        Debug.Log("🎉 Hoàn thành sửa TeamDropdown Text!");
        
        // Lưu scene
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
}
