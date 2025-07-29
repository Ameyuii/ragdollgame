using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Quản lý trạng thái game: Pause, Resume, Menu, Game Over
/// </summary>
public class GameStateManager : MonoBehaviour
{
    [Header("Game State")]
    [SerializeField] private GameState currentState = GameState.Menu;
    [SerializeField] private float timeScaleBeforePause = 1f;
    
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Text gameStateText;
    
    [Header("Input")]
    [SerializeField] private InputActionReference pauseAction;
    
    // Singleton pattern
    public static GameStateManager Instance { get; private set; }
    
    // Events
    public System.Action<GameState> OnGameStateChanged;
    
    // Private fields
    private BattleGameManager battleManager;
    private List<MonoBehaviour> pausableComponents = new List<MonoBehaviour>();
    
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver,
        Setup
    }
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        InitializeComponents();
        // Tạm thời không setup UI để tránh conflict
        // SetupUI();
        SetupInputActions();
        
        // Bắt đầu ở Menu mode thay vì Setup
        ChangeGameState(GameState.Menu);
    }
    
    void InitializeComponents()
    {
        // Tìm BattleGameManager
        if (battleManager == null)
            battleManager = FindFirstObjectByType<BattleGameManager>();
        
        // Tự động tìm UI elements nếu chưa được gán
        FindUIElements();
        
        // Tìm tất cả components có thể pause
        FindPausableComponents();
    }
    
    void FindUIElements()
    {
        // Tạm thời không tự tạo pause menu để tránh conflict
        // if (pauseMenuPanel == null)
        // {
        //     CreatePauseMenu();
        // }
        
        if (gameStateText == null)
        {
            GameObject statusTextObj = GameObject.Find("StatusText");
            if (statusTextObj != null)
                gameStateText = statusTextObj.GetComponent<Text>();
        }
    }
    
    void CreatePauseMenu()
    {
        // Tạo pause menu panel
        GameObject canvas = GameObject.Find("UI Canvas");
        if (canvas != null)
        {
            pauseMenuPanel = new GameObject("PauseMenuPanel");
            pauseMenuPanel.transform.SetParent(canvas.transform, false);
            
            // Thêm Image component cho background
            Image panelImage = pauseMenuPanel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f); // Semi-transparent black
            
            // Set RectTransform để fill màn hình
            RectTransform rect = pauseMenuPanel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // Tạo các button
            CreatePauseMenuButtons();
            
            // Ẩn menu ban đầu
            pauseMenuPanel.SetActive(false);
        }
    }
    
    void CreatePauseMenuButtons()
    {
        if (pauseMenuPanel == null) return;
        
        // Container cho buttons
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(pauseMenuPanel.transform, false);
        
        VerticalLayoutGroup layoutGroup = buttonContainer.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 20f;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = false;
        
        // Set RectTransform cho container
        RectTransform containerRect = buttonContainer.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(200, 300);
        containerRect.anchoredPosition = Vector2.zero;
        
        // Resume Button
        resumeButton = CreateButton("Resume", buttonContainer.transform);
        resumeButton.onClick.AddListener(ResumeGame);
        
        // Restart Button
        restartButton = CreateButton("Restart", buttonContainer.transform);
        restartButton.onClick.AddListener(RestartGame);
        
        // Exit Button
        exitButton = CreateButton("Exit", buttonContainer.transform);
        exitButton.onClick.AddListener(ExitGame);
    }
    
    Button CreateButton(string text, Transform parent)
    {
        GameObject buttonObj = new GameObject(text + "Button");
        buttonObj.transform.SetParent(parent, false);
        
        // Add Button component
        Button button = buttonObj.AddComponent<Button>();
        
        // Add Image component
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        
        // Set RectTransform
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(150, 40);
        
        // Add Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = text;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 16;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        
        // Set text RectTransform
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return button;
    }
    
    void SetupInputActions()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed += OnPauseInput;
            pauseAction.action.Enable();
        }
    }
    
    void SetupUI()
    {
        // Setup UI button listeners
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);
    }
    
    void FindPausableComponents()
    {
        // Tìm tất cả AI components và character controllers
        pausableComponents.Clear();
        
        AutoAIManager[] aiManagers = FindObjectsByType<AutoAIManager>(FindObjectsSortMode.None);
        foreach (var ai in aiManagers)
            pausableComponents.Add(ai);
        
        SimpleCharacterAI[] characterAIs = FindObjectsByType<SimpleCharacterAI>(FindObjectsSortMode.None);
        foreach (var ai in characterAIs)
            pausableComponents.Add(ai);
        
        RagdollCharacter[] characters = FindObjectsByType<RagdollCharacter>(FindObjectsSortMode.None);
        foreach (var character in characters)
            pausableComponents.Add(character);
    }
    
    void OnPauseInput(InputAction.CallbackContext context)
    {
        if (currentState == GameState.Playing)
            PauseGame();
        else if (currentState == GameState.Paused)
            ResumeGame();
    }
    
    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
            
            // Pause AI components
            foreach (var component in pausableComponents)
            {
                if (component != null)
                    component.enabled = false;
            }
            
            ChangeGameState(GameState.Paused);
            
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(true);
            
            Debug.Log("Game Paused");
        }
    }
    
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            Time.timeScale = timeScaleBeforePause;
            
            // Resume AI components
            foreach (var component in pausableComponents)
            {
                if (component != null)
                    component.enabled = true;
            }
            
            ChangeGameState(GameState.Playing);
            
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
            
            Debug.Log("Game Resumed");
        }
    }
    
    public void StartGame()
    {
        Time.timeScale = 1f;
        
        // Enable all components
        FindPausableComponents();
        foreach (var component in pausableComponents)
        {
            if (component != null)
                component.enabled = true;
        }
        
        ChangeGameState(GameState.Playing);
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        
        Debug.Log("Game Started");
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        
        // Reset battle through BattleGameManager
        if (battleManager != null)
        {
            battleManager.ResetBattle();
        }
        
        ChangeGameState(GameState.Setup);
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        
        Debug.Log("Game Restarted");
    }
    
    public void GameOver(int winningTeam)
    {
        Time.timeScale = 0f;
        
        ChangeGameState(GameState.GameOver);
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        Debug.Log($"Game Over - Team {winningTeam} wins!");
    }
    
    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    public void ChangeGameState(GameState newState)
    {
        if (currentState != newState)
        {
            GameState previousState = currentState;
            currentState = newState;
            
            UpdateUI();
            OnGameStateChanged?.Invoke(newState);
            
            Debug.Log($"Game State changed from {previousState} to {newState}");
        }
    }
    
    void UpdateUI()
    {
        if (gameStateText != null)
        {
            switch (currentState)
            {
                case GameState.Menu:
                    gameStateText.text = "Main Menu";
                    break;
                case GameState.Setup:
                    gameStateText.text = "Setup Mode: Drag characters to position them!";
                    break;
                case GameState.Playing:
                    gameStateText.text = "Battle in Progress";
                    break;
                case GameState.Paused:
                    gameStateText.text = "Game Paused";
                    break;
                case GameState.GameOver:
                    gameStateText.text = "Game Over";
                    break;
            }
        }
    }
    
    // Public getters
    public GameState CurrentState => currentState;
    public bool IsGamePaused => currentState == GameState.Paused;
    public bool IsGamePlaying => currentState == GameState.Playing;
    
    void OnDestroy()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPauseInput;
            pauseAction.action.Disable();
        }
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        // Tự động pause khi mất focus (tùy chọn)
        if (!hasFocus && currentState == GameState.Playing)
        {
            PauseGame();
        }
    }
}