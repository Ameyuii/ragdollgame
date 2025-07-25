using UnityEngine;
using System;

public static class CharacterEvents
{
    // Character lifecycle events
    public static event Action<ICharacter> OnCharacterSpawned;
    public static event Action<ICharacter> OnCharacterDied;
    public static event Action<ICharacter, int> OnCharacterTeamChanged;

    // Selection events
    public static event Action<string> OnCharacterSelected;
    public static event Action<string> OnVariantSelected;
    public static event Action<int> OnTeamSelected;

    // UI events
    public static event Action<CharacterSelectionResult> OnSelectionConfirmed;
    public static event Action OnSelectionCancelled;

    // Battle events
    public static event Action OnBattleStarted;
    public static event Action<int> OnTeamVictory;

    // Trigger methods
    public static void TriggerCharacterSpawned(ICharacter character)
    {
        OnCharacterSpawned?.Invoke(character);
    }

    public static void TriggerCharacterDied(ICharacter character)
    {
        OnCharacterDied?.Invoke(character);
    }

    public static void TriggerCharacterTeamChanged(ICharacter character, int newTeamID)
    {
        OnCharacterTeamChanged?.Invoke(character, newTeamID);
    }

    public static void TriggerCharacterSelected(string characterID)
    {
        OnCharacterSelected?.Invoke(characterID);
    }

    public static void TriggerVariantSelected(string variantID)
    {
        OnVariantSelected?.Invoke(variantID);
    }

    public static void TriggerTeamSelected(int teamID)
    {
        OnTeamSelected?.Invoke(teamID);
    }

    public static void TriggerSelectionConfirmed(CharacterSelectionResult result)
    {
        OnSelectionConfirmed?.Invoke(result);
    }

    public static void TriggerSelectionCancelled()
    {
        OnSelectionCancelled?.Invoke();
    }

    public static void TriggerBattleStarted()
    {
        OnBattleStarted?.Invoke();
    }

    public static void TriggerTeamVictory(int teamID)
    {
        OnTeamVictory?.Invoke(teamID);
    }
}

[System.Serializable]
public class CharacterSelectionResult
{
    public string characterID;
    public string variantID;
    public int teamID;
    public CharacterDefinition definition;
    public Vector3 spawnPosition;
    public DateTime selectionTime;

    public CharacterSelectionResult()
    {
        selectionTime = DateTime.Now;
    }

    public CharacterSelectionResult(string charID, string varID, int team, Vector3 position)
    {
        characterID = charID;
        variantID = varID;
        teamID = team;
        spawnPosition = position;
        selectionTime = DateTime.Now;
    }
}