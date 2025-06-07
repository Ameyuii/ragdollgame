using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace AnimalRevolt.GameModes
{
    /// <summary>
    /// Game Manager đơn giản cho demo battle
    /// Quản lý logic chiến đấu cơ bản giữa 2 fighters
    /// </summary>
    public class BasicGameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private float battleDuration = 60f;
        [SerializeField] private bool autoStartBattle = true;
        
        [Header("Fighter References")]
        [SerializeField] private GameObject fighter1;
        [SerializeField] private GameObject fighter2;
        
        [Header("Demo Controls")]
        [SerializeField] private bool enableDemoControls = true;
        [SerializeField] private float forceAmount = 500f;
        
        // Private variables
        private RagdollPhysicsController ragdoll1;
        private RagdollPhysicsController ragdoll2;
        private bool battleActive = false;
        private float battleTimer = 0f;
        
        // Input System variables
        private Keyboard keyboard;
        
        // Game states
        public enum GameState { Waiting, Battle, GameOver }
        private GameState currentState = GameState.Waiting;
        
        private void Start()
        {
            Debug.Log("BasicGameManager started - Simple battle demo");
            
            // Initialize Input System
            keyboard = Keyboard.current;
            
            // Auto-find fighters if not assigned
            if (fighter1 == null)
                fighter1 = GameObject.Find("Fighter1");
            if (fighter2 == null)
                fighter2 = GameObject.Find("Fighter2");
                
            // Get ragdoll controllers
            if (fighter1 != null)
                ragdoll1 = fighter1.GetComponent<RagdollPhysicsController>();
            if (fighter2 != null)
                ragdoll2 = fighter2.GetComponent<RagdollPhysicsController>();
                
            // Validate setup
            if (ValidateSetup())
            {
                if (autoStartBattle)
                {
                    StartCoroutine(StartBattleAfterDelay(2f));
                }
            }
        }
        
        private void Update()
        {
            HandleInput();
            UpdateBattle();
        }
        
        /// <summary>
        /// Validate game setup
        /// </summary>
        private bool ValidateSetup()
        {
            if (fighter1 == null || fighter2 == null)
            {
                Debug.LogError("BasicGameManager: Missing fighter references!");
                return false;
            }
            
            if (ragdoll1 == null || ragdoll2 == null)
            {
                Debug.LogError("BasicGameManager: Fighters missing RagdollPhysicsController components!");
                return false;
            }
            
            Debug.Log("BasicGameManager: Setup validation passed");
            return true;
        }
        
        /// <summary>
        /// Handle demo input using Input System
        /// </summary>
        private void HandleInput()
        {
            if (!enableDemoControls || keyboard == null) return;
            
            // Space - Start battle manually
            if (keyboard.spaceKey.wasPressedThisFrame && currentState == GameState.Waiting)
            {
                StartBattle();
            }
            
            // R - Restart battle
            if (keyboard.rKey.wasPressedThisFrame)
            {
                RestartBattle();
            }
            
            // 1 - Apply force to Fighter1
            if (keyboard.digit1Key.wasPressedThisFrame && battleActive)
            {
                ApplyRandomForceToFighter(ragdoll1, "Fighter1");
            }
            
            // 2 - Apply force to Fighter2
            if (keyboard.digit2Key.wasPressedThisFrame && battleActive)
            {
                ApplyRandomForceToFighter(ragdoll2, "Fighter2");
            }
        }
        
        /// <summary>
        /// Update battle logic
        /// </summary>
        private void UpdateBattle()
        {
            if (currentState != GameState.Battle) return;
            
            battleTimer += Time.deltaTime;
            
            // Check time limit
            if (battleTimer >= battleDuration)
            {
                EndBattle("Time limit reached!");
            }
        }
        
        /// <summary>
        /// Start battle after delay
        /// </summary>
        private IEnumerator StartBattleAfterDelay(float delay)
        {
            Debug.Log($"Battle starting in {delay} seconds...");
            yield return new WaitForSeconds(delay);
            StartBattle();
        }
        
        /// <summary>
        /// Start the battle
        /// </summary>
        public void StartBattle()
        {
            if (currentState == GameState.Battle) return;
            
            Debug.Log("=== BATTLE STARTED ===");
            currentState = GameState.Battle;
            battleActive = true;
            battleTimer = 0f;
            
            // Enable ragdoll on both fighters
            if (ragdoll1 != null)
                ragdoll1.EnableRagdoll();
            if (ragdoll2 != null)
                ragdoll2.EnableRagdoll();
                
            // Apply initial forces to start the action
            StartCoroutine(ApplyInitialForces());
        }
        
        /// <summary>
        /// Apply initial forces to fighters
        /// </summary>
        private IEnumerator ApplyInitialForces()
        {
            yield return new WaitForSeconds(0.5f);
            
            // Fighter1 moves toward Fighter2
            Vector3 force1 = (fighter2.transform.position - fighter1.transform.position).normalized * forceAmount;
            ragdoll1?.ApplyForce(force1, fighter1.transform.position + Vector3.up);
            
            yield return new WaitForSeconds(0.2f);
            
            // Fighter2 moves toward Fighter1
            Vector3 force2 = (fighter1.transform.position - fighter2.transform.position).normalized * forceAmount;
            ragdoll2?.ApplyForce(force2, fighter2.transform.position + Vector3.up);
            
            Debug.Log("Initial battle forces applied!");
        }
        
        /// <summary>
        /// Apply random force to fighter
        /// </summary>
        private void ApplyRandomForceToFighter(RagdollPhysicsController ragdoll, string fighterName)
        {
            if (ragdoll == null) return;
            
            // Enable ragdoll if not active
            if (!ragdoll.IsRagdollActive)
                ragdoll.EnableRagdoll();
                
            // Apply random force
            ragdoll.ApplyRandomForce();
            Debug.Log($"Applied random force to {fighterName}");
        }
        
        /// <summary>
        /// End the battle
        /// </summary>
        public void EndBattle(string reason)
        {
            if (currentState == GameState.GameOver) return;
            
            Debug.Log($"=== BATTLE ENDED: {reason} ===");
            currentState = GameState.GameOver;
            battleActive = false;
        }
        
        /// <summary>
        /// Restart the battle
        /// </summary>
        public void RestartBattle()
        {
            Debug.Log("=== RESTARTING BATTLE ===");
            
            // Reset fighters
            if (ragdoll1 != null)
                ragdoll1.DisableRagdoll();
            if (ragdoll2 != null)
                ragdoll2.DisableRagdoll();
                
            // Reset positions
            if (fighter1 != null)
            {
                fighter1.transform.position = new Vector3(-5, 1, 0);
                fighter1.transform.rotation = Quaternion.Euler(0, 45, 0);
            }
            if (fighter2 != null)
            {
                fighter2.transform.position = new Vector3(5, 1, 0);
                fighter2.transform.rotation = Quaternion.Euler(0, -45, 0);
            }
            
            // Reset state
            currentState = GameState.Waiting;
            battleTimer = 0f;
            
            // Auto-start after delay
            if (autoStartBattle)
            {
                StartCoroutine(StartBattleAfterDelay(1f));
            }
        }
        
        /// <summary>
        /// OnGUI for demo controls
        /// </summary>
        private void OnGUI()
        {
            if (!enableDemoControls) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            
            GUILayout.Label("=== BASIC GAME MANAGER DEMO ===", GUI.skin.box);
            GUILayout.Label($"State: {currentState}");
            
            if (battleActive)
            {
                GUILayout.Label($"Battle Time: {battleTimer:F1}s / {battleDuration}s");
            }
            
            GUILayout.Space(10);
            GUILayout.Label("Controls:");
            GUILayout.Label("Space - Start Battle");
            GUILayout.Label("R - Restart Battle");
            GUILayout.Label("1 - Force Fighter1");
            GUILayout.Label("2 - Force Fighter2");
            
            if (battleActive)
            {
                if (GUILayout.Button("End Battle"))
                {
                    EndBattle("Manual end");
                }
            }
            
            if (currentState == GameState.GameOver)
            {
                if (GUILayout.Button("Restart"))
                {
                    RestartBattle();
                }
            }
            
            GUILayout.EndArea();
        }
    }
}
