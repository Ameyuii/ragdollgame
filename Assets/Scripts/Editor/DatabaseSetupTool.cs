using UnityEngine;
using UnityEditor;
using System.IO;

public class DatabaseSetupTool : EditorWindow
{
    private CharacterDatabase database;
    private Vector2 scrollPosition;
    
    [MenuItem("Tools/Character System/Database Setup")]
    public static void ShowWindow()
    {
        GetWindow<DatabaseSetupTool>("Database Setup");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Character Database Setup Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        // Database reference
        database = (CharacterDatabase)EditorGUILayout.ObjectField("Character Database", database, typeof(CharacterDatabase), false);

        if (database == null)
        {
            EditorGUILayout.HelpBox("Please assign a Character Database or create a new one.", MessageType.Warning);
            
            if (GUILayout.Button("Create New Database"))
            {
                CreateNewDatabase();
            }
            
            if (GUILayout.Button("Load Existing Database"))
            {
                LoadExistingDatabase();
            }
            
            return;
        }

        EditorGUILayout.Space(10);
        
        // Quick setup buttons
        EditorGUILayout.LabelField("Quick Setup", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Setup Default Categories"))
        {
            SetupDefaultCategories();
        }
        
        if (GUILayout.Button("Auto-Import Existing Prefabs"))
        {
            AutoImportPrefabs();
        }
        
        if (GUILayout.Button("Validate and Fix Database"))
        {
            ValidateAndFixDatabase();
        }
        
        EditorGUILayout.Space(10);
        
        // Database status
        EditorGUILayout.LabelField("Database Status", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField($"Categories: {database.categories.Count}");
        EditorGUILayout.LabelField($"Characters: {database.characters.Count}");
        EditorGUILayout.LabelField($"Last Updated: {database.lastUpdated}");
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        // Categories overview
        if (database.categories.Count > 0)
        {
            EditorGUILayout.LabelField("Categories Overview", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            
            foreach (var category in database.categories)
            {
                EditorGUILayout.BeginHorizontal("box");
                EditorGUILayout.LabelField(category.displayName, EditorStyles.boldLabel);
                
                int characterCount = 0;
                foreach (var character in database.characters)
                {
                    if (character != null && character.CategoryID == category.categoryID)
                        characterCount++;
                }
                
                EditorGUILayout.LabelField($"({characterCount} characters)", GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }
    }

    private void CreateNewDatabase()
    {
        CharacterDatabase newDatabase = CreateInstance<CharacterDatabase>();
        
        // Ensure Resources folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        
        string path = "Assets/Resources/CharacterDatabase.asset";
        AssetDatabase.CreateAsset(newDatabase, path);
        AssetDatabase.SaveAssets();
        
        database = newDatabase;
        
        Debug.Log($"Created new Character Database at {path}");
    }

    private void LoadExistingDatabase()
    {
        string path = EditorUtility.OpenFilePanel("Select Character Database", "Assets", "asset");
        if (!string.IsNullOrEmpty(path))
        {
            // Convert absolute path to relative path
            if (path.StartsWith(Application.dataPath))
            {
                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                database = AssetDatabase.LoadAssetAtPath<CharacterDatabase>(relativePath);
                
                if (database == null)
                {
                    EditorUtility.DisplayDialog("Error", "Selected file is not a Character Database.", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please select a file within the Assets folder.", "OK");
            }
        }
    }

    private void SetupDefaultCategories()
    {
        if (database == null) return;

        string[] categories = { "ROBOT", "QUÁI VẬT", "CHIẾN BINH", "ZOMBIE" };
        string[] categoryIDs = { "robot", "quai_vat", "chien_binh", "zombie" };
        
        for (int i = 0; i < categories.Length; i++)
        {
            bool exists = false;
            foreach (var category in database.categories)
            {
                if (category.categoryID == categoryIDs[i])
                {
                    exists = true;
                    break;
                }
            }
            
            if (!exists)
            {
                CharacterCategory newCategory = new CharacterCategory
                {
                    categoryID = categoryIDs[i],
                    displayName = categories[i],
                    description = $"Danh mục {categories[i]}",
                    sortOrder = i,
                    isActive = true
                };
                
                database.categories.Add(newCategory);
            }
        }
        
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        
        Debug.Log("Setup default categories completed");
    }

    private void AutoImportPrefabs()
    {
        if (database == null) return;

        // Define mapping of prefab names to categories
        string[] robotPrefabs = { "robot", "mech", "droid" };
        string[] monsterPrefabs = { "monster", "beast", "creature", "bear" };
        string[] warriorPrefabs = { "warrior", "soldier", "fighter", "knight" };
        string[] zombiePrefabs = { "zombie", "undead" };

        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs", "Assets/Resources" });
        int importedCount = 0;
        
        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null && prefab.GetComponent<RagdollCharacter>() != null)
            {
                string prefabName = prefab.name.ToLower();
                string categoryID = "robot"; // default
                
                // Determine category based on name
                if (System.Array.Exists(robotPrefabs, name => prefabName.Contains(name)))
                    categoryID = "robot";
                else if (System.Array.Exists(monsterPrefabs, name => prefabName.Contains(name)))
                    categoryID = "quai_vat";
                else if (System.Array.Exists(warriorPrefabs, name => prefabName.Contains(name)))
                    categoryID = "chien_binh";
                else if (System.Array.Exists(zombiePrefabs, name => prefabName.Contains(name)))
                    categoryID = "zombie";
                
                // Check if character already exists
                bool exists = false;
                foreach (var character in database.characters)
                {
                    if (character != null && character.BasePrefab == prefab)
                    {
                        exists = true;
                        break;
                    }
                }
                
                if (!exists)
                {
                    // Create new character definition
                    CharacterDefinition newCharacter = CreateInstance<CharacterDefinition>();
                    newCharacter.CharacterID = prefabName.Replace(" ", "_");
                    newCharacter.DisplayName = prefab.name;  // Use original name with proper casing
                    newCharacter.CategoryID = categoryID;
                    
                    // Set the prefab reference
                    var serializedCharacter = new SerializedObject(newCharacter);
                    serializedCharacter.FindProperty("basePrefab").objectReferenceValue = prefab;
                    serializedCharacter.ApplyModifiedProperties();
                    
                    // Create folder if it doesn't exist
                    if (!AssetDatabase.IsValidFolder("Assets/CharacterDefinitions"))
                    {
                        AssetDatabase.CreateFolder("Assets", "CharacterDefinitions");
                    }
                    
                    // Save as asset
                    string assetPath = $"Assets/CharacterDefinitions/{newCharacter.CharacterID}_definition.asset";
                    AssetDatabase.CreateAsset(newCharacter, assetPath);
                    
                    // Add to database
                    database.characters.Add(newCharacter);
                    importedCount++;
                    
                    Debug.Log($"Imported character: {newCharacter.DisplayName} -> {categoryID}");
                }
            }
        }
        
        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        
        EditorUtility.DisplayDialog("Auto Import Complete", 
            $"Imported {importedCount} character(s) into the database.", "OK");
    }

    private void ValidateAndFixDatabase()
    {
        if (database == null) return;

        int fixedCount = 0;
        
        // Remove null references
        for (int i = database.characters.Count - 1; i >= 0; i--)
        {
            if (database.characters[i] == null)
            {
                database.characters.RemoveAt(i);
                fixedCount++;
            }
        }
        
        // Fix missing IDs
        foreach (var character in database.characters)
        {
            if (character != null && string.IsNullOrEmpty(character.CharacterID))
            {
                character.CharacterID = character.DisplayName.ToLower().Replace(" ", "_");
                EditorUtility.SetDirty(character);
                fixedCount++;
            }
        }
        
        // Fix missing display names
        foreach (var character in database.characters)
        {
            if (character != null && string.IsNullOrEmpty(character.DisplayName))
            {
                character.DisplayName = character.name.Replace("_definition", "").Replace("_", " ");
                EditorUtility.SetDirty(character);
                fixedCount++;
            }
        }
        
        if (fixedCount > 0)
        {
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
        }
        
        EditorUtility.DisplayDialog("Validation Complete", 
            $"Fixed {fixedCount} issue(s) in the database.", "OK");
    }
}