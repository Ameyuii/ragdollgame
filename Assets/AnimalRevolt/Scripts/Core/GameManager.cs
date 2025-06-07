using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnimalRevolt.Core
{
    /// <summary>
    /// Game Manager chính - Quản lý toàn bộ game
    /// Singleton pattern để đảm bảo chỉ có 1 instance
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] private GameSettings gameSettings;
        
        [Header("Managers")]
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private InputManager inputManager;
        
        // Singleton instance
        public static GameManager Instance { get; private set; }
        
        // Game States
        public enum GameState
        {
            MainMenu,
            CharacterSelection,
            Battle,
            Paused,
            GameOver
        }
        
        [Header("Current Game State")]
        public GameState currentState = GameState.MainMenu;
        
        // Events
        public System.Action<GameState> OnGameStateChanged;
        
        private void Awake()
        {
            // Singleton setup
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            ChangeGameState(GameState.MainMenu);
        }
        
        /// <summary>
        /// Khởi tạo game
        /// </summary>
        private void InitializeGame()
        {
            // Setup các managers
            if (audioManager == null)
                audioManager = FindObjectOfType<AudioManager>();
                
            if (inputManager == null)
                inputManager = FindObjectOfType<InputManager>();
            
            // Load settings
            LoadGameSettings();
            
            Debug.Log("[GameManager] Game initialized successfully!");
        }
        
        /// <summary>
        /// Thay đổi trạng thái game
        /// </summary>
        public void ChangeGameState(GameState newState)
        {
            if (currentState == newState) return;
            
            GameState previousState = currentState;
            currentState = newState;
            
            Debug.Log($"[GameManager] State changed: {previousState} -> {newState}");
            
            // Xử lý transition giữa các state
            HandleStateTransition(previousState, newState);
            
            // Trigger event
            OnGameStateChanged?.Invoke(newState);
        }
        
        /// <summary>
        /// Xử lý chuyển đổi giữa các state
        /// </summary>
        private void HandleStateTransition(GameState from, GameState to)
        {
            switch (to)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;
                    
                case GameState.CharacterSelection:
                    Time.timeScale = 1f;
                    break;
                    
                case GameState.Battle:
                    Time.timeScale = 1f;
                    break;
                    
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                    
                case GameState.GameOver:
                    Time.timeScale = 1f;
                    break;
            }
        }
        
        /// <summary>
        /// Load scene với loading screen
        /// </summary>
        public void LoadScene(string sceneName)
        {
            Debug.Log($"[GameManager] Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
        
        /// <summary>
        /// Load scene async
        /// </summary>
        public void LoadSceneAsync(string sceneName)
        {
            Debug.Log($"[GameManager] Loading scene async: {sceneName}");
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }
        
        private System.Collections.IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            while (!asyncLoad.isDone)
            {
                // Có thể hiển thị loading progress ở đây
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                Debug.Log($"Loading progress: {progress * 100}%");
                yield return null;
            }
        }
        
        /// <summary>
        /// Pause game
        /// </summary>
        public void PauseGame()
        {
            if (currentState != GameState.Battle) return;
            ChangeGameState(GameState.Paused);
        }
        
        /// <summary>
        /// Resume game
        /// </summary>
        public void ResumeGame()
        {
            if (currentState != GameState.Paused) return;
            ChangeGameState(GameState.Battle);
        }
        
        /// <summary>
        /// Quit game
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("[GameManager] Quitting game...");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        /// <summary>
        /// Load game settings
        /// </summary>
        private void LoadGameSettings()
        {
            // TODO: Load settings from PlayerPrefs hoặc file
            Debug.Log("[GameManager] Game settings loaded");
        }
        
        /// <summary>
        /// Save game settings
        /// </summary>
        public void SaveGameSettings()
        {
            // TODO: Save settings to PlayerPrefs hoặc file
            Debug.Log("[GameManager] Game settings saved");
        }
        
        /// <summary>
        /// Reset game data
        /// </summary>
        public void ResetGameData()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("[GameManager] Game data reset");
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && currentState == GameState.Battle)
            {
                PauseGame();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && currentState == GameState.Battle)
            {
                PauseGame();
            }
        }
    }
}