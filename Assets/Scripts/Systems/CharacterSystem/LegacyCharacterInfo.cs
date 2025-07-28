using UnityEngine;

[System.Serializable]
public struct CharacterInfo
{
    [Header("Basic Info")]
    public string characterName;
    public GameObject prefab;
    public Sprite uiIcon;
    
    [Header("Stats")]
    public int health;
    public float speed;
    public float attackDamage;
    public float attackRange;
    
    [Header("Visual")]
    public Color teamColor;
    [TextArea(2, 4)]
    public string description;
    
    [Header("Category")]
    public CharacterType characterType;

    public CharacterInfo(string name, GameObject prefabObj, int hp = 100, float spd = 5f)
    {
        characterName = name;
        prefab = prefabObj;
        uiIcon = null;
        health = hp;
        speed = spd;
        attackDamage = 20f;
        attackRange = 2f;
        teamColor = Color.white;
        description = "";
        characterType = CharacterType.Soldier;
    }
}

[System.Serializable]
public class CharacterCategoryData
{
    [Header("Category Info")]
    public string categoryName;
    public CharacterType categoryType;
    public Color categoryColor = Color.white;
    public Sprite categoryIcon;
    
    [Header("Characters")]
    public CharacterInfo[] characters = new CharacterInfo[0];
    
    [Header("UI Settings")]
    public bool isExpanded = true;
}

public enum CharacterType
{
    Soldier,
    Robot,
    Monster,
    Zombie,
    Mech,
    Beast,
    Knight,
    Archer,
    Mage,
    Assassin
}