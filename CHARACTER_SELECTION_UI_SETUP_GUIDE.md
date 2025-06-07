# 🎮 CHARACTER SELECTION UI SETUP GUIDE

## 📋 Tổng Quan

Hệ thống Character Selection UI cho phép người chơi chọn giữa 3 class character:
- ⚔️ **Warrior**: Chiến binh cận chiến với attack và defense cao
- 🔮 **Mage**: Pháp sư với magic mạnh mẽ nhưng defense thấp  
- 🏹 **Archer**: Cung thủ cân bằng với speed và tầm xa

## 🚀 Quick Setup

### Bước 1: Thêm CharacterSelectionUI vào Scene

1. Tạo empty GameObject trong scene
2. Đặt tên: `CharacterSelectionUI`
3. Add component `CharacterSelectionUI` script
4. Script sẽ tự động tạo UI structure nếu `autoCreateUIIfMissing = true`

### Bước 2: Cấu Hình Cơ Bản

Trong Inspector của CharacterSelectionUI:

```
🎮 UI References:
- Character Button Prefab: (để trống - sẽ tự tạo)
- Character List Parent: (để trống - sẽ tự tạo)
- Character Info Panel: (để trống - sẽ tự tạo)

🎛️ UI Settings:
- Auto Create UI If Missing: ✅ CHECKED
- Show UI On Start: ✅ CHECKED (nếu muốn hiện ngay khi start)
- Canvas Name: "CharacterSelectionCanvas"
- List Parent Name: "CharacterListParent"

🎨 UI Styling:
- Selected Color: Xanh lá (0.2, 0.8, 0.2, 1)
- Normal Color: Xanh dương đậm (0.2, 0.3, 0.5, 1)

⌨️ Input:
- Toggle Character UI Action: Keyboard/C
```

### Bước 3: Tích Hợp với UnifiedUIManager

1. Trong scene, tìm hoặc tạo `UnifiedUIManager`
2. CharacterSelectionUI sẽ tự động được đăng ký vào hệ thống
3. Có thể toggle qua UnifiedUIManager hoặc phím tắt riêng (C)

## 🔧 Advanced Setup

### Custom Button Prefab

Nếu muốn tùy chỉnh button:

1. Tạo prefab button trong `Assets/Prefabs/`
2. Structure yêu cầu:
   ```
   CharacterButton (GameObject + RectTransform + Image + Button)
   └── Text (GameObject + RectTransform + Text)
   ```
3. Assign prefab vào `Character Button Prefab` field

### Custom Canvas Setup

Nếu muốn sử dụng Canvas có sẵn:

1. Set `autoCreateUIIfMissing = false`
2. Manually assign:
   - `Character List Parent`: Transform chứa buttons
   - `Character Info Panel`: Panel hiển thị thông tin
   - Text components cho name và description

## 🎛️ Phím Tắt và Controls

### Phím Tắt Mặc Định

- **C**: Toggle Character Selection UI
- **F1**: Toggle tất cả UI (qua UnifiedUIManager)
- **F2**: Toggle UI camera hiện tại

### Programmatic Control

```csharp
// Lấy reference
var charUI = FindFirstObjectByType<AnimalRevolt.UI.CharacterSelectionUI>();

// Hiện/ẩn UI
charUI.SetUIVisibility(true);
charUI.ToggleUI();

// Chọn character theo index
charUI.SelectCharacter(0); // Warrior
charUI.SelectCharacter(1); // Mage  
charUI.SelectCharacter(2); // Archer

// Lấy thông tin character đã chọn
var selectedChar = charUI.GetSelectedCharacter();
if (selectedChar != null)
{
    Debug.Log($"Selected: {selectedChar.className}");
    Debug.Log($"Attack: {selectedChar.attack}");
}

// Load/Save selection
charUI.LoadCharacterSelection(); // Load từ PlayerPrefs
charUI.SaveCurrentSelection();   // Save vào PlayerPrefs
```

## 📊 Character Class Stats

### ⚔️ Warrior
- **Attack**: 8/10
- **Defense**: 9/10
- **Speed**: 4/10
- **Magic**: 2/10
- **Health**: 9/10

### 🔮 Mage
- **Attack**: 3/10
- **Defense**: 3/10
- **Speed**: 6/10
- **Magic**: 10/10
- **Health**: 4/10

### 🏹 Archer
- **Attack**: 7/10
- **Defense**: 5/10
- **Speed**: 9/10
- **Magic**: 4/10
- **Health**: 6/10

