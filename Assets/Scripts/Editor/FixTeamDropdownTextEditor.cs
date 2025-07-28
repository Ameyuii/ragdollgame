using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FixTeamDropdownTextEditor
{
    [MenuItem("Tools/Fix Team Dropdown Text")]
    public static void FixTeamDropdownText()
    {
        Debug.Log("🔧 Bắt đầu sửa Text components trong TeamDropdownPanel...");
        
        // Tìm TeamDropdownPanel
        GameObject dropdownPanel = GameObject.Find("TeamDropdownPanel");
        if (dropdownPanel == null)
        {
            Debug.LogError("❌ Không tìm thấy TeamDropdownPanel!");
            return;
        }
        
        Debug.Log("✅ Tìm thấy TeamDropdownPanel");
        
        // Lấy tất cả Text components trong panel
        Text[] textComponents = dropdownPanel.GetComponentsInChildren<Text>();
        Debug.Log($"🔍 Tìm thấy {textComponents.Length} Text components");
        
        // Sửa từng Text component
        for (int i = 0; i < textComponents.Length; i++)
        {
            Text textComp = textComponents[i];
            GameObject parent = textComp.transform.parent.gameObject;
            
            Debug.Log($"🔧 Đang sửa Text trong {parent.name}:");
            Debug.Log($"   - Text hiện tại: '{textComp.text}'");
            Debug.Log($"   - Font: {(textComp.font != null ? textComp.font.name : "NULL")}");
            Debug.Log($"   - Font Size: {textComp.fontSize}");
            Debug.Log($"   - Color: {textComp.color}");
            
            // Gán nội dung text dựa trên parent name
            if (parent.name.Contains("Team1"))
            {
                textComp.text = "🔵 TEAM 1";
            }
            else if (parent.name.Contains("Team2"))
            {
                textComp.text = "🔴 TEAM 2";
            }
            else if (parent.name.Contains("Team3"))
            {
                textComp.text = "🟢 TEAM 3";
            }
            else if (parent.name.Contains("Team4"))
            {
                textComp.text = "🟡 TEAM 4";
            }
            else
            {
                textComp.text = $"TEAM {i + 1}";
            }
            
            // Gán font nếu chưa có
            if (textComp.font == null)
            {
                textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                Debug.Log("   ✅ Đã gán font mặc định");
            }
            
            // Đặt font size
            if (textComp.fontSize <= 0)
            {
                textComp.fontSize = 14;
                Debug.Log("   ✅ Đã đặt font size = 14");
            }
            
            // Đặt màu text
            textComp.color = Color.white;
            
            // Đặt alignment
            textComp.alignment = TextAnchor.MiddleCenter;
            
            // Đảm bảo RectTransform được thiết lập đúng
            RectTransform textRect = textComp.GetComponent<RectTransform>();
            if (textRect != null)
            {
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                textRect.anchoredPosition = Vector2.zero;
            }
            
            Debug.Log($"   ✅ Đã sửa Text: '{textComp.text}' - Font: {textComp.font.name} - Size: {textComp.fontSize} - Color: {textComp.color}");
            
            // Đánh dấu object đã thay đổi để Unity lưu
            EditorUtility.SetDirty(textComp);
        }
        
        Debug.Log("🎉 Hoàn thành sửa Text components!");
    }
}
