using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace AnimalRevolt.UI
{
    /// <summary>
    /// UI quản lý việc chọn character class: Warrior, Mage, Archer
    /// Tích hợp với UnifiedUIManager để toggle hiển thị
    /// </summary>
    public class CharacterSelectionUI : MonoBehaviour
    {
        [Header("🎮 UI References")]
        [SerializeField, Tooltip("Prefab cho button chọn character")]
        private GameObject characterButtonPrefab;
        
        [SerializeField, Tooltip("Parent transform chứa các button character")]
        private Transform characterListParent;
        
        [SerializeField, Tooltip("Panel hiển thị thông tin character")]
        private GameObject characterInfoPanel;
        
        [SerializeField, Tooltip("Text hiển thị tên character")]
        private Text characterNameText;
        
        [SerializeField, Tooltip("Text hiển thị mô tả character")]
        private Text characterDescriptionText;
        
        [SerializeField, Tooltip("Image hiển thị hình ảnh character")]
        private Image characterImage;

        [Header("⚔️ Character Classes")]
        [SerializeField] private CharacterClassData[] characterClasses = new CharacterClassData[]
        {
            new CharacterClassData("⚔️ Warrior", "Chiến binh mạnh mẽ với sức tấn công cao và khả năng phòng thủ tốt. Thích hợp cho combat cận chiến.", Color.red),
            new CharacterClassData("🔮 Mage", "Pháp sư với khả năng magic mạnh mẽ và tầm tấn công xa. Damage cao nhưng defense thấp.", Color.blue),
            new CharacterClassData("🏹 Archer", "Cung thủ với tốc độ cao và tấn công tầm xa chính xác. Cân bằng giữa damage và mobility.", Color.green)
        };

        [Header("🎛️ UI Settings")]
        [SerializeField, Tooltip("Tự động tạo UI nếu thiếu")]
        private bool autoCreateUIIfMissing = true;
        
        [SerializeField, Tooltip("Hiển thị UI khi start")]
        private bool showUIOnStart = false;
        
        [SerializeField, Tooltip("Tên canvas")]
        private string canvasName = "CharacterSelectionCanvas";
        
        [SerializeField, Tooltip("Tên parent chứa list")]
        private string listParentName = "CharacterListParent";

        [Header("🎨 UI Styling")]
        [SerializeField, Tooltip("Màu button khi được chọn")]
        private Color selectedColor = new Color(0.2f, 0.8f, 0.2f, 1f);
        
        [SerializeField, Tooltip("Màu button bình thường")]
        private Color normalColor = new Color(0.2f, 0.3f, 0.5f, 1f);

        [Header("⌨️ Input")]
        [SerializeField, Tooltip("Phím tắt toggle Character Selection UI")]
        private InputAction toggleCharacterUIAction = new InputAction("ToggleCharacterUI", InputActionType.Button, "<Keyboard>/c");

        // UI State
        private bool isUIVisible = false;
        private int selectedCharacterIndex = -1;
        private Button[] characterButtons;
        private Canvas uiCanvas;

        // Character Class Data Structure
        [System.Serializable]
        public class CharacterClassData
        {
            [Header("📊 Basic Info")]
            public string className;
            public string description;
            public Color themeColor;
            
            [Header("⚔️ Stats")]
            [Range(1, 10)] public int attack = 5;
            [Range(1, 10)] public int defense = 5;
            [Range(1, 10)] public int speed = 5;
            [Range(1, 10)] public int magic = 5;
            [Range(1, 10)] public int health = 5;

            public CharacterClassData(string name, string desc, Color color)
            {
                className = name;
                description = desc;
                themeColor = color;
                
                // Default stats based on class
                if (name.Contains("Warrior"))
                {
                    attack = 8; defense = 9; speed = 4; magic = 2; health = 9;
                }
                else if (name.Contains("Mage"))
                {
                    attack = 3; defense = 3; speed = 6; magic = 10; health = 4;
                }
                else if (name.Contains("Archer"))
                {
                    attack = 7; defense = 5; speed = 9; magic = 4; health = 6;
                }
            }
        }

        private void Awake()
        {
            InitializeUI();
        }

        private void Start()
        {
            if (ValidateAndSetupUI())
            {
                CreateCharacterButtons();
                SetUIVisibility(showUIOnStart);
                
                // Enable input
                toggleCharacterUIAction.Enable();
                
                Debug.Log("🎮 CharacterSelectionUI đã khởi tạo thành công");
            }
            else
            {
                Debug.LogError("❌ CharacterSelectionUI: Không thể setup UI");
            }
        }

        private void Update()
        {
            // Handle toggle input
            if (toggleCharacterUIAction.WasPressedThisFrame())
            {
                ToggleUI();
            }
        }

        /// <summary>
        /// Khởi tạo UI system
        /// </summary>
        private void InitializeUI()
        {
            // Find or create UI canvas
            uiCanvas = GetComponentInParent<Canvas>();
            if (uiCanvas == null && autoCreateUIIfMissing)
            {
                CreateUICanvas();
            }
        }

        /// <summary>
        /// Validate và setup UI components
        /// </summary>
        private bool ValidateAndSetupUI()
        {
            bool setupSuccessful = true;

            // Check character list parent
            if (characterListParent == null)
            {
                Debug.Log("🔍 Tìm kiếm CharacterListParent...");
                characterListParent = FindOrCreateListParent();
                
                if (characterListParent == null)
                {
                    Debug.LogError("❌ Không thể tìm hoặc tạo CharacterListParent!");
                    setupSuccessful = false;
                }
            }

            // Check character button prefab
            if (characterButtonPrefab == null)
            {
                Debug.Log("🔧 Tạo button prefab cơ bản...");
                characterButtonPrefab = CreateBasicButtonPrefab();
                
                if (characterButtonPrefab == null)
                {
                    Debug.LogError("❌ Không thể tạo button prefab!");
                    setupSuccessful = false;
                }
            }

            // Setup info panel if missing
            if (characterInfoPanel == null && autoCreateUIIfMissing)
            {
                CreateCharacterInfoPanel();
            }

            return setupSuccessful;
        }

        /// <summary>
        /// Tạo UI Canvas
        /// </summary>
        private void CreateUICanvas()
        {
            GameObject canvasObj = new GameObject(canvasName);
            uiCanvas = canvasObj.AddComponent<Canvas>();
            uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            uiCanvas.sortingOrder = 100;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Set this UI as child of canvas
            transform.SetParent(uiCanvas.transform, false);
            
            Debug.Log($"🎨 Đã tạo Canvas: {canvasName}");
        }

        /// <summary>
        /// Tìm hoặc tạo list parent
        /// </summary>
        private Transform FindOrCreateListParent()
        {
            // Try to find existing
            Transform existingParent = transform.Find(listParentName);
            if (existingParent != null)
            {
                return existingParent;
            }

            // Try to find in scene
            GameObject foundObject = GameObject.Find(listParentName);
            if (foundObject != null)
            {
                return foundObject.transform;
            }

            // Create new if auto create enabled
            if (autoCreateUIIfMissing)
            {
                return CreateCharacterListParent();
            }

            return null;
        }

        /// <summary>
        /// Tạo character list parent
        /// </summary>
        private Transform CreateCharacterListParent()
        {
            GameObject listParentObj = new GameObject(listParentName);
            listParentObj.transform.SetParent(transform, false);
            
            RectTransform listRect = listParentObj.AddComponent<RectTransform>();
            listRect.anchorMin = new Vector2(0.1f, 0.3f);
            listRect.anchorMax = new Vector2(0.6f, 0.8f);
            listRect.offsetMin = Vector2.zero;
            listRect.offsetMax = Vector2.zero;

            // Add layout group
            VerticalLayoutGroup layoutGroup = listParentObj.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.spacing = 15f;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;

            ContentSizeFitter sizeFitter = listParentObj.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            Debug.Log($"📝 Đã tạo Character List Parent: {listParentName}");
            return listParentObj.transform;
        }

        /// <summary>
        /// Tạo character info panel
        /// </summary>
        private void CreateCharacterInfoPanel()
        {
            GameObject infoPanelObj = new GameObject("CharacterInfoPanel");
            infoPanelObj.transform.SetParent(transform, false);
            
            RectTransform infoRect = infoPanelObj.AddComponent<RectTransform>();
            infoRect.anchorMin = new Vector2(0.65f, 0.3f);
            infoRect.anchorMax = new Vector2(0.95f, 0.8f);
            infoRect.offsetMin = Vector2.zero;
            infoRect.offsetMax = Vector2.zero;

            // Background
            Image background = infoPanelObj.AddComponent<Image>();
            background.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

            characterInfoPanel = infoPanelObj;

            // Create text elements
            CreateInfoTextElements();
            
            Debug.Log("📋 Đã tạo Character Info Panel");
        }

        /// <summary>
        /// Tạo các text elements cho info panel
        /// </summary>
        private void CreateInfoTextElements()
        {
            // Character Name
            GameObject nameObj = new GameObject("CharacterName");
            nameObj.transform.SetParent(characterInfoPanel.transform, false);
            
            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0f, 0.8f);
            nameRect.anchorMax = new Vector2(1f, 1f);
            nameRect.offsetMin = new Vector2(10, 0);
            nameRect.offsetMax = new Vector2(-10, 0);

            characterNameText = nameObj.AddComponent<Text>();
            characterNameText.text = "Character Name";
            characterNameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            characterNameText.fontSize = 20;
            characterNameText.color = Color.white;
            characterNameText.alignment = TextAnchor.MiddleCenter;
            characterNameText.fontStyle = FontStyle.Bold;

            // Character Description
            GameObject descObj = new GameObject("CharacterDescription");
            descObj.transform.SetParent(characterInfoPanel.transform, false);
            
            RectTransform descRect = descObj.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0f, 0.4f);
            descRect.anchorMax = new Vector2(1f, 0.8f);
            descRect.offsetMin = new Vector2(10, 0);
            descRect.offsetMax = new Vector2(-10, 0);

            characterDescriptionText = descObj.AddComponent<Text>();
            characterDescriptionText.text = "Character Description";
            characterDescriptionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            characterDescriptionText.fontSize = 14;
            characterDescriptionText.color = Color.white;
            characterDescriptionText.alignment = TextAnchor.UpperLeft;
        }

        /// <summary>
        /// Tạo button prefab cơ bản
        /// </summary>
        private GameObject CreateBasicButtonPrefab()
        {
            GameObject buttonObj = new GameObject("CharacterButton");
            
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(250, 60);

            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = normalColor;

            Button button = buttonObj.AddComponent<Button>();
            
            // Create text child
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text buttonText = textObj.AddComponent<Text>();
            buttonText.text = "Character";
            buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            buttonText.fontSize = 18;
            buttonText.color = Color.white;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.fontStyle = FontStyle.Bold;

            Debug.Log("🔘 Đã tạo button prefab cơ bản");
            return buttonObj;
        }

        /// <summary>
        /// Tạo các button cho character classes
        /// </summary>
        private void CreateCharacterButtons()
        {
            if (characterListParent == null || characterButtonPrefab == null || characterClasses == null)
            {
                Debug.LogError("❌ Thiếu components để tạo character buttons");
                return;
            }

            // Clear existing buttons
            foreach (Transform child in characterListParent)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }

            characterButtons = new Button[characterClasses.Length];

            for (int i = 0; i < characterClasses.Length; i++)
            {
                GameObject buttonObj = Instantiate(characterButtonPrefab, characterListParent);
                buttonObj.name = $"Button_{characterClasses[i].className}";
                
                // Setup button text
                Text buttonText = buttonObj.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = characterClasses[i].className;
                }

                // Setup button component
                Button button = buttonObj.GetComponent<Button>();
                characterButtons[i] = button;
                
                if (button != null)
                {
                    int characterIndex = i; // Capture for lambda
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => SelectCharacter(characterIndex));
                    
                    // Set theme color
                    Image buttonImage = button.GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        buttonImage.color = characterClasses[i].themeColor;
                    }
                }

                Debug.Log($"✅ Đã tạo button cho: {characterClasses[i].className}");
            }

            Debug.Log($"🎮 Hoàn thành tạo {characterClasses.Length} character buttons");
        }

        /// <summary>
        /// Chọn character
        /// </summary>
        public void SelectCharacter(int characterIndex)
        {
            if (characterIndex < 0 || characterIndex >= characterClasses.Length)
            {
                Debug.LogError($"❌ Character index không hợp lệ: {characterIndex}");
                return;
            }

            selectedCharacterIndex = characterIndex;
            CharacterClassData selectedClass = characterClasses[characterIndex];
            
            // Update UI
            UpdateCharacterSelection();
            UpdateCharacterInfo(selectedClass);
            
            Debug.Log($"⚔️ Đã chọn character: {selectedClass.className}");
            
            // Save selection
            PlayerPrefs.SetInt("SelectedCharacterIndex", characterIndex);
            PlayerPrefs.SetString("SelectedCharacterClass", selectedClass.className);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Cập nhật visual selection
        /// </summary>
        private void UpdateCharacterSelection()
        {
            if (characterButtons == null) return;

            for (int i = 0; i < characterButtons.Length; i++)
            {
                if (characterButtons[i] != null)
                {
                    Image buttonImage = characterButtons[i].GetComponent<Image>();
                    if (buttonImage != null)
                    {
                        if (i == selectedCharacterIndex)
                        {
                            buttonImage.color = selectedColor;
                        }
                        else
                        {
                            buttonImage.color = characterClasses[i].themeColor;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cập nhật thông tin character
        /// </summary>
        private void UpdateCharacterInfo(CharacterClassData characterData)
        {
            if (characterNameText != null)
            {
                characterNameText.text = characterData.className;
                characterNameText.color = characterData.themeColor;
            }

            if (characterDescriptionText != null)
            {
                string statsInfo = $"\n📊 STATS:\n" +
                                  $"⚔️ Attack: {characterData.attack}/10\n" +
                                  $"🛡️ Defense: {characterData.defense}/10\n" +
                                  $"⚡ Speed: {characterData.speed}/10\n" +
                                  $"🔮 Magic: {characterData.magic}/10\n" +
                                  $"❤️ Health: {characterData.health}/10";
                
                characterDescriptionText.text = characterData.description + statsInfo;
            }
        }

        /// <summary>
        /// Toggle UI visibility
        /// </summary>
        [ContextMenu("🎛️ Toggle UI")]
        public void ToggleUI()
        {
            SetUIVisibility(!isUIVisible);
        }

        /// <summary>
        /// Đặt hiển thị UI
        /// </summary>
        public void SetUIVisibility(bool visible)
        {
            isUIVisible = visible;
            gameObject.SetActive(visible);
            
            Debug.Log($"🎮 Character Selection UI: {(visible ? "HIỆN" : "ẨN")}");
        }

        /// <summary>
        /// Lấy character class được chọn
        /// </summary>
        public CharacterClassData GetSelectedCharacter()
        {
            if (selectedCharacterIndex >= 0 && selectedCharacterIndex < characterClasses.Length)
            {
                return characterClasses[selectedCharacterIndex];
            }
            return null;
        }

        /// <summary>
        /// Lấy character class theo index
        /// </summary>
        public CharacterClassData GetCharacterClass(int index)
        {
            if (index >= 0 && index < characterClasses.Length)
            {
                return characterClasses[index];
            }
            return null;
        }

        /// <summary>
        /// Load character selection từ PlayerPrefs
        /// </summary>
        public void LoadCharacterSelection()
        {
            int savedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", -1);
            if (savedIndex >= 0 && savedIndex < characterClasses.Length)
            {
                SelectCharacter(savedIndex);
                Debug.Log($"📥 Đã load character selection: {characterClasses[savedIndex].className}");
            }
        }

        private void OnDestroy()
        {
            toggleCharacterUIAction?.Dispose();
        }

        #region Context Menu Actions

        [ContextMenu("🔄 Refresh Character List")]
        public void RefreshCharacterList()
        {
            if (Application.isPlaying)
            {
                CreateCharacterButtons();
                Debug.Log("🔄 Đã refresh character list");
            }
        }

        [ContextMenu("📊 Check Setup")]
        public void CheckSetup()
        {
            Debug.Log("=== CHARACTER SELECTION UI SETUP ===");
            Debug.Log($"🎮 Character Button Prefab: {(characterButtonPrefab != null ? "✅" : "❌")}");
            Debug.Log($"📝 Character List Parent: {(characterListParent != null ? "✅" : "❌")}");
            Debug.Log($"📋 Character Info Panel: {(characterInfoPanel != null ? "✅" : "❌")}");
            Debug.Log($"⚔️ Character Classes Count: {characterClasses.Length}");
            Debug.Log($"🎛️ Auto Create UI: {autoCreateUIIfMissing}");
            Debug.Log($"👁️ UI Visible: {isUIVisible}");
            Debug.Log($"🎯 Selected Character: {(selectedCharacterIndex >= 0 ? characterClasses[selectedCharacterIndex].className : "None")}");
        }

        [ContextMenu("💾 Save Current Selection")]
        public void SaveCurrentSelection()
        {
            if (selectedCharacterIndex >= 0)
            {
                PlayerPrefs.SetInt("SelectedCharacterIndex", selectedCharacterIndex);
                PlayerPrefs.SetString("SelectedCharacterClass", characterClasses[selectedCharacterIndex].className);
                PlayerPrefs.Save();
                Debug.Log($"💾 Đã save selection: {characterClasses[selectedCharacterIndex].className}");
            }
            else
            {
                Debug.LogWarning("⚠️ Chưa có character nào được chọn để save");
            }
        }

        [ContextMenu("📥 Load Saved Selection")]
        public void DebugLoadSelection()
        {
            LoadCharacterSelection();
        }

        #endregion
    }
}
