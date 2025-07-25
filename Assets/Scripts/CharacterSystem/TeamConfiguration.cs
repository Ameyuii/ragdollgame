using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TeamConfiguration
{
    [Header("Identity")]
    public int teamID;
    public string teamName;
    [TextArea(2, 3)]
    public string teamDescription;

    [Header("Colors")]
    public Color primaryColor = Color.blue;
    public Color secondaryColor = Color.white;

    [Header("Materials")]
    public Material baseMaterial;
    public List<MaterialOverride> materialOverrides = new List<MaterialOverride>();

    [Header("Visual")]
    public Sprite teamIcon;
    public Texture2D teamFlag;

    [Header("Settings")]
    public bool isPlayerTeam = false;
    public bool isNeutralTeam = false;

    public TeamConfiguration()
    {
        materialOverrides = new List<MaterialOverride>();
    }

    /// <summary>
    /// Get the appropriate material for a character part
    /// </summary>
    public Material GetMaterialForPart(string partName)
    {
        MaterialOverride materialOverride = materialOverrides.Find(m => m.partName == partName);
        return materialOverride?.material ?? baseMaterial;
    }

    /// <summary>
    /// Check if this team is enemy to another team
    /// </summary>
    public bool IsEnemyTeam(TeamConfiguration otherTeam)
    {
        if (isNeutralTeam || otherTeam.isNeutralTeam)
            return false;

        return teamID != otherTeam.teamID;
    }

    /// <summary>
    /// Get team display name with ID
    /// </summary>
    public string GetDisplayName()
    {
        return $"Team {teamID}: {teamName}";
    }
}

[System.Serializable]
public class MaterialOverride
{
    [Header("Part Settings")]
    public string partName;
    public Material material;

    [Header("Optional Color Override")]
    public bool useColorOverride = false;
    public Color colorOverride = Color.white;

    public MaterialOverride()
    {
    }

    public MaterialOverride(string part, Material mat)
    {
        partName = part;
        material = mat;
    }
}