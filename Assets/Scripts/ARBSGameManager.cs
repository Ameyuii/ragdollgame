using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Main game manager that integrates all systems like Animal Revolt Battle Simulator
/// Handles UI events, map state, and AI logic generation
/// </summary>
public class ARBSGameManager : MonoBehaviour
{
    [Header("System References")]
    public BattleGameManager battleManager;
    public MapStateManager mapStateManager;
    public AdvancedUIManager uiManager;
    
    [Header("Game Configuration")]
    public bool enableGrid = false;
    public float gridSize = 1f;
    public bool showCoordinates = true;
    
    [Header("AI Configuration")]
    public bool autoGenerateAI = true;
    public float detectionRadius = 5f;
    public float engagementDistance = 2f;
    
    private bool isSimulationRunning = false;
    private List<UIEvent> eventHistory = new List<UIEvent>();
    
    [System.Serializable]
    public class UIEvent
    {
        public string eventType; // DRAG_START, DROP, SELECT_TEAM, etc.
        public string characterID;
        public Vector3 position;
        public int team;
        public float timestamp;
        public string context;
        
        public UIEvent(string type, string charID, Vector3 pos, int teamID, string ctx = "")
        {
            eventType = type;
            characterID = charID;
            position = pos;
            team = teamID;
            timestamp = Time.time;
            context = ctx;
        }
    }
    
    void Start()
    {
        InitializeSystems();
        SetupEventListeners();
    }
    
    void InitializeSystems()
    {
        // Get or create system components
        if (battleManager == null)
            battleManager = GetComponent<BattleGameManager>();
        
        if (mapStateManager == null)
            mapStateManager = GetComponent<MapStateManager>();
        
        if (uiManager == null)
            uiManager = FindObjectOfType<AdvancedUIManager>();
        
        // Initialize map bounds based on ground object
        GameObject ground = GameObject.Find("Ground");
        if (ground != null && mapStateManager != null)
        {
            Renderer groundRenderer = ground.GetComponent<Renderer>();
            if (groundRenderer != null)
            {
                Vector3 size = groundRenderer.bounds.size;
                mapStateManager.mapBounds = new Vector2(size.x, size.z);
            }
        }
        
        Debug.Log("ARBS Game Manager initialized");
    }
    
    void SetupEventListeners()
    {
        // This would set up listeners for various UI events
        // In a full implementation, this would connect to UI elements
    }
    
    /// <summary>
    /// Called when user starts dragging a character from UI
    /// </summary>
    public void OnCharacterDragStart(string characterID, int team)
    {
        UIEvent dragEvent = new UIEvent("DRAG_START", characterID, Vector3.zero, team);
        eventHistory.Add(dragEvent);
        
        Debug.Log($"Drag started: {characterID} for team {team}");
    }
    
    /// <summary>
    /// Called when user drops a character on the map
    /// </summary>
    public void OnCharacterDrop(string characterID, Vector3 position, int team)
    {
        UIEvent dropEvent = new UIEvent("DROP", characterID, position, team, 
            GetContextualInfo(position));
        eventHistory.Add(dropEvent);
        
        // Analyze context and generate AI behavior
        if (autoGenerateAI)
        {
            GenerateAIBehavior(characterID, position, team);
        }
        
        Debug.Log($"Character dropped: {characterID} at {position} for team {team}");
    }
    
    /// <summary>
    /// Called when user selects a team
    /// </summary>
    public void OnTeamSelected(int team)
    {
        UIEvent teamEvent = new UIEvent("SELECT_TEAM", "", Vector3.zero, team);
        eventHistory.Add(teamEvent);
        
        Debug.Log($"Team selected: {team}");
    }
    
    /// <summary>
    /// Get contextual information about a position on the map
    /// </summary>
    string GetContextualInfo(Vector3 position)
    {
        List<string> context = new List<string>();
        
        // Check nearby characters
        if (mapStateManager != null)
        {
            List<CharacterInstance> nearbyCharacters = 
                mapStateManager.GetCharactersInRadius(position, detectionRadius);
            
            foreach (CharacterInstance character in nearbyCharacters)
            {
                float distance = Vector3.Distance(position, character.position);
                context.Add($"Near {character.characterID} (Team {character.team}) at {distance:F1}m");
            }
        }
        
        // Check terrain type (could be expanded)
        context.Add($"Terrain: Ground at Y={position.y:F1}");
        
        return string.Join("; ", context);
    }
    
    /// <summary>
    /// Generate AI behavior based on context
    /// </summary>
    void GenerateAIBehavior(string characterID, Vector3 position, int team)
    {
        if (mapStateManager == null) return;
        
        // Find the character instance
        List<CharacterInstance> teamCharacters = mapStateManager.GetCharactersByTeam(team);
        CharacterInstance newCharacter = teamCharacters.Find(c => 
            Vector3.Distance(c.position, position) < 0.1f);
        
        if (newCharacter == null) return;
        
        // Analyze nearby enemies
        List<CharacterInstance> nearbyEnemies = new List<CharacterInstance>();
        foreach (CharacterInstance character in mapStateManager.characterInstances)
        {
            if (character.team != team && 
                Vector3.Distance(character.position, position) <= detectionRadius)
            {
                nearbyEnemies.Add(character);
            }
        }
        
        // Generate AI behavior based on context
        RagdollCharacter ragdoll = newCharacter.gameObject?.GetComponent<RagdollCharacter>();
        if (ragdoll != null)
        {
            if (nearbyEnemies.Count > 0)
            {
                // Aggressive behavior if enemies nearby
                Debug.Log($"Generated AI: {characterID} set to AGGRESSIVE (enemies detected)");
                // ragdoll.SetAIBehavior(AIBehavior.Aggressive);
            }
            else
            {
                // Patrol behavior if no immediate threats
                Debug.Log($"Generated AI: {characterID} set to PATROL (no immediate threats)");
                // ragdoll.SetAIBehavior(AIBehavior.Patrol);
            }
        }
    }
    
