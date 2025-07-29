#pragma warning disable CS0618 // Type or member is obsolete
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(CharacterManager))]
public class CharacterManagerEditor : Editor
{
    private CharacterManager manager;
    private Vector2 scrollPosition;
    private bool[] categoryFoldouts;
    private bool showAddCharacterSection = false;
    private bool showToolsSection = false;
    
    // Add character fields
    private GameObject newCharacterPrefab;
    private Sprite newCharacterIcon;
    private string newCharacterName = "";
    private int selectedCategoryIndex = 0;
    
    void OnEnable()
    {
        manager = (CharacterManager)target;
        RefreshFoldouts();
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        GUILayout.Label("Character Manager", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        // Draw default inspector for basic fields
        DrawDefaultInspector();
        
        GUILayout.Space(10);
        
        // Quick actions
        DrawQuickActions();
        GUILayout.Space(10);
        
        // Add character section
        DrawAddCharacterSection();
        GUILayout.Space(10);
        
        // Tools section
        DrawToolsSection();
        GUILayout.Space(10);
        
        // Categories section
        DrawCategoriesSection();
        
        // Statistics
        DrawStatistics();
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(manager);
            serializedObject.ApplyModifiedProperties();
        }
    }
    
    void DrawQuickActions()
    {
        GUILayout.Label("Quick Actions", EditorStyles.boldLabel);
        
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Initialize Default Categories"))
        {
            if (EditorUtility.DisplayDialog("Initialize Categories", 
                "This will clear all existing categories and create default ones. Continue?", 
                "Yes", "Cancel"))
            {
                manager.InitializeDefaultCategories();
                RefreshFoldouts();
            }
        }
        
        if (GUILayout.Button("Add New Category"))
        {
            manager.AddNewCategory();
            RefreshFoldouts();
        }
        
        if (GUILayout.Button("Refresh UI"))
        {
            manager.RefreshUI();
        }
        
        GUILayout.EndHorizontal();
    }
    
    void DrawAddCharacterSection()
    {
        showAddCharacterSection = EditorGUILayout.Foldout(showAddCharacterSection, "Add New Character", true);
        
        if (showAddCharacterSection)
        {
            EditorGUI.indentLevel++;
            
            EditorGUI.BeginChangeCheck();
            
            newCharacterPrefab = (GameObject)EditorGUILayout.ObjectField("Character Prefab", newCharacterPrefab, typeof(GameObject), false);
            newCharacterIcon = (Sprite)EditorGUILayout.ObjectField("UI Icon", newCharacterIcon, typeof(Sprite), false);
            newCharacterName = EditorGUILayout.TextField("Character Name", newCharacterName);
            
            // Category selection
            if (manager.Categories.Count > 0)
            {
                string[] categoryNames = new string[manager.Categories.Count];
                for (int i = 0; i < manager.Categories.Count; i++)
                {
                    categoryNames[i] = manager.Categories[i].categoryName;
                }
                selectedCategoryIndex = EditorGUILayout.Popup("Target Category", selectedCategoryIndex, categoryNames);
            }
            else
            {
                EditorGUILayout.HelpBox("No categories available. Create categories first.", MessageType.Warning);
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                if (newCharacterPrefab != null && string.IsNullOrEmpty(newCharacterName))
                {
                    newCharacterName = newCharacterPrefab.name;
                }
            }
            
            GUI.enabled = newCharacterPrefab != null && !string.IsNullOrEmpty(newCharacterName) && manager.Categories.Count > 0;
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Character"))
            {
                AddNewCharacter();
            }
            
            if (GUILayout.Button("Database Setup"))
            {
                DatabaseSetupTool.ShowWindow();
            }
            GUILayout.EndHorizontal();
            
            GUI.enabled = true;
            
            GUILayout.Space(5);
            
            // Batch add from selection
            if (Selection.gameObjects.Length > 0)
            {
                GUILayout.Label($"Selected Objects: {Selection.gameObjects.Length}", EditorStyles.miniLabel);
                if (GUILayout.Button("Add All Selected Objects"))
                {
                    AddSelectedObjects();
                }
            }
            
            EditorGUI.indentLevel--;
        }
    }
    
