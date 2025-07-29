using UnityEngine;
using UnityEditor;

public class ForceClearMissingScripts
{
    public static void Execute()
    {
        GameObject bearFish = GameObject.Find("QuaiVat_BearFish");
        if (bearFish == null)
        {
            Debug.LogError("Không tìm thấy QuaiVat_BearFish!");
            return;
        }

        Debug.Log("Bắt đầu force clear missing scripts...");

        // Sử dụng GameObjectUtility để xóa missing scripts
        int totalRemoved = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(bearFish);
        
        // Xóa missing scripts từ tất cả children
        Transform[] allTransforms = bearFish.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in allTransforms)
        {
            totalRemoved += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
        }

        Debug.Log($"Đã xóa {totalRemoved} missing scripts.");

        // Thử tạo prefab
        try
        {
            string prefabPath = "Assets/Resources/Characters/QuaiVat/QuaiVat_BearFish.prefab";
            
            // Xóa prefab cũ nếu tồn tại
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath))
            {
                AssetDatabase.DeleteAsset(prefabPath);
                AssetDatabase.Refresh();
            }

            // Tạo prefab mới
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(bearFish, prefabPath, InteractionMode.AutomatedAction);
            
            if (newPrefab != null)
            {
                Debug.Log("✅ Prefab đã được tạo thành công tại: " + prefabPath);
            }
            else
            {
                Debug.LogError("❌ Không thể tạo prefab");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Lỗi khi tạo prefab: " + e.Message);
        }
    }
}