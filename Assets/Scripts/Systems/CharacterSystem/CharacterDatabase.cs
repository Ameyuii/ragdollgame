using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Character System/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    [Header("Database Info")]
    public string databaseVersion = "1.0";
    public string lastUpdated;

    [Header("Character Definitions")]
    public List<CharacterDefinition> characters = new List<CharacterDefinition>();
    public List<CharacterCategory> categories = new List<CharacterCategory>();

    [Header("Team Configurations")]
    public List<TeamConfiguration> teams = new List<TeamConfiguration>();

    // Runtime lookup optimization
    private Dictionary<string, CharacterDefinition> characterLookup;
    private Dictionary<string, List<CharacterDefinition>> categoryLookup;
    private Dictionary<int, TeamConfiguration> teamLookup;

    private void OnEnable()
    {
        InitializeLookupTables();
    }

    private void OnValidate()
    {
        // Update timestamp
        lastUpdated = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        // Initialize lists if null
        if (characters == null) characters = new List<CharacterDefinition>();
        if (categories == null) categories = new List<CharacterCategory>();
        if (teams == null) teams = new List<TeamConfiguration>();

        // Ensure we have default teams
        EnsureDefaultTeams();

        // Initialize lookup tables
        InitializeLookupTables();
    }

    /// <summary>
    /// Initialize lookup tables for fast runtime access
    /// </summary>
    private void InitializeLookupTables()
    {
        // Character lookup by ID
        characterLookup = new Dictionary<string, CharacterDefinition>();
        foreach (CharacterDefinition character in characters)
        {
            if (character != null && !string.IsNullOrEmpty(character.CharacterID))
            {
                characterLookup[character.CharacterID] = character;
            }
        }

        // Category lookup
        categoryLookup = new Dictionary<string, List<CharacterDefinition>>();
        foreach (CharacterDefinition character in characters)
        {
            if (character != null && !string.IsNullOrEmpty(character.CategoryID))
            {
                if (!categoryLookup.ContainsKey(character.CategoryID))
                {
                    categoryLookup[character.CategoryID] = new List<CharacterDefinition>();
                }
                categoryLookup[character.CategoryID].Add(character);
            }
        }

        // Team lookup by ID
        teamLookup = new Dictionary<int, TeamConfiguration>();
        foreach (TeamConfiguration team in teams)
        {
            if (team != null)
            {
                teamLookup[team.teamID] = team;
            }
        }
    }

    /// <summary>
    /// Get character definition by ID
    /// </summary>
    public CharacterDefinition GetCharacter(string characterID)
    {
        if (characterLookup == null) InitializeLookupTables();

        characterLookup.TryGetValue(characterID, out CharacterDefinition character);
        return character;
    }

    /// <summary>
    /// Get all characters in a category
    /// </summary>
    public List<CharacterDefinition> GetCharactersByCategory(string categoryID)
    {
        if (categoryLookup == null) InitializeLookupTables();

        categoryLookup.TryGetValue(categoryID, out List<CharacterDefinition> categoryCharacters);
        return categoryCharacters ?? new List<CharacterDefinition>();
    }

    /// <summary>
    /// Get all unlocked characters
    /// </summary>
    public List<CharacterDefinition> GetUnlockedCharacters()
    {
        return characters.Where(c => c != null && c.IsUnlocked()).ToList();
    }

    /// <summary>
    /// Get all unlocked characters in a category
    /// </summary>
    public List<CharacterDefinition> GetUnlockedCharactersByCategory(string categoryID)
    {
        return GetCharactersByCategory(categoryID).Where(c => c.IsUnlocked()).ToList();
    }

    /// <summary>
    /// Get team configuration by ID
    /// </summary>
    public TeamConfiguration GetTeam(int teamID)
    {
        if (teamLookup == null) InitializeLookupTables();

        teamLookup.TryGetValue(teamID, out TeamConfiguration team);
        return team;
    }

    /// <summary>
    /// Get all available teams
    /// </summary>
    public List<TeamConfiguration> GetAllTeams()
    {
        return teams.Where(t => t != null).ToList();
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    public CharacterCategory GetCategory(string categoryID)
    {
        return categories.Find(c => c.categoryID == categoryID);
    }

    /// <summary>
    /// Get all categories
    /// </summary>
    public List<CharacterCategory> GetAllCategories()
    {
        return categories.Where(c => c != null).ToList();
    }

    /// <summary>
    /// Add a new character definition
    /// </summary>
    public void AddCharacter(CharacterDefinition character)
    {
        if (character == null) return;

        if (!characters.Contains(character))
        {
            characters.Add(character);
            InitializeLookupTables();
        }
    }

    /// <summary>
    /// Remove a character definition
    /// </summary>
    public void RemoveCharacter(CharacterDefinition character)
    {
        if (character == null) return;

        characters.Remove(character);
        InitializeLookupTables();
    }

    /// <summary>
    /// Check if character exists
    /// </summary>
    public bool HasCharacter(string characterID)
    {
        return GetCharacter(characterID) != null;
    }

    /// <summary>
    /// Get available variants for a character
    /// </summary>
    public List<string> GetAvailableVariants(string characterID)
    {
        CharacterDefinition character = GetCharacter(characterID);
        if (character == null) return new List<string>();

        return character.Variants.Where(v => v.IsUnlocked()).Select(v => v.variantID).ToList();
    }

    /// <summary>
    /// Get character count by category
    /// </summary>
    public int GetCharacterCountByCategory(string categoryID)
    {
        return GetCharactersByCategory(categoryID).Count;
    }

    /// <summary>
    /// Ensure default teams exist
    /// </summary>
    private void EnsureDefaultTeams()
    {
        // Check if we have Team 1
        if (!teams.Any(t => t.teamID == 1))
        {
            TeamConfiguration team1 = new TeamConfiguration
            {
                teamID = 1,
                teamName = "Blue Team",
                teamDescription = "Default blue team",
                primaryColor = Color.blue,
                secondaryColor = Color.white,
                isPlayerTeam = true
            };
            teams.Add(team1);
        }

        // Check if we have Team 2
        if (!teams.Any(t => t.teamID == 2))
        {
            TeamConfiguration team2 = new TeamConfiguration
            {
                teamID = 2,
                teamName = "Red Team",
                teamDescription = "Default red team",
                primaryColor = Color.red,
                secondaryColor = Color.white,
                isPlayerTeam = false
            };
            teams.Add(team2);
        }
    }

    /// <summary>
    /// Create database from existing character assets
    /// </summary>
    [ContextMenu("Rebuild Database")]
    public void RebuildDatabase()
    {
#if UNITY_EDITOR
        // Clear existing data
        characters.Clear();
        
        // Find all CharacterDefinition assets
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:CharacterDefinition");
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            CharacterDefinition character = UnityEditor.AssetDatabase.LoadAssetAtPath<CharacterDefinition>(path);
            if (character != null && !characters.Contains(character))
            {
                characters.Add(character);
            }
        }
#endif

        // Sort by category and sort order
        characters = characters.OrderBy(c => c.CategoryID).ThenBy(c => c.SortOrder).ToList();

        // Rebuild lookup tables
        InitializeLookupTables();

        Debug.Log($"Database rebuilt with {characters.Count} characters.");
    }
}

[System.Serializable]
public class CharacterCategory
{
    [Header("Category Info")]
    public string categoryID;
    public string displayName;
    [TextArea(2, 3)]
    public string description;

    [Header("Visual")]
    public Sprite categoryIcon;
    public Color categoryColor = Color.white;

    [Header("Settings")]
    public int sortOrder;
    public bool isActive = true;

    public CharacterCategory()
    {
    }

    public CharacterCategory(string id, string name, string desc = "")
    {
        categoryID = id;
        displayName = name;
        description = desc;
        isActive = true;
    }
}