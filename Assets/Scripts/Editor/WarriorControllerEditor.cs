using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Inspector cho WarriorController để hiển thị thông tin đặc thù của Warrior
/// </summary>
#if UNITY_EDITOR
[CustomEditor(typeof(WarriorController))]
public class WarriorControllerEditor : NPCBaseControllerEditor
{
    private WarriorController warriorController;
    
    void OnEnable()
    {
        warriorController = (WarriorController)target;
    }
      public override void OnInspectorGUI()
    {
        // Kiểm tra null safety
        if (warriorController == null)
        {
            warriorController = (WarriorController)target;
        }
        
        if (warriorController == null)
        {
            EditorGUILayout.HelpBox("❌ WarriorController reference is null!", MessageType.Error);
            return;
        }
        
        // Vẽ Inspector của base class trước
        base.OnInspectorGUI();
        
        // Thêm section đặc thù cho Warrior
        EditorGUILayout.Space(10);
        
        // Header cho Warrior specific
        var warriorHeaderStyle = new GUIStyle(GUI.skin.box);
        warriorHeaderStyle.normal.background = MakeTexture(1, 1, new Color(0.8f, 0.3f, 0.3f, 0.3f)); // Màu đỏ cho Warrior
        
        EditorGUILayout.BeginVertical(warriorHeaderStyle);
        EditorGUILayout.LabelField("🗡️ Warrior Specific", EditorStyles.boldLabel);
        
        var infoStyle = new GUIStyle(GUI.skin.label);
        infoStyle.richText = true;
        
        // Hiển thị thông tin đặc thù của Warrior
        EditorGUILayout.LabelField($"<b>Warrior Type:</b> <color=yellow>Melee Fighter</color>", infoStyle);
        
        if (warriorController.CharacterData != null)
        {
            EditorGUILayout.LabelField($"<b>Combat Role:</b> <color=red>Front Line Warrior</color>", infoStyle);
            
            // Hiển thị thông tin combat đặc thù
            float damagePerSecond = warriorController.CharacterData.baseDamage / warriorController.CharacterData.attackCooldown;
            EditorGUILayout.LabelField($"<b>DPS:</b> <color=orange>{damagePerSecond:F1}</color>", infoStyle);
            
            // Đánh giá warrior type dựa trên stats
            string warriorType = GetWarriorType(warriorController.CharacterData);
            EditorGUILayout.LabelField($"<b>Build Type:</b> <color=cyan>{warriorType}</color>", infoStyle);
        }
        
        // Button debug đặc thù cho Warrior
        EditorGUILayout.Space(5);
        if (GUILayout.Button("🗡️ Debug Warrior Info", GUILayout.Height(25)))
        {
            warriorController.DebugWarriorInfo();
        }
        
        if (Application.isPlaying)
        {
            if (GUILayout.Button("🧪 Test Warrior Attack", GUILayout.Height(25)))
            {
                warriorController.TestWarriorAttack();
            }
        }
        
        EditorGUILayout.EndVertical();
        
        // Warning nếu thiết lập không phù hợp cho Warrior
        if (warriorController.CharacterData != null)
        {
            var characterData = warriorController.CharacterData;
            
            if (characterData.attackRange > 5f)
            {
                EditorGUILayout.HelpBox("⚠️ Attack Range > 5m có vẻ quá xa cho Warrior (melee fighter). Cân nhắc giảm xuống 2-3m.", MessageType.Warning);
            }
            
            if (characterData.baseDamage < 15f)
            {
                EditorGUILayout.HelpBox("💡 Base Damage < 15 có vẻ thấp cho Warrior. Cân nhắc tăng lên 20-30.", MessageType.Info);
            }
        }
    }
    
    private string GetWarriorType(CharacterData data)
    {
        float healthRatio = data.maxHealth / 100f; // Baseline 100 HP
        float damageRatio = data.baseDamage / 20f; // Baseline 20 damage
        float speedRatio = data.moveSpeed / 3.5f; // Baseline 3.5 speed
        
        if (healthRatio > 1.5f && speedRatio < 0.8f)
            return "Tank (High HP, Slow)";
        else if (damageRatio > 1.5f && speedRatio > 1.2f)
            return "Berserker (High Damage, Fast)";
        else if (speedRatio > 1.3f)
            return "Scout (Fast, Mobile)";
        else if (healthRatio > 1.2f && damageRatio > 1.2f)
            return "Heavy Warrior (Balanced+)";
        else
            return "Standard Warrior";
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