## 🎨 UI Layout

### Màn Hình Layout
```
┌─────────────────────────────────────────────────────────────┐
│                    Character Selection UI                    │
├─────────────────────────┬───────────────────────────────────┤
│   Character List        │      Character Info Panel        │
│                         │                                   │
│  ⚔️ [  Warrior  ]       │  ⚔️ Warrior                      │
│  🔮 [   Mage    ]       │                                   │
│  🏹 [  Archer   ]       │  Chiến binh mạnh mẽ với sức      │
│                         │  tấn công cao và khả năng        │
│                         │  phòng thủ tốt...                │
│                         │                                   │
│                         │  📊 STATS:                       │
│                         │  ⚔️ Attack: 8/10                 │
│                         │  🛡️ Defense: 9/10                │
│                         │  ⚡ Speed: 4/10                   │
│                         │  🔮 Magic: 2/10                   │
│                         │  ❤️ Health: 9/10                  │
└─────────────────────────┴───────────────────────────────────┘
```

## 🛠️ Troubleshooting

### UI Không Hiện

1. **Kiểm tra Console**: Xem có lỗi nào không
2. **Check Setup**: Chạy context menu "Check Setup"
3. **Auto Create**: Đảm bảo `autoCreateUIIfMissing = true`
4. **Canvas**: Kiểm tra có Canvas trong scene không

### Button Không Hoạt Động

1. **EventSystem**: Đảm bảo có EventSystem trong scene
2. **GraphicRaycaster**: Canvas phải có GraphicRaycaster
3. **Button Component**: Kiểm tra button prefab có Button component

### Character Không Được Chọn

1. **Console Logs**: Kiểm tra debug logs khi click
2. **Character Index**: Đảm bảo index hợp lệ (0-2)
3. **Button Events**: Verify button onClick events được setup

### UI Bị Che Khuất

1. **Canvas Sort Order**: Tăng sortingOrder của Canvas
2. **RectTransform**: Kiểm tra anchor và position
3. **Screen Resolution**: Test trên độ phân giải khác nhau

## 🔄 Context Menu Actions

Trong Inspector của CharacterSelectionUI:

- **🎛️ Toggle UI**: Test toggle hiển thị
- **🔄 Refresh Character List**: Tạo lại buttons
- **📊 Check Setup**: Kiểm tra cấu hình hiện tại
- **💾 Save Current Selection**: Lưu selection hiện tại
- **📥 Load Saved Selection**: Load selection đã lưu

## 📱 Integration với UnifiedUIManager

CharacterSelectionUI tự động tích hợp với UnifiedUIManager:

```csharp
// Qua UnifiedUIManager
var uiManager = FindFirstObjectByType<UnifiedUIManager>();
uiManager.ShowCategoryUI("CharacterUI");    // Hiện Character UI
uiManager.HideCategoryUI("CharacterUI");    // Ẩn Character UI
uiManager.ToggleAllUI();                    // Toggle tất cả UI
```

## 💾 Data Persistence

Selection được lưu vào PlayerPrefs:
- `SelectedCharacterIndex`: Index của character (0-2)
- `SelectedCharacterClass`: Tên class đã chọn

```csharp
// Lấy saved data
int savedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", -1);
string savedClass = PlayerPrefs.GetString("SelectedCharacterClass", "");
```

## 🎯 Best Practices

### Performance
- UI chỉ active khi cần thiết
- Sử dụng object pooling cho buttons nếu có nhiều characters
- Cache references để tránh FindObjectByType

### User Experience  
- Visual feedback rõ ràng khi chọn character
- Animation smooth khi chuyển đổi
- Responsive design cho nhiều resolution

### Code Quality
- Validate tất cả references trước khi sử dụng
- Error handling đầy đủ
- Debug logs có emoji để dễ đọc

## 🔗 Tích Hợp Game Systems

### Combat System
```csharp
var selectedChar = charUI.GetSelectedCharacter();
if (selectedChar != null)
{
    // Apply stats to player character
    playerCombat.SetStats(selectedChar.attack, selectedChar.defense);
}
```

### Save System
```csharp
// Auto save selection khi chọn
void SelectCharacter(int index)
{
    // ... selection logic
    
    // Auto save
    PlayerPrefs.SetInt("SelectedCharacterIndex", index);
    PlayerPrefs.Save();
}
```

### Scene Management
```csharp
// Load character data ở scene khác
void Start()
{
    int charIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
    LoadCharacterData(charIndex);
}