    void DrawToolsSection()
    {
        showToolsSection = EditorGUILayout.Foldout(showToolsSection, "Tools & Utilities", true);
        
        if (showToolsSection)
        {
            EditorGUI.indentLevel++;
            
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Auto Setup All Prefabs"))
            {
                manager.AutoSetupAllPrefabs();
            }
            
            if (GUILayout.Button("Validate Character Data"))
            {
                manager.ValidateCharacterData();
            }
            
            if (GUILayout.Button("Print Statistics"))
            {
                manager.PrintStatistics();
            }
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            
            // Character Setup Tool button
            if (GUILayout.Button("Open Character Setup Tool"))
            {
                // CharacterSetupTool.ShowWindow(); // Disabled - replaced with new system
                Debug.Log("Character Setup Tool is disabled. Use Character System Setup instead.");
            }
            
            // Database Setup button
            if (GUILayout.Button("Open Database Setup"))
            {
                DatabaseSetupTool.ShowWindow();
            }
            
            EditorGUI.indentLevel--;
        }
    }
    
    void DrawCategoriesSection()
    {
        GUILayout.Label("Categories", EditorStyles.boldLabel);
        
        if (manager.Categories == null || manager.Categories.Count == 0)
        {
            EditorGUILayout.HelpBox("No categories found. Click 'Initialize Default Categories' to create default categories.", MessageType.Info);
            return;
        }
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(400));
        
        for (int i = 0; i < manager.Categories.Count; i++)
        {
            DrawCategory(i);
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    void DrawCategory(int categoryIndex)
    {
        var category = manager.Categories[categoryIndex];
        
        GUILayout.BeginVertical("box");
        
        // Category header
        GUILayout.BeginHorizontal();
        
        if (categoryFoldouts != null && categoryFoldouts.Length > categoryIndex)
        {
            categoryFoldouts[categoryIndex] = EditorGUILayout.Foldout(categoryFoldouts[categoryIndex], 
                $"{category.categoryName} ({category.characters.Count} characters)", true);
        }
        
        GUILayout.FlexibleSpace();
        
        // Category color
        category.categoryColor = EditorGUILayout.ColorField(GUIContent.none, category.categoryColor, false, false, false, GUILayout.Width(50));
        
        // Delete category button
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("×", GUILayout.Width(25)))
        {
            if (EditorUtility.DisplayDialog("Delete Category", 
                $"Delete category '{category.categoryName}' and all its characters?", 
                "Delete", "Cancel"))
            {
                manager.Categories.RemoveAt(categoryIndex);
                RefreshFoldouts();
                return;
            }
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.EndHorizontal();
        
        // Category details (when expanded)
        if (categoryFoldouts != null && categoryFoldouts.Length > categoryIndex && categoryFoldouts[categoryIndex])
        {
            EditorGUI.indentLevel++;
            
            // Category properties
            category.categoryName = EditorGUILayout.TextField("Name", category.categoryName);
            category.categoryIcon = (Sprite)EditorGUILayout.ObjectField("Icon", category.categoryIcon, typeof(Sprite), false);
            
            GUILayout.Space(5);
            
            // Characters in this category
            if (category.characters.Count > 0)
            {
                GUILayout.Label("Characters:", EditorStyles.boldLabel);
                
                for (int j = 0; j < category.characters.Count; j++)
                {
                    DrawCharacter(categoryIndex, j);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No characters in this category.", MessageType.Info);
            }
            
            EditorGUI.indentLevel--;
        }
        
        GUILayout.EndVertical();
        GUILayout.Space(5);
    }
    
    void DrawCharacter(int categoryIndex, int characterIndex)
    {
        var character = manager.Categories[categoryIndex].characters[characterIndex];
        
        GUILayout.BeginHorizontal("box");
        
        // Character icon
        if (character.uiIcon != null)
        {
            GUILayout.Label(character.uiIcon.texture, GUILayout.Width(32), GUILayout.Height(32));
        }
        else
        {
            GUILayout.Label("No Icon", GUILayout.Width(32), GUILayout.Height(32));
        }
        
        GUILayout.BeginVertical();
        
        // Character name and prefab
        GUILayout.BeginHorizontal();
        character.characterName = EditorGUILayout.TextField(character.characterName, GUILayout.Width(120));
        character.prefab = (GameObject)EditorGUILayout.ObjectField(character.prefab, typeof(GameObject), false, GUILayout.Width(100));
        GUILayout.EndHorizontal();
        
        // Character stats (compact view)
        GUILayout.BeginHorizontal();
        GUILayout.Label("HP:", GUILayout.Width(25));
        character.health = EditorGUILayout.IntField(character.health, GUILayout.Width(50));
        GUILayout.Label("Speed:", GUILayout.Width(40));
        character.speed = EditorGUILayout.FloatField(character.speed, GUILayout.Width(50));
        GUILayout.Label("Dmg:", GUILayout.Width(30));
        character.attackDamage = EditorGUILayout.FloatField(character.attackDamage, GUILayout.Width(50));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        
        GUILayout.EndVertical();
        
        // Edit/Delete buttons
        if (GUILayout.Button("Edit", GUILayout.Width(50)))
        {
            ShowCharacterEditWindow(character);
        }
        
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("×", GUILayout.Width(25)))
        {
            if (EditorUtility.DisplayDialog("Delete Character", 
                $"Delete character '{character.characterName}'?", 
                "Delete", "Cancel"))
            {
                manager.Categories[categoryIndex].characters.RemoveAt(characterIndex);
            }
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.EndHorizontal();
    }
    
    void DrawStatistics()
    {
        GUILayout.Space(10);
        GUILayout.Label("Statistics", EditorStyles.boldLabel);
        
        GUILayout.Label($"Total Categories: {manager.CategoryCount}");
        GUILayout.Label($"Total Characters: {manager.TotalCharacterCount}");
        
        // Category breakdown
        foreach (var category in manager.Categories)
        {
            GUILayout.Label($"  {category.categoryName}: {category.characters.Count}");
        }
    }
    
    void AddNewCharacter()
    {
        if (selectedCategoryIndex >= 0 && selectedCategoryIndex < manager.Categories.Count)
        {
            CharacterEntry newCharacter = new CharacterEntry();
            newCharacter.characterName = newCharacterName;
            newCharacter.prefab = newCharacterPrefab;
            newCharacter.uiIcon = newCharacterIcon;
            
            manager.Categories[selectedCategoryIndex].characters.Add(newCharacter);
            
            // Reset form
            newCharacterPrefab = null;
            newCharacterIcon = null;
            newCharacterName = "";
            
            // Refresh UI
            manager.RefreshUI();
        }
    }
    
    void AddSelectedObjects()
    {
        if (selectedCategoryIndex >= 0 && selectedCategoryIndex < manager.Categories.Count)
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                CharacterEntry newCharacter = new CharacterEntry();
                newCharacter.characterName = obj.name;
                newCharacter.prefab = obj;
                
                manager.Categories[selectedCategoryIndex].characters.Add(newCharacter);
            }
            
            // Refresh UI
            manager.RefreshUI();
        }
    }
    
