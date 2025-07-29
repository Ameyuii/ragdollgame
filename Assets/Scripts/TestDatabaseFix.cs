using UnityEngine;

public class TestDatabaseFix
{
    public static void Execute()
    {
        Debug.Log("🔍 Testing CharacterDatabase fix...");
        
        // Get GameDatabase instance
        GameDatabase gameDB = GameDatabase.Instance;
        if (gameDB == null)
        {
            Debug.LogError("❌ GameDatabase.Instance is null!");
            return;
        }
        
        if (gameDB.characterDatabase == null)
        {
            Debug.LogError("❌ GameDatabase.characterDatabase is null!");
            return;
        }
        
        // Test getting robot_basic_default_01
        string testCharacterId = "robot_basic_default_01";
        CharacterDefinition charDef = gameDB.characterDatabase.GetCharacter(testCharacterId);
        
        if (charDef != null)
        {
            Debug.Log($"✅ Found character: {charDef.DisplayName} ({charDef.CharacterID})");
            Debug.Log($"   - Category: {charDef.CategoryID}");
            Debug.Log($"   - Prefab: {charDef.BasePrefab?.name}");
        }
        else
        {
            Debug.LogError($"❌ Character not found: {testCharacterId}");
            
            // List all characters in database
            Debug.Log("📋 All characters in database:");
            var allChars = gameDB.characterDatabase.characters;
            for (int i = 0; i < allChars.Count; i++)
            {
                if (allChars[i] != null)
                {
                    Debug.Log($"   {i}: {allChars[i].CharacterID} ({allChars[i].DisplayName})");
                }
            }
        }
        
        Debug.Log("🔍 CharacterDatabase test completed!");
    }
}