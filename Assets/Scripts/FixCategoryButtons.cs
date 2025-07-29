using UnityEngine;
using UnityEngine.UI;

public class FixCategoryButtons
{
    public static void Execute()
    {
        Debug.Log("🔧 Fixing category buttons...");
        
        // Find AutoUIGenerator
        GameObject autoUIGenObj = GameObject.Find("AutoUIGenerator");
        if (autoUIGenObj == null)
        {
            Debug.LogError("❌ AutoUIGenerator not found!");
            return;
        }
        
        AutoUIGenerator autoUIGen = autoUIGenObj.GetComponent<AutoUIGenerator>();
        if (autoUIGen == null)
        {
            Debug.LogError("❌ AutoUIGenerator component not found!");
            return;
        }
        
        // Setup category buttons manually
        SetupCategoryButton("RobotButton", "robot", autoUIGen);
        SetupCategoryButton("MonsterButton", "quaivat", autoUIGen);
        SetupCategoryButton("WarriorButton", "chienbinh", autoUIGen);
        SetupCategoryButton("ZombieButton", "zombie", autoUIGen);
        
        Debug.Log("✅ Category buttons fixed!");
    }
    
    private static void SetupCategoryButton(string buttonName, string category, AutoUIGenerator autoUIGen)
    {
        GameObject buttonObj = GameObject.Find(buttonName);
        if (buttonObj != null)
        {
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => {
                    Debug.Log($"🖱️ {buttonName} clicked - selecting category: {category}");
                    autoUIGen.SelectCategory(category);
                });
                Debug.Log($"✅ Setup category button: {buttonName} → {category}");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Button not found: {buttonName}");
        }
    }
}