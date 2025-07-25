# Character Management System - User Guide

## 🎯 Quick Start

### Enabling the New System
```csharp
// Enable new character system
LegacySystemBridge.Instance.EnableNewSystemIntegration();

// Disable (fallback to legacy)
LegacySystemBridge.Instance.DisableNewSystemIntegration();
```

### Basic Usage
1. **Click on map** → Character Selection UI opens automatically
2. **Follow 5-step selection process**
3. **Character spawns** with all configurations applied

---

## 📋 Step-by-Step Character Selection

### Step 1: Category Selection
- Choose from: **Warrior**, **Archer**, **Mage**, **Support**, **Monster**, **Robot**
- Each category shows available character count
- Only unlocked categories are selectable

### Step 2: Character Selection
- Browse characters in selected category
- View character icons, names, and basic stats
- Read character descriptions
- Only unlocked characters are available

### Step 3: Variant Selection
- Choose character variant (Default, Elite, etc.)
- View stat differences between variants
- Check unlock requirements for locked variants
- Default variant auto-selected if available

### Step 4: Team Assignment
- Select from available teams (Team 1, Team 2, etc.)
- Preview character with team colors
- View team information and colors

### Step 5: Final Preview
- **3D Preview**: Real-time character model with team materials
- **Stats Display**: Final stats after all modifiers
- **Confirm or Go Back**: Make final decision

---

## 🛠️ For Developers

### Adding New Characters

#### Method 1: Migration Tool (Recommended)
```
Tools > Character System > Migration Tool
1. Scan for existing RagdollCharacter prefabs
2. Set category mapping
3. Create backup (recommended)
4. Run migration
5. Verify results in CharacterDatabase
```

#### Method 2: Manual Creation
```csharp
// Create new CharacterDefinition
// Assets > Create > Character System > Character Definition

CharacterDefinition newChar = ScriptableObject.CreateInstance<CharacterDefinition>();
newChar.CharacterID = "warrior_heavy_armored_01";  // Format: category_type_variant_version
newChar.DisplayName = "Heavy Armored Warrior";
newChar.CategoryID = "warrior";
newChar.BasePrefab = yourPrefab;
newChar.UIIcon = yourIcon;

// Set stats
newChar.BaseStats = new CharacterStats
{
    maxHealth = 150f,
    moveSpeed = 2.5f,
    attackDamage = 30f,
    attackRange = 2f,
    armor = 10f
};

// Add to database
CharacterDatabase database = GameDatabase.Instance.characterDatabase;
database.AddCharacter(newChar);
```

### Creating Character Variants
```csharp
CharacterVariant eliteVariant = new CharacterVariant
{
    variantID = "elite",
    variantName = "Elite",
    description = "Enhanced version with better stats",
    statModifiers = new CharacterStats
    {
        maxHealth = 50f,    // +50 health
        attackDamage = 10f, // +10 damage
        armor = 5f          // +5 armor
    },
    isDefault = false
};

characterDefinition.Variants.Add(eliteVariant);
```

### Setting Up Teams
```csharp
TeamConfiguration newTeam = new TeamConfiguration
{
    teamID = 3,
    teamName = "Green Team",
    teamDescription = "Nature-themed team",
    primaryColor = Color.green,
    secondaryColor = Color.white,
    isPlayerTeam = true
};

database.teams.Add(newTeam);
```

---

## 🔧 System Configuration

### Performance Settings
```csharp
// Configure PerformanceManager
PerformanceManager perfManager = PerformanceManager.Instance;
perfManager.maxActiveCharacters = 50;
perfManager.enableLODSystem = true;
perfManager.enableFrustumCulling = true;
perfManager.maxRenderDistance = 100f;
```

### Object Pooling
```csharp
// Configure CharacterPool
CharacterPool pool = CharacterPool.Instance;
pool.enablePooling = true;
pool.defaultPoolSize = 10;
pool.maxPoolSize = 50;

// Prewarm specific characters
pool.PrewarmPool("warrior_basic_default_01", 5);
```

