# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 6 (6000.2.0b1) project for a ragdoll-based battle game called "Animal Revolt" (based on git history). The project features:

- **Ragdoll Physics Combat**: Characters that can switch between animated states and ragdoll physics when hit or dying
- **Team-Based Battles**: Blue team (ID 1) vs Red team (ID 2) with automatic enemy detection and combat
- **Health System**: Visual health bars that float above characters and update in real-time
- **Drag & Drop Character Placement**: UI system for placing characters on the battlefield
- **Battle Management**: GameManager singleton that controls battle flow and victory conditions

## Core Architecture

### Character System
The project has two main character implementations:
- **`RagdollCharacter`** (`Assets/Scripts/RagdollCharacter.cs`): Full physics-based character with ragdoll capabilities (main implementation)
- **`StableCharacter`** (`Assets/Scripts/StableCharacter.cs`): Simplified character without physics (for testing/fallback)
- **`ICharacter`** interface defines core character contract

**Note**: Removed problematic scripts that caused physics conflicts:
- `RagdollSetup.cs` - Conflicted with Unity Ragdoll Wizard setup
- `SimpleRagdollCharacter.cs`, `ProperRagdollCharacter.cs` - Redundant variants
- `FixRagdollPhysics.cs`, `FixCharacterPhysics.cs` - Workaround scripts no longer needed

### Game Management
- **`GameManager`** (`Assets/Scripts/GameManager.cs`): Singleton that manages battle state, character registration, team tracking, and victory conditions
- Battle states: Setup → Battle In Progress → Victory/Draw → Reset

### Key Components
- **Health System**: Canvas-based health bars that follow characters and face the camera
- **Team System**: Characters assigned to teams (1=Blue, 2=Red) with automatic enemy detection
- **Ragdoll Physics**: Temporary ragdoll on hit, permanent on death with velocity limiting to prevent "flying"
- **Animation Integration**: Animator controllers with Speed parameter and Attack triggers

## Development Commands

### Unity Editor
- **Play Mode**: Use Unity Editor play button to test scenes
- **Build**: File → Build Settings → Build (no specific build script found)
- **Scene Navigation**: Main scenes are in `Assets/Scenes/`:
  - `backup.unity` - Main battle scene
  - `SimpleDemo.unity` - Basic demo
  - `SimpleRagdollDemo.unity` - Ragdoll testing
  - `TestScene.unity` - Testing environment

### Package Management
Key packages used:
- Universal Render Pipeline (URP) 17.2.0
- Input System 1.14.0
- AI Navigation 2.0.7
- Unity AI Assistant/Generators (experimental AI tools)
- Coplay plugin (collaborative development)
- MCP Unity plugin (model context protocol integration)

## Code Conventions

### Naming
- Public fields use camelCase with explicit headers: `[Header("Character Stats")] public float maxHealth = 100f;`
- Private fields use camelCase: `private bool isDead = false;`
- Components cached in Start(): `animator = GetComponent<Animator>();`

### Physics Management
- Characters use kinematic rigidbodies for controlled movement during normal state
- Ragdoll parts are disabled by default, enabled only when needed
- Velocity limits and damping prevent excessive physics forces
- Ground clamping keeps characters within map bounds (-20 to 20 on X/Z axes)

### Team System
- Team IDs: 1 = Blue, 2 = Red
- Enemy detection via `FindObjectsOfType<RagdollCharacter>()` with team filtering
- Materials: `Team1_Blue.mat`, `Team2_Red.mat` for visual identification

### UI Integration
- Health bars use World Space Canvas with camera facing
- GameManager updates team counts and battle status in real-time
- Drag & drop system uses specific materials for valid/invalid drop zones

## Important Notes

- **Coplay Rules**: Vietnamese comment states "không tạo script khi không được yêu cầu" (don't create scripts when not requested)
- **Physics Stability**: Characters are designed to minimize physics glitches through careful rigidbody management
- **Performance**: Uses object pooling concepts and efficient enemy detection within range limits
- **Animation Events**: Animator controllers expect Speed (float) and Attack (trigger) parameters
- **Scene Management**: Backup scenes preserve NavMesh data in subdirectories

## File Structure Focus
- `Assets/Scripts/` - Main gameplay scripts
- `Assets/Animation/` - Animator controllers and animation clips
- `Assets/Materials/` - Team colors and physics materials
- `Assets/Prefabs/` - Character and ground prefabs with NPC variants
- `Assets/Scenes/` - Test and demo scenes with NavMesh data

## Testing
- Use `GameManager.StartBattle()` to begin combat simulation
- Characters automatically engage nearest enemies when battle starts
- Health bars and team counters provide visual feedback
- Reset functionality available through `GameManager.ResetBattle()`