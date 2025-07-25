# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 6 (6000.2.0b1) project for a ragdoll-based battle game called "Animal Revolt". The project has evolved from a simple battle simulator into a comprehensive character management system with Vietnamese localization. Key features:

- **Ragdoll Physics Combat**: Characters switch between animated states and ragdoll physics when hit or dying
- **Advanced Character System**: ScriptableObject-based character database with categories, variants, and stats
- **Team-Based Battles**: Up to 4 teams with automatic enemy detection and customizable configurations
- **Event-Driven Architecture**: Centralized event system for character lifecycle and UI interactions
- **Object Pooling**: Performance-optimized character spawning and management
- **Hybrid Legacy/Modern System**: Gradual migration with fallback support

## Core Architecture

### Character System Architecture
The project uses a sophisticated **dual-system architecture**:

**Modern System (Primary):**
- **`CharacterDatabase`** (`Assets/Scripts/CharacterSystem/CharacterDatabase.cs`): Central ScriptableObject managing all characters, categories, and teams with optimized lookup tables
- **`CharacterDefinition`** (`Assets/Scripts/CharacterSystem/CharacterDefinition.cs`): ScriptableObject defining individual characters with stats, variants, materials, and metadata
- **`GameDatabase`** (`Assets/Scripts/CharacterSystem/GameDatabase.cs`): Singleton providing runtime access to character data
- **`CharacterSystemManager`**: Singleton with object pooling via `CharacterPool`

**Legacy System (Fallback):**
- **`RagdollCharacter`** (`Assets/Scripts/RagdollCharacter.cs`): Full physics-based character with ragdoll capabilities
- **`CharacterManager`**: Original character management with category organization

### Manager System Hierarchy
Multiple specialized managers coordinate the game systems:
- **`ARBSGameManager`**: Main game manager integrating all systems, handling UI events and AI logic generation
- **`BattleGameManager`**: Battle-specific logic with setup mode, team counters, and victory conditions
- **`MapStateManager`**: Map state persistence and character instance tracking with save/load functionality
- **`CharacterSystemManager`**: Modern character system with object pooling and performance optimization

### Event-Driven Architecture
**`CharacterEvents`** provides centralized static event management:
- Character lifecycle (spawned, died, team changed)
- Selection events (character, variant, team selection)
- UI events (selection confirmed/cancelled)
- Battle events (started, team victory)

### Data Architecture
**Core Enums & Data Structures:**
- **`UnlockType`**: AlwaysUnlocked, PlayerLevel, CharacterUsage, BattlesWon, etc.
- **`Rarity`**: Common, Uncommon, Rare, Epic, Legendary
- **`CharacterType`**: Soldier (ChienBinh), Robot, Monster (QuaiVat), Zombie
- **`CharacterStats`**: Comprehensive stats system with modifiers and cloning
- **`TeamConfiguration`**: Advanced team setup with materials, colors, and enemy detection
- **`CharacterVariant`**: System for character variations with custom prefabs and stat modifiers

### Scene Architecture & GameObject Structure
**Main scenes serve specific purposes:**
- **`backup.unity`**: Main battle scene with NavMesh data (subdirectory: `/backup/`)
- **`SimpleDemo.unity`**, `SimpleRagdollDemo.unity`, `TestScene.unity`: Testing environments

**Expected GameObject hierarchy:**
- **Ground**: Main battle area with Renderer bounds for map calculations
- **UI Canvas**: Team counters (Team1Counter, Team2Counter), StatusText, StartButton, ResetButton
- **Character spawn system**: Drop zones with material-based validation

## Development Commands

### Unity Editor Workflow
- **Play Mode**: Use Unity Editor play button to test scenes
- **Build**: File → Build Settings → Build (no automated build scripts)
- **Character Database**: Use Context Menu "Rebuild Database", "Initialize Default Categories"
- **Validation**: Run `CharacterSystemValidator` for character data integrity
- **Testing**: Use `ForceStabilizeCharacters.Execute()` to fix physics issues

### MCP Integration
The project uses **Model Context Protocol (MCP)** for development assistance:
- **Port**: 8090 (configured in `ProjectSettings/McpUnitySettings.json`)
- **Auto-start server**: Enabled
- **Log monitoring**: Use MCP connections to read Unity console logs for debugging

### Package Dependencies
**Core packages:**
- Universal Render Pipeline (URP) 17.2.0
- Input System 1.14.0  
- AI Navigation 2.0.7
- Unity AI Assistant/Generators (experimental AI tools)

**Development plugins:**
- **Coplay**: `com.coplaydev.coplay` (collaborative development)
- **MCP Unity**: `com.gamelovers.mcp-unity` (Model Context Protocol integration)

