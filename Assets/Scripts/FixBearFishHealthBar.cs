using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Object = UnityEngine.Object;

public class FixBearFishHealthBar
{
    public static void Execute()
    {
        GameObject bearFish = GameObject.Find("QuaiVat_BearFish");
        if (bearFish == null)
        {
            Debug.LogError("Không tìm thấy QuaiVat_BearFish!");
            return;
        }

        Debug.Log("Đang sửa cấu trúc HealthBar của QuaiVat_BearFish...");

        // Tìm HealthBar hiện tại
        Transform healthBar = bearFish.transform.Find("HealthBar");
        if (healthBar == null)
        {
            Debug.LogError("Không tìm thấy HealthBar!");
            return;
        }

        // Xóa cấu trúc cũ (nếu có)
        Transform oldBackground = healthBar.Find("Background");
        Transform oldHealthSlider = healthBar.Find("HealthSlider");
        if (oldBackground) Object.DestroyImmediate(oldBackground.gameObject);
        if (oldHealthSlider) Object.DestroyImmediate(oldHealthSlider.gameObject);

        // Xóa Slider và CanvasScaler cũ trên HealthBar
        Slider oldSlider = healthBar.GetComponent<Slider>();
        CanvasScaler oldScaler = healthBar.GetComponent<CanvasScaler>();
        if (oldSlider) Object.DestroyImmediate(oldSlider);
        if (oldScaler) Object.DestroyImmediate(oldScaler);

        // Thêm Slider mới trên HealthBar
        Slider newSlider = healthBar.gameObject.AddComponent<Slider>();
        newSlider.interactable = false;
        newSlider.transition = Selectable.Transition.None;
        newSlider.minValue = 0f;
        newSlider.maxValue = 1f;
        newSlider.value = 1f;
        newSlider.direction = Slider.Direction.LeftToRight;

        // Tạo HealthBarBG (Background)
        GameObject healthBarBG = new GameObject("HealthBarBG");
        healthBarBG.transform.SetParent(healthBar);
        
        RectTransform bgRect = healthBarBG.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        bgRect.localPosition = Vector3.zero;
        bgRect.localScale = Vector3.one;

        Image bgImage = healthBarBG.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 1f); // Màu đen
        bgImage.type = Image.Type.Simple;

        // Tạo HealthBarFill (Fill)
        GameObject healthBarFill = new GameObject("HealthBarFill");
        healthBarFill.transform.SetParent(healthBarBG.transform);
        
        RectTransform fillRect = healthBarFill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        fillRect.localPosition = Vector3.zero;
        fillRect.localScale = Vector3.one;

        Image fillImage = healthBarFill.AddComponent<Image>();
        fillImage.color = new Color(0f, 1f, 0f, 1f); // Màu xanh
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillAmount = 1f;

        // Assign fillRect cho Slider
        newSlider.fillRect = fillRect;

        Debug.Log("✅ Đã sửa cấu trúc HealthBar thành công!");

        // Tạo prefab mới
        try
        {
            string prefabPath = "Assets/Resources/Characters/QuaiVat/QuaiVat_BearFish.prefab";
            
            // Xóa prefab cũ
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath))
            {
                AssetDatabase.DeleteAsset(prefabPath);
                AssetDatabase.Refresh();
            }

            // Xóa missing scripts trước khi tạo prefab
            int removedCount = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(bearFish);
            Transform[] allTransforms = bearFish.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allTransforms)
            {
                removedCount += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
            }

            Debug.Log($"Đã xóa {removedCount} missing scripts.");

            // Tạo prefab mới
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(bearFish, prefabPath, InteractionMode.AutomatedAction);
            
            if (newPrefab != null)
            {
                Debug.Log("✅ Prefab đã được tạo thành công với cấu trúc HealthBar đúng!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Lỗi khi tạo prefab: " + e.Message);
        }
    }
}