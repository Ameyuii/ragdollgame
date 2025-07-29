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

    // Enhanced character creation fields
    private GameObject newCharacterPrefab;
    private Sprite newCharacterIcon;
    private RuntimeAnimatorController newAnimatorController;
    private string newCharacterDescription = "";

    // Stats fields
    private float newMaxHealth = 100f;
    private float newMoveSpeed = 5f;
    private float newAttackDamage = 25f;
    private float newAttackRange = 2f;
    private float newAttackCooldown = 1f;

    // Auto-extraction settings
    private bool autoExtractStats = true;
    private bool autoGenerateIcon = true;
    private bool showAdvancedOptions = false;
    
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

    private new void DrawHeader()
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
        EditorGUILayout.LabelField("🎯 Complete Character Creation", EditorStyles.boldLabel);

        // Basic Info Section
        EditorGUILayout.LabelField("📝 Basic Information", EditorStyles.miniBoldLabel);
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

        newCharacterDescription = EditorGUILayout.TextArea(newCharacterDescription, GUILayout.Height(40));

        EditorGUILayout.Space(5);

        // Assets Section
        EditorGUILayout.LabelField("🎨 Visual Assets", EditorStyles.miniBoldLabel);

        GameObject oldPrefab = newCharacterPrefab;
        newCharacterPrefab = (GameObject)EditorGUILayout.ObjectField("Base Prefab", newCharacterPrefab, typeof(GameObject), false);

        // Auto-extract when prefab changes
        if (newCharacterPrefab != oldPrefab && newCharacterPrefab != null && autoExtractStats)
        {
            ExtractStatsFromPrefab();
        }

        newCharacterIcon = (Sprite)EditorGUILayout.ObjectField("UI Icon", newCharacterIcon, typeof(Sprite), false);
        newAnimatorController = (RuntimeAnimatorController)EditorGUILayout.ObjectField("Animator Controller", newAnimatorController, typeof(RuntimeAnimatorController), false);

        EditorGUILayout.Space(5);

        // Stats Section
        showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "⚙️ Character Stats");
        if (showAdvancedOptions)
        {
            EditorGUI.indentLevel++;
            newMaxHealth = EditorGUILayout.FloatField("Max Health", newMaxHealth);
            newMoveSpeed = EditorGUILayout.FloatField("Move Speed", newMoveSpeed);
            newAttackDamage = EditorGUILayout.FloatField("Attack Damage", newAttackDamage);
            newAttackRange = EditorGUILayout.FloatField("Attack Range", newAttackRange);
            newAttackCooldown = EditorGUILayout.FloatField("Attack Cooldown", newAttackCooldown);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(5);

        // Auto-extraction options
        EditorGUILayout.LabelField("🔧 Auto-Extraction Options", EditorStyles.miniBoldLabel);
        autoExtractStats = EditorGUILayout.Toggle("Auto Extract Stats from Prefab", autoExtractStats);
        autoGenerateIcon = EditorGUILayout.Toggle("Auto Generate Icon", autoGenerateIcon);

        EditorGUILayout.Space(10);

        // Action buttons
        EditorGUILayout.BeginHorizontal();

        GUI.enabled = !string.IsNullOrEmpty(newCharacterName) && newCharacterPrefab != null;
        if (GUILayout.Button("🎯 Create Complete Character", GUILayout.Height(30)))
        {
            CreateCompleteCharacter();
        }
        GUI.enabled = true;

        if (GUILayout.Button("Cancel", GUILayout.Height(30)))
        {
            showAddCharacter = false;
            ResetAddCharacterFields();
        }
        EditorGUILayout.EndHorizontal();

        // Validation messages
        if (string.IsNullOrEmpty(newCharacterName))
        {
            EditorGUILayout.HelpBox("⚠️ Display Name is required", MessageType.Warning);
        }
        if (newCharacterPrefab == null)
        {
            EditorGUILayout.HelpBox("⚠️ Base Prefab is required", MessageType.Warning);
        }

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

        EditorGUILayout.Space(5);

        // Batch operations
        EditorGUILayout.LabelField("🚀 Batch Operations", EditorStyles.miniBoldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("📦 Import from Prefabs Folder"))
        {
            BatchImportFromPrefabsFolder();
        }

        if (GUILayout.Button("🎨 Auto-Assign Icons"))
        {
            BatchAutoAssignIcons();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🎭 Auto-Assign Animators"))
        {
            BatchAutoAssignAnimators();
        }

        if (GUILayout.Button("📊 Extract All Stats"))
        {
            BatchExtractStats();
        }
        EditorGUILayout.EndHorizontal();

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

    private void CreateCompleteCharacter()
    {
        if (string.IsNullOrEmpty(newCharacterName) || string.IsNullOrEmpty(newCharacterID))
        {
            EditorUtility.DisplayDialog("Error", "Please fill in all required fields.", "OK");
            return;
        }

        if (newCharacterPrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Base Prefab is required!", "OK");
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

        // Use SerializedObject to set private fields
        SerializedObject serializedCharacter = new SerializedObject(newCharacter);

        // Set visual assets
        serializedCharacter.FindProperty("basePrefab").objectReferenceValue = newCharacterPrefab;
        serializedCharacter.FindProperty("uiIcon").objectReferenceValue = newCharacterIcon;
        serializedCharacter.FindProperty("animatorController").objectReferenceValue = newAnimatorController;
        serializedCharacter.FindProperty("description").stringValue = newCharacterDescription;

        // Set stats
        SerializedProperty statsProperty = serializedCharacter.FindProperty("baseStats");
        statsProperty.FindPropertyRelative("maxHealth").floatValue = newMaxHealth;
        statsProperty.FindPropertyRelative("moveSpeed").floatValue = newMoveSpeed;
        statsProperty.FindPropertyRelative("attackDamage").floatValue = newAttackDamage;
        statsProperty.FindPropertyRelative("attackRange").floatValue = newAttackRange;
        statsProperty.FindPropertyRelative("attackCooldown").floatValue = newAttackCooldown;

        // Create default variant
        SerializedProperty variantsProperty = serializedCharacter.FindProperty("variants");
        variantsProperty.arraySize = 1;
        SerializedProperty variantElement = variantsProperty.GetArrayElementAtIndex(0);

        variantElement.FindPropertyRelative("variantID").stringValue = "default";
        variantElement.FindPropertyRelative("variantName").stringValue = "Default";
        variantElement.FindPropertyRelative("description").stringValue = "Default variant";
        variantElement.FindPropertyRelative("isDefault").boolValue = true;
        variantElement.FindPropertyRelative("customPrefab").objectReferenceValue = newCharacterPrefab;

        // Auto-generate icon if needed
        if (newCharacterIcon == null && autoGenerateIcon)
        {
            Sprite generatedIcon = GenerateIconFromPrefab(newCharacterPrefab);
            if (generatedIcon != null)
            {
                serializedCharacter.FindProperty("uiIcon").objectReferenceValue = generatedIcon;
            }
        }

        // Apply changes
        serializedCharacter.ApplyModifiedProperties();

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

        Debug.Log($"✅ Created complete character: {newCharacterName} at {path}");
        EditorUtility.DisplayDialog("Success", $"Character '{newCharacterName}' created successfully!", "OK");
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

        // Reset enhanced fields
        newCharacterPrefab = null;
        newCharacterIcon = null;
        newAnimatorController = null;
        newCharacterDescription = "";

        // Reset stats to defaults
        newMaxHealth = 100f;
        newMoveSpeed = 5f;
        newAttackDamage = 25f;
        newAttackRange = 2f;
        newAttackCooldown = 1f;

        showAdvancedOptions = false;
    }

    private void ExtractStatsFromPrefab()
    {
        if (newCharacterPrefab == null) return;

        // Try to extract stats from RagdollCharacter component
        RagdollCharacter ragdoll = newCharacterPrefab.GetComponent<RagdollCharacter>();
        if (ragdoll != null)
        {
            newMaxHealth = ragdoll.maxHealth;
            newMoveSpeed = ragdoll.moveSpeed;
            newAttackDamage = ragdoll.attackDamage;
            newAttackRange = ragdoll.attackRange;
            newAttackCooldown = ragdoll.attackCooldown;

            Debug.Log($"📊 Extracted stats from {newCharacterPrefab.name}");
        }

        // Auto-extract animator controller
        Animator animator = newCharacterPrefab.GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            newAnimatorController = animator.runtimeAnimatorController;
            Debug.Log($"🎭 Extracted animator controller from {newCharacterPrefab.name}");
        }

        // Auto-generate character ID if empty
        if (string.IsNullOrEmpty(newCharacterID) && !string.IsNullOrEmpty(newCharacterName))
        {
            string categoryPrefix = "";
            if (database.categories.Count > 0 && selectedCategoryIndex < database.categories.Count)
            {
                categoryPrefix = database.categories[selectedCategoryIndex].categoryID + "_";
            }
            newCharacterID = categoryPrefix + newCharacterName.ToLower().Replace(" ", "_");
        }
    }

    private Sprite GenerateIconFromPrefab(GameObject prefab)
    {
        // This is a placeholder - in a real implementation, you might:
        // 1. Take a screenshot of the prefab
        // 2. Use a default icon based on category
        // 3. Extract icon from prefab's renderer

        Debug.Log($"🖼️ Auto-generating icon for {prefab.name} (placeholder)");
        return null; // Return generated sprite
    }

    private void ResetAddCategoryFields()
    {
        newCategoryName = "";
        newCategoryID = "";
    }

    #region Batch Operations

    private void BatchImportFromPrefabsFolder()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
        int importedCount = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null && prefab.GetComponent<RagdollCharacter>() != null)
            {
                string characterName = prefab.name;
                string characterID = characterName.ToLower().Replace(" ", "_");

                // Skip if already exists
                if (database.characters.Any(c => c != null && c.CharacterID == characterID))
                    continue;

                // Determine category from folder structure
                string categoryID = DetermineCategoryFromPath(path);

                // Create character definition
                CharacterDefinition newCharacter = CreateInstance<CharacterDefinition>();
                newCharacter.CharacterID = characterID;
                newCharacter.DisplayName = characterName;
                newCharacter.CategoryID = categoryID;

                // Use SerializedObject to set private fields
                SerializedObject serializedCharacter = new SerializedObject(newCharacter);
                serializedCharacter.FindProperty("basePrefab").objectReferenceValue = prefab;

                // Extract stats
                RagdollCharacter ragdoll = prefab.GetComponent<RagdollCharacter>();
                if (ragdoll != null)
                {
                    SerializedProperty statsProperty = serializedCharacter.FindProperty("baseStats");
                    statsProperty.FindPropertyRelative("maxHealth").floatValue = ragdoll.maxHealth;
                    statsProperty.FindPropertyRelative("moveSpeed").floatValue = ragdoll.moveSpeed;
                    statsProperty.FindPropertyRelative("attackDamage").floatValue = ragdoll.attackDamage;
                    statsProperty.FindPropertyRelative("attackRange").floatValue = ragdoll.attackRange;
                    statsProperty.FindPropertyRelative("attackCooldown").floatValue = ragdoll.attackCooldown;
                }

                // Create default variant
                SerializedProperty variantsProperty = serializedCharacter.FindProperty("variants");
                variantsProperty.arraySize = 1;
                SerializedProperty variantElement = variantsProperty.GetArrayElementAtIndex(0);

                variantElement.FindPropertyRelative("variantID").stringValue = "default";
                variantElement.FindPropertyRelative("variantName").stringValue = "Default";
                variantElement.FindPropertyRelative("description").stringValue = "Default variant";
                variantElement.FindPropertyRelative("isDefault").boolValue = true;
                variantElement.FindPropertyRelative("customPrefab").objectReferenceValue = prefab;

                // Apply changes
                serializedCharacter.ApplyModifiedProperties();

                // Save asset
                string assetPath = $"Assets/CharacterDefinitions/{characterID}_definition.asset";
                AssetDatabase.CreateAsset(newCharacter, assetPath);
                database.characters.Add(newCharacter);

                importedCount++;
            }
        }

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log($"📦 Batch imported {importedCount} characters from prefabs folder");
        EditorUtility.DisplayDialog("Batch Import", $"Successfully imported {importedCount} characters!", "OK");
    }

    private void BatchAutoAssignIcons()
    {
        int assignedCount = 0;

        foreach (var character in database.characters)
        {
            if (character != null && character.UIIcon == null)
            {
                // Try to find icon based on character name
                string[] iconGuids = AssetDatabase.FindAssets($"{character.DisplayName} t:Sprite");
                if (iconGuids.Length > 0)
                {
                    string iconPath = AssetDatabase.GUIDToAssetPath(iconGuids[0]);
                    Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
                    if (icon != null)
                    {
                        SerializedObject serializedCharacter = new SerializedObject(character);
                        serializedCharacter.FindProperty("uiIcon").objectReferenceValue = icon;
                        serializedCharacter.ApplyModifiedProperties();
                        assignedCount++;
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"🎨 Auto-assigned {assignedCount} icons");
        EditorUtility.DisplayDialog("Auto-Assign Icons", $"Assigned {assignedCount} icons!", "OK");
    }

    private void BatchAutoAssignAnimators()
    {
        int assignedCount = 0;

        foreach (var character in database.characters)
        {
            if (character != null && character.AnimatorController == null && character.BasePrefab != null)
            {
                Animator animator = character.BasePrefab.GetComponent<Animator>();
                if (animator != null && animator.runtimeAnimatorController != null)
                {
                    SerializedObject serializedCharacter = new SerializedObject(character);
                    serializedCharacter.FindProperty("animatorController").objectReferenceValue = animator.runtimeAnimatorController;
                    serializedCharacter.ApplyModifiedProperties();
                    assignedCount++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"🎭 Auto-assigned {assignedCount} animator controllers");
        EditorUtility.DisplayDialog("Auto-Assign Animators", $"Assigned {assignedCount} animator controllers!", "OK");
    }

    private void BatchExtractStats()
    {
        int extractedCount = 0;

        foreach (var character in database.characters)
        {
            if (character != null && character.BasePrefab != null)
            {
                RagdollCharacter ragdoll = character.BasePrefab.GetComponent<RagdollCharacter>();
                if (ragdoll != null)
                {
                    SerializedObject serializedCharacter = new SerializedObject(character);
                    SerializedProperty statsProperty = serializedCharacter.FindProperty("baseStats");

                    statsProperty.FindPropertyRelative("maxHealth").floatValue = ragdoll.maxHealth;
                    statsProperty.FindPropertyRelative("moveSpeed").floatValue = ragdoll.moveSpeed;
                    statsProperty.FindPropertyRelative("attackDamage").floatValue = ragdoll.attackDamage;
                    statsProperty.FindPropertyRelative("attackRange").floatValue = ragdoll.attackRange;
                    statsProperty.FindPropertyRelative("attackCooldown").floatValue = ragdoll.attackCooldown;

                    serializedCharacter.ApplyModifiedProperties();
                    extractedCount++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"📊 Extracted stats from {extractedCount} characters");
        EditorUtility.DisplayDialog("Extract Stats", $"Extracted stats from {extractedCount} characters!", "OK");
    }

    private string DetermineCategoryFromPath(string path)
    {
        if (path.Contains("ChienBinh") || path.Contains("Warrior")) return "chien_binh";
        if (path.Contains("Robot")) return "robot";
        if (path.Contains("Zombie")) return "zombie";
        if (path.Contains("QuaiVat") || path.Contains("Monster")) return "quai_vat";
        return "unknown";
    }

    #endregion
}