using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(CharacterDatabase))]
public class CharacterDatabaseManager : Editor
{
    private CharacterDatabase database;
    private Vector2 scrollPosition;
    private int selectedTab = 0;
    private string[] tabNames = { "Characters", "Categories", "Tools" };
    
    // Character Management
    private bool showCharacterList = true;
    private bool showAddCharacter = false;
    private string newCharacterName = "";
    private string newCharacterID = "";
    private int selectedCategoryIndex = 0;
    
    // Category Management
    private bool showCategoryList = true;
    private bool showAddCategory = false;
    private string newCategoryName = "";
    private string newCategoryID = "";
    
    private void OnEnable()
    {
        database = (CharacterDatabase)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawHeader();
        DrawTabs();
        
        EditorGUILayout.Space(10);
        
        switch (selectedTab)
        {
            case 0: DrawCharactersTab(); break;
            case 1: DrawCategoriesTab(); break;
            case 2: DrawToolsTab(); break;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical("box");
        
        EditorGUILayout.LabelField("Character Database Manager", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Version: {database.databaseVersion}");
        EditorGUILayout.LabelField($"Last Updated: {database.lastUpdated}");
        EditorGUILayout.LabelField($"Characters: {database.characters.Count}");
        EditorGUILayout.LabelField($"Categories: {database.categories.Count}");
        
        EditorGUILayout.EndVertical();
    }

    private void DrawTabs()
    {
        selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
    }

    private void DrawCharactersTab()
    {
        EditorGUILayout.BeginVertical();
        
        // Add Character Button
        EditorGUILayout.BeginHorizontal();
        showCharacterList = EditorGUILayout.Foldout(showCharacterList, $"Characters ({database.characters.Count})", true);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add New Character", GUILayout.Width(150)))
        {
            showAddCharacter = !showAddCharacter;
        }
        EditorGUILayout.EndHorizontal();

        if (showAddCharacter)
        {
            DrawAddCharacterSection();
        }

        if (showCharacterList)
        {
            DrawCharacterList();
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawAddCharacterSection()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Add New Character", EditorStyles.boldLabel);
        
        newCharacterName = EditorGUILayout.TextField("Display Name", newCharacterName);
        newCharacterID = EditorGUILayout.TextField("Character ID", newCharacterID);
        
        // Auto-generate ID from name
        if (!string.IsNullOrEmpty(newCharacterName) && string.IsNullOrEmpty(newCharacterID))
        {
            newCharacterID = newCharacterName.ToLower().Replace(" ", "_");
        }
        
        // Category dropdown
        if (database.categories.Count > 0)
        {
            string[] categoryNames = database.categories.Select(c => c.displayName).ToArray();
            selectedCategoryIndex = EditorGUILayout.Popup("Category", selectedCategoryIndex, categoryNames);
        }
        else
        {
            EditorGUILayout.LabelField("No categories available. Create categories first.");
        }
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Character"))
        {
            CreateNewCharacter();
        }
        if (GUILayout.Button("Cancel"))
        {
            showAddCharacter = false;
            ResetAddCharacterFields();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
    }

    private void DrawCharacterList()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
        
        for (int i = 0; i < database.characters.Count; i++)
        {
            var character = database.characters[i];
            if (character == null) continue;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            
            // Character info
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(character.DisplayName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"ID: {character.CharacterID}");
            EditorGUILayout.LabelField($"Category: {character.CategoryID}");
            
            // Show prefab info
            if (character.BasePrefab != null)
            {
                EditorGUILayout.LabelField($"Prefab: {character.BasePrefab.name}");
            }
            else
            {
                EditorGUILayout.LabelField("Prefab: [Missing]", EditorStyles.centeredGreyMiniLabel);
            }
            EditorGUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            
            // Actions
            EditorGUILayout.BeginVertical(GUILayout.Width(100));
            if (GUILayout.Button("Edit"))
            {
                Selection.activeObject = character;
                EditorGUIUtility.PingObject(character);
            }
            if (GUILayout.Button("Remove") && EditorUtility.DisplayDialog("Remove Character", 
                $"Are you sure you want to remove {character.DisplayName}?", "Yes", "No"))
            {
                database.characters.RemoveAt(i);
                EditorUtility.SetDirty(database);
                break;
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.EndScrollView();
    }

    private void DrawCategoriesTab()
    {
        EditorGUILayout.BeginVertical();
        
        // Add Category Button
        EditorGUILayout.BeginHorizontal();
        showCategoryList = EditorGUILayout.Foldout(showCategoryList, $"Categories ({database.categories.Count})", true);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add New Category", GUILayout.Width(150)))
        {
            showAddCategory = !showAddCategory;
        }
        EditorGUILayout.EndHorizontal();

        if (showAddCategory)
        {
            DrawAddCategorySection();
        }

        if (showCategoryList)
        {
            DrawCategoryList();
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawAddCategorySection()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Add New Category", EditorStyles.boldLabel);
        
        newCategoryName = EditorGUILayout.TextField("Category Name", newCategoryName);
        newCategoryID = EditorGUILayout.TextField("Category ID", newCategoryID);
        
        // Auto-generate ID from name
        if (!string.IsNullOrEmpty(newCategoryName) && string.IsNullOrEmpty(newCategoryID))
        {
            newCategoryID = newCategoryName.ToLower().Replace(" ", "_");
        }
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Category"))
        {
            CreateNewCategory();
        }
        if (GUILayout.Button("Cancel"))
        {
            showAddCategory = false;
            ResetAddCategoryFields();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
    }

    private void DrawCategoryList()
    {
        for (int i = 0; i < database.categories.Count; i++)
        {
            var category = database.categories[i];
            if (category == null) continue;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            
            // Category info
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(category.displayName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"ID: {category.categoryID}");
            
            // Count characters in this category
            int characterCount = database.characters.Count(c => c != null && c.CategoryID == category.categoryID);
            EditorGUILayout.LabelField($"Characters: {characterCount}");
            EditorGUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            
            // Actions
            if (GUILayout.Button("Remove", GUILayout.Width(80)) && 
                EditorUtility.DisplayDialog("Remove Category", 
                $"Are you sure you want to remove {category.displayName}?", "Yes", "No"))
            {
                database.categories.RemoveAt(i);
                EditorUtility.SetDirty(database);
                break;
            }
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawToolsTab()
    {
        EditorGUILayout.LabelField("Database Tools", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("Auto-Generate Character IDs"))
        {
            AutoGenerateCharacterIDs();
        }
        
        if (GUILayout.Button("Validate Database"))
        {
            ValidateDatabase();
        }
        
        if (GUILayout.Button("Create Default Categories"))
        {
            CreateDefaultCategories();
        }
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("Scan Project for Character Prefabs"))
        {
            ScanForCharacterPrefabs();
        }

        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("Clear All Data"))
        {
            if (EditorUtility.DisplayDialog("Clear Database", 
                "This will remove all characters and categories. Are you sure?", "Yes", "No"))
            {
                database.characters.Clear();
                database.categories.Clear();
                EditorUtility.SetDirty(database);
            }
        }
    }

    private void CreateNewCharacter()
    {
        if (string.IsNullOrEmpty(newCharacterName) || string.IsNullOrEmpty(newCharacterID))
        {
            EditorUtility.DisplayDialog("Error", "Please fill in all required fields.", "OK");
            return;
        }

        // Check if ID already exists
        if (database.characters.Any(c => c != null && c.CharacterID == newCharacterID))
        {
            EditorUtility.DisplayDialog("Error", "Character ID already exists!", "OK");
            return;
        }

        // Create new CharacterDefinition asset
        CharacterDefinition newCharacter = CreateInstance<CharacterDefinition>();
        newCharacter.CharacterID = newCharacterID;
        newCharacter.DisplayName = newCharacterName;
        
        // Set category if available
        if (database.categories.Count > 0 && selectedCategoryIndex < database.categories.Count)
        {
            newCharacter.CategoryID = database.categories[selectedCategoryIndex].categoryID;
        }

        // Create folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/CharacterDefinitions"))
        {
            AssetDatabase.CreateFolder("Assets", "CharacterDefinitions");
        }

        // Save as asset
        string path = $"Assets/CharacterDefinitions/{newCharacterID}_definition.asset";
        AssetDatabase.CreateAsset(newCharacter, path);
        
        // Add to database
        database.characters.Add(newCharacter);
        
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        
        showAddCharacter = false;
        ResetAddCharacterFields();
        
        Debug.Log($"Created new character: {newCharacterName} at {path}");
    }

    private void CreateNewCategory()
    {
        if (string.IsNullOrEmpty(newCategoryName) || string.IsNullOrEmpty(newCategoryID))
        {
            EditorUtility.DisplayDialog("Error", "Please fill in all required fields.", "OK");
            return;
        }

        // Check if ID already exists
        if (database.categories.Any(c => c.categoryID == newCategoryID))
        {
            EditorUtility.DisplayDialog("Error", "Category ID already exists!", "OK");
            return;
        }

        CharacterCategory newCategory = new CharacterCategory
        {
            categoryID = newCategoryID,
            displayName = newCategoryName,
            description = "",
            sortOrder = database.categories.Count
        };

        database.categories.Add(newCategory);
        EditorUtility.SetDirty(database);
        
        showAddCategory = false;
        ResetAddCategoryFields();
        
        Debug.Log($"Created new category: {newCategoryName}");
    }

    private void AutoGenerateCharacterIDs()
    {
        int updated = 0;
        foreach (var character in database.characters)
        {
            if (character != null && string.IsNullOrEmpty(character.CharacterID))
            {
                character.CharacterID = character.DisplayName.ToLower().Replace(" ", "_");
                EditorUtility.SetDirty(character);
                updated++;
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"Auto-generated {updated} character IDs");
    }

    private void ValidateDatabase()
    {
        List<string> issues = new List<string>();
        
        // Check for duplicate IDs
        var duplicateCharacters = database.characters
            .Where(c => c != null)
            .GroupBy(c => c.CharacterID)
            .Where(g => g.Count() > 1);
            
        foreach (var group in duplicateCharacters)
        {
            issues.Add($"Duplicate character ID: {group.Key}");
        }
        
        // Check for missing references
        foreach (var character in database.characters)
        {
            if (character == null)
            {
                issues.Add("Found null character reference");
                continue;
            }
            
            if (character.BasePrefab == null)
            {
                issues.Add($"Character {character.DisplayName} has no base prefab");
            }
            
            if (string.IsNullOrEmpty(character.DisplayName))
            {
                issues.Add($"Character {character.CharacterID} has no display name");
            }
        }
        
        if (issues.Count == 0)
        {
            EditorUtility.DisplayDialog("Validation", "Database validation passed!", "OK");
        }
        else
        {
            string message = "Issues found:\n" + string.Join("\n", issues);
            EditorUtility.DisplayDialog("Validation Issues", message, "OK");
        }
    }

    private void CreateDefaultCategories()
    {
        string[] defaultCategories = { "ROBOT", "QUÁI VẬT", "CHIẾN BINH", "ZOMBIE" };
        
        foreach (string categoryName in defaultCategories)
        {
            string categoryID = categoryName.ToLower().Replace(" ", "_").Replace("ạ", "a").Replace("ế", "e").Replace("ì", "i").Replace("ộ", "o").Replace("ị", "i");
            
            if (!database.categories.Any(c => c.categoryID == categoryID))
            {
                CharacterCategory newCategory = new CharacterCategory
                {
                    categoryID = categoryID,
                    displayName = categoryName,
                    description = $"Danh mục {categoryName}",
                    sortOrder = database.categories.Count
                };
                
                database.categories.Add(newCategory);
            }
        }
        
        EditorUtility.SetDirty(database);
        Debug.Log("Created default categories");
    }

    private void ScanForCharacterPrefabs()
    {
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs", "Assets/Resources" });
        List<string> foundPrefabs = new List<string>();
        
        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null && prefab.GetComponent<RagdollCharacter>() != null)
            {
                foundPrefabs.Add($"{prefab.name} -> {path}");
            }
        }
        
        if (foundPrefabs.Count > 0)
        {
            string message = "Found character prefabs:\n" + string.Join("\n", foundPrefabs);
            Debug.Log(message);
            EditorUtility.DisplayDialog("Prefab Scan Results", message, "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Prefab Scan Results", "No character prefabs found.", "OK");
        }
    }

    private void ResetAddCharacterFields()
    {
        newCharacterName = "";
        newCharacterID = "";
        selectedCategoryIndex = 0;
    }

    private void ResetAddCategoryFields()
    {
        newCategoryName = "";
        newCategoryID = "";
    }
}