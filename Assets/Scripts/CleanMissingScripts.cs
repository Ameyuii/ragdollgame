using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CleanMissingScripts
{
    public static void Execute()
    {
        // Tìm GameObject QuaiVat_BearFish
        GameObject bearFish = GameObject.Find("QuaiVat_BearFish");
        if (bearFish == null)
        {
            Debug.LogError("Không tìm thấy QuaiVat_BearFish!");
            return;
        }

        Debug.Log("Đang dọn dẹp missing scripts từ " + bearFish.name);
        
        // Dọn dẹp missing scripts từ object chính và tất cả children
        int totalCleaned = CleanMissingScriptsRecursive(bearFish);
        
        Debug.Log($"Đã dọn dẹp {totalCleaned} missing script references.");

        // Bây giờ thử tạo prefab
        try
        {
            string prefabPath = "Assets/Resources/Characters/QuaiVat/QuaiVat_BearFish_Clean.prefab";
            PrefabUtility.SaveAsPrefabAssetAndConnect(bearFish, prefabPath, InteractionMode.AutomatedAction);
            Debug.Log("Đã tạo prefab thành công tại: " + prefabPath);

            // Xóa prefab cũ nếu tồn tại
            string oldPrefabPath = "Assets/Resources/Characters/QuaiVat/QuaiVat_BearFish_Old.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(oldPrefabPath))
            {
                AssetDatabase.DeleteAsset(oldPrefabPath);
                Debug.Log("Đã xóa prefab cũ: " + oldPrefabPath);
            }

            // Đổi tên prefab mới thành tên gốc
            AssetDatabase.RenameAsset(prefabPath, "QuaiVat_BearFish");
            AssetDatabase.Refresh();
            
            Debug.Log("Prefab đã được cập nhật thành công!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi khi tạo prefab: " + e.Message);
        }
    }

    private static int CleanMissingScriptsRecursive(GameObject go)
    {
        int cleanedCount = 0;
        
        // Dọn dẹp missing scripts từ object hiện tại
        SerializedObject serializedObject = new SerializedObject(go);
        SerializedProperty prop = serializedObject.FindProperty("m_Component");
        
        List<int> indicesToRemove = new List<int>();
        
        for (int i = 0; i < prop.arraySize; i++)
        {
            SerializedProperty componentProp = prop.GetArrayElementAtIndex(i);
            SerializedProperty componentRef = componentProp.FindPropertyRelative("component");
            
            if (componentRef.objectReferenceValue == null)
            {
                indicesToRemove.Add(i);
                cleanedCount++;
                Debug.Log($"Tìm thấy missing script tại index {i} trên {go.name}");
            }
        }
        
        // Xóa các missing components (từ cuối về đầu để không ảnh hưởng index)
        for (int i = indicesToRemove.Count - 1; i >= 0; i--)
        {
            prop.DeleteArrayElementAtIndex(indicesToRemove[i]);
        }
        
        if (indicesToRemove.Count > 0)
        {
            serializedObject.ApplyModifiedProperties();
            Debug.Log($"Đã xóa {indicesToRemove.Count} missing scripts từ {go.name}");
        }
        
        // Dọn dẹp recursively cho tất cả children
        foreach (Transform child in go.transform)
        {
            cleanedCount += CleanMissingScriptsRecursive(child.gameObject);
        }
        
        return cleanedCount;
    }
}