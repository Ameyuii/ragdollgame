using UnityEngine;

public class TestZombieFixed
{
    public static void Execute()
    {
        Debug.Log("🔍 Testing Zombie after fix...");
        
        GameDatabase gameDB = GameDatabase.Instance;
        if (gameDB?.characterDatabase != null)
        {
            // Force reinitialize lookup tables
            gameDB.characterDatabase.InitializeLookupTables();
            
            var zombieChars = gameDB.characterDatabase.GetCharactersByCategory("zombie");
            Debug.Log($"📊 Found {zombieChars.Count} characters in 'zombie' category");
            
            foreach (var character in zombieChars)
            {
                if (character != null)
                {
                    Debug.Log($"🧟 Character: {character.CharacterID} ({character.DisplayName})");
                    Debug.Log($"   - BasePrefab: {(character.BasePrefab != null ? character.BasePrefab.name : "NULL")}");
                    
                    if (character.BasePrefab != null)
                    {
                        Debug.Log($"   ✅ Prefab is valid and ready for UI!");
                    }
                }
            }
        }
        
        Debug.Log("🔍 Zombie fix test completed!");
    }
}