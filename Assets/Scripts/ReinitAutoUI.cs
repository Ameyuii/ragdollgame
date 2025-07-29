using UnityEngine;

public class ReinitAutoUI
{
    public static void Execute()
    {
        Debug.Log("🔄 Reinitializing AutoUIGenerator...");
        
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
        
        // Disable and enable to trigger Start() again
        autoUIGen.enabled = false;
        autoUIGen.enabled = true;
        
        Debug.Log("✅ AutoUIGenerator reinitialized!");
        
        // Wait a frame then test MonsterButton
        UnityEngine.Object.FindFirstObjectByType<UnityEngine.MonoBehaviour>().StartCoroutine(TestAfterFrame());
    }
    
    private static System.Collections.IEnumerator TestAfterFrame()
    {
        yield return null; // Wait one frame
        
        Debug.Log("🔍 Testing MonsterButton after reinit...");
        
        GameObject buttonObj = GameObject.Find("MonsterButton");
        if (buttonObj != null)
        {
            UnityEngine.UI.Button button = buttonObj.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                Debug.Log($"🔍 MonsterButton listeners after reinit: {button.onClick.GetPersistentEventCount()}");
                button.onClick.Invoke();
            }
        }
    }
}