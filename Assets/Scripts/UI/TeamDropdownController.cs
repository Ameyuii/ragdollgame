using UnityEngine;
using UnityEngine.UI;

public class TeamDropdownController : MonoBehaviour
{
    [Header("UI References")]
    public Button teamDropdownButton;
    public GameObject teamDropdownPanel;
    public Button team1Option;
    public Button team2Option;
    public Button team3Option;
    public Button team4Option;
    public Text teamDropdownButtonText;
    
    private int selectedTeam = 1;
    private bool isDropdownOpen = false;
    
    void Start()
    {
        // Ensure default team is 1
        selectedTeam = 1;

        // Initialize UnifiedGameManager with default team
        UnifiedGameManager unifiedManager = UnifiedGameManager.Instance;
        if (unifiedManager != null)
        {
            unifiedManager.SetSelectedTeam(selectedTeam);
            Debug.Log($"✅ Initialized UnifiedGameManager.selectedTeam to {selectedTeam}");
        }

        // Hide dropdown initially
        if (teamDropdownPanel) teamDropdownPanel.SetActive(false);

        // Connect main button
        if (teamDropdownButton)
        {
            teamDropdownButton.onClick.RemoveAllListeners();
            teamDropdownButton.onClick.AddListener(ToggleDropdown);
        }

        // Connect team options
        if (team1Option)
        {
            team1Option.onClick.RemoveAllListeners();
            team1Option.onClick.AddListener(() => SelectTeam(1));
        }

        if (team2Option)
        {
            team2Option.onClick.RemoveAllListeners();
            team2Option.onClick.AddListener(() => SelectTeam(2));
        }

        if (team3Option)
        {
            team3Option.onClick.RemoveAllListeners();
            team3Option.onClick.AddListener(() => SelectTeam(3));
        }

        if (team4Option)
        {
            team4Option.onClick.RemoveAllListeners();
            team4Option.onClick.AddListener(() => SelectTeam(4));
        }

        // Update button text and color to show Team 1 initially
        UpdateButtonText();
        UpdateButtonColor();

        // Fix text materials after a short delay
        Invoke("FixTextMaterials", 0.1f);

        Debug.Log("TeamDropdownController initialized with Team 1");
    }
    
    public void ToggleDropdown()
    {
        if (teamDropdownPanel)
        {
            isDropdownOpen = !isDropdownOpen;
            teamDropdownPanel.SetActive(isDropdownOpen);
            Debug.Log($"Team dropdown {(isDropdownOpen ? "opened" : "closed")}");
        }
    }
    
    public void SelectTeam(int teamId)
    {
        selectedTeam = teamId;
        UpdateButtonText();
        
        // Close dropdown
        if (teamDropdownPanel)
        {
            teamDropdownPanel.SetActive(false);
            isDropdownOpen = false;
        }
        
        // Update game managers
        BattleGameManager gameManager = FindAnyObjectByType<BattleGameManager>();
        if (gameManager)
        {
            gameManager.selectedTeam = teamId;
            Debug.Log($"✅ Updated BattleGameManager.selectedTeam to {teamId}");
        }

        // Update UnifiedGameManager (new system)
        UnifiedGameManager unifiedManager = UnifiedGameManager.Instance;
        if (unifiedManager != null)
        {
            unifiedManager.SetSelectedTeam(teamId);
            Debug.Log($"✅ Updated UnifiedGameManager.selectedTeam to {teamId}");
        }
        else
        {
            Debug.LogWarning("⚠️ UnifiedGameManager not found!");
        }
        
        // Update button color
        UpdateButtonColor();
        
        Debug.Log($"Selected Team {teamId}");
    }
    
    void UpdateButtonText()
    {
        if (teamDropdownButtonText)
        {
            teamDropdownButtonText.text = $"TEAM {selectedTeam}";
        }
    }
    
    void UpdateButtonColor()
    {
        if (teamDropdownButton)
        {
            Image buttonImage = teamDropdownButton.GetComponent<Image>();
            if (buttonImage)
            {
                switch (selectedTeam)
                {
                    case 1:
                        buttonImage.color = new Color(0.2f, 0.4f, 0.8f, 1f); // Blue
                        break;
                    case 2:
                        buttonImage.color = new Color(0.8f, 0.2f, 0.2f, 1f); // Red
                        break;
                    case 3:
                        buttonImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green
                        break;
                    case 4:
                        buttonImage.color = new Color(0.8f, 0.8f, 0.2f, 1f); // Yellow
                        break;
                    default:
                        buttonImage.color = Color.white;
                        break;
                }
            }
        }
    }
    
    void FixTextMaterials()
    {
        Debug.Log("🔧 Fixing text materials in TeamDropdownPanel...");

        if (teamDropdownPanel == null)
        {
            Debug.LogError("❌ TeamDropdownPanel is null!");
            return;
        }

        // Get all Text components in the dropdown panel
        Text[] textComponents = teamDropdownPanel.GetComponentsInChildren<Text>();
        Debug.Log($"🔍 Found {textComponents.Length} text components");

        foreach (Text textComp in textComponents)
        {
            if (textComp == null) continue;

            GameObject parent = textComp.transform.parent.gameObject;
            Debug.Log($"🔧 Fixing text in {parent.name}");

            // Set default UI material
            textComp.material = Canvas.GetDefaultCanvasMaterial();

            // Ensure font is set
            if (textComp.font == null)
            {
                textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            // Set text content based on parent name
            if (parent.name.Contains("Team1"))
            {
                textComp.text = "🔵 TEAM 1";
            }
            else if (parent.name.Contains("Team2"))
            {
                textComp.text = "🔴 TEAM 2";
            }
            else if (parent.name.Contains("Team3"))
            {
                textComp.text = "🟢 TEAM 3";
            }
            else if (parent.name.Contains("Team4"))
            {
                textComp.text = "🟡 TEAM 4";
            }

            // Set text properties
            textComp.color = Color.white;
            textComp.fontSize = 16;
            textComp.alignment = TextAnchor.MiddleCenter;

            // Ensure RectTransform is properly set
            RectTransform textRect = textComp.GetComponent<RectTransform>();
            if (textRect != null)
            {
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.sizeDelta = Vector2.zero;
                textRect.anchoredPosition = Vector2.zero;
            }

            Debug.Log($"✅ Fixed text: '{textComp.text}' with material: {textComp.material.name}");
        }

        Debug.Log("🎉 Completed fixing text materials!");
    }

    // Simplified - no auto-close for now to avoid Input System conflicts
    void Update()
    {
        // Removed Input handling to avoid conflicts with new Input System
    }
    
    // Manual close method that can be called by other UI elements
    public void CloseDropdown()
    {
        if (teamDropdownPanel)
        {
            teamDropdownPanel.SetActive(false);
            isDropdownOpen = false;
            Debug.Log("Team dropdown closed manually");
        }
    }
    
    // Public method to get currently selected team
    public int GetSelectedTeam()
    {
        return selectedTeam;
    }
}