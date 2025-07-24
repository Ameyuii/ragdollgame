using UnityEngine;
using UnityEngine.AI;

public class SetupCompleteCharacter : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("Setting up complete character components...");
        
        // Find all characters
        string[] characterNames = { "Character_Team1_1", "Character_Team1_2", "Character_Team2_1", "Character_Team2_2" };
        
        foreach (string charName in characterNames)
        {
            GameObject character = GameObject.Find(charName);
            if (character == null)
            {
                Debug.LogWarning($"Character {charName} not found!");
                continue;
            }
            
            Debug.Log($"Setting up {charName}...");
            
            // Add Rigidbody if not present
            Rigidbody rb = character.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = character.AddComponent<Rigidbody>();
            }
            
            // Configure Rigidbody
            rb.mass = 1f;
            rb.linearDamping = 5f;
            rb.angularDamping = 10f;
            rb.useGravity = false;
            rb.isKinematic = true;
            
            // Ensure NavMeshAgent is properly configured
            NavMeshAgent agent = character.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = 3.5f;
                agent.acceleration = 8f;
                agent.angularSpeed = 120f;
                agent.stoppingDistance = 1.5f; // Stop at attack range
                agent.radius = 0.5f;
                agent.height = 2f;
                agent.baseOffset = 0f;
            }
            
            // Ensure CapsuleCollider is properly configured
            CapsuleCollider collider = character.GetComponent<CapsuleCollider>();
            if (collider != null)
            {
                collider.radius = 0.5f;
                collider.height = 2f;
                collider.center = new Vector3(0, 1f, 0);
                collider.isTrigger = false;
            }
            
            // Add CharacterDragDrop if not present
            CharacterDragDrop dragDrop = character.GetComponent<CharacterDragDrop>();
            if (dragDrop == null)
            {
                dragDrop = character.AddComponent<CharacterDragDrop>();
            }
            
            // Configure CharacterDragDrop
            dragDrop.raycastDistance = 100f;
            dragDrop.groundOffset = 0.1f;
            
            // Ensure RagdollCharacter is properly configured
            RagdollCharacter ragdollChar = character.GetComponent<RagdollCharacter>();
            if (ragdollChar != null)
            {
                ragdollChar.maxHealth = 100f;
                ragdollChar.moveSpeed = 3f;
                ragdollChar.attackDamage = 25f;
                ragdollChar.attackRange = 2f;
                ragdollChar.attackCooldown = 1.5f;
                
                // Set team based on character name
                if (charName.Contains("Team1"))
                {
                    ragdollChar.teamId = 1;
                }
                else if (charName.Contains("Team2"))
                {
                    ragdollChar.teamId = 2;
                }
            }
            
            // Set appropriate tags
            if (charName.Contains("Team1"))
            {
                character.tag = "Team1";
            }
            else if (charName.Contains("Team2"))
            {
                character.tag = "Team2";
            }
            
            // Create health bar for character
            CreateHealthBar(character);
            
            Debug.Log($"Completed setup for {charName}");
        }
        
        Debug.Log("All characters setup completed!");
    }
    
    static void CreateHealthBar(GameObject character)
    {
        // Check if health bar already exists
        Transform existingHealthBar = character.transform.Find("HealthBar");
        if (existingHealthBar != null)
        {
            Debug.Log($"Health bar already exists for {character.name}");
            return;
        }
        
        // Create health bar canvas
        GameObject healthBarObj = new GameObject("HealthBar");
        healthBarObj.transform.SetParent(character.transform);
        healthBarObj.transform.localPosition = new Vector3(0, 2.5f, 0);
        healthBarObj.transform.localRotation = Quaternion.identity;
        healthBarObj.transform.localScale = Vector3.one;
        
        Canvas canvas = healthBarObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        canvas.sortingOrder = 10;
        
        // Set canvas size
        RectTransform canvasRect = healthBarObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(2f, 0.3f);
        
        // Create background
        GameObject healthBarBG = new GameObject("HealthBarBG");
        healthBarBG.transform.SetParent(healthBarObj.transform, false);
        
        UnityEngine.UI.Image bgImage = healthBarBG.AddComponent<UnityEngine.UI.Image>();
        bgImage.color = Color.black;
        
        RectTransform bgRect = healthBarBG.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // Create health fill
        GameObject healthBarFill = new GameObject("HealthBarFill");
        healthBarFill.transform.SetParent(healthBarBG.transform, false);
        
        UnityEngine.UI.Image fillImage = healthBarFill.AddComponent<UnityEngine.UI.Image>();
        fillImage.color = Color.green;
        fillImage.type = UnityEngine.UI.Image.Type.Filled;
        fillImage.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;
        
        RectTransform fillRect = healthBarFill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        // Add slider component to health bar
        UnityEngine.UI.Slider slider = healthBarObj.AddComponent<UnityEngine.UI.Slider>();
        slider.fillRect = fillRect;
        slider.value = 1f;
        slider.interactable = false;
        
        // Link health bar to RagdollCharacter
        RagdollCharacter ragdollChar = character.GetComponent<RagdollCharacter>();
        if (ragdollChar != null)
        {
            ragdollChar.healthBarCanvas = canvas;
            ragdollChar.healthSlider = slider;
        }
        
        Debug.Log($"Created health bar for {character.name}");
    }
}