    void ShowCharacterEditWindow(CharacterEntry character)
    {
        CharacterEntryEditWindow.ShowWindow(character, manager);
    }
    
    void RefreshFoldouts()
    {
        if (manager?.Categories != null)
        {
            categoryFoldouts = new bool[manager.Categories.Count];
            for (int i = 0; i < categoryFoldouts.Length; i++)
            {
                categoryFoldouts[i] = true;
            }
        }
    }
}

// Character edit window
public class CharacterEntryEditWindow : EditorWindow
{
    private CharacterEntry character;
    private CharacterManager manager;
    
    public static void ShowWindow(CharacterEntry character, CharacterManager manager)
    {
        CharacterEntryEditWindow window = GetWindow<CharacterEntryEditWindow>();
        window.character = character;
        window.manager = manager;
        window.titleContent = new GUIContent($"Edit {character.characterName}");
        window.Show();
    }
    
    void OnGUI()
    {
        if (character == null) return;
        
        GUILayout.Label($"Editing: {character.characterName}", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        // Basic info
        character.characterName = EditorGUILayout.TextField("Name", character.characterName);
        character.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", character.prefab, typeof(GameObject), false);
        character.uiIcon = (Sprite)EditorGUILayout.ObjectField("UI Icon", character.uiIcon, typeof(Sprite), false);
        
        GUILayout.Space(10);
        
        // Stats
        GUILayout.Label("Stats", EditorStyles.boldLabel);
        character.health = EditorGUILayout.IntField("Health", character.health);
        character.speed = EditorGUILayout.FloatField("Speed", character.speed);
        character.attackDamage = EditorGUILayout.FloatField("Attack Damage", character.attackDamage);
        character.attackRange = EditorGUILayout.FloatField("Attack Range", character.attackRange);
        
        GUILayout.Space(10);
        
        // Visual
        GUILayout.Label("Visual", EditorStyles.boldLabel);
        // ✅ DISABLED: Team color editor - loại bỏ hoàn toàn chức năng màu team
        // character.teamColor = EditorGUILayout.ColorField("Team Color", character.teamColor);
        character.description = EditorGUILayout.TextArea(character.description, GUILayout.Height(60));
        
        GUILayout.Space(20);
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(manager);
            if (manager != null) manager.RefreshUI();
            Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
        GUILayout.EndHorizontal();
    }
}