### Validation
```csharp
// Validate system integrity
CharacterSystemValidator validator = FindObjectOfType<CharacterSystemValidator>();
validator.ValidateSystem();

// Get validation results
ValidationSummary summary = validator.GetValidationSummary();
Debug.Log($"Found {summary.totalIssues} issues");
```

---

## 🧪 Testing

### Running Tests
```csharp
// Method 1: Component context menu
// Right-click CharacterSystemTest component > "Test Character System"

// Method 2: Code
CharacterSystemTest.Execute();

// Method 3: Individual tests
CharacterSystemTest tester = FindObjectOfType<CharacterSystemTest>();
tester.TestCharacterSystem();
```

### Test Coverage
- ✅ Database loading and integrity
- ✅ Character spawning and initialization
- ✅ Object pooling functionality
- ✅ Legacy system bridge
- ✅ Validation system
- ✅ Performance monitoring

---

## 🚨 Troubleshooting

### Common Issues

#### "Character not found" Error
```csharp
// Check if character exists in database
CharacterDatabase db = GameDatabase.Instance.characterDatabase;
bool exists = db.HasCharacter("your_character_id");

// Rebuild database if needed
db.RebuildDatabase(); // Context menu on database asset
```

#### UI Not Showing
```csharp
// Check if CharacterSelectionUI exists in scene
CharacterSelectionUI ui = FindObjectOfType<CharacterSelectionUI>();
if (ui == null)
{
    Debug.LogError("CharacterSelectionUI not found in scene");
}

// Check if new system is enabled
bool enabled = LegacySystemBridge.Instance.IsNewSystemReady();
Debug.Log($"New system ready: {enabled}");
```

#### Performance Issues
```csharp
// Check performance stats
PerformanceManager.Instance.LogPoolStatistics();

// Reduce quality if needed
PerformanceManager perfManager = PerformanceManager.Instance;
perfManager.maxActiveCharacters = 30; // Reduce from 50
perfManager.enableLODSystem = true;   // Enable LOD
```

#### Migration Problems
```csharp
// Use migration tool with backup
// Tools > Character System > Migration Tool
// 1. Enable "Create Backup"
// 2. Check "Overwrite Existing" if needed
// 3. Run migration
// 4. Check console for errors
```

---

## 📁 File Structure Reference

```
Assets/
├── Scripts/CharacterSystem/
│   ├── Core/
│   │   ├── CharacterDefinition.cs
│   │   ├── CharacterDatabase.cs
│   │   ├── GameDatabase.cs
│   │   └── EnhancedCharacterController.cs
│   ├── UI/
│   │   └── CharacterSelectionUI.cs
│   ├── Performance/
│   │   ├── CharacterPool.cs
│   │   └── PerformanceManager.cs
│   ├── Integration/
│   │   └── LegacySystemBridge.cs
│   ├── Validation/
│   │   ├── CharacterSystemValidator.cs
│   │   └── CharacterSystemTest.cs
│   └── Editor/
│       └── CharacterMigrationTool.cs
├── Data/Characters/
│   ├── CharacterDatabase.asset
│   ├── GameDatabase.asset
│   └── [Character Definitions]/
└── Resources/
    └── GameDatabase.asset (for runtime loading)
```

---

## 🎉 Best Practices

### Character ID Naming
- **Format**: `category_type_variant_version`
- **Example**: `warrior_heavy_armored_01`
- **Always lowercase** with underscores

### Database Management
- **Use Migration Tool** for bulk operations
- **Validate regularly** with CharacterSystemValidator
- **Backup before** major changes
- **Test after** adding new content

### Performance Optimization
- **Enable Object Pooling** for frequently spawned characters
- **Use LOD System** for large battles
- **Monitor Performance** with PerformanceManager
- **Limit Active Characters** based on target platform

### Testing
- **Run tests** after system changes
- **Validate data** before production
- **Test UI workflow** end-to-end
- **Check backward compatibility** with legacy code

---

*Last Updated: July 2024*
*System Version: 2.0*
