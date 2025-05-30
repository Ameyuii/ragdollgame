using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Inspector cho NPCBaseController để hiển thị thông tin CharacterData
/// </summary>
#if UNITY_EDITOR
[CustomEditor(typeof(NPCBaseController), true)]
public class NPCBaseControllerEditor : Editor
{
    private NPCBaseController npcController;
    
    void OnEnable()
    {
        npcController = (NPCBaseController)target;
    }
      public override void OnInspectorGUI()
    {
        // Kiểm tra null safety
        if (npcController == null)
        {
            npcController = (NPCBaseController)target;
        }
        
        if (npcController == null)
        {
            EditorGUILayout.HelpBox("❌ NPCBaseController reference is null!", MessageType.Error);
            return;
        }
        
        // Vẽ Inspector mặc định
        DrawDefaultInspector();
        
        // Thêm section hiển thị thông tin CharacterData
        if (npcController.CharacterData != null)
        {
            EditorGUILayout.Space(10);
            
            // Header với background color
            var headerStyle = new GUIStyle(GUI.skin.box);
            headerStyle.normal.background = MakeTexture(1, 1, new Color(0.3f, 0.7f, 1f, 0.3f));
            
            EditorGUILayout.BeginVertical(headerStyle);
            EditorGUILayout.LabelField("📜 CharacterData Info", EditorStyles.boldLabel);
            
            var characterData = npcController.CharacterData;
            
            // Hiển thị thông tin CharacterData với màu sắc
            var infoStyle = new GUIStyle(GUI.skin.label);
            infoStyle.richText = true;
              EditorGUILayout.LabelField($"<b>Character Name:</b> <color=yellow>{characterData.characterName}</color>", infoStyle);
            EditorGUILayout.LabelField($"<b>Max Health:</b> <color=green>{characterData.maxHealth}</color>", infoStyle);
            EditorGUILayout.LabelField($"<b>Move Speed:</b> <color=cyan>{characterData.moveSpeed}</color>", infoStyle);
            EditorGUILayout.LabelField($"<b>Rotation Speed:</b> <color=cyan>{characterData.rotationSpeed}°/s</color>", infoStyle);
            EditorGUILayout.LabelField($"<b>Acceleration:</b> <color=cyan>{characterData.acceleration}</color>", infoStyle);
            EditorGUILayout.LabelField($"<b>Team ID:</b> <color=orange>{characterData.teamId}</color>", infoStyle);
            
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("<b>Combat Stats:</b>", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"<b>Base Damage:</b> <color=red>{characterData.baseDamage}</color>", infoStyle);
            EditorGUILayout.LabelField($"<b>Attack Range:</b> <color=magenta>{characterData.attackRange}m</color>", infoStyle);
            EditorGUILayout.LabelField($"<b>Attack Cooldown:</b> <color=white>{characterData.attackCooldown}s</color>", infoStyle);
            EditorGUILayout.LabelField($"<b>Attack Duration:</b> <color=white>{characterData.attackAnimationDuration}s</color>", infoStyle);
            EditorGUILayout.LabelField($"<b>Hit Timing:</b> <color=white>{characterData.attackHitTiming:F2}</color>", infoStyle);
            EditorGUILayout.LabelField($"<b>Detection Range:</b> <color=gray>{characterData.detectionRange}m</color>", infoStyle);
            
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("<b>Attack Variations:</b>", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"<b>Basic/Attack1/Attack2:</b> <color=yellow>{characterData.basicAttackChance:F0}%/{characterData.attack1Chance:F0}%/{characterData.attack2Chance:F0}%</color>", infoStyle);
            
            if (characterData.useVariableAttackCooldown)
            {
                EditorGUILayout.LabelField($"<b>Variable Cooldowns:</b> <color=white>A1:{characterData.attack1Cooldown}s, A2:{characterData.attack2Cooldown}s</color>", infoStyle);
            }
            
            EditorGUILayout.Space(5);
            
            // Button để cập nhật từ CharacterData
            if (GUILayout.Button("🔄 Update Inspector From CharacterData", GUILayout.Height(30)))
            {
                npcController.UpdateFromCharacterData();
            }
            
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("⚠️ Chưa có CharacterData. Kéo thả CharacterData asset vào slot 'Character Data' để sử dụng hệ thống ScriptableObject.", MessageType.Warning);
            
            if (GUILayout.Button("➕ Create New CharacterData", GUILayout.Height(25)))
            {
                CreateNewCharacterData();
            }
        }
        
        // Hiển thị runtime stats nếu đang chạy
        if (Application.isPlaying)
        {
            EditorGUILayout.Space(10);
            var runtimeStyle = new GUIStyle(GUI.skin.box);
            runtimeStyle.normal.background = MakeTexture(1, 1, new Color(1f, 0.5f, 0.3f, 0.3f));
            
            EditorGUILayout.BeginVertical(runtimeStyle);
            EditorGUILayout.LabelField("⚡ Runtime Stats", EditorStyles.boldLabel);
            
            var infoStyle = new GUIStyle(GUI.skin.label);
            infoStyle.richText = true;
            
            // Truy cập các field protected thông qua reflection hoặc property public
            EditorGUILayout.LabelField($"<b>Current Health:</b> <color=green>{GetCurrentHealth()}/{npcController.maxHealth}</color>", infoStyle);
            EditorGUILayout.LabelField($"<b>Is Dead:</b> <color={(GetIsDead() ? "red>YES" : "green>NO")}</color>", infoStyle);
            EditorGUILayout.LabelField($"<b>Has Target:</b> <color={(GetHasTarget() ? "red>YES" : "gray>NO")}</color>", infoStyle);
            
            EditorGUILayout.EndVertical();
        }
    }
    
    // Helper methods để access protected fields
    private float GetCurrentHealth()
    {
        var field = typeof(NPCBaseController).GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null ? (float)field.GetValue(npcController) : 0f;
    }
    
    private bool GetIsDead()
    {
        var field = typeof(NPCBaseController).GetField("isDead", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null ? (bool)field.GetValue(npcController) : false;
    }
    
    private bool GetHasTarget()
    {
        var field = typeof(NPCBaseController).GetField("targetEnemy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var target = field?.GetValue(npcController);
        return target != null;
    }
    
    private void CreateNewCharacterData()
    {
        // Tạo CharacterData asset mới
        var characterData = ScriptableObject.CreateInstance<CharacterData>();
        characterData.characterName = npcController.gameObject.name;
        
        string path = EditorUtility.SaveFilePanelInProject(
            "Create New CharacterData",
            npcController.gameObject.name + "_Data",
            "asset",
            "Chọn vị trí lưu CharacterData asset mới"
        );
        
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(characterData, path);
            AssetDatabase.SaveAssets();
            
            // Assign vào NPCController
            var characterDataField = typeof(NPCBaseController).GetField("characterData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (characterDataField != null)
            {
                characterDataField.SetValue(npcController, characterData);
                EditorUtility.SetDirty(npcController);
            }
            
            Debug.Log($"✅ Đã tạo CharacterData mới: {path}");
        }
    }
    
    private Texture2D MakeTexture(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = color;
            
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
#endif
