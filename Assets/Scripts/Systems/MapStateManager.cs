using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class CharacterInstance
{
    public string instanceID;
    public string characterID;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public int team;
    public GameObject gameObject;
    public Dictionary<string, object> customProperties;
    
    public CharacterInstance()
    {
        customProperties = new Dictionary<string, object>();
        scale = Vector3.one;
    }
}

public class MapStateManager : MonoBehaviour
{
    [Header("Map Configuration")]
    public Vector2 mapBounds = new Vector2(20f, 20f);
    public LayerMask groundLayer = 1;
    
    [Header("State Management")]
    public List<CharacterInstance> characterInstances = new List<CharacterInstance>();
    
    private int instanceCounter = 0;
    
    public static MapStateManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public string AddCharacterInstance(string characterID, Vector3 position, int team, GameObject prefab)
    {
        // Generate unique instance ID
        string instanceID = $"{characterID}_{instanceCounter:000}";
        instanceCounter++;
        
        // Create character instance
        CharacterInstance instance = new CharacterInstance
        {
            instanceID = instanceID,
            characterID = characterID,
            position = position,
            rotation = Vector3.zero,
            scale = Vector3.one,
            team = team
        };
        
        // Instantiate GameObject
        GameObject newCharacter = Instantiate(prefab, position, Quaternion.identity);
        newCharacter.name = instanceID;
        instance.gameObject = newCharacter;
        
        // Set team on RagdollCharacter if exists
        RagdollCharacter ragdoll = newCharacter.GetComponent<RagdollCharacter>();
        if (ragdoll != null)
        {
            ragdoll.teamId = team;
        }
        
        // Add to list
        characterInstances.Add(instance);
        
        Debug.Log($"Added character instance: {instanceID} at {position} for team {team}");
        
        return instanceID;
    }
    
    public bool RemoveCharacterInstance(string instanceID)
    {
        CharacterInstance instance = GetCharacterInstance(instanceID);
        if (instance != null)
        {
            if (instance.gameObject != null)
            {
                DestroyImmediate(instance.gameObject);
            }
            
            characterInstances.Remove(instance);
            Debug.Log($"Removed character instance: {instanceID}");
            return true;
        }
        
        return false;
    }
    
    public CharacterInstance GetCharacterInstance(string instanceID)
    {
        return characterInstances.Find(x => x.instanceID == instanceID);
    }
    
    public List<CharacterInstance> GetCharactersByTeam(int team)
    {
        return characterInstances.FindAll(x => x.team == team);
    }
    
    public List<CharacterInstance> GetCharactersInRadius(Vector3 center, float radius)
    {
        List<CharacterInstance> nearbyCharacters = new List<CharacterInstance>();
        
        foreach (CharacterInstance instance in characterInstances)
        {
            if (Vector3.Distance(instance.position, center) <= radius)
            {
                nearbyCharacters.Add(instance);
            }
        }
        
        return nearbyCharacters;
    }
    
    public bool IsValidPosition(Vector3 position, float minDistance = 1.5f)
    {
        // Check map bounds
        if (Mathf.Abs(position.x) > mapBounds.x / 2 || Mathf.Abs(position.z) > mapBounds.y / 2)
        {
            return false;
        }
        
        // Check distance from other characters
        foreach (CharacterInstance instance in characterInstances)
        {
            if (Vector3.Distance(instance.position, position) < minDistance)
            {
                return false;
            }
        }
        
        return true;
    }
    
    public void ClearAllCharacters()
    {
        foreach (CharacterInstance instance in characterInstances)
        {
            if (instance.gameObject != null)
            {
                DestroyImmediate(instance.gameObject);
            }
        }
        
        characterInstances.Clear();
        instanceCounter = 0;
        Debug.Log("Cleared all character instances");
    }
    
    public Vector3 GetGroundPosition(Vector3 worldPosition)
    {
        // Raycast down to find ground
        Ray ray = new Ray(worldPosition + Vector3.up * 10f, Vector3.down);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 20f, groundLayer))
        {
            return hit.point;
        }
        
        // Fallback to y = 0
        return new Vector3(worldPosition.x, 0f, worldPosition.z);
    }
    
    // Save/Load functionality
    [Serializable]
    public class MapSaveData
    {
        public List<CharacterInstanceData> characters = new List<CharacterInstanceData>();
    }
    
    [Serializable]
    public class CharacterInstanceData
    {
        public string instanceID;
        public string characterID;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public int team;
    }
    
    public string SaveMapState()
    {
        MapSaveData saveData = new MapSaveData();
        
        foreach (CharacterInstance instance in characterInstances)
        {
            CharacterInstanceData data = new CharacterInstanceData
            {
                instanceID = instance.instanceID,
                characterID = instance.characterID,
                position = instance.position,
                rotation = instance.rotation,
                scale = instance.scale,
                team = instance.team
            };
            saveData.characters.Add(data);
        }
        
        return JsonUtility.ToJson(saveData, true);
    }
    
    public void LoadMapState(string jsonData)
    {
        try
        {
            ClearAllCharacters();
            
            MapSaveData saveData = JsonUtility.FromJson<MapSaveData>(jsonData);
            
            foreach (CharacterInstanceData data in saveData.characters)
            {
                // Would need reference to prefabs to fully implement loading
                Debug.Log($"Would load: {data.instanceID} at {data.position}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load map state: {e.Message}");
        }
    }
}