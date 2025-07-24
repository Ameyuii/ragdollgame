using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AdvancedUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject characterSelectionPanel;
    public GameObject mapArea;
    public GameObject propertiesPanel;
    public GameObject controlsPanel;
    
    [Header("Character Selection")]
    public Transform characterListParent;
    public GameObject characterButtonPrefab;
    public InputField searchField;
    public Dropdown filterDropdown;
    
    [Header("Properties Panel")]
    public Text selectedCharacterInfo;
    public Button deleteButton;
    public Button duplicateButton;
    public Slider scaleSlider;
    public Slider rotationSlider;
    
    [Header("Controls")]
    public Button startSimulationButton;
    public Button saveButton;
    public Button loadButton;
    public Button undoButton;
    public Button redoButton;
    public Button clearAllButton;
    
    [Header("Team Selection")]
    public Button team1Button;
    public Button team2Button;
    public Text currentTeamText;
    
    [Header("Map Controls")]
    public Toggle gridToggle;
    public Slider gridSizeSlider;
    public Text coordinatesText;
    
    private BattleGameManager battleManager;
    private MapStateManager mapStateManager;
    private int selectedTeam = 1;
    private int selectedCharacterType = 0;
    private string selectedInstanceID = "";
    private List<string> actionHistory = new List<string>();
    private int historyIndex = -1;
    
    void Start()
    {
        battleManager = FindObjectOfType<BattleGameManager>();
        mapStateManager = FindObjectOfType<MapStateManager>();
        
        if (mapStateManager == null)
        {
            GameObject mapManager = new GameObject("MapStateManager");
            mapStateManager = mapManager.AddComponent<MapStateManager>();
        }
        
        InitializeUI();
        SetupEventListeners();
    }
    
    void InitializeUI()
    {
        // Create main UI structure if not exists
        CreateUIStructure();
        
        // Initialize character selection
        PopulateCharacterList();
        
        // Initialize team selection
        UpdateTeamSelection();
        
        // Initialize properties panel (hidden by default)
        if (propertiesPanel != null)
            propertiesPanel.SetActive(false);
    }
    
    void CreateUIStructure()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        // Character Selection Panel (Left side)
        if (characterSelectionPanel == null)
        {
            characterSelectionPanel = CreatePanel("CharacterSelectionPanel", canvas.transform);
            RectTransform rect = characterSelectionPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0.25f, 1);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // Add background
            Image bg = characterSelectionPanel.GetComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        }
        
        // Properties Panel (Right side)
        if (propertiesPanel == null)
        {
            propertiesPanel = CreatePanel("PropertiesPanel", canvas.transform);
            RectTransform rect = propertiesPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.75f, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // Add background
            Image bg = propertiesPanel.GetComponent<Image>();
            bg.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        }
        
        // Controls Panel (Top)
        if (controlsPanel == null)
        {
            controlsPanel = CreatePanel("ControlsPanel", canvas.transform);
            RectTransform rect = controlsPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.25f, 0.9f);
            rect.anchorMax = new Vector2(0.75f, 1);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // Add background
            Image bg = controlsPanel.GetComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        }
    }
    
    GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        Image image = panel.AddComponent<Image>();
        
        return panel;
    }
    
    void PopulateCharacterList()
    {
        if (battleManager == null || battleManager.characterPrefabs == null) return;
        
        // Create search field
        CreateSearchField();
        
        // Create filter dropdown
        CreateFilterDropdown();
        
        // Create character buttons
        for (int i = 0; i < battleManager.characterPrefabs.Length; i++)
        {
            CreateAdvancedCharacterButton(battleManager.characterPrefabs[i], i);
        }
        
        // Create team selection
        CreateTeamSelectionButtons();
    }
    
    void CreateSearchField()
    {
        if (characterSelectionPanel == null) return;
        
        GameObject searchObj = new GameObject("SearchField");
        searchObj.transform.SetParent(characterSelectionPanel.transform, false);
        
        RectTransform rect = searchObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.9f);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = new Vector2(10, -40);
        rect.offsetMax = new Vector2(-10, -10);
        
        Image bg = searchObj.AddComponent<Image>();
        bg.color = Color.white;
        
        searchField = searchObj.AddComponent<InputField>();
        
        // Create placeholder text
        GameObject placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(searchObj.transform, false);
        Text placeholderText = placeholder.AddComponent<Text>();
        placeholderText.text = "Search characters...";
        placeholderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        placeholderText.fontSize = 12;
        placeholderText.color = Color.gray;
        
        RectTransform placeholderRect = placeholder.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = new Vector2(5, 0);
        placeholderRect.offsetMax = new Vector2(-5, 0);
        
        searchField.placeholder = placeholderText;
        searchField.onValueChanged.AddListener(OnSearchChanged);
    }
    
    void CreateFilterDropdown()
    {
        // Implementation for filter dropdown
        // This would filter characters by type, size, etc.
    }
    
    void CreateAdvancedCharacterButton(GameObject prefab, int index)
    {
        if (characterSelectionPanel == null) return;
        
        GameObject button = new GameObject($"CharacterButton_{index}");
        button.transform.SetParent(characterSelectionPanel.transform, false);
        
        RectTransform rect = button.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 80);
        
        Image bg = button.AddComponent<Image>();
        bg.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        
        Button btn = button.AddComponent<Button>();
        
        // Add character preview image (if available)
        GameObject imageObj = new GameObject("CharacterImage");
        imageObj.transform.SetParent(button.transform, false);
        
        RectTransform imageRect = imageObj.AddComponent<RectTransform>();
        imageRect.anchorMin = new Vector2(0, 0);
        imageRect.anchorMax = new Vector2(0.3f, 1);
        imageRect.offsetMin = new Vector2(5, 5);
        imageRect.offsetMax = new Vector2(-5, -5);
        
        Image characterImage = imageObj.AddComponent<Image>();
        characterImage.color = Color.gray; // Placeholder
        
        // Add character name and info
        GameObject textObj = new GameObject("CharacterInfo");
        textObj.transform.SetParent(button.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.3f, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = new Vector2(5, 5);
        textRect.offsetMax = new Vector2(-5, -5);
        
        Text text = textObj.AddComponent<Text>();
        text.text = $"{prefab.name}\nType: Character"; // Could add more info
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 10;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;
        
        // Add drag component
        CharacterDragSource dragSource = button.AddComponent<CharacterDragSource>();
        dragSource.characterPrefab = prefab;
        dragSource.gameManager = battleManager;
        
        btn.onClick.AddListener(() => SelectCharacterType(index));
    }
    
    void CreateTeamSelectionButtons()
    {
        if (characterSelectionPanel == null) return;
        
        // Team 1 Button
        GameObject team1Obj = new GameObject("Team1Button");
        team1Obj.transform.SetParent(characterSelectionPanel.transform, false);
        
        RectTransform rect1 = team1Obj.AddComponent<RectTransform>();
        rect1.sizeDelta = new Vector2(0, 50);
        
        Image bg1 = team1Obj.AddComponent<Image>();
        bg1.color = new Color(0.2f, 0.4f, 0.8f, 1f);
        
        team1Button = team1Obj.AddComponent<Button>();
        team1Button.onClick.AddListener(() => SelectTeam(1));
        
        GameObject text1Obj = new GameObject("Text");
        text1Obj.transform.SetParent(team1Obj.transform, false);
        Text text1 = text1Obj.AddComponent<Text>();
        text1.text = "Team 1 (Blue)";
        text1.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text1.fontSize = 14;
        text1.color = Color.white;
        text1.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect1 = text1Obj.GetComponent<RectTransform>();
        textRect1.anchorMin = Vector2.zero;
        textRect1.anchorMax = Vector2.one;
        textRect1.offsetMin = Vector2.zero;
        textRect1.offsetMax = Vector2.zero;
        
        // Team 2 Button
        GameObject team2Obj = new GameObject("Team2Button");
        team2Obj.transform.SetParent(characterSelectionPanel.transform, false);
        
        RectTransform rect2 = team2Obj.AddComponent<RectTransform>();
        rect2.sizeDelta = new Vector2(0, 50);
        
        Image bg2 = team2Obj.AddComponent<Image>();
        bg2.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        
        team2Button = team2Obj.AddComponent<Button>();
        team2Button.onClick.AddListener(() => SelectTeam(2));
        
        GameObject text2Obj = new GameObject("Text");
        text2Obj.transform.SetParent(team2Obj.transform, false);
        Text text2 = text2Obj.AddComponent<Text>();
        text2.text = "Team 2 (Red)";
        text2.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text2.fontSize = 14;
        text2.color = Color.white;
        text2.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect2 = text2Obj.GetComponent<RectTransform>();
        textRect2.anchorMin = Vector2.zero;
        textRect2.anchorMax = Vector2.one;
        textRect2.offsetMin = Vector2.zero;
        textRect2.offsetMax = Vector2.zero;
    }
    
    void SetupEventListeners()
    {
        // This would set up all the event listeners for buttons, sliders, etc.
    }
    
    void SelectCharacterType(int index)
    {
        selectedCharacterType = index;
        Debug.Log($"Selected character type: {index}");
    }
    
    void SelectTeam(int team)
    {
        selectedTeam = team;
        
        // Reset drag states when changing team
        CharacterDragSource.ResetAllDragStates();
        
        UpdateTeamSelection();
        Debug.Log($"Selected team: {team}");
    }
    
    void UpdateTeamSelection()
    {
        if (currentTeamText != null)
        {
            currentTeamText.text = $"Current Team: {selectedTeam}";
        }
        
        // Update button highlights
        if (team1Button != null)
        {
            ColorBlock colors1 = team1Button.colors;
            colors1.normalColor = selectedTeam == 1 ? new Color(0.3f, 0.6f, 1f) : new Color(0.2f, 0.4f, 0.8f);
            team1Button.colors = colors1;
        }
        
        if (team2Button != null)
        {
            ColorBlock colors2 = team2Button.colors;
            colors2.normalColor = selectedTeam == 2 ? new Color(1f, 0.3f, 0.3f) : new Color(0.8f, 0.2f, 0.2f);
            team2Button.colors = colors2;
        }
    }
    
    void OnSearchChanged(string searchText)
    {
        // Filter character list based on search text
        // Implementation would hide/show character buttons based on name matching
    }
    
    public void OnCharacterPlaced(Vector3 position)
    {
        if (battleManager != null && battleManager.characterPrefabs != null && 
            selectedCharacterType < battleManager.characterPrefabs.Length)
        {
            GameObject prefab = battleManager.characterPrefabs[selectedCharacterType];
            string instanceID = mapStateManager.AddCharacterInstance(
                prefab.name, position, selectedTeam, prefab);
            
            // Add to action history for undo/redo
            actionHistory.Add($"PLACE:{instanceID}");
            historyIndex = actionHistory.Count - 1;
        }
    }
    
    public void OnCharacterSelected(string instanceID)
    {
        selectedInstanceID = instanceID;
        ShowPropertiesPanel(instanceID);
    }
    
    void ShowPropertiesPanel(string instanceID)
    {
        if (propertiesPanel != null)
        {
            propertiesPanel.SetActive(true);
            
            CharacterInstance instance = mapStateManager.GetCharacterInstance(instanceID);
            if (instance != null && selectedCharacterInfo != null)
            {
                selectedCharacterInfo.text = $"Selected: {instance.characterID}\nTeam: {instance.team}\nPosition: {instance.position}";
            }
        }
    }
    
    public void DeleteSelectedCharacter()
    {
        if (!string.IsNullOrEmpty(selectedInstanceID))
        {
            mapStateManager.RemoveCharacterInstance(selectedInstanceID);
            actionHistory.Add($"DELETE:{selectedInstanceID}");
            historyIndex = actionHistory.Count - 1;
            selectedInstanceID = "";
            
            if (propertiesPanel != null)
                propertiesPanel.SetActive(false);
        }
    }
    
    public void StartSimulation()
    {
        if (battleManager != null)
        {
            battleManager.StartBattle();
        }
    }
    
    public void ClearAll()
    {
        mapStateManager.ClearAllCharacters();
        actionHistory.Add("CLEAR_ALL");
        historyIndex = actionHistory.Count - 1;
        
        if (propertiesPanel != null)
            propertiesPanel.SetActive(false);
    }
}