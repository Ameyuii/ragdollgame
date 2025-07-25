using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CategoryButtonHandler : MonoBehaviour
{
    [System.Serializable]
    public class CategoryData
    {
        public string categoryName;
        public GameObject[] prefabs;
        public Color categoryColor = Color.white;
    }
    
    public CategoryData[] categories;
    private GameObject modelSelectionPanel;
    private GameObject modelContainer;
    private Text categoryTitleText;
    private Text pageInfoText;
    private Button prevButton;
    private Button nextButton;
    
    private int selectedCategoryIndex = -1;
    private int currentPage = 0;
    private int modelsPerPage = 6;
    private int totalPages = 0;
    
    void Start()
    {
        InitializeCategories();
        SetupUI();
        SetupCategoryButtons();
    }
    
    void InitializeCategories()
    {
        BattleGameManager gameManager = FindObjectOfType<BattleGameManager>();
        if (gameManager == null || gameManager.characterPrefabs == null)
        {
            Debug.LogError("BattleGameManager not found!");
            return;
        }
        
        List<CategoryData> categoryList = new List<CategoryData>();
        
        // Robot category
        CategoryData robotCategory = new CategoryData();
        robotCategory.categoryName = "🤖 ROBOT";
        robotCategory.categoryColor = new Color(0.3f, 0.6f, 1f, 1f);
        
        // Monster category
        CategoryData monsterCategory = new CategoryData();
        monsterCategory.categoryName = "👹 QUÁI VẬT";
        monsterCategory.categoryColor = new Color(1f, 0.3f, 0.3f, 1f);
        
        // Soldier category
        CategoryData soldierCategory = new CategoryData();
        soldierCategory.categoryName = "🪖 CHIẾN BINH";
        soldierCategory.categoryColor = new Color(0.3f, 1f, 0.3f, 1f);
        
        // Distribute prefabs
        List<GameObject> robots = new List<GameObject>();
        List<GameObject> monsters = new List<GameObject>();
        List<GameObject> soldiers = new List<GameObject>();
        
        foreach (GameObject prefab in gameManager.characterPrefabs)
        {
            if (prefab != null)
            {
                string prefabName = prefab.name.ToLower();
                
                if (prefabName.Contains("complete") || prefabName.Contains("character"))
                {
                    robots.Add(prefab);
                }
                else if (prefabName.Contains("npc") || prefabName.Contains("variant"))
                {
                    monsters.Add(prefab);
                }
                else if (prefabName.Contains("battle"))
                {
                    soldiers.Add(prefab);
                }
                else
                {
                    // Distribute evenly
                    int index = System.Array.IndexOf(gameManager.characterPrefabs, prefab);
                    if (index % 3 == 0) robots.Add(prefab);
                    else if (index % 3 == 1) soldiers.Add(prefab);
                    else monsters.Add(prefab);
                }
            }
        }
        
        robotCategory.prefabs = robots.ToArray();
        monsterCategory.prefabs = monsters.ToArray();
        soldierCategory.prefabs = soldiers.ToArray();
        
        categoryList.Add(robotCategory);
        categoryList.Add(monsterCategory);
        categoryList.Add(soldierCategory);
        
        categories = categoryList.ToArray();
        
        Debug.Log($"Initialized {categories.Length} categories:");
        for (int i = 0; i < categories.Length; i++)
        {
            Debug.Log($"  {categories[i].categoryName}: {categories[i].prefabs.Length} prefabs");
        }
    }
    
    void SetupUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        // Find ModelSelectionPanel
        modelSelectionPanel = canvas.transform.Find("ModelSelectionPanel")?.gameObject;
        if (modelSelectionPanel == null)
        {
            Debug.LogError("ModelSelectionPanel not found!");
            return;
        }
        
        // Find UI components
        Transform titleArea = modelSelectionPanel.transform.Find("TitleArea");
        if (titleArea != null)
        {
            categoryTitleText = titleArea.Find("CategoryTitle")?.GetComponent<Text>();
            pageInfoText = titleArea.Find("PageInfo")?.GetComponent<Text>();
        }
        
        Transform navArea = modelSelectionPanel.transform.Find("NavigationArea");
        if (navArea != null)
        {
            prevButton = navArea.Find("PrevButton")?.GetComponent<Button>();
            nextButton = navArea.Find("NextButton")?.GetComponent<Button>();
            
            if (prevButton != null)
                prevButton.onClick.AddListener(PreviousPage);
            if (nextButton != null)
                nextButton.onClick.AddListener(NextPage);
        }
        
        modelContainer = modelSelectionPanel.transform.Find("ModelContainer")?.gameObject;
    }
    
    void SetupCategoryButtons()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        Transform categoryPanel = canvas.transform.Find("CharacterCategoryPanel");
        if (categoryPanel == null)
        {
            Debug.LogError("CharacterCategoryPanel not found!");
            return;
        }
        
        Transform buttonContainer = categoryPanel.Find("ButtonContainer");
        if (buttonContainer == null)
        {
            Debug.LogError("ButtonContainer not found!");
            return;
        }
        
        // Setup existing category buttons
        for (int i = 0; i < buttonContainer.childCount && i < categories.Length; i++)
        {
            Transform buttonTransform = buttonContainer.GetChild(i);
            Button button = buttonTransform.GetComponent<Button>();
            
            if (button != null)
            {
                // Clear existing listeners
                button.onClick.RemoveAllListeners();
                
                // Add new listener
                int categoryIndex = i;
                button.onClick.AddListener(() => SelectCategory(categoryIndex));
                
                // Update button text
                Text buttonText = buttonTransform.GetComponentInChildren<Text>();
                if (buttonText != null && i < categories.Length)
                {
                    buttonText.text = categories[i].categoryName;
                }
                
                // Update button color
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null && i < categories.Length)
                {
                    ColorBlock colors = button.colors;
                    colors.normalColor = categories[i].categoryColor * 0.7f;
                    colors.highlightedColor = categories[i].categoryColor;
                    colors.pressedColor = categories[i].categoryColor * 0.5f;
                    button.colors = colors;
                }
                
                Debug.Log($"Setup button {i}: {categories[i].categoryName}");
            }
        }
    }
    
    public void SelectCategory(int categoryIndex)
    {
        Debug.Log($"SelectCategory called with index: {categoryIndex}");
        
        if (categoryIndex < 0 || categoryIndex >= categories.Length)
        {
            Debug.LogError($"Invalid category index: {categoryIndex}");
            return;
        }
        
        selectedCategoryIndex = categoryIndex;
        currentPage = 0;
        
        CategoryData category = categories[categoryIndex];
        
        // Update title
        if (categoryTitleText != null)
        {
            categoryTitleText.text = category.categoryName;
            categoryTitleText.color = category.categoryColor;
        }
        
        // Calculate pages
        totalPages = Mathf.CeilToInt((float)category.prefabs.Length / modelsPerPage);
        
        // Show panel
        if (modelSelectionPanel != null)
            modelSelectionPanel.SetActive(true);
        
        // Update display
        UpdateModelDisplay();
        
        Debug.Log($"Selected category: {category.categoryName} with {category.prefabs.Length} models");
    }
    
    void UpdateModelDisplay()
    {
        if (selectedCategoryIndex < 0 || selectedCategoryIndex >= categories.Length || modelContainer == null)
            return;
        
        CategoryData category = categories[selectedCategoryIndex];
        
        // Clear existing models
        foreach (Transform child in modelContainer.transform)
        {
            DestroyImmediate(child.gameObject);
        }
        
        // Calculate range
        int startIndex = currentPage * modelsPerPage;
        int endIndex = Mathf.Min(startIndex + modelsPerPage, category.prefabs.Length);
        
        // Create model buttons
        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject prefab = category.prefabs[i];
            if (prefab != null)
            {
                CreateModelButton(prefab, i);
            }
        }
        
        // Update page info
        if (pageInfoText != null)
        {
            pageInfoText.text = $"Trang {currentPage + 1}/{totalPages}";
        }
        
        // Update navigation buttons
        if (prevButton != null)
            prevButton.interactable = currentPage > 0;
        if (nextButton != null)
            nextButton.interactable = currentPage < totalPages - 1;
        
        Debug.Log($"Displaying {endIndex - startIndex} models on page {currentPage + 1}/{totalPages}");
    }
    
    void CreateModelButton(GameObject prefab, int index)
    {
        GameObject buttonObj = new GameObject($"ModelButton_{index}");
        buttonObj.transform.SetParent(modelContainer.transform, false);
        
        Image bg = buttonObj.AddComponent<Image>();
        bg.color = new Color(0.4f, 0.4f, 0.4f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(() => SelectModel(prefab));
        
        // Add drag source
        CharacterDragSource dragSource = buttonObj.AddComponent<CharacterDragSource>();
        dragSource.characterPrefab = prefab;
        
        BattleGameManager gameManager = FindObjectOfType<BattleGameManager>();
        if (gameManager != null)
        {
            dragSource.gameManager = gameManager;
        }
        
        // Model name text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 0.3f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Text text = textObj.AddComponent<Text>();
        text.text = prefab.name;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 10;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        
        Debug.Log($"Created model button for {prefab.name}");
    }
    
    void SelectModel(GameObject prefab)
    {
        Debug.Log($"Selected model: {prefab.name}");
    }
    
    void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateModelDisplay();
        }
    }
    
    void NextPage()
    {
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            UpdateModelDisplay();
        }
    }
}