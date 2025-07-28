using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Auto UI Generator - Drop-in replacement for CategoryButtonHandler
/// Generates UI dynamically from CharacterRegistry while maintaining existing UI structure
/// </summary>
public class AutoUIGenerator : MonoBehaviour
{
    [Header("🎯 Core References")]
    [Tooltip("Character registry to generate UI from")]
    public CharacterRegistry characterRegistry;
    
    [Tooltip("Unified game manager for character operations")]
    public UnifiedGameManager unifiedGameManager;
    
    [Header("📱 UI References")]
    [Tooltip("Panel containing character selection UI")]
    public GameObject modelSelectionPanel;
    
    [Tooltip("Container for character buttons")]
    public GameObject modelContainer;
    
    [Tooltip("Title text showing current category")]
    public Text categoryTitleText;
    
    [Tooltip("Page info text (e.g., '1/3')")]
    public Text pageInfoText;
    
    [Tooltip("Previous page button")]
    public Button prevButton;
    
    [Tooltip("Next page button")]
    public Button nextButton;
    
    [Header("⚙️ UI Settings")]
    [Tooltip("Number of characters per page")]
    public int charactersPerPage = 6;
    
    [Tooltip("Button prefab for character selection")]
    public GameObject characterButtonPrefab;
    
    [Tooltip("Default button size")]
    public Vector2 buttonSize = new Vector2(100, 100);
    
    [Header("📊 Current State")]
    [SerializeField] private string currentCategory = "ROBOT";
    [SerializeField] private int currentPage = 0;
    [SerializeField] private bool uiInitialized = false;
    
    // Runtime data
    private List<CharacterRegistry.CharacterEntry> currentCharacters = new List<CharacterRegistry.CharacterEntry>();
    private List<GameObject> currentButtons = new List<GameObject>();
    
    private void Start()
    {
        InitializeUI();
    }
    
    private void InitializeUI()
    {
        Debug.Log("🎨 AutoUIGenerator: Initializing UI...");
        
        try
        {
            // Find UI elements if not assigned
            FindUIElements();
            
            // Validate references
            if (!ValidateReferences())
            {
                Debug.LogError("❌ AutoUIGenerator: Missing required references!");
                return;
            }
            
            // Setup button listeners
            SetupButtonListeners();
            
            // Generate initial UI
            GenerateUI();
            
            uiInitialized = true;
            Debug.Log("✅ AutoUIGenerator: UI initialized successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ AutoUIGenerator initialization failed: {e.Message}");
        }
    }
    
    private void FindUIElements()
    {
        // Find UI elements by name if not assigned
        if (modelSelectionPanel == null)
        {
            modelSelectionPanel = GameObject.Find("BottomCharacterPanel");
        }
        
        if (modelContainer == null)
        {
            GameObject contentArea = GameObject.Find("ModelContentArea");
            if (contentArea != null)
            {
                modelContainer = contentArea;
            }
        }
        
        if (categoryTitleText == null)
        {
            GameObject titleObj = GameObject.Find("PanelTitle");
            if (titleObj != null)
            {
                categoryTitleText = titleObj.GetComponent<Text>();
            }
        }
        
        // Find pagination buttons (may not exist in current UI)
        if (prevButton == null)
        {
            GameObject prevObj = GameObject.Find("PrevButton");
            if (prevObj != null)
            {
                prevButton = prevObj.GetComponent<Button>();
            }
        }
        
        if (nextButton == null)
        {
            GameObject nextObj = GameObject.Find("NextButton");
            if (nextObj != null)
            {
                nextButton = nextObj.GetComponent<Button>();
            }
        }
    }
    
    private bool ValidateReferences()
    {
        if (characterRegistry == null)
        {
            Debug.LogError("❌ CharacterRegistry not assigned!");
            return false;
        }
        
        if (unifiedGameManager == null)
        {
            unifiedGameManager = UnifiedGameManager.Instance;
            if (unifiedGameManager == null)
            {
                Debug.LogError("❌ UnifiedGameManager not found!");
                return false;
            }
        }
        
        if (modelContainer == null)
        {
            Debug.LogError("❌ ModelContainer not found!");
            return false;
        }
        
        return true;
    }
    
