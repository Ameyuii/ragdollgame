using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Editor script to migrate from RagdollController to SimpleRagdollController
/// Run this script to automatically update all NPCs in the project
/// </summary>
public class RagdollSystemMigrator : EditorWindow
{
    private List<GameObject> foundNPCs = new List<GameObject>();
    private Vector2 scrollPosition;
    private bool showDetailed = false;
    
    [MenuItem("Tools/Ragdoll System Migration")]
    public static void ShowWindow()
    {
        GetWindow<RagdollSystemMigrator>("Ragdoll Migration");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Ragdoll System Migration Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("This tool migrates from RagdollController to SimpleRagdollController", EditorStyles.helpBox);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Scan for NPCs with RagdollController", GUILayout.Height(30)))
        {
            ScanForNPCs();
        }
        
        GUILayout.Space(10);
        
        if (foundNPCs.Count > 0)
        {
            GUILayout.Label($"Found {foundNPCs.Count} NPCs with RagdollController:", EditorStyles.boldLabel);
            
            showDetailed = EditorGUILayout.Toggle("Show Details", showDetailed);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            
            foreach (var npc in foundNPCs)
            {
                if (npc == null) continue;
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(npc, typeof(GameObject), true);
                
                if (showDetailed)
                {
                    var ragdollController = npc.GetComponent<RagdollController>();
                    if (ragdollController != null)                    {
                        GUILayout.Label($"Has RagdollController", GUILayout.Width(150));
                    }
                    
                    var simpleRagdoll = npc.GetComponent<RagdollController>();
                    if (simpleRagdoll != null)
                    {
                        GUILayout.Label($"Already has RagdollController", GUILayout.Width(180));
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Migrate All NPCs", GUILayout.Height(40)))
            {
                MigrateAllNPCs();
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("Migrate Selected NPCs", GUILayout.Height(30)))
            {
                MigrateSelectedNPCs();
            }
        }
        else
        {
            GUILayout.Label("No NPCs with RagdollController found. Click 'Scan' to search.", EditorStyles.helpBox);
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("Manual Migration Steps:", EditorStyles.boldLabel);
        GUILayout.Label("1. Remove RagdollController component", EditorStyles.label);
        GUILayout.Label("2. Add SimpleRagdollController component", EditorStyles.label);
        GUILayout.Label("3. Run Unity's Ragdoll Builder on character", EditorStyles.label);
        GUILayout.Label("4. Test ragdoll activation in play mode", EditorStyles.label);
    }
    
    void ScanForNPCs()
    {
        foundNPCs.Clear();
        
        // Search in all prefabs
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                var ragdollController = prefab.GetComponent<RagdollController>();
                if (ragdollController != null)
                {
                    foundNPCs.Add(prefab);
                }
            }
        }
          // Search in current scene
        RagdollController[] sceneRagdolls = FindObjectsByType<RagdollController>(FindObjectsSortMode.None);
        foreach (var ragdoll in sceneRagdolls)
        {
            if (!foundNPCs.Contains(ragdoll.gameObject))
            {
                foundNPCs.Add(ragdoll.gameObject);
            }
        }
        
        Debug.Log($"Found {foundNPCs.Count} NPCs with RagdollController");
    }
    
    void MigrateAllNPCs()
    {
        if (EditorUtility.DisplayDialog("Migrate All NPCs", 
            $"This will migrate {foundNPCs.Count} NPCs from RagdollController to SimpleRagdollController.\n\nThis action cannot be undone. Continue?", 
            "Yes", "Cancel"))
        {
            int migratedCount = 0;
            
            foreach (var npc in foundNPCs)
            {
                if (MigrateNPC(npc))
                {
                    migratedCount++;
                }
            }
            
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Migration Complete", 
                $"Successfully migrated {migratedCount} out of {foundNPCs.Count} NPCs.", "OK");
                
            // Refresh the scan
            ScanForNPCs();
        }
    }
    
    void MigrateSelectedNPCs()
    {
        var selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select NPCs in the scene or project to migrate.", "OK");
            return;
        }
        
        int migratedCount = 0;
        
        foreach (var obj in selectedObjects)
        {
            if (MigrateNPC(obj))
            {
                migratedCount++;
            }
        }
        
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Migration Complete", 
            $"Successfully migrated {migratedCount} out of {selectedObjects.Length} selected objects.", "OK");
            
        // Refresh the scan
        ScanForNPCs();
    }
    
    bool MigrateNPC(GameObject npc)
    {
        if (npc == null) return false;
        
        var oldRagdoll = npc.GetComponent<RagdollController>();        if (oldRagdoll == null) return false;
        
        // Check if already has RagdollController
        var existingSimple = npc.GetComponent<RagdollController>();
        if (existingSimple != null)
        {
            Debug.LogWarning($"{npc.name} already has RagdollController. Skipping.");
            return false;
        }
        
        try        {
            // Record for undo
            Undo.RecordObject(npc, "Migrate Ragdoll System");
            
            // Add RagdollController
            var newRagdoll = npc.AddComponent<RagdollController>();
            
            // Try to copy some settings if possible
            // (RagdollController has different structure, so limited copying)
            
            // Remove old component
            Undo.DestroyObjectImmediate(oldRagdoll);
            
            // Mark prefab dirty if it's a prefab
            if (PrefabUtility.IsPartOfPrefabAsset(npc))
            {
                EditorUtility.SetDirty(npc);
            }
            
            Debug.Log($"Successfully migrated {npc.name} to SimpleRagdollController");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to migrate {npc.name}: {e.Message}");
            return false;
        }
    }
    
    [MenuItem("Tools/Create Ragdoll Setup Instructions")]
    public static void CreateRagdollInstructions()
    {
        string instructions = @"RAGDOLL SETUP INSTRUCTIONS

After migrating to SimpleRagdollController, you need to set up ragdoll physics:

1. SELECT YOUR NPC PREFAB
   - Open the prefab for editing
   - Make sure it has a proper bone hierarchy

2. OPEN RAGDOLL BUILDER
   - Go to 'GameObject > 3D Object > Ragdoll...'
   - Or use 'Window > Animation > Ragdoll Builder' (Unity 6.2+)

3. ASSIGN BONE TRANSFORMS
   Required bones:
   - Pelvis (hip bone)
   - Left Hip, Right Hip
   - Left Knee, Right Knee  
   - Left Foot, Right Foot
   - Spine (chest/torso)
   - Head
   - Left Arm, Right Arm (upper arm)
   - Left Elbow, Right Elbow (forearm)
   - Left Hand, Right Hand

4. CONFIGURE PHYSICS
   - Total Mass: 70-80 (human-like weight)
   - Strength: 0 (for death ragdoll)
   - Click 'Create' to generate ragdoll components

5. TEST THE SETUP
   - Play the scene
   - NPCs should activate ragdoll when taking damage
   - Check console for debug logs

6. FINE-TUNE (Optional)
   - Adjust joint limits for realistic movement
   - Set appropriate mass distribution
   - Configure collision layers if needed

The SimpleRagdollController will automatically find and manage these ragdoll components.";

        Debug.Log(instructions);
        
        EditorUtility.DisplayDialog("Ragdoll Setup Instructions", 
            "Setup instructions have been logged to the Console. Check the Console window for detailed steps.", "OK");
    }
}
