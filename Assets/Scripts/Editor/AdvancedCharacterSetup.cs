using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class AdvancedCharacterSetup : EditorWindow
{
    private GameObject targetPrefab;
    private string characterName = "";
    private CharacterManager characterManager;
    
    [Header("Components to Add")]
    private bool addRagdollCharacter = true;
    private bool addNavMeshAgent = true;
    private bool addAnimator = true;
    private bool addRigidbody = true;
    private bool addCapsuleCollider = true;
    private bool addAudioSource = true;
    
    [Header("AI Components")]
    private bool addCharacterAI = true;
    private bool addHealthSystem = true;
    private bool addWeaponSystem = false;
    
    [Header("Animation")]
    private RuntimeAnimatorController animatorController;
    private Avatar characterAvatar;
    
    [Header("Physics")]
    private PhysicsMaterial physicMaterial;
    private float mass = 1f;
    private bool useGravity = true;
    private bool isKinematic = false;
    
    [Header("Stats")]
    private int health = 100;
    private float speed = 5f;
    private float attackDamage = 20f;
    private float attackRange = 2f;
    
    private Vector2 scrollPosition;
    private List<string> setupLog = new List<string>();
    
    [MenuItem("Tools/Advanced Character Setup")]
    public static void ShowWindow()
    {
        AdvancedCharacterSetup window = GetWindow<AdvancedCharacterSetup>();
        window.titleContent = new GUIContent("Advanced Character Setup");
        window.Show();
    }
    
    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        GUILayout.Label("Advanced Character Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        // Target Selection
        DrawTargetSelection();
        GUILayout.Space(10);
        
        // Component Selection
        DrawComponentSelection();
        GUILayout.Space(10);
        
        // Animation Setup
        DrawAnimationSetup();
        GUILayout.Space(10);
        
        // Physics Setup
        DrawPhysicsSetup();
        GUILayout.Space(10);
        
        // Stats Setup
        DrawStatsSetup();
        GUILayout.Space(10);
        
        // Setup Button
        DrawSetupButton();
        GUILayout.Space(10);
        
        // Setup Log
        DrawSetupLog();
        
        EditorGUILayout.EndScrollView();
    }
    
    void DrawTargetSelection()
    {
        GUILayout.Label("Target Selection", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        targetPrefab = (GameObject)EditorGUILayout.ObjectField("Target Prefab", targetPrefab, typeof(GameObject), false);
        
        if (EditorGUI.EndChangeCheck() && targetPrefab != null)
        {
            characterName = targetPrefab.name;
            AutoDetectComponents();
        }
        
        characterName = EditorGUILayout.TextField("Character Name", characterName);
        
        characterManager = (CharacterManager)EditorGUILayout.ObjectField("Character Manager", characterManager, typeof(CharacterManager), true);
        
        if (characterManager == null && GUILayout.Button("Find Character Manager"))
        {
            characterManager = FindObjectOfType<CharacterManager>();
        }
        
        // Quick selection from scene
        if (Selection.activeGameObject != null && GUILayout.Button("Use Selected GameObject"))
        {
            targetPrefab = Selection.activeGameObject;
            characterName = targetPrefab.name;
            AutoDetectComponents();
        }
    }
    
    void DrawComponentSelection()
    {
        GUILayout.Label("Components to Add", EditorStyles.boldLabel);
        
        // Core Components
        GUILayout.Label("Core Components:", EditorStyles.miniBoldLabel);
        addRagdollCharacter = EditorGUILayout.Toggle("RagdollCharacter", addRagdollCharacter);
        addNavMeshAgent = EditorGUILayout.Toggle("NavMeshAgent", addNavMeshAgent);
        addAnimator = EditorGUILayout.Toggle("Animator", addAnimator);
        addRigidbody = EditorGUILayout.Toggle("Rigidbody", addRigidbody);
        addCapsuleCollider = EditorGUILayout.Toggle("CapsuleCollider", addCapsuleCollider);
        addAudioSource = EditorGUILayout.Toggle("AudioSource", addAudioSource);
        
        GUILayout.Space(5);
        
        // AI Components
        GUILayout.Label("AI Components:", EditorStyles.miniBoldLabel);
        addCharacterAI = EditorGUILayout.Toggle("Character AI", addCharacterAI);
        addHealthSystem = EditorGUILayout.Toggle("Health System", addHealthSystem);
        addWeaponSystem = EditorGUILayout.Toggle("Weapon System", addWeaponSystem);
        
        GUILayout.Space(5);
        
        // Auto-detect button
        if (targetPrefab != null && GUILayout.Button("Auto-Detect Missing Components"))
        {
            AutoDetectComponents();
        }
    }
    
    void DrawAnimationSetup()
    {
        GUILayout.Label("Animation Setup", EditorStyles.boldLabel);
        
        animatorController = (RuntimeAnimatorController)EditorGUILayout.ObjectField("Animator Controller", animatorController, typeof(RuntimeAnimatorController), false);
        characterAvatar = (Avatar)EditorGUILayout.ObjectField("Avatar", characterAvatar, typeof(Avatar), false);
        
        if (animatorController == null && GUILayout.Button("Find Default Animator Controller"))
        {
            // Try to find existing animator controller
            string[] guids = AssetDatabase.FindAssets("t:RuntimeAnimatorController");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                animatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
            }
        }
    }
    
    void DrawPhysicsSetup()
    {
        GUILayout.Label("Physics Setup", EditorStyles.boldLabel);
        
        physicMaterial = (PhysicsMaterial)EditorGUILayout.ObjectField("Physic Material", physicMaterial, typeof(PhysicsMaterial), false);
        mass = EditorGUILayout.FloatField("Mass", mass);
        useGravity = EditorGUILayout.Toggle("Use Gravity", useGravity);
        isKinematic = EditorGUILayout.Toggle("Is Kinematic", isKinematic);
    }
    
    void DrawStatsSetup()
    {
        GUILayout.Label("Character Stats", EditorStyles.boldLabel);
        
        health = EditorGUILayout.IntField("Health", health);
        speed = EditorGUILayout.FloatField("Speed", speed);
        attackDamage = EditorGUILayout.FloatField("Attack Damage", attackDamage);
        attackRange = EditorGUILayout.FloatField("Attack Range", attackRange);
    }
    
    void DrawSetupButton()
    {
        GUI.enabled = targetPrefab != null && !string.IsNullOrEmpty(characterName);
        
        if (GUILayout.Button("Setup Character with All Components", GUILayout.Height(40)))
        {
            SetupCharacter();
        }
        
        GUI.enabled = true;
        
        GUILayout.Space(5);
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Setup Multiple Selected"))
        {
            SetupMultipleCharacters();
        }
        
        if (GUILayout.Button("Clear Log"))
        {
            setupLog.Clear();
        }
        GUILayout.EndHorizontal();
    }
    
    void DrawSetupLog()
    {
        if (setupLog.Count > 0)
        {
            GUILayout.Label("Setup Log", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical("box", GUILayout.MaxHeight(150));
            Vector2 logScrollPosition = EditorGUILayout.BeginScrollView(Vector2.zero);
            
            foreach (string log in setupLog)
            {
                EditorGUILayout.LabelField(log, EditorStyles.wordWrappedMiniLabel);
            }
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
    
    void AutoDetectComponents()
    {
        if (targetPrefab == null) return;
        
        setupLog.Add($"Auto-detecting components for {targetPrefab.name}...");
        
        // Check existing components
        addRagdollCharacter = targetPrefab.GetComponent<RagdollCharacter>() == null;
        addNavMeshAgent = targetPrefab.GetComponent<UnityEngine.AI.NavMeshAgent>() == null;
        addAnimator = targetPrefab.GetComponent<Animator>() == null;
        addRigidbody = targetPrefab.GetComponent<Rigidbody>() == null;
        addCapsuleCollider = targetPrefab.GetComponent<CapsuleCollider>() == null;
        addAudioSource = targetPrefab.GetComponent<AudioSource>() == null;
        
        // Try to find avatar from existing components
        Animator existingAnimator = targetPrefab.GetComponent<Animator>();
        if (existingAnimator != null && existingAnimator.avatar != null)
        {
            characterAvatar = existingAnimator.avatar;
        }
        
        setupLog.Add("Auto-detection completed.");
    }
    
    void SetupCharacter()
    {
        if (targetPrefab == null) return;
        
        setupLog.Clear();
        setupLog.Add($"Starting setup for {characterName}...");
        
        // Create a copy to work with
        GameObject workingCopy = PrefabUtility.InstantiatePrefab(targetPrefab) as GameObject;
        if (workingCopy == null)
        {
            workingCopy = Instantiate(targetPrefab);
        }
        
        workingCopy.name = characterName;
        
        try
        {
            // Add components
            AddComponents(workingCopy);
            
            // Setup animation
            SetupAnimation(workingCopy);
            
            // Setup physics
            SetupPhysics(workingCopy);
            
            // Setup stats
            SetupStats(workingCopy);
            
            // Save as prefab
            SaveAsPrefab(workingCopy);
            
            // Add to Character Manager
            AddToCharacterManager(workingCopy);
            
            setupLog.Add("✅ Character setup completed successfully!");
        }
        catch (System.Exception e)
        {
            setupLog.Add($"❌ Error during setup: {e.Message}");
        }
        finally
        {
            // Clean up
            DestroyImmediate(workingCopy);
        }
    }
    
    void AddComponents(GameObject target)
    {
        setupLog.Add("Adding components...");
        
        if (addRagdollCharacter && target.GetComponent<RagdollCharacter>() == null)
        {
            target.AddComponent<RagdollCharacter>();
            setupLog.Add("+ Added RagdollCharacter");
        }
        
        if (addNavMeshAgent && target.GetComponent<UnityEngine.AI.NavMeshAgent>() == null)
        {
            var agent = target.AddComponent<UnityEngine.AI.NavMeshAgent>();
            agent.speed = speed;
            setupLog.Add("+ Added NavMeshAgent");
        }
        
        if (addAnimator && target.GetComponent<Animator>() == null)
        {
            target.AddComponent<Animator>();
            setupLog.Add("+ Added Animator");
        }
        
        if (addRigidbody && target.GetComponent<Rigidbody>() == null)
        {
            target.AddComponent<Rigidbody>();
            setupLog.Add("+ Added Rigidbody");
        }
        
        if (addCapsuleCollider && target.GetComponent<CapsuleCollider>() == null)
        {
            target.AddComponent<CapsuleCollider>();
            setupLog.Add("+ Added CapsuleCollider");
        }
        
        if (addAudioSource && target.GetComponent<AudioSource>() == null)
        {
            target.AddComponent<AudioSource>();
            setupLog.Add("+ Added AudioSource");
        }
        
        // Add AI components (these might not exist, so use reflection)
        if (addCharacterAI)
        {
            AddComponentByName(target, "CharacterAI");
        }
        
        if (addHealthSystem)
        {
            AddComponentByName(target, "HealthSystem");
        }
        
        if (addWeaponSystem)
        {
            AddComponentByName(target, "WeaponSystem");
        }
    }
    
    void AddComponentByName(GameObject target, string componentName)
    {
        System.Type componentType = System.Type.GetType(componentName);
        if (componentType != null && target.GetComponent(componentType) == null)
        {
            target.AddComponent(componentType);
            setupLog.Add($"+ Added {componentName}");
        }
        else if (componentType == null)
        {
            setupLog.Add($"⚠️ {componentName} script not found");
        }
    }
    
    void SetupAnimation(GameObject target)
    {
        Animator animator = target.GetComponent<Animator>();
        if (animator != null)
        {
            if (animatorController != null)
            {
                animator.runtimeAnimatorController = animatorController;
                setupLog.Add("+ Set Animator Controller");
            }
            
            if (characterAvatar != null)
            {
                animator.avatar = characterAvatar;
                setupLog.Add("+ Set Avatar");
            }
        }
    }
    
    void SetupPhysics(GameObject target)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = mass;
            rb.useGravity = useGravity;
            rb.isKinematic = isKinematic;
            setupLog.Add("+ Configured Rigidbody");
        }
        
        CapsuleCollider collider = target.GetComponent<CapsuleCollider>();
        if (collider != null && physicMaterial != null)
        {
            collider.material = physicMaterial;
            setupLog.Add("+ Set Physic Material");
        }
    }
    
    void SetupStats(GameObject target)
    {
        // Try to set stats on RagdollCharacter if it exists
        RagdollCharacter ragdoll = target.GetComponent<RagdollCharacter>();
        if (ragdoll != null)
        {
            // Use reflection to set private fields if needed
            var healthField = typeof(RagdollCharacter).GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (healthField != null)
            {
                healthField.SetValue(ragdoll, health);
                setupLog.Add("+ Set Health");
            }
            
            var speedField = typeof(RagdollCharacter).GetField("speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (speedField != null)
            {
                speedField.SetValue(ragdoll, speed);
                setupLog.Add("+ Set Speed");
            }
        }
    }
    
    void SaveAsPrefab(GameObject target)
    {
        string prefabPath = $"Assets/Prefabs/{characterName}.prefab";
        
        // Ensure Prefabs directory exists
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(target, prefabPath);
        setupLog.Add($"+ Saved as prefab: {prefabPath}");
        
        // Select the prefab in project
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);
    }
    
    void AddToCharacterManager(GameObject prefab)
    {
        if (characterManager != null)
        {
            // Create character entry
            CharacterEntry newCharacter = new CharacterEntry();
            newCharacter.characterName = characterName;
            newCharacter.prefab = prefab;
            newCharacter.health = health;
            newCharacter.speed = speed;
            newCharacter.attackDamage = attackDamage;
            newCharacter.attackRange = attackRange;
            
            // Add to first available category or create new one
            if (characterManager.CategoryCount > 0)
            {
                characterManager.Categories[0].characters.Add(newCharacter);
            }
            else
            {
                characterManager.InitializeDefaultCategories();
                characterManager.Categories[0].characters.Add(newCharacter);
            }
            
            EditorUtility.SetDirty(characterManager);
            setupLog.Add("+ Added to Character Manager");
        }
    }
    
    void SetupMultipleCharacters()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select one or more GameObjects to setup.", "OK");
            return;
        }
        
        foreach (GameObject obj in selectedObjects)
        {
            targetPrefab = obj;
            characterName = obj.name;
            AutoDetectComponents();
            SetupCharacter();
        }
    }
}