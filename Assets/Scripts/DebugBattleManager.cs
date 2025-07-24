using UnityEngine;

public class DebugBattleManager : MonoBehaviour
{
    public static void Execute()
    {
        BattleGameManager manager = FindObjectOfType<BattleGameManager>();
        if (manager != null)
        {
            Debug.Log($"BattleGameManager found: setupMode={manager.setupMode}, gameStarted={manager.gameStarted}");
            Debug.Log($"setupPanel: {(manager.setupPanel != null ? "exists" : "null")}");
            Debug.Log($"characterPrefabs count: {(manager.characterPrefabs != null ? manager.characterPrefabs.Length : 0)}");
            
            // Force initialize setup mode
            manager.InitializeSetupMode();
        }
        else
        {
            Debug.LogError("BattleGameManager not found!");
        }
    }
}