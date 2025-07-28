using UnityEngine;

public class CharacterTeamManager : MonoBehaviour
{
    [Header("Team Configuration")]
    public TeamConfiguration currentTeam;
    public int teamID;

    [Header("Team Relations")]
    public bool isNeutral = false;

    /// <summary>
    /// Set team configuration
    /// </summary>
    public void SetTeam(TeamConfiguration team)
    {
        currentTeam = team;
        teamID = team?.teamID ?? 0;
        isNeutral = team?.isNeutralTeam ?? false;
    }

    /// <summary>
    /// Set team by ID
    /// </summary>
    public void SetTeam(int id)
    {
        teamID = id;
        TeamConfiguration team = GameDatabase.Instance?.GetTeam(id);
        if (team != null)
        {
            SetTeam(team);
        }
    }

    /// <summary>
    /// Check if another character is an enemy
    /// </summary>
    public bool IsEnemy(CharacterTeamManager other)
    {
        if (other == null) return false;
        if (isNeutral || other.isNeutral) return false;
        
        return teamID != other.teamID;
    }

    /// <summary>
    /// Check if another character is an ally
    /// </summary>
    public bool IsAlly(CharacterTeamManager other)
    {
        if (other == null) return false;
        if (isNeutral || other.isNeutral) return false;
        
        return teamID == other.teamID;
    }

    /// <summary>
    /// Get team color
    /// </summary>
    public Color GetTeamColor()
    {
        return currentTeam?.primaryColor ?? Color.white;
    }

    /// <summary>
    /// Get team name
    /// </summary>
    public string GetTeamName()
    {
        return currentTeam?.teamName ?? $"Team {teamID}";
    }

    /// <summary>
    /// Check if this is player team
    /// </summary>
    public bool IsPlayerTeam()
    {
        return currentTeam?.isPlayerTeam ?? false;
    }
}