using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace AnimalRevolt.UI.Editor
{
    /// <summary>
    /// Editor script để tạo Prefab cho UI Toggle System
    /// Tự động tạo và cấu hình các component cần thiết
    /// </summary>
    public class UITogglePrefabGenerator : EditorWindow
    {
        [Header("🎛️ Prefab Settings")]
        private string prefabName = "UIToggleSystem";
        private RectTransform.Edge buttonPosition = RectTransform.Edge.Top;
        private Vector2 buttonOffset = new Vector2(20, 20);
        private Vector2 buttonSize = new Vector2(60, 60);
        private string hotkey = "<Keyboard>/f1";

        [Header("🎨 Styling")]
        private Color activeColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
        private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);
        
        [MenuItem("AnimalRevolt/UI Tools/🎛️ UI Toggle Prefab Generator")]
        public static void ShowWindow()
        {
            UITogglePrefabGenerator window = GetWindow<UITogglePrefabGenerator>();
            window.titleContent = new GUIContent("UI Toggle Prefab Generator");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("🎛️ UI Toggle System Prefab Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("📋 Prefab Configuration", EditorStyles.boldLabel);
            prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);
            buttonPosition = (RectTransform.Edge)EditorGUILayout.EnumPopup("Button Position", buttonPosition);
            buttonOffset = EditorGUILayout.Vector2Field("Button Offset", buttonOffset);
            buttonSize = EditorGUILayout.Vector2Field("Button Size", buttonSize);
            hotkey = EditorGUILayout.TextField("Hotkey Binding", hotkey);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("🎨 Visual Styling", EditorStyles.boldLabel);
            activeColor = EditorGUILayout.ColorField("Active Color", activeColor);
            inactiveColor = EditorGUILayout.ColorField("Inactive Color", inactiveColor);

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Tạo Prefab hoàn chỉnh cho UI Toggle System bao gồm:\n" +
                "• UIToggleManager component\n" +
                "• Canvas với proper setup\n" +
                "• Button với icon camera\n" +
                "• Tự động cấu hình input system", 
                MessageType.Info
            );

            EditorGUILayout.Space();
            if (GUILayout.Button("🚀 Generate UI Toggle Prefab", GUILayout.Height(40)))
            {
                GenerateUITogglePrefab();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("📱 Add to Current Scene", GUILayout.Height(30)))
            {
                AddToCurrentScene();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("💡 Quick Actions", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🎮 Test Setup"))
            {
                TestUIToggleSetup();
            }
            if (GUILayout.Button("🔍 Find Existing"))
            {
                FindExistingUIToggle();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("📚 Open Guide"))
            {
                OpenSetupGuide();
            }
            if (GUILayout.Button("🧹 Cleanup"))
            {
                CleanupOldSystems();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Tạo UI Toggle Prefab hoàn chỉnh
        /// </summary>
        private void GenerateUITogglePrefab()
        {
            Debug.Log("🚀 Bắt đầu tạo UI Toggle Prefab...");

            // Tạo root object
            GameObject rootObj = new GameObject(prefabName);
            
            // Thêm UIToggleManager
            UIToggleManager toggleManager = rootObj.AddComponent<UIToggleManager>();
            
            // Thêm UIToggleDemo cho convenience
            UIToggleDemo toggleDemo = rootObj.AddComponent<UIToggleDemo>();

            // Tạo Canvas
            GameObject canvasObj = new GameObject("Canvas");
            canvasObj.transform.SetParent(rootObj.transform);
            
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();

            // Tạo Button
            GameObject buttonObj = new GameObject("ToggleButton");
            buttonObj.transform.SetParent(canvasObj.transform, false);
            
            // Setup RectTransform
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            SetupButtonPosition(buttonRect);
            
            // Tạo Image component
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.sprite = CreateCameraIcon();
            buttonImage.color = inactiveColor;
            
            // Tạo Button component
            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = buttonImage;

            // Configure UIToggleManager với references
            SetupUIToggleManagerReferences(toggleManager, canvas, buttonImage, button);

            // Tạo thư mục Prefabs nếu chưa có
            string prefabPath = "Assets/AnimalRevolt/UI/Prefabs";
            if (!AssetDatabase.IsValidFolder(prefabPath))
            {
                AssetDatabase.CreateFolder("Assets/AnimalRevolt/UI", "Prefabs");
            }

            // Lưu thành Prefab
            string fullPath = $"{prefabPath}/{prefabName}.prefab";
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(rootObj, fullPath);
            
            // Cleanup scene object
            DestroyImmediate(rootObj);

            // Select prefab trong Project
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);

            Debug.Log($"✅ UI Toggle Prefab đã được tạo: {fullPath}");
            ShowNotification(new GUIContent("✅ Prefab Created Successfully!"));
        }

        /// <summary>
        /// Thiết lập vị trí button
        /// </summary>
        private void SetupButtonPosition(RectTransform buttonRect)
        {
            buttonRect.sizeDelta = buttonSize;
            
            switch (buttonPosition)
            {
                case RectTransform.Edge.Top:
                    buttonRect.anchorMin = new Vector2(1f, 1f);
                    buttonRect.anchorMax = new Vector2(1f, 1f);
                    buttonRect.anchoredPosition = new Vector2(-buttonOffset.x, -buttonOffset.y);
                    break;
                case RectTransform.Edge.Bottom:
                    buttonRect.anchorMin = new Vector2(1f, 0f);
                    buttonRect.anchorMax = new Vector2(1f, 0f);
                    buttonRect.anchoredPosition = new Vector2(-buttonOffset.x, buttonOffset.y);
                    break;
                case RectTransform.Edge.Left:
                    buttonRect.anchorMin = new Vector2(0f, 1f);
                    buttonRect.anchorMax = new Vector2(0f, 1f);
                    buttonRect.anchoredPosition = new Vector2(buttonOffset.x, -buttonOffset.y);
                    break;
                case RectTransform.Edge.Right:
                    buttonRect.anchorMin = new Vector2(1f, 1f);
                    buttonRect.anchorMax = new Vector2(1f, 1f);
                    buttonRect.anchoredPosition = new Vector2(-buttonOffset.x, -buttonOffset.y);
                    break;
            }
        }

        /// <summary>
        /// Tạo icon camera
        /// </summary>
        private Sprite CreateCameraIcon()
        {
            int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            
            // Tạo icon camera đơn giản
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Color pixelColor = Color.clear;
                    
                    // Body camera
                    if (x >= 8 && x < size - 8 && y >= 16 && y < size - 8)
                    {
                        pixelColor = new Color(0.8f, 0.8f, 0.8f, 1f);
                    }
                    // Lens
                    else if (Vector2.Distance(new Vector2(x, y), new Vector2(size/2, size/2)) < 12)
                    {
                        pixelColor = new Color(0.1f, 0.1f, 0.1f, 1f);
                    }
                    // Flash
                    else if (x >= size - 16 && x < size - 8 && y >= size - 12 && y < size - 4)
                    {
                        pixelColor = Color.white;
                    }
                    
                    texture.SetPixel(x, y, pixelColor);
                }
            }
            
            texture.Apply();
            
            // Lưu texture
            string iconPath = "Assets/AnimalRevolt/UI/Icons";
            if (!AssetDatabase.IsValidFolder(iconPath))
            {
                AssetDatabase.CreateFolder("Assets/AnimalRevolt/UI", "Icons");
            }
            
            AssetDatabase.CreateAsset(texture, $"{iconPath}/CameraIcon.asset");
            
            // Tạo sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            AssetDatabase.CreateAsset(sprite, $"{iconPath}/CameraIcon_Sprite.asset");
            
            return sprite;
        }

        /// <summary>
        /// Cấu hình UIToggleManager references
        /// </summary>
        private void SetupUIToggleManagerReferences(UIToggleManager manager, Canvas canvas, Image buttonImage, Button button)
        {
            // Sử dụng reflection để set private fields nếu cần
            // Hoặc tạo public setters trong UIToggleManager
            
            SerializedObject so = new SerializedObject(manager);
            
            // Set canvas reference
            SerializedProperty canvasProp = so.FindProperty("uiCanvas");
            if (canvasProp != null)
                canvasProp.objectReferenceValue = canvas;
            
            // Set colors
            SerializedProperty activeColorProp = so.FindProperty("mauNutBat");
            if (activeColorProp != null)
                activeColorProp.colorValue = activeColor;
                
            SerializedProperty inactiveColorProp = so.FindProperty("mauNutTat");
            if (inactiveColorProp != null)
                inactiveColorProp.colorValue = inactiveColor;
            
            // Set button size and offset
            SerializedProperty sizeProp = so.FindProperty("kichThuocNut");
            if (sizeProp != null)
                sizeProp.vector2Value = buttonSize;
                
            SerializedProperty offsetProp = so.FindProperty("offsetTuGoc");
            if (offsetProp != null)
                offsetProp.vector2Value = buttonOffset;
            
            // Set position
            SerializedProperty positionProp = so.FindProperty("viTriNutToggle");
            if (positionProp != null)
                positionProp.enumValueIndex = (int)buttonPosition;
            
            so.ApplyModifiedProperties();
        }

        /// <summary>
        /// Thêm vào scene hiện tại
        /// </summary>
        private void AddToCurrentScene()
        {
            string prefabPath = $"Assets/AnimalRevolt/UI/Prefabs/{prefabName}.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab != null)
            {
                GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                Selection.activeGameObject = instance;
                Debug.Log("✅ UI Toggle System đã được thêm vào scene hiện tại");
            }
            else
            {
                Debug.LogWarning("❌ Prefab không tìm thấy. Hãy tạo Prefab trước.");
            }
        }

        /// <summary>
        /// Test UI Toggle setup
        /// </summary>
        private void TestUIToggleSetup()
        {
            UIToggleManager existing = FindObjectOfType<UIToggleManager>();
            if (existing != null)
            {
                Debug.Log("✅ UIToggleManager đã có trong scene");
                
                UIToggleDemo demo = existing.GetComponent<UIToggleDemo>();
                if (demo != null)
                {
                    demo.TestToggleUI();
                }
            }
            else
            {
                Debug.LogWarning("❌ UIToggleManager không tìm thấy trong scene");
            }
        }

        /// <summary>
        /// Tìm UI Toggle hiện có
        /// </summary>
        private void FindExistingUIToggle()
        {
            UIToggleManager[] managers = FindObjectsOfType<UIToggleManager>();
            Debug.Log($"🔍 Tìm thấy {managers.Length} UIToggleManager(s) trong scene");
            
            for (int i = 0; i < managers.Length; i++)
            {
                Debug.Log($"  • {managers[i].name} - {(managers[i].DangHienThiUI() ? "ACTIVE" : "INACTIVE")}");
            }
            
            if (managers.Length > 0)
            {
                Selection.activeGameObject = managers[0].gameObject;
            }
        }

        /// <summary>
        /// Mở hướng dẫn setup
        /// </summary>
        private void OpenSetupGuide()
        {
            string guidePath = "Assets/AnimalRevolt/Scripts/UI/UI_TOGGLE_SETUP_GUIDE.md";
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<TextAsset>(guidePath));
        }

        /// <summary>
        /// Cleanup hệ thống cũ
        /// </summary>
        private void CleanupOldSystems()
        {
            UIToggleManager[] oldManagers = FindObjectsOfType<UIToggleManager>();
            
            if (oldManagers.Length > 1)
            {
                Debug.Log($"🧹 Tìm thấy {oldManagers.Length} UIToggleManager, giữ lại 1 cái mới nhất");
                
                for (int i = 0; i < oldManagers.Length - 1; i++)
                {
                    DestroyImmediate(oldManagers[i].gameObject);
                }
                
                Debug.Log("✅ Cleanup hoàn tất");
            }
            else
            {
                Debug.Log("✅ Không có hệ thống cũ cần cleanup");
            }
        }
    }
}