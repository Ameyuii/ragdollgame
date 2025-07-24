using UnityEngine;

public class FixUISetup : MonoBehaviour
{
    public static void Execute()
    {
        // Find BattleGameManager and force recreate UI
        BattleGameManager manager = FindObjectOfType<BattleGameManager>();
        if (manager != null)
        {
            Debug.Log("Recreating UI setup...");
            
            // Clear existing setup panel content
            GameObject setupPanel = GameObject.Find("SetupPanel");
            if (setupPanel != null)
            {
                // Clear all children except CharacterList
                Transform characterList = setupPanel.transform.Find("CharacterList");
                
                // Clear CharacterList children
                if (characterList != null)
                {
                    for (int i = characterList.childCount - 1; i >= 0; i--)
                    {
                        DestroyImmediate(characterList.GetChild(i).gameObject);
                    }
                }
                
                // Reinitialize
                manager.InitializeSetupMode();
            }
        }
        else
        {
            Debug.LogError("BattleGameManager not found!");
        }
    }
}