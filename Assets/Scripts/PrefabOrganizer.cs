using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class PrefabOrganizer : EditorWindow
{
    [MenuItem("Tools/Prefab Organizer")]
    public static void ShowWindow()
    {
        GetWindow<PrefabOrganizer>("Prefab Organizer");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Prefab Organization Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("Thư mục đã tạo:", EditorStyles.label);
        GUILayout.Label("• Assets/Prefabs/Robot - Cho các robot, mech", EditorStyles.helpBox);
        GUILayout.Label("• Assets/Prefabs/QuaiVat - Cho các quái vật, zombie", EditorStyles.helpBox);
        GUILayout.Label("• Assets/Prefabs/ChienBinh - Cho các chiến binh, soldier", EditorStyles.helpBox);
        GUILayout.Label("• Assets/Prefabs/LoaiNhanVat - Cho các nhân vật khác", EditorStyles.helpBox);
        
        GUILayout.Space(10);
        GUILayout.Label("Thư mục Resources (để script tự động tải):", EditorStyles.label);
        GUILayout.Label("• Assets/Resources/Robot", EditorStyles.helpBox);
        GUILayout.Label("• Assets/Resources/QuaiVat", EditorStyles.helpBox);
        GUILayout.Label("• Assets/Resources/ChienBinh", EditorStyles.helpBox);
        GUILayout.Label("• Assets/Resources/LoaiNhanVat", EditorStyles.helpBox);
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Di chuyển prefab hiện tại vào thư mục phù hợp"))
        {
            OrganizeExistingPrefabs();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Hướng dẫn:", EditorStyles.boldLabel);
        GUILayout.Label("1. Kéo thả prefab vào thư mục Prefabs tương ứng", EditorStyles.wordWrappedLabel);
        GUILayout.Label("2. Copy prefab vào thư mục Resources tương ứng", EditorStyles.wordWrappedLabel);
        GUILayout.Label("3. Script sẽ tự động tải từ thư mục Resources", EditorStyles.wordWrappedLabel);
    }
    
    void OrganizeExistingPrefabs()
    {
        // Get current prefabs from BattleGameManager
        BattleGameManager gameManager = FindObjectOfType<BattleGameManager>();
        if (gameManager == null || gameManager.characterPrefabs == null)
        {
            Debug.LogWarning("Không tìm thấy BattleGameManager hoặc characterPrefabs");
            return;
        }
        
        foreach (GameObject prefab in gameManager.characterPrefabs)
        {
            if (prefab == null) continue;
            
            string prefabPath = AssetDatabase.GetAssetPath(prefab);
            string prefabName = prefab.name.ToLower();
            string targetFolder = "";
            string resourcesFolder = "";
            
            // Determine target folder based on name
            if (prefabName.Contains("robot") || prefabName.Contains("mech"))
            {
                targetFolder = "Assets/Prefabs/Robot/";
                resourcesFolder = "Assets/Resources/Robot/";
            }
            else if (prefabName.Contains("monster") || prefabName.Contains("zombie") || prefabName.Contains("npc"))
            {
                targetFolder = "Assets/Prefabs/QuaiVat/";
                resourcesFolder = "Assets/Resources/QuaiVat/";
            }
            else if (prefabName.Contains("battle") || prefabName.Contains("soldier"))
            {
                targetFolder = "Assets/Prefabs/ChienBinh/";
                resourcesFolder = "Assets/Resources/ChienBinh/";
            }
            else
            {
                targetFolder = "Assets/Prefabs/LoaiNhanVat/";
                resourcesFolder = "Assets/Resources/LoaiNhanVat/";
            }
            
            // Create directories if they don't exist
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);
            if (!Directory.Exists(resourcesFolder))
                Directory.CreateDirectory(resourcesFolder);
            
            // Copy to both locations
            string fileName = Path.GetFileName(prefabPath);
            string newPrefabPath = targetFolder + fileName;
            string newResourcePath = resourcesFolder + fileName;
            
            try
            {
                // Copy to Prefabs folder
                if (!File.Exists(newPrefabPath))
                {
                    AssetDatabase.CopyAsset(prefabPath, newPrefabPath);
                    Debug.Log($"Copied {prefab.name} to {newPrefabPath}");
                }
                
                // Copy to Resources folder
                if (!File.Exists(newResourcePath))
                {
                    AssetDatabase.CopyAsset(prefabPath, newResourcePath);
                    Debug.Log($"Copied {prefab.name} to {newResourcePath}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error copying {prefab.name}: {e.Message}");
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log("Hoàn thành tổ chức prefab!");
    }
}
#endif