using UnityEngine;

[CreateAssetMenu(fileName = "GameDatabase", menuName = "Character System/Game Database")]
public class GameDatabase : ScriptableObject
{
    [Header("Database References")]
    public CharacterDatabase characterDatabase;

    [Header("Settings")]
    public bool enableNewSystem = false;
    public bool enableDebugLogging = true;

    // Singleton instance for runtime access
    private static GameDatabase _instance;
    public static GameDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GameDatabase>("GameDatabase");
                if (_instance == null)
                {
                    Debug.LogWarning("GameDatabase not found in Resources folder. Creating temporary instance.");
                    _instance = CreateInstance<GameDatabase>();
                }
            }
            return _instance;
        }
    }

    void OnEnable()
    {
        _instance = this;
        InitializeLookupTables();
    }

    /// <summary>
    /// Initialize lookup tables for performance
    /// </summary>
    private void InitializeLookupTables()
    {
        if (characterDatabase != null)
        {
            // The character database will handle its own lookup tables
        }
    }

    /// <summary>
    /// Get character definition by ID
    /// </summary>
    public CharacterDefinition GetCharacter(string characterID)
    {
        if (characterDatabase == null) return null;
        return characterDatabase.GetCharacter(characterID);
    }

    /// <summary>
    /// Get team configuration by ID
    /// </summary>
    public TeamConfiguration GetTeam(int teamID)
    {
        if (characterDatabase == null) return null;
        return characterDatabase.GetTeam(teamID);
    }

    /// <summary>
    /// Check if new system is enabled
    /// </summary>
    public bool IsNewSystemEnabled()
    {
        return enableNewSystem && characterDatabase != null;
    }

    /// <summary>
    /// Log debug message if debug logging is enabled
    /// </summary>
    public void DebugLog(string message)
    {
        if (enableDebugLogging)
        {
            Debug.Log($"[GameDatabase] {message}");
        }
    }
}