    private void SetupButtonListeners()
    {
        // Setup pagination buttons if they exist
        if (prevButton != null)
        {
            prevButton.onClick.RemoveAllListeners();
            prevButton.onClick.AddListener(PreviousPage);
        }
        
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(NextPage);
        }
        
        // Setup category buttons
        SetupCategoryButtons();
    }
    
    private void SetupCategoryButtons()
    {
        // Find and setup category buttons
        SetupCategoryButton("RobotButton", "ROBOT");
        SetupCategoryButton("MonsterButton", "QUAIVAT");
        SetupCategoryButton("WarriorButton", "CHIENBINH");
        SetupCategoryButton("ZombieButton", "ZOMBIE");
    }
    
    private void SetupCategoryButton(string buttonName, string category)
    {
        GameObject buttonObj = GameObject.Find(buttonName);
        if (buttonObj != null)
        {
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectCategory(category));
                Debug.Log($"✅ Setup category button: {buttonName} → {category}");
            }
        }
    }
    
    #region Public API
    
    /// <summary>
    /// Generate UI from character registry
    /// </summary>
    [ContextMenu("🎨 Generate UI")]
    public void GenerateUI()
    {
        if (!uiInitialized && !ValidateReferences()) return;
        
        Debug.Log($"🎨 Generating UI for category: {currentCategory}");
        
        // Get characters for current category
        currentCharacters = characterRegistry.GetCharactersByCategory(currentCategory);
        
        // Clear existing buttons
        ClearCurrentButtons();
        
        // Update category title
        UpdateCategoryTitle();
        
        // Create character buttons
        CreateCharacterButtons();
        
        // Update pagination
        UpdatePagination();
        
        Debug.Log($"✅ Generated {currentButtons.Count} character buttons");
    }
    
    /// <summary>
    /// Select a category and refresh UI
    /// </summary>
    public void SelectCategory(string category)
    {
        if (currentCategory == category) return;
        
        currentCategory = category;
        currentPage = 0;
        
        // Update unified game manager
        if (unifiedGameManager != null)
        {
            unifiedGameManager.SetSelectedCategory(category);
        }
        
        GenerateUI();
        
        Debug.Log($"📂 Selected category: {category}");
    }
    
    /// <summary>
    /// Go to previous page
    /// </summary>
    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            GenerateUI();
        }
    }
    
    /// <summary>
    /// Go to next page
    /// </summary>
    public void NextPage()
    {
        int maxPages = Mathf.CeilToInt((float)currentCharacters.Count / charactersPerPage);
        if (currentPage < maxPages - 1)
        {
            currentPage++;
            GenerateUI();
        }
    }
    
    #endregion

    #region UI Generation

    private void ClearCurrentButtons()
    {
        foreach (GameObject button in currentButtons)
        {
            if (button != null)
            {
                DestroyImmediate(button);
            }
        }
        currentButtons.Clear();
    }

    private void UpdateCategoryTitle()
    {
        if (categoryTitleText != null)
        {
            categoryTitleText.text = GetCategoryDisplayName(currentCategory);
        }
    }

    private string GetCategoryDisplayName(string category)
    {
        switch (category.ToUpper())
        {
            case "ROBOT": return "🤖 Robot Characters";
            case "QUAIVAT": return "👹 Monster Characters";
            case "CHIENBINH": return "⚔️ Warrior Characters";
            case "ZOMBIE": return "🧟 Zombie Characters";
            default: return category;
        }
    }

    private void CreateCharacterButtons()
    {
        if (currentCharacters == null || currentCharacters.Count == 0)
        {
            Debug.LogWarning($"⚠️ No characters found for category: {currentCategory}");
            return;
        }

        // Calculate pagination
        int startIndex = currentPage * charactersPerPage;
        int endIndex = Mathf.Min(startIndex + charactersPerPage, currentCharacters.Count);

        // Create buttons for current page
        for (int i = startIndex; i < endIndex; i++)
        {
            var character = currentCharacters[i];
            CreateCharacterButton(character, i - startIndex);
        }
    }

    private void CreateCharacterButton(CharacterRegistry.CharacterEntry character, int index)
    {
        if (character?.prefab == null) return;

        // Create button GameObject
        GameObject buttonObj = new GameObject($"CharacterButton_{character.displayName}");
        buttonObj.transform.SetParent(modelContainer.transform, false);

        // Add Button component
        Button button = buttonObj.AddComponent<Button>();

        // Add Image component for background
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = Color.white;

        // Set button size
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = buttonSize;

        // Position button (simple grid layout)
        float spacing = 10f;
        int columns = 3;
        int row = index / columns;
        int col = index % columns;

        float x = col * (buttonSize.x + spacing);
        float y = -row * (buttonSize.y + spacing);
        rectTransform.anchoredPosition = new Vector2(x, y);

        // Add character icon if available
        if (character.icon != null)
        {
            buttonImage.sprite = character.icon;
        }
        else
        {
            // Create placeholder icon
            CreatePlaceholderIcon(buttonImage, character);
        }

        // Add text label
        CreateButtonLabel(buttonObj, character.displayName);

        // Add click listener
        button.onClick.AddListener(() => OnCharacterButtonClick(character));

        // Add drag component
        AddDragComponent(buttonObj, character);

        // Store button reference
        currentButtons.Add(buttonObj);

        Debug.Log($"✅ Created button for: {character.displayName}");
    }

    private void CreatePlaceholderIcon(Image buttonImage, CharacterRegistry.CharacterEntry character)
    {
        // Create simple colored background based on category
        Color categoryColor = GetCategoryColor(currentCategory);
        buttonImage.color = categoryColor;
    }

    private Color GetCategoryColor(string category)
    {
        switch (category.ToUpper())
        {
            case "ROBOT": return new Color(0.5f, 0.8f, 1f, 0.8f); // Light blue
            case "QUAIVAT": return new Color(1f, 0.5f, 0.5f, 0.8f); // Light red
            case "CHIENBINH": return new Color(0.5f, 1f, 0.5f, 0.8f); // Light green
            case "ZOMBIE": return new Color(0.8f, 0.8f, 0.5f, 0.8f); // Light yellow
            default: return Color.gray;
        }
    }

    private void CreateButtonLabel(GameObject buttonObj, string labelText)
    {
        // Create text child object
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(buttonObj.transform, false);

        // Add Text component
        Text text = textObj.AddComponent<Text>();
        text.text = labelText;
        text.font = FontHelper.GetSafeBuiltinFont();
        text.fontSize = 12;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;

        // Position text at bottom of button
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 0.3f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }

    private void AddDragComponent(GameObject buttonObj, CharacterRegistry.CharacterEntry character)
    {
        // Add SimpleCharacterDrag component
        SimpleCharacterDrag dragComponent = buttonObj.AddComponent<SimpleCharacterDrag>();
        dragComponent.characterEntry = character;
        dragComponent.unifiedGameManager = unifiedGameManager;
    }

    private void OnCharacterButtonClick(CharacterRegistry.CharacterEntry character)
    {
        Debug.Log($"🖱️ Character button clicked: {character.displayName}");

        // Optional: Show character details or perform other actions
        // For now, just log the click
    }

    private void UpdatePagination()
    {
        int totalPages = Mathf.CeilToInt((float)currentCharacters.Count / charactersPerPage);

        // Update page info text
        if (pageInfoText != null)
        {
            pageInfoText.text = $"{currentPage + 1}/{totalPages}";
        }

        // Update button states
        if (prevButton != null)
        {
            prevButton.interactable = currentPage > 0;
        }

        if (nextButton != null)
        {
            nextButton.interactable = currentPage < totalPages - 1;
        }
    }

    #endregion

    #region Debug & Maintenance

    [ContextMenu("📊 Show UI Status")]
    public void ShowUIStatus()
    {
        Debug.Log("=== AUTO UI GENERATOR STATUS ===");
        Debug.Log($"Initialized: {uiInitialized}");
        Debug.Log($"Current Category: {currentCategory}");
        Debug.Log($"Current Page: {currentPage + 1}");
        Debug.Log($"Characters in Category: {currentCharacters.Count}");
        Debug.Log($"Active Buttons: {currentButtons.Count}");
        Debug.Log($"Characters Per Page: {charactersPerPage}");
    }

    [ContextMenu("🔄 Refresh UI")]
    public void RefreshUI()
    {
        if (characterRegistry != null)
        {
            characterRegistry.RebuildLookupTables();
        }
        GenerateUI();
        Debug.Log("🔄 UI refreshed");
    }

    #endregion
}
