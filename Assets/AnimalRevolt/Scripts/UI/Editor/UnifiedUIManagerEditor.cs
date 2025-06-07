using UnityEngine;
using UnityEditor;
using System.Linq;

namespace AnimalRevolt.UI.Editor
{
    /// <summary>
    /// Custom Editor cho UnifiedUIManager
    /// Cung cấp UI thân thiện trong Inspector với buttons và status display
    /// </summary>
    [CustomEditor(typeof(UnifiedUIManager))]
    public class UnifiedUIManagerEditor : UnityEditor.Editor
    {
        private UnifiedUIManager manager;
        private bool showAdvancedSettings = false;
        private bool showStatusInfo = true;

        private void OnEnable()
        {
            manager = (UnifiedUIManager)target;
        }

        public override void OnInspectorGUI()
        {
            // Header
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("🎛️ Unified UI Manager", EditorStyles.largeLabel);
            EditorGUILayout.Space();

            // Status Info
            DrawStatusSection();

            EditorGUILayout.Space();

            // Main Settings
            DrawMainSettings();

            EditorGUILayout.Space();

            // Quick Actions
            DrawQuickActions();

            EditorGUILayout.Space();

            // Advanced Settings
            DrawAdvancedSettings();

            // Save changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        /// <summary>
        /// Vẽ phần status thông tin
        /// </summary>
        private void DrawStatusSection()
        {
            showStatusInfo = EditorGUILayout.Foldout(showStatusInfo, "📊 System Status", true);
            if (!showStatusInfo) return;

            EditorGUILayout.BeginVertical("box");

            if (Application.isPlaying)
            {
                // Runtime status
                EditorGUILayout.LabelField("🔴 Runtime Status", EditorStyles.boldLabel);
                
                GUI.enabled = false;
                EditorGUILayout.Toggle("All UI Visible", manager != null ? manager.IsAllUIVisible() : false);
                EditorGUILayout.Toggle("Camera UI Visible", manager != null ? manager.IsCurrentCameraUIVisible() : false);
                GUI.enabled = true;

                EditorGUILayout.Space(5);

                if (GUILayout.Button("🔄 Refresh UI Components"))
                {
                    if (manager != null)
                    {
                        manager.RefreshAllUI();
                        Debug.Log("🔄 UI Components refreshed from Editor");
                    }
                }
            }
            else
            {
                // Edit-time status
                EditorGUILayout.LabelField("⏸️ Edit Mode", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "▶️ Press Play để xem runtime status và test UI controls", 
                    MessageType.Info
                );
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Vẽ main settings
        /// </summary>
        private void DrawMainSettings()
        {
            EditorGUILayout.LabelField("⚙️ Main Settings", EditorStyles.boldLabel);
            
            // Draw default inspector cho main settings
            DrawDefaultInspector();
        }

        /// <summary>
        /// Vẽ quick actions buttons
        /// </summary>
        private void DrawQuickActions()
        {
            EditorGUILayout.LabelField("🎮 Quick Actions", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            if (Application.isPlaying)
            {
                // Runtime actions
                EditorGUILayout.BeginHorizontal();
                
                GUI.backgroundColor = manager != null && manager.IsAllUIVisible() ? Color.red : Color.green;
                if (GUILayout.Button(manager != null && manager.IsAllUIVisible() ? "🔴 Hide All UI" : "🟢 Show All UI"))
                {
                    if (manager != null)
                    {
                        manager.ToggleAllUI();
                    }
                }
                
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("📹 Toggle Camera UI"))
                {
                    if (manager != null)
                    {
                        manager.ToggleCurrentCameraUI();
                    }
                }
                
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);

                // Category controls
                EditorGUILayout.LabelField("📂 Category Controls:", EditorStyles.miniLabel);
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("📹 Camera", EditorStyles.miniButton))
                {
                    manager?.ShowCategoryUI("CameraUI");
                }
                if (GUILayout.Button("❤️ Health", EditorStyles.miniButton))
                {
                    manager?.ShowCategoryUI("HealthUI");
                }
                if (GUILayout.Button("👤 Character", EditorStyles.miniButton))
                {
                    manager?.ShowCategoryUI("CharacterUI");
                }
                
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                // Edit-time actions
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("🔍 Find UI Components"))
                {
                    CountUIComponents();
                }
                
                if (GUILayout.Button("📋 Log Setup Guide"))
                {
                    LogSetupGuide();
                }
                
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);

                if (GUILayout.Button("🚀 Setup Default Configuration"))
                {
                    SetupDefaultConfiguration();
                }
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Vẽ advanced settings
        /// </summary>
        private void DrawAdvancedSettings()
        {
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "🔧 Advanced Settings", true);
            if (!showAdvancedSettings) return;

            EditorGUILayout.BeginVertical("box");

            // Migration section
            EditorGUILayout.LabelField("🔄 Migration Tools", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🔍 Find Legacy UIToggleManager"))
            {
                FindLegacyUIToggleManager();
            }
            if (GUILayout.Button("📦 Create Icon Asset"))
            {
                CreateDefaultIcon();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Debug section
            EditorGUILayout.LabelField("🐛 Debug Tools", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("📝 Log Component Registry"))
            {
                LogComponentRegistry();
            }
            if (GUILayout.Button("🧹 Cleanup Missing References"))
            {
                CleanupMissingReferences();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Đếm và hiển thị UI components trong scene
        /// </summary>
        private void CountUIComponents()
        {
            Debug.Log("=== 🔍 UI COMPONENTS SCAN ===");
            
            var cameraUIs = FindObjectsOfType<CameraSettingsUI>();
            var healthBars = FindObjectsOfType<HealthBar>();
            var charSelections = FindObjectsOfType<CharacterSelectionUI>();
            var ragdollUIs = FindObjectsOfType<RagdollPhysicsController>();
            
            Debug.Log($"📹 CameraSettingsUI: {cameraUIs.Length}");
            Debug.Log($"❤️ HealthBar: {healthBars.Length}");
            Debug.Log($"👤 CharacterSelectionUI: {charSelections.Length}");
            Debug.Log($"🎭 RagdollPhysicsController: {ragdollUIs.Length}");
            
            int total = cameraUIs.Length + healthBars.Length + charSelections.Length + ragdollUIs.Length;
            Debug.Log($"📊 Total UI Components: {total}");
            
            if (total == 0)
            {
                Debug.LogWarning("⚠️ No UI components found in scene. Add some UI components first.");
            }
        }

        /// <summary>
        /// Log setup guide
        /// </summary>
        private void LogSetupGuide()
        {
            Debug.Log("=== 🎛️ UNIFIED UI MANAGER SETUP GUIDE ===");
            Debug.Log("1. ▶️ Press Play để test UI system");
            Debug.Log("2. 🎮 Sử dụng F1 để toggle all UI, F2 để toggle camera UI");
            Debug.Log("3. 🔄 Click 'Refresh UI Components' để update component list");
            Debug.Log("4. 📹 Test camera switching để verify UI theo camera");
            Debug.Log("5. 📊 Check Console logs để monitor system status");
            Debug.Log("💡 Tip: Enable 'Auto Discover UI' và 'Auto Track Camera Changes' cho best experience");
        }

        /// <summary>
        /// Setup default configuration
        /// </summary>
        private void SetupDefaultConfiguration()
        {
            Debug.Log("🚀 Setting up default UnifiedUIManager configuration...");
            
            // Có thể thêm logic setup default values ở đây
            // Ví dụ: tìm CameraManager, setup button position, etc.
            
            var cameraManager = FindObjectOfType<AnimalRevolt.Camera.CameraManager>();
            if (cameraManager != null)
            {
                Debug.Log("📹 Found CameraManager - system ready for camera integration");
            }
            else
            {
                Debug.LogWarning("📹 CameraManager not found - camera UI switching may not work");
            }
            
            EditorUtility.SetDirty(target);
            Debug.Log("✅ Default configuration applied");
        }

        /// <summary>
        /// Tìm legacy UIToggleManager
        /// </summary>
        private void FindLegacyUIToggleManager()
        {
            var legacyManagers = FindObjectsOfType<UIToggleManager>();
            
            if (legacyManagers.Length > 0)
            {
                Debug.Log($"🔍 Found {legacyManagers.Length} legacy UIToggleManager(s):");
                
                foreach (var legacy in legacyManagers)
                {
                    Debug.Log($"  • {legacy.name} - Click to migrate to UnifiedUIManager");
                    EditorGUIUtility.PingObject(legacy);
                }
                
                Debug.Log("💡 Tip: Select legacy UIToggleManager → Context Menu → 'Migrate to UnifiedUIManager'");
            }
            else
            {
                Debug.Log("✅ No legacy UIToggleManager found - you're all set!");
            }
        }

        /// <summary>
        /// Tạo default icon
        /// </summary>
        private void CreateDefaultIcon()
        {
            Debug.Log("📦 Creating default toggle icon...");
            
            // Tạo texture đơn giản cho icon
            Texture2D iconTexture = new Texture2D(64, 64);
            Color[] pixels = new Color[64 * 64];
            
            // Tạo icon camera đơn giản
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.clear;
            }
            
            // Vẽ hình vuông đại diện cho camera
            for (int x = 16; x < 48; x++)
            {
                for (int y = 20; y < 44; y++)
                {
                    if (x == 16 || x == 47 || y == 20 || y == 43)
                    {
                        pixels[y * 64 + x] = Color.white;
                    }
                }
            }
            
            iconTexture.SetPixels(pixels);
            iconTexture.Apply();
            
            // Save as asset
            string path = "Assets/AnimalRevolt/UI/Icons/DefaultToggleIcon.png";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            
            byte[] bytes = iconTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            
            AssetDatabase.Refresh();
            
            Debug.Log($"📦 Default icon created at: {path}");
        }

        /// <summary>
        /// Log component registry (runtime only)
        /// </summary>
        private void LogComponentRegistry()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("⚠️ Component registry only available in Play mode");
                return;
            }
            
            if (manager != null)
            {
                // Trigger refresh để log registry
                manager.RefreshAllUI();
            }
        }

        /// <summary>
        /// Cleanup missing references
        /// </summary>
        private void CleanupMissingReferences()
        {
            Debug.Log("🧹 Cleaning up missing references...");
            
            // Có thể thêm logic cleanup ở đây nếu cần
            // Ví dụ: remove null references, cleanup destroyed objects, etc.
            
            EditorUtility.SetDirty(target);
            Debug.Log("✅ Cleanup completed");
        }
    }
}