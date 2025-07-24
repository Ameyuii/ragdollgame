using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class CharacterCategory
{
    public string categoryName;
    public GameObject[] characterPrefabs;
    public Color categoryColor = Color.white;
}

public class CharacterSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    public Canvas mainCanvas;
    public BattleGameManager gameManager;
    
    [Header("Character Categories")]
    public CharacterCategory[] categories;
    
    [Header("UI Settings")]
    public int modelsPerPage = 6;
    public float buttonSpacing = 10f;
    
    // UI Components
    private GameObject leftPanel;
    private GameObject bottomPanel;
    private GameObject modelContainer;
    private GameObject teamSelectionPanel;
    private GameObject teamDropdownPanel;
    private Button prevButton;
    private Button nextButton;
    private Text pageInfoText;
    private Button teamDropdownButton;
    private Text teamDropdownText;
    private Text selectedTeamText;
    private bool isTeamDropdownOpen = false;
    
    // State
    private int selectedCategoryIndex = -1;
    private int currentPage = 0;
    private int totalPages = 0;
    private int selectedTeam = 1;
    private int maxTeams = 4;
    
    void Start()
    {
        if (mainCanvas == null)
            mainCanvas = FindObjectOfType<Canvas>();
        
        if (gameManager == null)
            gameManager = FindObjectOfType<BattleGameManager>();
        
        InitializeCategories();
        CreateUI();
    }
    
    void InitializeCategories()
    {
        if (categories == null || categories.Length == 0)
        {
            // Create default categories from existing prefabs
            List<CharacterCategory> defaultCategories = new List<CharacterCategory>();
            
            // Robot category
            CharacterCategory robotCategory = new CharacterCategory();
            robotCategory.categoryName = "🤖 ROBOT";
            robotCategory.categoryColor = new Color(0.3f, 0.6f, 1f, 1f);
            
            // Monster category  
            CharacterCategory monsterCategory = new CharacterCategory();
            monsterCategory.categoryName = "👹 QUÁI VẬT";
            monsterCategory.categoryColor = new Color(1f, 0.3f, 0.3f, 1f);
            
            // Soldier category
            CharacterCategory soldierCategory = new CharacterCategory();
            soldierCategory.categoryName = "🪖 CHIẾN BINH";
            soldierCategory.categoryColor = new Color(0.3f, 1f, 0.3f, 1f);
            
            // Load prefabs from gameManager or Resources
            if (gameManager != null && gameManager.characterPrefabs != null && gameManager.characterPrefabs.Length > 0)
            {
                List<GameObject> robots = new List<GameObject>();
                List<GameObject> monsters = new List<GameObject>();
                List<GameObject> soldiers = new List<GameObject>();
                
                foreach (GameObject prefab in gameManager.characterPrefabs)
                {
                    if (prefab != null)
                    {
                        if (prefab.name.ToLower().Contains("robot") || prefab.name.ToLower().Contains("mech"))
                        {
                            robots.Add(prefab);
                        }
                        else if (prefab.name.ToLower().Contains("monster") || prefab.name.ToLower().Contains("zombie"))
                        {
                            monsters.Add(prefab);
                        }
                        else
                        {
                            soldiers.Add(prefab);
                        }
                    }
                }
                
                robotCategory.characterPrefabs = robots.ToArray();
                monsterCategory.characterPrefabs = monsters.ToArray();
                soldierCategory.characterPrefabs = soldiers.ToArray();
            }
            else
            {
                // Fallback: try to load from Resources
                GameObject[] resourcePrefabs = Resources.LoadAll<GameObject>("");
                List<GameObject> fallbackPrefabs = new List<GameObject>();
                
                foreach (GameObject prefab in resourcePrefabs)
                {
                    if (prefab != null && prefab.GetComponent<RagdollCharacter>() != null)
                    {
                        fallbackPrefabs.Add(prefab);
                    }
                }
                
                robotCategory.characterPrefabs = fallbackPrefabs.ToArray();
                monsterCategory.characterPrefabs = fallbackPrefabs.ToArray();
                soldierCategory.characterPrefabs = fallbackPrefabs.ToArray();
                
                Debug.LogWarning("GameManager prefabs not found, using fallback from Resources");
            }
            
            defaultCategories.Add(robotCategory);
            defaultCategories.Add(monsterCategory);
            defaultCategories.Add(soldierCategory);
            
            categories = defaultCategories.ToArray();
        }
    }
    
    void CreateUI()
    {
        CreateLeftPanel();
        CreateBottomPanel();
        CreateTeamSelectionPanel();
    }
    
    void CreateLeftPanel()
    {
        // Create left panel container
        leftPanel = new GameObject("CharacterCategoryPanel");
        leftPanel.transform.SetParent(mainCanvas.transform, false);
        
        RectTransform leftRect = leftPanel.AddComponent<RectTransform>();
        leftRect.anchorMin = new Vector2(0, 0.25f);
        leftRect.anchorMax = new Vector2(0.2f, 0.8f);
        leftRect.offsetMin = new Vector2(10, 0);
        leftRect.offsetMax = new Vector2(-5, 0);
        
        Image leftBg = leftPanel.AddComponent<Image>();
        leftBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        // Add title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(leftPanel.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.9f);
        titleRect.anchorMax = new Vector2(1, 1f);
        titleRect.offsetMin = new Vector2(5, 0);
        titleRect.offsetMax = new Vector2(-5, 0);
        
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "LOẠI NHÂN VẬT";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 16;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
        
        // Create category buttons
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(leftPanel.transform, false);
        
        RectTransform containerRect = buttonContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 0);
        containerRect.anchorMax = new Vector2(1, 0.9f);
        containerRect.offsetMin = new Vector2(5, 5);
        containerRect.offsetMax = new Vector2(-5, 0);
        
        VerticalLayoutGroup layout = buttonContainer.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 10;
        layout.padding = new RectOffset(5, 5, 10, 10);
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
        
        // Create buttons for each category
        for (int i = 0; i < categories.Length; i++)
        {
            CreateCategoryButton(categories[i], i, buttonContainer.transform);
        }
    }
    
    void CreateCategoryButton(CharacterCategory category, int index, Transform parent)
    {
        GameObject buttonObj = new GameObject($"CategoryButton_{index}");
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(0, 60);
        
        Image buttonBg = buttonObj.AddComponent<Image>();
        buttonBg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonBg;
        
        // Add text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5, 0);
        textRect.offsetMax = new Vector2(-5, 0);
        
        Text text = textObj.AddComponent<Text>();
        text.text = category.categoryName;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        
        // Add click handler
        int categoryIndex = index;
        button.onClick.AddListener(() => SelectCategory(categoryIndex));
        
        // Add hover effect
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        colors.highlightedColor = category.categoryColor;
        colors.pressedColor = new Color(category.categoryColor.r * 0.8f, category.categoryColor.g * 0.8f, category.categoryColor.b * 0.8f, 1f);
        button.colors = colors;
    }
    
    void CreateBottomPanel()
    {
        // Create bottom panel container
        bottomPanel = new GameObject("ModelSelectionPanel");
        bottomPanel.transform.SetParent(mainCanvas.transform, false);
        
        RectTransform bottomRect = bottomPanel.AddComponent<RectTransform>();
        bottomRect.anchorMin = new Vector2(0, 0);
        bottomRect.anchorMax = new Vector2(0.8f, 0.25f);
        bottomRect.offsetMin = new Vector2(10, 10);
        bottomRect.offsetMax = new Vector2(-10, 0);
        
        Image bottomBg = bottomPanel.AddComponent<Image>();
        bottomBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        // Initially hidden
        bottomPanel.SetActive(false);
        
        // Create title area
        GameObject titleArea = new GameObject("TitleArea");
        titleArea.transform.SetParent(bottomPanel.transform, false);
        
        RectTransform titleAreaRect = titleArea.AddComponent<RectTransform>();
        titleAreaRect.anchorMin = new Vector2(0, 0.8f);
        titleAreaRect.anchorMax = new Vector2(1, 1f);
        titleAreaRect.offsetMin = Vector2.zero;
        titleAreaRect.offsetMax = Vector2.zero;
        
        // Category title
        GameObject categoryTitle = new GameObject("CategoryTitle");
        categoryTitle.transform.SetParent(titleArea.transform, false);
        
        RectTransform categoryTitleRect = categoryTitle.AddComponent<RectTransform>();
        categoryTitleRect.anchorMin = new Vector2(0, 0);
        categoryTitleRect.anchorMax = new Vector2(0.7f, 1f);
        categoryTitleRect.offsetMin = new Vector2(10, 0);
        categoryTitleRect.offsetMax = Vector2.zero;
        
        Text categoryTitleText = categoryTitle.AddComponent<Text>();
        categoryTitleText.text = "CHỌN NHÂN VẬT";
        categoryTitleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        categoryTitleText.fontSize = 16;
        categoryTitleText.color = Color.white;
        categoryTitleText.alignment = TextAnchor.MiddleLeft;
        categoryTitleText.fontStyle = FontStyle.Bold;
        
        // Page info
        GameObject pageInfo = new GameObject("PageInfo");
        pageInfo.transform.SetParent(titleArea.transform, false);
        
        RectTransform pageInfoRect = pageInfo.AddComponent<RectTransform>();
        pageInfoRect.anchorMin = new Vector2(0.7f, 0);
        pageInfoRect.anchorMax = new Vector2(1f, 1f);
        pageInfoRect.offsetMin = Vector2.zero;
        pageInfoRect.offsetMax = new Vector2(-10, 0);
        
        pageInfoText = pageInfo.AddComponent<Text>();
        pageInfoText.text = "";
        pageInfoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        pageInfoText.fontSize = 14;
        pageInfoText.color = Color.yellow;
        pageInfoText.alignment = TextAnchor.MiddleRight;
        
        // Create navigation area
        CreateNavigationArea();
        
        // Create model container
        CreateModelContainer();
    }
    
    void CreateNavigationArea()
    {
        GameObject navArea = new GameObject("NavigationArea");
        navArea.transform.SetParent(bottomPanel.transform, false);
        
        RectTransform navRect = navArea.AddComponent<RectTransform>();
        navRect.anchorMin = new Vector2(0, 0);
        navRect.anchorMax = new Vector2(1, 0.2f);
        navRect.offsetMin = Vector2.zero;
        navRect.offsetMax = Vector2.zero;
        
        // Previous button
        GameObject prevObj = new GameObject("PrevButton");
        prevObj.transform.SetParent(navArea.transform, false);
        
        RectTransform prevRect = prevObj.AddComponent<RectTransform>();
        prevRect.anchorMin = new Vector2(0, 0);
        prevRect.anchorMax = new Vector2(0.15f, 1f);
        prevRect.offsetMin = new Vector2(10, 5);
        prevRect.offsetMax = new Vector2(0, -5);
        
        Image prevBg = prevObj.AddComponent<Image>();
        prevBg.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        
        prevButton = prevObj.AddComponent<Button>();
        prevButton.targetGraphic = prevBg;
        prevButton.onClick.AddListener(PreviousPage);
        
        GameObject prevText = new GameObject("Text");
        prevText.transform.SetParent(prevObj.transform, false);
        
        RectTransform prevTextRect = prevText.AddComponent<RectTransform>();
        prevTextRect.anchorMin = Vector2.zero;
        prevTextRect.anchorMax = Vector2.one;
        prevTextRect.offsetMin = Vector2.zero;
        prevTextRect.offsetMax = Vector2.zero;
        
        Text prevTextComp = prevText.AddComponent<Text>();
        prevTextComp.text = "◀ TRƯỚC";
        prevTextComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        prevTextComp.fontSize = 14;
        prevTextComp.color = Color.white;
        prevTextComp.alignment = TextAnchor.MiddleCenter;
        prevTextComp.fontStyle = FontStyle.Bold;
        
        // Next button
        GameObject nextObj = new GameObject("NextButton");
        nextObj.transform.SetParent(navArea.transform, false);
        
        RectTransform nextRect = nextObj.AddComponent<RectTransform>();
        nextRect.anchorMin = new Vector2(0.85f, 0);
        nextRect.anchorMax = new Vector2(1f, 1f);
        nextRect.offsetMin = new Vector2(0, 5);
        nextRect.offsetMax = new Vector2(-10, -5);
        
        Image nextBg = nextObj.AddComponent<Image>();
        nextBg.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        
        nextButton = nextObj.AddComponent<Button>();
        nextButton.targetGraphic = nextBg;
        nextButton.onClick.AddListener(NextPage);
        
        GameObject nextText = new GameObject("Text");
        nextText.transform.SetParent(nextObj.transform, false);
        
        RectTransform nextTextRect = nextText.AddComponent<RectTransform>();
        nextTextRect.anchorMin = Vector2.zero;
        nextTextRect.anchorMax = Vector2.one;
        nextTextRect.offsetMin = Vector2.zero;
        nextTextRect.offsetMax = Vector2.zero;
        
        Text nextTextComp = nextText.AddComponent<Text>();
        nextTextComp.text = "SAU ▶";
        nextTextComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nextTextComp.fontSize = 14;
        nextTextComp.color = Color.white;
        nextTextComp.alignment = TextAnchor.MiddleCenter;
        nextTextComp.fontStyle = FontStyle.Bold;
    }
    
    void CreateModelContainer()
    {
        modelContainer = new GameObject("ModelContainer");
        modelContainer.transform.SetParent(bottomPanel.transform, false);
        
        RectTransform containerRect = modelContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.15f, 0.2f);
        containerRect.anchorMax = new Vector2(0.85f, 0.8f);
        containerRect.offsetMin = new Vector2(10, 0);
        containerRect.offsetMax = new Vector2(-10, 0);
        
        HorizontalLayoutGroup layout = modelContainer.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = buttonSpacing;
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = true;
        layout.childForceExpandWidth = true;
    }
    
    void CreateTeamSelectionPanel()
    {
        // Create team selection button in bottom right
        teamSelectionPanel = new GameObject("TeamSelectionPanel");
        teamSelectionPanel.transform.SetParent(mainCanvas.transform, false);
        
        RectTransform teamRect = teamSelectionPanel.AddComponent<RectTransform>();
        teamRect.anchorMin = new Vector2(0.8f, 0);
        teamRect.anchorMax = new Vector2(1f, 0.25f);
        teamRect.offsetMin = new Vector2(-10, 10);
        teamRect.offsetMax = new Vector2(-10, 0);
        
        Image teamBg = teamSelectionPanel.AddComponent<Image>();
        teamBg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        // Create main team button
        GameObject mainButtonObj = new GameObject("TeamDropdownButton");
        mainButtonObj.transform.SetParent(teamSelectionPanel.transform, false);
        
        RectTransform mainButtonRect = mainButtonObj.AddComponent<RectTransform>();
        mainButtonRect.anchorMin = new Vector2(0, 0.7f);
        mainButtonRect.anchorMax = new Vector2(1, 1f);
        mainButtonRect.offsetMin = new Vector2(5, 0);
        mainButtonRect.offsetMax = new Vector2(-5, -5);
        
        Image mainButtonBg = mainButtonObj.AddComponent<Image>();
        mainButtonBg.color = GetTeamColor(selectedTeam);
        
        teamDropdownButton = mainButtonObj.AddComponent<Button>();
        teamDropdownButton.targetGraphic = mainButtonBg;
        teamDropdownButton.onClick.AddListener(ToggleTeamDropdown);
        
        // Add text to main button
        GameObject mainTextObj = new GameObject("Text");
        mainTextObj.transform.SetParent(mainButtonObj.transform, false);
        
        RectTransform mainTextRect = mainTextObj.AddComponent<RectTransform>();
        mainTextRect.anchorMin = Vector2.zero;
        mainTextRect.anchorMax = Vector2.one;
        mainTextRect.offsetMin = Vector2.zero;
        mainTextRect.offsetMax = Vector2.zero;
        
        teamDropdownText = mainTextObj.AddComponent<Text>();
        teamDropdownText.text = $"ĐỘI {selectedTeam} ▼";
        teamDropdownText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        teamDropdownText.fontSize = 16;
        teamDropdownText.color = Color.white;
        teamDropdownText.alignment = TextAnchor.MiddleCenter;
        teamDropdownText.fontStyle = FontStyle.Bold;
        
        // Create dropdown panel (initially hidden)
        CreateTeamDropdownPanel();
        
        // Create selected team display
        GameObject selectedObj = new GameObject("SelectedTeamDisplay");
        selectedObj.transform.SetParent(mainCanvas.transform, false);
        
        RectTransform selectedRect = selectedObj.AddComponent<RectTransform>();
        selectedRect.anchorMin = new Vector2(0.2f, 0.8f);
        selectedRect.anchorMax = new Vector2(0.8f, 1f);
        selectedRect.offsetMin = new Vector2(10, -10);
        selectedRect.offsetMax = new Vector2(-10, -10);
        
        Image selectedBg = selectedObj.AddComponent<Image>();
        selectedBg.color = GetTeamColor(selectedTeam);
        
        // Create separate text object
        GameObject selectedTextObj = new GameObject("Text");
        selectedTextObj.transform.SetParent(selectedObj.transform, false);
        
        RectTransform selectedTextRect = selectedTextObj.AddComponent<RectTransform>();
        selectedTextRect.anchorMin = Vector2.zero;
        selectedTextRect.anchorMax = Vector2.one;
        selectedTextRect.offsetMin = new Vector2(10, 5);
        selectedTextRect.offsetMax = new Vector2(-10, -5);
        
        selectedTeamText = selectedTextObj.AddComponent<Text>();
        selectedTeamText.text = $"{GetTeamIcon(selectedTeam)} ĐỘI {selectedTeam} ĐƯỢC CHỌN - Kéo thả nhân vật vào bản đồ";
        selectedTeamText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        selectedTeamText.fontSize = 16;
        selectedTeamText.color = Color.white;
        selectedTeamText.alignment = TextAnchor.MiddleCenter;
        selectedTeamText.fontStyle = FontStyle.Bold;
    }
    
    void CreateTeamDropdownPanel()
    {
        teamDropdownPanel = new GameObject("TeamDropdownPanel");
        teamDropdownPanel.transform.SetParent(teamSelectionPanel.transform, false);
        
        RectTransform dropdownRect = teamDropdownPanel.AddComponent<RectTransform>();
        dropdownRect.anchorMin = new Vector2(0, 0);
        dropdownRect.anchorMax = new Vector2(1, 0.7f);
        dropdownRect.offsetMin = new Vector2(5, 5);
        dropdownRect.offsetMax = new Vector2(-5, 0);
        
        Image dropdownBg = teamDropdownPanel.AddComponent<Image>();
        dropdownBg.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);
        
        // Create vertical layout for team buttons
        VerticalLayoutGroup layout = teamDropdownPanel.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 2;
        layout.padding = new RectOffset(2, 2, 2, 2);
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = true;
        layout.childForceExpandWidth = true;
        
        // Create team buttons
        for (int i = 1; i <= maxTeams; i++)
        {
            CreateTeamOptionButton(i, teamDropdownPanel.transform);
        }
        
        // Initially hidden
        teamDropdownPanel.SetActive(false);
    }
    
    void CreateTeamOptionButton(int teamId, Transform parent)
    {
        GameObject buttonObj = new GameObject($"TeamOption_{teamId}");
        buttonObj.transform.SetParent(parent, false);
        
        Image buttonBg = buttonObj.AddComponent<Image>();
        buttonBg.color = GetTeamColor(teamId);
        
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonBg;
        
        // Add text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Text text = textObj.AddComponent<Text>();
        text.text = $"{GetTeamIcon(teamId)} ĐỘI {teamId}";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        
        // Add click handler
        button.onClick.AddListener(() => SelectTeam(teamId));
        
        // Add hover effect
        ColorBlock colors = button.colors;
        Color teamColor = GetTeamColor(teamId);
        colors.normalColor = teamColor;
        colors.highlightedColor = new Color(teamColor.r * 1.2f, teamColor.g * 1.2f, teamColor.b * 1.2f, 1f);
        colors.pressedColor = new Color(teamColor.r * 0.8f, teamColor.g * 0.8f, teamColor.b * 0.8f, 1f);
        button.colors = colors;
    }
    
    Color GetTeamColor(int teamId)
    {
        switch (teamId)
        {
            case 1: return new Color(0.2f, 0.4f, 0.8f, 1f); // Blue
            case 2: return new Color(0.8f, 0.2f, 0.2f, 1f); // Red
            case 3: return new Color(0.2f, 0.8f, 0.2f, 1f); // Green
            case 4: return new Color(0.8f, 0.8f, 0.2f, 1f); // Yellow
            default: return new Color(0.5f, 0.5f, 0.5f, 1f); // Gray
        }
    }
    
    string GetTeamIcon(int teamId)
    {
        switch (teamId)
        {
            case 1: return "🔵";
            case 2: return "🔴";
            case 3: return "🟢";
            case 4: return "🟡";
            default: return "⚪";
        }
    }
    
    public void ToggleTeamDropdown()
    {
        isTeamDropdownOpen = !isTeamDropdownOpen;
        teamDropdownPanel.SetActive(isTeamDropdownOpen);
        
        // Update button text
        string arrow = isTeamDropdownOpen ? "▲" : "▼";
        teamDropdownText.text = $"ĐỘI {selectedTeam} {arrow}";
    }
    
    public void SelectTeam(int teamId)
    {
        selectedTeam = teamId;
        
        // Update gameManager if available
        if (gameManager != null)
        {
            gameManager.selectedTeam = teamId;
        }
        
        // Update UI
        UpdateTeamSelection();
        
        // Close dropdown
        isTeamDropdownOpen = false;
        teamDropdownPanel.SetActive(false);
        teamDropdownText.text = $"ĐỘI {selectedTeam} ▼";
        
        Debug.Log($"Selected team: {teamId}");
    }
    
    void UpdateTeamSelection()
    {
        // Update main button color
        if (teamDropdownButton != null)
        {
            Image buttonBg = teamDropdownButton.GetComponent<Image>();
            if (buttonBg != null)
            {
                buttonBg.color = GetTeamColor(selectedTeam);
            }
        }
        
        // Update selected team display
        if (selectedTeamText != null)
        {
            selectedTeamText.text = $"{GetTeamIcon(selectedTeam)} ĐỘI {selectedTeam} ĐƯỢC CHỌN - Kéo thả nhân vật vào bản đồ";
            
            // Update background color - get parent's Image component
            if (selectedTeamText.transform.parent != null)
            {
                Image bg = selectedTeamText.transform.parent.GetComponent<Image>();
                if (bg != null)
                {
                    bg.color = GetTeamColor(selectedTeam);
                }
            }
        }
    }
    
    public void SelectCategory(int categoryIndex)
    {
        if (categoryIndex < 0 || categoryIndex >= categories.Length)
            return;
        
        selectedCategoryIndex = categoryIndex;
        currentPage = 0;
        
        CharacterCategory category = categories[categoryIndex];
        
        // Update bottom panel title
        Transform titleTransform = bottomPanel.transform.Find("TitleArea/CategoryTitle");
        if (titleTransform != null)
        {
            Text titleText = titleTransform.GetComponent<Text>();
            if (titleText != null)
            {
                titleText.text = category.categoryName;
                titleText.color = category.categoryColor;
            }
        }
        
        // Calculate total pages
        int totalModels = category.characterPrefabs.Length;
        totalPages = Mathf.CeilToInt((float)totalModels / modelsPerPage);
        
        // Show bottom panel
        bottomPanel.SetActive(true);
        
        // Update model display
        UpdateModelDisplay();
        
        Debug.Log($"Selected category: {category.categoryName} with {totalModels} models");
    }
    
    void UpdateModelDisplay()
    {
        if (selectedCategoryIndex < 0 || selectedCategoryIndex >= categories.Length)
            return;
        
        CharacterCategory category = categories[selectedCategoryIndex];
        
        // Clear existing models
        foreach (Transform child in modelContainer.transform)
        {
            DestroyImmediate(child.gameObject);
        }
        
        // Calculate range for current page
        int startIndex = currentPage * modelsPerPage;
        int endIndex = Mathf.Min(startIndex + modelsPerPage, category.characterPrefabs.Length);
        
        // Create model buttons for current page
        for (int i = startIndex; i < endIndex; i++)
        {
            CreateModelButton(category.characterPrefabs[i], i);
        }
        
        // Update navigation buttons
        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < totalPages - 1;
        
        // Update page info
        pageInfoText.text = $"Trang {currentPage + 1}/{totalPages}";
    }
    
    void CreateModelButton(GameObject prefab, int index)
    {
        GameObject buttonObj = new GameObject($"ModelButton_{index}");
        buttonObj.transform.SetParent(modelContainer.transform, false);
        
        Image buttonBg = buttonObj.AddComponent<Image>();
        buttonBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonBg;
        
        // Create layout for image and text
        VerticalLayoutGroup layout = buttonObj.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 5;
        layout.padding = new RectOffset(5, 5, 5, 5);
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandWidth = true;
        
        // Add preview image
        GameObject imageObj = new GameObject("PreviewImage");
        imageObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform imageRect = imageObj.AddComponent<RectTransform>();
        imageRect.sizeDelta = new Vector2(0, 80);
        
        Image previewImage = imageObj.AddComponent<Image>();
        previewImage.color = Color.gray; // Placeholder
        
        // Generate preview if possible
        StartCoroutine(GeneratePreviewAsync(prefab, previewImage));
        
        // Add name text
        GameObject nameObj = new GameObject("NameText");
        nameObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.sizeDelta = new Vector2(0, 30);
        
        Text nameText = nameObj.AddComponent<Text>();
        nameText.text = prefab.name;
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 12;
        nameText.color = Color.white;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.fontStyle = FontStyle.Bold;
        
        // Add drag component
        CharacterDragSource dragSource = buttonObj.AddComponent<CharacterDragSource>();
        dragSource.characterPrefab = prefab;
        dragSource.gameManager = gameManager;
        
        // Update gameManager team selection
        if (gameManager != null)
        {
            gameManager.selectedTeam = selectedTeam;
        }
        
        // Add click handler for selection
        button.onClick.AddListener(() => {
            if (gameManager != null && gameManager.characterPrefabs != null)
            {
                // Update gameManager selection
                for (int i = 0; i < gameManager.characterPrefabs.Length; i++)
                {
                    if (gameManager.characterPrefabs[i] == prefab)
                    {
                        gameManager.selectedCharacterType = i;
                        break;
                    }
                }
            }
            Debug.Log($"Selected model: {(prefab != null ? prefab.name : "null")}");
        });
        
        // Add hover effect
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        colors.highlightedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        colors.pressedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        button.colors = colors;
    }
    
    IEnumerator GeneratePreviewAsync(GameObject prefab, Image targetImage)
    {
        yield return new WaitForEndOfFrame();
        
        // Try to generate preview texture
        Texture2D previewTexture = PrefabPreviewGenerator.GeneratePreviewTexture(prefab, 128, 128);
        if (previewTexture != null)
        {
            Sprite previewSprite = Sprite.Create(previewTexture, 
                new Rect(0, 0, previewTexture.width, previewTexture.height), 
                new Vector2(0.5f, 0.5f));
            targetImage.sprite = previewSprite;
            targetImage.color = Color.white;
        }
    }
    
    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateModelDisplay();
        }
    }
    
    public void NextPage()
    {
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            UpdateModelDisplay();
        }
    }
    
    public void HideBottomPanel()
    {
        if (bottomPanel != null)
            bottomPanel.SetActive(false);
    }
}