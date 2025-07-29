using UnityEngine;

public class ShowUIPanel : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("👁️ Showing UI Panel...");
        
        // Find and activate BottomCharacterPanel
        GameObject bottomPanel = GameObject.Find("BottomCharacterPanel");
        if (bottomPanel != null)
        {
            bottomPanel.SetActive(true);
            Debug.Log("✅ BottomCharacterPanel activated!");
        }
        else
        {
            Debug.LogError("❌ BottomCharacterPanel not found!");
        }
        
        // Make sure robot category is selected
        AutoUIGenerator autoUIGen = FindFirstObjectByType<AutoUIGenerator>();
        if (autoUIGen != null)
        {
            // Force change to ensure SelectCategory works
            System.Reflection.FieldInfo categoryField = typeof(AutoUIGenerator).GetField("currentCategory", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (categoryField != null)
            {
                categoryField.SetValue(autoUIGen, "temp");
            }
            
            autoUIGen.SelectCategory("robot");
            Debug.Log("🤖 Robot category selected!");
        }
        
        Debug.Log("✅ UI Panel setup completed!");
    }
}