    /// <summary>
    /// Start the battle simulation
    /// </summary>
    public void StartSimulation()
    {
        if (isSimulationRunning)
        {
            Debug.Log("Simulation already running!");
            return;
        }
        
        isSimulationRunning = true;
        
        // Generate final AI setup based on all placed characters
        GenerateFinalAISetup();
        
        // Start the battle through BattleGameManager
        if (battleManager != null)
        {
            battleManager.StartBattle();
        }
        
        Debug.Log("Simulation started!");
    }
    
    /// <summary>
    /// Generate comprehensive AI setup for all characters
    /// </summary>
    void GenerateFinalAISetup()
    {
        if (mapStateManager == null) return;
        
        Debug.Log("=== Generating Final AI Setup ===");
        
        foreach (CharacterInstance character in mapStateManager.characterInstances)
        {
            // Find nearest enemies
            CharacterInstance nearestEnemy = null;
            float nearestDistance = float.MaxValue;
            
            foreach (CharacterInstance other in mapStateManager.characterInstances)
            {
                if (other.team != character.team)
                {
                    float distance = Vector3.Distance(character.position, other.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEnemy = other;
                    }
                }
            }
            
            // Generate AI code (conceptual)
            if (nearestEnemy != null)
            {
                Debug.Log($"AI Setup for {character.instanceID}:");
                Debug.Log($"  - Target: {nearestEnemy.instanceID} at {nearestDistance:F1}m");
                Debug.Log($"  - Behavior: {(nearestDistance < engagementDistance ? "ATTACK" : "APPROACH")}");
                
                // In a real implementation, this would generate actual AI code:
                /*
                GameObject characterObj = character.gameObject;
                AIController ai = characterObj.GetComponent<AIController>();
                if (ai == null) ai = characterObj.AddComponent<AIController>();
                
                ai.SetTarget(nearestEnemy.gameObject);
                ai.SetBehavior(nearestDistance < engagementDistance ? 
                    AIBehavior.Attack : AIBehavior.Approach);
                */
            }
        }
    }
    
    /// <summary>
    /// Reset the simulation
    /// </summary>
    public void ResetSimulation()
    {
        isSimulationRunning = false;
        
        if (mapStateManager != null)
        {
            mapStateManager.ClearAllCharacters();
        }
        
        if (battleManager != null)
        {
            battleManager.ResetBattle();
        }
        
        eventHistory.Clear();
        
        Debug.Log("Simulation reset!");
    }
    
    /// <summary>
    /// Save current map state
    /// </summary>
    public void SaveMapState()
    {
        if (mapStateManager != null)
        {
            string saveData = mapStateManager.SaveMapState();
            // In a real implementation, this would save to file
            Debug.Log("Map state saved:\n" + saveData);
        }
    }
    
    /// <summary>
    /// Load map state
    /// </summary>
    public void LoadMapState(string jsonData)
    {
        if (mapStateManager != null)
        {
            mapStateManager.LoadMapState(jsonData);
            Debug.Log("Map state loaded");
        }
    }
    
    void Update()
    {
        // Update coordinates display if enabled
        if (showCoordinates)
        {
            UpdateCoordinatesDisplay();
        }
    }
    
    void UpdateCoordinatesDisplay()
    {
        // Show mouse coordinates on map
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        if (mouseWorldPos != Vector3.zero)
        {
            // This would update a UI text element showing coordinates
            // Debug.Log($"Mouse: {mouseWorldPos}");
        }
    }
    
    Vector3 GetMouseWorldPosition()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return Vector3.zero;
        
        // Use Mouse.current from Input System
        var mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse == null) return Vector3.zero;
        
        Vector2 mousePos = mouse.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        
        return Vector3.zero;
    }
    
    void OnDrawGizmos()
    {
        // Draw map bounds
        if (mapStateManager != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = transform.position;
            Vector3 size = new Vector3(mapStateManager.mapBounds.x, 0.1f, mapStateManager.mapBounds.y);
            Gizmos.DrawWireCube(center, size);
        }
        
        // Draw grid if enabled
        if (enableGrid)
        {
            DrawGrid();
        }
    }
    
    void DrawGrid()
    {
        if (mapStateManager == null) return;
        
        Gizmos.color = new Color(1, 1, 1, 0.3f);
        
        Vector2 bounds = mapStateManager.mapBounds;
        Vector3 center = transform.position;
        
        // Draw vertical lines
        for (float x = -bounds.x/2; x <= bounds.x/2; x += gridSize)
        {
            Vector3 start = new Vector3(x, center.y, -bounds.y/2) + center;
            Vector3 end = new Vector3(x, center.y, bounds.y/2) + center;
            Gizmos.DrawLine(start, end);
        }
        
        // Draw horizontal lines
        for (float z = -bounds.y/2; z <= bounds.y/2; z += gridSize)
        {
            Vector3 start = new Vector3(-bounds.x/2, center.y, z) + center;
            Vector3 end = new Vector3(bounds.x/2, center.y, z) + center;
            Gizmos.DrawLine(start, end);
        }
    }
}