## Code Conventions & Development Rules

### Vietnamese Development Context
**Language Requirements (from .cursor/rules):**
- Always respond in Vietnamese ("luôn trả lời bằng tiếng việt")
- All comments in Vietnamese ("các chú thích luôn bằng tiếng việt")
- Character categories follow Vietnamese naming: "🪖 CHIẾN BINH" (Warriors), "🤖 ROBOT", "👹 QUÁI VẬT" (Monsters), "🧟 ZOMBIE"

**Development Constraints (from .coplayrules.md):**
- **Never create scripts without being asked** ("không tạo script khi không được yêu cầu")
- **Never auto-create new features/UI without permission** ("không tự tạo chức năng mới khi không hỏi ý kiến")
- **Fix errors properly, don't delete files** ("khi có lỗi phải cố gắng sửa phải, không phải xoá file")
- **Prefer MCP connections for development work** ("ưu tiên sử dụng kết nối mcp để làm việc")
- **Use logging instead of test files** ("hạn chế tạo các file test, sử dụng log để đọc log console bằng mcp")
- **Reload Unity after code completion** ("sau khi hoàn thành mã hay reload lại unity để check lỗi")

### Coding Standards
**Naming Conventions:**
- Public fields: `[Header("Character Stats")] public float maxHealth = 100f;`
- Private fields: `private bool isDead = false;`
- Character IDs: `category_type_variant_version` format (e.g., `warrior_soldier_default_01`)
- Component caching in Start(): `animator = GetComponent<Animator>();`

**Architecture Patterns:**
- **Singleton Pattern**: Used for managers (`GameDatabase.Instance`, `MapStateManager.Instance`)
- **ScriptableObject Pattern**: All character data uses ScriptableObjects for modularity
- **Event-Driven**: Use `CharacterEvents` for decoupled communication
- **Object Pooling**: Use `CharacterPool` for performance optimization

### Physics & Performance Management
**Ragdoll System:**
- Characters use kinematic rigidbodies for controlled movement
- Ragdoll parts disabled by default, enabled only when needed
- Velocity limits and damping prevent excessive physics forces
- Ground clamping keeps characters within map bounds (-20 to 20 on X/Z axes)

**Team System:**
- Team IDs: 1=Blue, 2=Red, 3=Green, 4=Yellow (expandable to maxTeams=4)
- Enemy detection via optimized lookup tables, not `FindObjectsOfType`
- Automatic material assignment: `Team1_Blue.mat`, `Team2_Red.mat`, etc.

### Asset Organization & Resources
**Structured asset hierarchy:**
- **`Assets/Resources/`**: Runtime-loadable ScriptableObjects and prefabs
- **`Assets/CharacterDefinitions/`**: Individual character ScriptableObject definitions
- **`Assets/Prefabs/ChienBinh/`, `/QuaiVat/`, `/Robot/`**: Organized by Vietnamese character categories
- **`Assets/Scripts/CharacterSystem/`**: Modern character system components
- **`Assets/Scripts/Editor/`**: Custom editor tools and validators

### Development Workflow Integration
**System Migration Approach:**
- Use `enableNewSystem` flag in GameDatabase for gradual migration
- Provide fallback to legacy system when new system disabled
- Migration tools available: `CharacterDefinition.CreateFromRagdollCharacter()`

**Performance Monitoring:**
- Use `PerformanceManager` for system monitoring
- Dictionary-based lookups for runtime performance
- Object pooling for character spawning/despawning
- MCP log monitoring for debugging without test files

### Recent Optimizations (Updated)
**UI System Cleanup (2024):**
- **Removed 9 redundant script files**: TeamSelectionHandler, TeamSelectionFix, SimpleFix, and various test scripts
- **Unified UI system**: Now uses only BattleGameManager for all UI operations
- **Eliminated UI conflicts**: No more dual-system competition for team selection
- **Refactored BattleGameManager**: Split large methods into smaller, maintainable functions
- **Improved error handling**: Better validation and Vietnamese debug messages

**Current System Status:**
- ✅ **Stable UI system** with single source of truth (BattleGameManager)
- ✅ **Drag & drop functionality** working correctly
- ✅ **Team selection** integrated into main UI
- ⚠️ **Character AI movement/combat disabled** in `RagdollCharacter` (line 141: `if (false)`)
- ⚠️ **Position validation disabled** in `CharacterDragSource` (allows unlimited spawning)

### Testing & Validation
Use `OptimizationTest.cs` for system verification:
- Context menu: "Test All Systems" - Comprehensive system check
- Context menu: "Test Character Spawning" - Spawn functionality test
- Context menu: "Reset All Drag States" - Drag system reset