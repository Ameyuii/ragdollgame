using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleGameManager : MonoBehaviour
{
    [Header("UI References")]
    public Text team1CounterText;
    public Text team2CounterText;
    public Text statusText;
    public Button startButton;
    public Button resetButton;
    
    [Header("Setup UI")]
    public GameObject setupPanel;
    public Transform characterListParent;
    public GameObject characterButtonPrefab;
    
    [Header("Game State")]
    public bool gameStarted = false;
    public bool setupMode = true;
    public int team1AliveCount = 0;
    public int team2AliveCount = 0;
    
    [Header("Character Prefabs")]
    public GameObject[] characterPrefabs;
    
    private List<RagdollCharacter> allCharacters = new List<RagdollCharacter>();
    private List<GameObject> spawnedCharacters = new List<GameObject>();
    
    void Start()
    {
        // Find UI elements automatically if not assigned
        if (team1CounterText == null)
            team1CounterText = GameObject.Find("Team1Counter")?.GetComponent<Text>();
        if (team2CounterText == null)
            team2CounterText = GameObject.Find("Team2Counter")?.GetComponent<Text>();
        if (statusText == null)
            statusText = GameObject.Find("StatusText")?.GetComponent<Text>();
        if (startButton == null)
            startButton = GameObject.Find("StartButton")?.GetComponent<Button>();
        if (resetButton == null)
            resetButton = GameObject.Find("ResetButton")?.GetComponent<Button>();
        
        // Add button listeners
        if (startButton != null)
            startButton.onClick.AddListener(StartBattle);
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetBattle);
        
        // Reset drag states on start
        CharacterDragSource.ResetAllDragStates();
        
        // Initialize setup mode
        InitializeSetupMode();
    }
    
    public void InitializeSetupMode()
    {
        setupMode = true;
        gameStarted = false;
        
        // Reset drag states when entering setup mode
        CharacterDragSource.ResetAllDragStates();
        
        // Load character prefabs if not assigned
        if (characterPrefabs == null || characterPrefabs.Length == 0)
        {
            LoadCharacterPrefabs();
        }
        
        // Create setup UI
        CreateSetupUI();
        
        // Update UI
        if (statusText != null)
            statusText.text = "Setup Mode: Drag characters to map, then click Start";
        
        if (startButton != null)
            startButton.interactable = true;
        
        UpdateUI();
        
        Debug.Log("Setup mode initialized");
    }
    
    void LoadCharacterPrefabs()
    {
        // Load prefabs from Resources or assign manually
        List<GameObject> prefabList = new List<GameObject>();
        
        // Try to load from Assets/Prefabs
        GameObject completeChar = Resources.Load<GameObject>("CompleteCharacter");
        if (completeChar != null) prefabList.Add(completeChar);
        
        GameObject battleChar = Resources.Load<GameObject>("BattleCharacter");
        if (battleChar != null) prefabList.Add(battleChar);
        
        GameObject npc = Resources.Load<GameObject>("npc");
        if (npc != null) prefabList.Add(npc);
        
        // If no prefabs found in Resources, try to find them in scene
        if (prefabList.Count == 0)
        {
            // Use existing characters as templates
            RagdollCharacter[] existingChars = FindObjectsOfType<RagdollCharacter>();
            if (existingChars.Length > 0)
            {
                // Create a simple prefab reference from existing character
                prefabList.Add(existingChars[0].gameObject);
            }
        }
        
        characterPrefabs = prefabList.ToArray();
    }
    
    void CreateSetupUI()
    {
        // Create setup panel if it doesn't exist
        if (setupPanel == null)
        {
            GameObject canvas = GameObject.Find("UI Canvas");
            if (canvas != null)
            {
                setupPanel = new GameObject("SetupPanel");
                setupPanel.transform.SetParent(canvas.transform, false);
                
                RectTransform rect = setupPanel.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0.3f, 1);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                
                Image bg = setupPanel.AddComponent<Image>();
                bg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                
                // Create character list parent
                GameObject listParent = new GameObject("CharacterList");
                listParent.transform.SetParent(setupPanel.transform, false);
                
                RectTransform listRect = listParent.AddComponent<RectTransform>();
                listRect.anchorMin = new Vector2(0, 0);
                listRect.anchorMax = new Vector2(1, 1);
                listRect.offsetMin = new Vector2(10, 10);
                listRect.offsetMax = new Vector2(-10, -10);
                
                // Add vertical layout group
                VerticalLayoutGroup layout = listParent.AddComponent<VerticalLayoutGroup>();
                layout.spacing = 5;
                layout.padding = new RectOffset(5, 5, 5, 5);
                layout.childControlHeight = false;
                layout.childControlWidth = true;
                layout.childForceExpandWidth = true;
                
                // Add scroll rect for scrolling
                ScrollRect scrollRect = listParent.gameObject.AddComponent<ScrollRect>();
                scrollRect.content = listParent.GetComponent<RectTransform>();
                scrollRect.vertical = true;
                scrollRect.horizontal = false;
                scrollRect.scrollSensitivity = 20f;
                
                characterListParent = listParent.transform;
            }
        }
        
        // Create selected display text
        CreateSelectedDisplay();
        
        // Create header
        CreateUIHeader();
        
        // Create character buttons
        CreateCharacterButtons();
        
        // Initialize display
        UpdateTeamButtonHighlight();
        UpdateSelectedDisplay();
    }
    
    void CreateCharacterButtons()
    {
        if (characterListParent == null || characterPrefabs == null) return;
        
        // Clear existing buttons
        foreach (Transform child in characterListParent)
        {
            DestroyImmediate(child.gameObject);
        }
        
        // Create buttons for each character type
        for (int i = 0; i < characterPrefabs.Length; i++)
        {
            CreateCharacterButton(characterPrefabs[i], i);
        }
        
        // Add team selection buttons
        CreateTeamButton("Team 1 (Blue)", 1);
        CreateTeamButton("Team 2 (Red)", 2);
    }
    
    void CreateCharacterButton(GameObject prefab, int index)
    {
        GameObject button = new GameObject($"CharacterButton_{index}");
        button.transform.SetParent(characterListParent, false);
        
        RectTransform rect = button.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 100); // Taller for image + text
        
        Image bg = button.AddComponent<Image>();
        bg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        Button btn = button.AddComponent<Button>();
        
        // Add preview image
        GameObject imageObj = new GameObject("PreviewImage");
        imageObj.transform.SetParent(button.transform, false);
        
        RectTransform imageRect = imageObj.AddComponent<RectTransform>();
        imageRect.anchorMin = new Vector2(0.05f, 0.3f);
        imageRect.anchorMax = new Vector2(0.4f, 0.95f);
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;
        
        Image previewImage = imageObj.AddComponent<Image>();
        
        // Generate preview
        Texture2D previewTexture = PrefabPreviewGenerator.GeneratePreviewTexture(prefab, 64, 64);
        if (previewTexture != null)
        {
            Sprite previewSprite = Sprite.Create(previewTexture, 
                new Rect(0, 0, previewTexture.width, previewTexture.height), 
                new Vector2(0.5f, 0.5f));
            previewImage.sprite = previewSprite;
        }
        else
        {
            previewImage.color = Color.gray;
        }
        
        // Add border to image
        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(imageObj.transform, false);
        
        RectTransform borderRect = borderObj.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(-2, -2);
        borderRect.offsetMax = new Vector2(2, 2);
        
        Image border = borderObj.AddComponent<Image>();
        border.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        borderObj.transform.SetAsFirstSibling(); // Put border behind image
        
        // Add character name
        GameObject nameObj = new GameObject("CharacterName");
        nameObj.transform.SetParent(button.transform, false);
        
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.45f, 0.6f);
        nameRect.anchorMax = new Vector2(0.95f, 0.95f);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        
        Text nameText = nameObj.AddComponent<Text>();
        nameText.text = prefab.name;
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        nameText.fontSize = 12;
        nameText.color = Color.white;
        nameText.alignment = TextAnchor.UpperLeft;
        nameText.fontStyle = FontStyle.Bold;
        
        // Add character info
        GameObject infoObj = new GameObject("CharacterInfo");
        infoObj.transform.SetParent(button.transform, false);
        
        RectTransform infoRect = infoObj.AddComponent<RectTransform>();
        infoRect.anchorMin = new Vector2(0.45f, 0.05f);
        infoRect.anchorMax = new Vector2(0.95f, 0.6f);
        infoRect.offsetMin = Vector2.zero;
        infoRect.offsetMax = Vector2.zero;
        
        Text infoText = infoObj.AddComponent<Text>();
        infoText.text = GetCharacterInfo(prefab);
        infoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        infoText.fontSize = 10;
        infoText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        infoText.alignment = TextAnchor.UpperLeft;
        
        // Add drag component
        CharacterDragSource dragSource = button.AddComponent<CharacterDragSource>();
        dragSource.characterPrefab = prefab;
        dragSource.gameManager = this;
        
        // Add hover effect
        CharacterButtonHover hoverEffect = button.AddComponent<CharacterButtonHover>();
        hoverEffect.normalColor = bg.color;
        hoverEffect.hoverColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        hoverEffect.backgroundImage = bg;
        
        btn.onClick.AddListener(() => SelectCharacterType(index));
    }
    
    string GetCharacterInfo(GameObject prefab)
    {
        // Get basic info about the character
        string info = "Type: Character\n";
        
        // Check for specific components
        RagdollCharacter ragdoll = prefab.GetComponent<RagdollCharacter>();
        if (ragdoll != null)
        {
            info += $"Health: {ragdoll.maxHealth}\n";
            info += $"Damage: {ragdoll.attackDamage}";
        }
        else
        {
            info += "Click to select\nDrag to place";
        }
        
        return info;
    }
    
    void CreateTeamButton(string teamName, int teamId)
    {
        GameObject button = new GameObject($"TeamButton_{teamId}");
        button.transform.SetParent(characterListParent, false);
        
        RectTransform rect = button.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 50);
        
        Image bg = button.AddComponent<Image>();
        bg.color = teamId == 1 ? new Color(0.2f, 0.4f, 0.8f, 1f) : new Color(0.8f, 0.2f, 0.2f, 1f);
        
        Button btn = button.AddComponent<Button>();
        
        // Add text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform, false);
        
        Text text = textObj.AddComponent<Text>();
        text.text = teamName;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontStyle = FontStyle.Bold;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        btn.onClick.AddListener(() => SelectTeam(teamId));
        
        // Store reference for highlighting
        if (teamId == 1)
            team1Button = bg;
        else
            team2Button = bg;
    }
    
    public int selectedCharacterType = 0;
    public int selectedTeam = 1;
    private Image team1Button;
    private Image team2Button;
    private Text selectedTeamText;
    
    void SelectCharacterType(int index)
    {
        selectedCharacterType = index;
        Debug.Log($"Selected character type: {characterPrefabs[index].name}");
        UpdateSelectedDisplay();
    }
    
    void SelectTeam(int teamId)
    {
        selectedTeam = teamId;
        Debug.Log($"Selected team: {teamId}");
        
        // Reset drag states when changing team
        CharacterDragSource.ResetAllDragStates();
        
        UpdateTeamButtonHighlight();
        UpdateSelectedDisplay();
    }
    
    void UpdateTeamButtonHighlight()
    {
        if (team1Button != null)
        {
            team1Button.color = selectedTeam == 1 ? 
                new Color(0.3f, 0.6f, 1f, 1f) : 
                new Color(0.2f, 0.4f, 0.8f, 1f);
        }
        
        if (team2Button != null)
        {
            team2Button.color = selectedTeam == 2 ? 
                new Color(1f, 0.3f, 0.3f, 1f) : 
                new Color(0.8f, 0.2f, 0.2f, 1f);
        }
    }
    
    void UpdateSelectedDisplay()
    {
        if (selectedTeamText != null)
        {
            string characterName = characterPrefabs != null && selectedCharacterType < characterPrefabs.Length ? 
                characterPrefabs[selectedCharacterType].name : "None";
            
            string teamIcon = selectedTeam == 1 ? "🔵" : "🔴";
            string teamName = selectedTeam == 1 ? "Team 1 (Blue)" : "Team 2 (Red)";
            
            if (characterName == "None")
            {
                selectedTeamText.text = $"📋 READY TO PLACE\n{teamIcon} {teamName}\n\n👆 Click character → Drag to map";
            }
            else
            {
                selectedTeamText.text = $"✅ SELECTED: {characterName}\n{teamIcon} {teamName}\n\n🖱️ Drag to map to place";
            }
        }
    }
    
    void CreateSelectedDisplay()
    {
        if (setupPanel == null) return;
        
        GameObject displayObj = new GameObject("SelectedDisplay");
        displayObj.transform.SetParent(setupPanel.transform, false);
        
        RectTransform rect = displayObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.75f);
        rect.anchorMax = new Vector2(1, 0.9f);
        rect.offsetMin = new Vector2(5, 0);
        rect.offsetMax = new Vector2(-5, 0);
        
        Image bg = displayObj.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.4f, 0.1f, 0.8f);
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(displayObj.transform, false);
        
        selectedTeamText = textObj.AddComponent<Text>();
        selectedTeamText.text = "📋 READY TO PLACE\n🔵 Team 1 Selected\n\n👆 Click character → Drag to map";
        selectedTeamText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        selectedTeamText.fontSize = 11;
        selectedTeamText.color = Color.white;
        selectedTeamText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(5, 5);
        textRect.offsetMax = new Vector2(-5, -5);
    }
    
    void CreateUIHeader()
    {
        if (setupPanel == null) return;
        
        GameObject headerObj = new GameObject("UIHeader");
        headerObj.transform.SetParent(setupPanel.transform, false);
        
        RectTransform rect = headerObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.9f);
        rect.anchorMax = new Vector2(1, 1f);
        rect.offsetMin = new Vector2(5, -50);
        rect.offsetMax = new Vector2(-5, -5);
        
        Image bg = headerObj.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.3f, 0.6f, 0.8f);
        
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(headerObj.transform, false);
        
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "CHARACTER SELECTION";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 14;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = Vector2.zero;
        titleRect.anchorMax = Vector2.one;
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
    }
    
    public void SpawnCharacterAtPosition(Vector3 position)
    {
        if (!setupMode || characterPrefabs == null || selectedCharacterType >= characterPrefabs.Length)
            return;
        
        GameObject prefab = characterPrefabs[selectedCharacterType];
        GameObject newCharacter = Instantiate(prefab, position, Quaternion.identity);
        
        // Set team
        RagdollCharacter ragdoll = newCharacter.GetComponent<RagdollCharacter>();
        if (ragdoll != null)
        {
            ragdoll.teamId = selectedTeam;
        }
        
        spawnedCharacters.Add(newCharacter);
        Debug.Log($"Spawned {prefab.name} for team {selectedTeam} at {position}");
    }
    
    public void StartBattle()
    {
        if (gameStarted)
        {
            Debug.Log("Battle already started!");
            return;
        }
        
        // Switch from setup mode to battle mode
        setupMode = false;
        gameStarted = true;
        
        // Reset drag states when starting battle
        CharacterDragSource.ResetAllDragStates();
        
        // Hide setup panel
        if (setupPanel != null)
            setupPanel.SetActive(false);
        
        // Initialize battle with spawned characters
        InitializeBattle();
        
        Debug.Log("Battle started!");
    }
    
    void InitializeBattle()
    {
        // Find all characters (including spawned ones)
        allCharacters.Clear();
        RagdollCharacter[] characters = FindObjectsOfType<RagdollCharacter>();
        allCharacters.AddRange(characters);
        
        // Count teams
        team1AliveCount = 0;
        team2AliveCount = 0;
        
        foreach (RagdollCharacter character in allCharacters)
        {
            if (character.teamId == 1)
                team1AliveCount++;
            else if (character.teamId == 2)
                team2AliveCount++;
            
            // Enable character for battle
            character.enabled = true;
            var agent = character.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
                agent.enabled = true;
        }
        
        // Update UI
        if (statusText != null)
            statusText.text = "Battle in progress!";
        
        if (startButton != null)
            startButton.interactable = false;
        
        UpdateUI();
    }
    
    public void ResetBattle()
    {
        gameStarted = false;
        
        // Reset all drag states first
        CharacterDragSource.ResetAllDragStates();
        
        // Clear spawned characters
        foreach (GameObject character in spawnedCharacters)
        {
            if (character != null)
                DestroyImmediate(character);
        }
        spawnedCharacters.Clear();
        
        // Reset existing characters
        RagdollCharacter[] existingChars = FindObjectsOfType<RagdollCharacter>();
        foreach (RagdollCharacter character in existingChars)
        {
            if (character != null)
            {
                character.ResetCharacter();
            }
        }
        
        // Return to setup mode
        InitializeSetupMode();
        
        // Show setup panel
        if (setupPanel != null)
            setupPanel.SetActive(true);
        
        Debug.Log("Battle reset - returned to setup mode!");
    }
    
    public void OnCharacterDeath(RagdollCharacter character)
    {
        if (!gameStarted) return;
        
        // Update team counts
        if (character.teamId == 1)
            team1AliveCount--;
        else if (character.teamId == 2)
            team2AliveCount--;
        
        // Update UI
        UpdateUI();
        
        // Check for victory
        CheckVictoryCondition();
    }
    
    void CheckVictoryCondition()
    {
        if (team1AliveCount <= 0)
        {
            // Team 2 wins
            if (statusText != null)
                statusText.text = "Team 2 Wins!";
            EndBattle();
        }
        else if (team2AliveCount <= 0)
        {
            // Team 1 wins
            if (statusText != null)
                statusText.text = "Team 1 Wins!";
            EndBattle();
        }
    }
    
    void EndBattle()
    {
        gameStarted = false;
        
        // Disable all remaining characters
        foreach (RagdollCharacter character in allCharacters)
        {
            if (character != null)
            {
                character.enabled = false;
            }
        }
        
        // Enable reset button
        if (resetButton != null)
            resetButton.interactable = true;
        
        Debug.Log("Battle ended!");
    }
    
    void UpdateUI()
    {
        if (team1CounterText != null)
            team1CounterText.text = $"Team 1: {team1AliveCount}";
        
        if (team2CounterText != null)
            team2CounterText.text = $"Team 2: {team2AliveCount}";
    }
}