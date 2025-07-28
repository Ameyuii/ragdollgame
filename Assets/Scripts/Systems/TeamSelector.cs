using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TeamSelector : MonoBehaviour
{
    [Header("UI References")]
    public Button? teamDropdownButton;
    public Text? teamDisplayText;
    public GameObject? dropdownPanel;
    public Transform? dropdownContent;
    public GameObject? teamButtonPrefab;
    
    [Header("Team Configuration")]
    public TeamData[] availableTeams = new TeamData[]
    {
        new TeamData { teamId = 1, teamName = "TEAM 1" },
        new TeamData { teamId = 2, teamName = "TEAM 2" },
        new TeamData { teamId = 3, teamName = "TEAM 3" },
        new TeamData { teamId = 4, teamName = "TEAM 4" }
    };
    
    [System.Serializable]
    public class TeamData
    {
        public int teamId;
        public string teamName;
        // ✅ DISABLED: Team color - loại bỏ hoàn toàn chức năng màu team
        // public Color teamColor;
    }
    
    // Current selected team
    private int selectedTeamId = 1;
    private bool isDropdownOpen = false;
    
    // Events
    public System.Action<int>? OnTeamChanged;
    
    void Start()
    {
        InitializeTeamSelector();
        SetupUI();
    }
    
    void InitializeTeamSelector()
    {
        // Find UI elements if not assigned
        if (teamDropdownButton == null)
            teamDropdownButton = GetComponentInChildren<Button>();
        
        if (teamDisplayText == null)
            teamDisplayText = GetComponentInChildren<Text>();
        
        // Set default team to Team 1
        selectedTeamId = 1;
        UpdateDisplayText();

        // Update UnifiedGameManager with initial team
        UnifiedGameManager unifiedManager = UnifiedGameManager.Instance;
        if (unifiedManager != null)
        {
            unifiedManager.SetSelectedTeam(selectedTeamId);
            Debug.Log($"✅ Initialized UnifiedGameManager.selectedTeam to {selectedTeamId}");
        }

        // Notify BattleGameManager about initial team selection
        OnTeamChanged?.Invoke(selectedTeamId);
        
        // Setup button listener
        if (teamDropdownButton != null)
            teamDropdownButton.onClick.AddListener(ToggleDropdown);
    }
    
    void SetupUI()
    {
        // Create dropdown panel if it doesn't exist
        if (dropdownPanel == null)
        {
            CreateDropdownPanel();
        }
        
        // Find content if not set
        if (dropdownContent == null && dropdownPanel != null)
        {
            // Look for existing content or create new one
            Transform existingContent = dropdownPanel.transform.Find("Content");
            if (existingContent != null)
            {
                dropdownContent = existingContent;
            }
            else
            {
                // Create content area
                GameObject content = new GameObject("Content");
                content.transform.SetParent(dropdownPanel.transform, false);
                
                RectTransform contentRect = content.AddComponent<RectTransform>();
                contentRect.anchorMin = Vector2.zero;
                contentRect.anchorMax = Vector2.one;
                contentRect.offsetMin = new Vector2(5, 5);
                contentRect.offsetMax = new Vector2(-5, -5);
                
                // Add Vertical Layout Group
                VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
                layoutGroup.spacing = 2f;
                layoutGroup.padding = new RectOffset(5, 5, 5, 5);
                layoutGroup.childControlHeight = true;
                layoutGroup.childControlWidth = true;
                layoutGroup.childForceExpandHeight = false;
                layoutGroup.childForceExpandWidth = true;
                
                dropdownContent = content.transform;
            }
        }
        
        // Initially hide dropdown
        if (dropdownPanel != null)
            dropdownPanel.SetActive(false);
        
        // Create team buttons
        CreateTeamButtons();
    }
    
    void CreateDropdownPanel()
    {
        // Create dropdown panel as child of this object
        GameObject panel = new GameObject("DropdownPanel");
        panel.transform.SetParent(transform, false);
        
        // Add RectTransform
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0);
        panelRect.anchorMax = new Vector2(1, 0);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0, -5);
        panelRect.sizeDelta = new Vector2(0, 160); // Smaller height
        
        // Add background image
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        
        // Create content area
        GameObject content = new GameObject("Content");
        content.transform.SetParent(panel.transform, false);
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = new Vector2(5, 5);
        contentRect.offsetMax = new Vector2(-5, -5);
        
        // Add Vertical Layout Group
        VerticalLayoutGroup layoutGroup = content.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 2f;
        layoutGroup.padding = new RectOffset(5, 5, 5, 5);
        layoutGroup.childControlHeight = true;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;
        
        dropdownPanel = panel;
        dropdownContent = content.transform;
    }
    
    void CreateTeamButtons()
    {
        if (dropdownContent == null) return;
        
        // Clear existing buttons
        foreach (Transform child in dropdownContent)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
        
        // Create button for each team
        foreach (TeamData team in availableTeams)
        {
            CreateTeamButton(team);
        }
    }
    
    void CreateTeamButton(TeamData teamData)
    {
        // Create button GameObject
        GameObject buttonObj = new GameObject($"TeamButton_{teamData.teamId}");
        buttonObj.transform.SetParent(dropdownContent, false);
        
        // Add RectTransform
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(0, 35);
        
        // Add LayoutElement to ensure proper sizing
        UnityEngine.UI.LayoutElement layoutElement = buttonObj.AddComponent<UnityEngine.UI.LayoutElement>();
        layoutElement.minHeight = 35;
        layoutElement.preferredHeight = 35;
        
        // Add Image component
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Add Button component
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
        // ✅ DISABLED: Set button colors - loại bỏ hoàn toàn chức năng màu team
        /*
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        colors.highlightedColor = teamData.teamColor * 0.8f;
        colors.pressedColor = teamData.teamColor * 0.6f;
        colors.selectedColor = teamData.teamColor * 0.8f;
        button.colors = colors;
        */
        
        // Create text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 0);
        textRect.offsetMax = new Vector2(-10, 0);
        
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = teamData.teamName;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 14;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleLeft;
        
        // Add button listener
        int teamId = teamData.teamId; // Capture for closure
        button.onClick.AddListener(() => SelectTeam(teamId));
    }
    
    public void ToggleDropdown()
    {
        Debug.Log($"ToggleDropdown called. dropdownPanel null: {dropdownPanel == null}");
        
        if (dropdownPanel == null) 
        {
            Debug.LogError("DropdownPanel is null! Cannot toggle dropdown.");
            return;
        }
        
        isDropdownOpen = !isDropdownOpen;
        dropdownPanel.SetActive(isDropdownOpen);
        
        Debug.Log($"Team dropdown {(isDropdownOpen ? "opened" : "closed")}. Panel active: {dropdownPanel.activeSelf}");
    }
    
    public void SelectTeam(int teamId)
    {
        selectedTeamId = teamId;
        UpdateDisplayText();
        
        // Close dropdown
        if (dropdownPanel != null)
            dropdownPanel.SetActive(false);
        isDropdownOpen = false;
        
        // Update UnifiedGameManager (new system)
        UnifiedGameManager unifiedManager = UnifiedGameManager.Instance;
        if (unifiedManager != null)
        {
            unifiedManager.SetSelectedTeam(selectedTeamId);
            Debug.Log($"✅ Updated UnifiedGameManager.selectedTeam to {selectedTeamId}");
        }

        // Notify listeners
        OnTeamChanged?.Invoke(selectedTeamId);

        Debug.Log($"Selected team: {selectedTeamId}");
    }
    
    void UpdateDisplayText()
    {
        if (teamDisplayText == null) return;
        
        TeamData selectedTeam = GetTeamData(selectedTeamId);
        if (selectedTeam != null)
        {
            teamDisplayText.text = selectedTeam.teamName;

            // ✅ DISABLED: Update button color - loại bỏ hoàn toàn chức năng màu team
            /*
            if (teamDropdownButton != null)
            {
                Image buttonImage = teamDropdownButton.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = selectedTeam.teamColor;
                }
            }
            */
        }
    }
    
    TeamData GetTeamData(int teamId)
    {
        foreach (TeamData team in availableTeams)
        {
            if (team.teamId == teamId)
                return team;
        }
        return null;
    }
    
    // Public methods for external access
    public int GetSelectedTeam()
    {
        return selectedTeamId;
    }
    
    public void SetSelectedTeam(int teamId)
    {
        if (GetTeamData(teamId) != null)
        {
            SelectTeam(teamId);
        }
    }
    
    public string GetSelectedTeamName()
    {
        TeamData team = GetTeamData(selectedTeamId);
        return team != null ? team.teamName : "Unknown Team";
    }
    
    // ✅ DISABLED: Get selected team color - loại bỏ hoàn toàn chức năng màu team
    /*
    public Color GetSelectedTeamColor()
    {
        TeamData team = GetTeamData(selectedTeamId);
        return team != null ? team.teamColor : Color.white;
    }
    */
    
    // Close dropdown when clicking outside
    void Update()
    {
        if (isDropdownOpen)
        {
            var mouse = UnityEngine.InputSystem.Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePos = mouse.position.ReadValue();
                RectTransform dropdownRect = dropdownPanel.GetComponent<RectTransform>();
                
                if (!RectTransformUtility.RectangleContainsScreenPoint(dropdownRect, mousePos, null))
                {
                    ToggleDropdown();
                }
            }
        }
    }
}