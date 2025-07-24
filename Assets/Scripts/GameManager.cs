using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager? Instance { get; private set; }

    // UI References
    [Header("UI References")]
    public Text? team1CountText;
    public Text? team2CountText;
    public Text? gameStatusText;
    public Button? startBattleButton;
    public Button? resetBattleButton;

    // Game References
    [Header("Game References")]
    public GameObject? characterPrefab;
    public Transform? team1SpawnArea;
    public Transform? team2SpawnArea;

    // Dictionary to hold characters by team
    private Dictionary<int, List<RagdollCharacter>> teamCharacters = new Dictionary<int, List<RagdollCharacter>>();

    // List of all alive characters
    private List<RagdollCharacter> aliveCharacters = new List<RagdollCharacter>();

    // Game state
    private bool battleInProgress = false;
    
    // Public method to check if battle is in progress
    public bool IsBattleInProgress()
    {
        return battleInProgress;
    }

    void Awake()
    {
        // Singleton pattern enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize team dictionary if needed
        teamCharacters.Clear();
        aliveCharacters.Clear();
    }

    void Start()
    {
        // Initialize with proper game state message
        if (gameStatusText != null)
        {
            gameStatusText.text = "Press START BATTLE to begin the epic ragdoll fight!";
        }
        
        UpdateUI();
    }

    // Register a character to the manager (only during battle)
    public void RegisterCharacter(RagdollCharacter character)
    {
        if (character == null || !battleInProgress) return;

        // Add to team list
        if (!teamCharacters.ContainsKey(character.GetTeamId()))
        {
            teamCharacters[character.GetTeamId()] = new List<RagdollCharacter>();
        }
        if (!teamCharacters[character.GetTeamId()].Contains(character))
        {
            teamCharacters[character.GetTeamId()].Add(character);
        }

        // Add to alive list
        if (!aliveCharacters.Contains(character))
        {
            aliveCharacters.Add(character);
        }

        UpdateUI();
    }

    // Called when a character dies
    public void OnCharacterDied(RagdollCharacter character)
    {
        if (character == null) return;

        // Remove from alive list
        if (aliveCharacters.Contains(character))
        {
            aliveCharacters.Remove(character);
        }

        // Check for team elimination or game over conditions
        CheckGameStatus();
        UpdateUI();
    }

    // Get all characters of a team
    public List<RagdollCharacter> GetTeamCharacters(int team)
    {
        if (teamCharacters.ContainsKey(team))
        {
            return teamCharacters[team];
        }
        return new List<RagdollCharacter>();
    }

    // Get all alive characters
    public List<RagdollCharacter> GetAliveCharacters()
    {
        return new List<RagdollCharacter>(aliveCharacters);
    }

    // Start battle
    public void StartBattle()
    {
        if (battleInProgress) return;

        battleInProgress = true;
        
        // Register characters with delay to prevent collision conflicts
        RagdollCharacter[] allCharacters = Object.FindObjectsByType<RagdollCharacter>(FindObjectsSortMode.None);
        StartCoroutine(RegisterCharactersWithDelay(allCharacters));

        if (gameStatusText != null)
        {
            gameStatusText.text = "BATTLE IN PROGRESS!";
        }

        if (startBattleButton != null)
        {
            startBattleButton.interactable = false;
        }

        Debug.Log("Battle started!");
    }

    // Register characters with delay to prevent physics conflicts
    private System.Collections.IEnumerator RegisterCharactersWithDelay(RagdollCharacter[] characters)
    {
        foreach (var character in characters)
        {
            RegisterCharacter(character);
            yield return new WaitForSeconds(0.05f); // Small delay between registrations
        }
    }

    // Reset battle
    public void ResetBattle()
    {
        battleInProgress = false;

        // Clear all character lists
        teamCharacters.Clear();
        aliveCharacters.Clear();

        // Reset all ragdoll characters
        RagdollCharacter[] allCharacters = Object.FindObjectsByType<RagdollCharacter>(FindObjectsSortMode.None);
        foreach (var character in allCharacters)
        {
            character.ResetCharacter();
        }

        if (gameStatusText != null)
        {
            gameStatusText.text = "Press START BATTLE to begin the epic ragdoll fight!";
        }

        if (startBattleButton != null)
        {
            startBattleButton.interactable = true;
        }

        UpdateUI();
        Debug.Log("Battle reset!");
    }

    // Check game status, e.g., if a team has no alive characters left
    private void CheckGameStatus()
    {
        if (!battleInProgress) return;

        // Count alive characters per team
        Dictionary<int, int> alivePerTeam = new Dictionary<int, int>();
        
        foreach (var character in aliveCharacters)
        {
            if (!alivePerTeam.ContainsKey(character.GetTeamId()))
            {
                alivePerTeam[character.GetTeamId()] = 0;
            }
            alivePerTeam[character.GetTeamId()]++;
        }

        // Check for victory conditions
        if (alivePerTeam.Count <= 1)
        {
            battleInProgress = false;
            
            if (alivePerTeam.Count == 1)
            {
                int winningTeam = 0;
                foreach (var kvp in alivePerTeam)
                {
                    winningTeam = kvp.Key;
                    break;
                }
                
                if (gameStatusText != null)
                {
                    gameStatusText.text = $"TEAM {winningTeam} WINS!";
                }
                Debug.Log($"Team {winningTeam} wins the battle!");
            }
            else
            {
                if (gameStatusText != null)
                {
                    gameStatusText.text = "DRAW! All teams eliminated!";
                }
                Debug.Log("Battle ended in a draw!");
            }

            if (startBattleButton != null)
            {
                startBattleButton.interactable = true;
            }
        }
    }

    // Update UI display
    private void UpdateUI()
    {
        if (team1CountText != null)
        {
            int team1Count = GetAliveTeamCount(1);
            team1CountText.text = $"TEAM 1 (BLUE): {team1Count}";
        }

        if (team2CountText != null)
        {
            int team2Count = GetAliveTeamCount(2);
            team2CountText.text = $"TEAM 2 (RED): {team2Count}";
        }
    }

    // Get count of alive characters in a team
    private int GetAliveTeamCount(int team)
    {
        int count = 0;
        foreach (var character in aliveCharacters)
        {
            if (character.GetTeamId() == team)
            {
                count++;
            }
        }
        return count;
    }
}