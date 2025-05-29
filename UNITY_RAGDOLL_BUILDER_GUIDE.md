# Unity Ragdoll Builder Integration Guide

## Overview
This project has been migrated from a complex custom ragdoll system to Unity 6.2's built-in Ragdoll Builder for better performance and reliability.

## What Changed

### Old System (RagdollController.cs)
- Complex hybrid animation-physics blending
- Custom transition states and timers
- Vietnamese naming convention
- 450+ lines of complex code

### New System (SimpleRagdollController.cs)
- Uses Unity's Ragdoll Builder components
- Simple activate/deactivate ragdoll modes
- Clean, efficient implementation
- ~200 lines of focused code
- Backward compatibility methods

## Setup Instructions

### 1. Create Ragdoll with Unity's Ragdoll Builder

1. **Select your NPC prefab** in the Project window
2. **Open Ragdoll Wizard**: 
   - Go to `GameObject > 3D Object > Ragdoll...`
   - Or use `Window > Animation > Ragdoll Builder` (Unity 6.2+)
3. **Assign bone transforms**:
   - Drag bone transforms from your character's hierarchy
   - Required bones: Pelvis, Left/Right Hip, Left/Right Knee, Left/Right Foot, Spine, Head, Left/Right Arm, Left/Right Elbow, Left/Right Hand
4. **Configure physics settings**:
   - Total Mass: 70-80 (human-like)
   - Strength: 0 (for death ragdoll)
   - Click "Create" to generate ragdoll components

### 2. Replace RagdollController with SimpleRagdollController

1. **Remove old component**:
   - Select your NPC prefab
   - Remove the `RagdollController` component
2. **Add new component**:
   - Add `SimpleRagdollController` component
   - It will auto-find Animator, Rigidbody, and Collider
3. **Configure settings**:
   - Ragdoll Recovery Time: 5 seconds (how long before auto-recovery)
   - Force Threshold: 10 (minimum force to trigger ragdoll)
   - Show Debug Logs: true (for testing)

### 3. Test the System

1. **Play the scene**
2. **NPCs should**:
   - Move and find each other normally
   - Attack and deal damage with physics impact
   - Activate ragdoll when health < 30% or on death
   - Show debug logs confirming ragdoll activation

## SimpleRagdollController Features

### Key Methods
- `ActivateRagdoll(Vector3 force, Vector3 impactPoint)` - Activate with force
- `DeactivateRagdoll()` - Return to animated mode
- `ForceRagdoll()` - Immediate activation (for death)
- `ShouldActivateRagdoll(float force)` - Check if force is sufficient

### Properties
- `CurrentState` - Current ragdoll state (Animated/Ragdoll/Transitioning)
- `IsRagdoll` - Quick check if in ragdoll mode
- `IsAnimated` - Quick check if in animated mode

### Events
- `OnRagdollActivated` - Called when ragdoll activates
- `OnRagdollDeactivated` - Called when ragdoll deactivates

### Legacy Compatibility
- `KichHoatRagdoll()` - Maps to `ActivateRagdoll()` for existing code
- `DangLaRagdoll` - Maps to `IsRagdoll` property

## Troubleshooting

### No Ragdoll Components Found
- Make sure you ran Unity's Ragdoll Builder on your character
- Check that ragdoll Rigidbodies and Colliders are children of the NPC

### NPCs Not Activating Ragdoll
- Check force threshold settings
- Verify `SimpleRagdollController` is attached to NPCs
- Look for debug logs in Console

### Ragdoll Physics Issues
- Adjust ragdoll joint limits in Unity's Ragdoll Builder
- Reduce ragdoll mass if too heavy
- Check collision layers and physics materials

## Benefits of New System

### Performance
- Uses Unity's optimized ragdoll components
- No complex state management overhead
- Fewer Update() calls

### Reliability
- Battle-tested Unity system
- Better physics stability
- Easier debugging

### Maintainability
- Simpler codebase
- Standard Unity workflow
- Better documentation

## Migration Checklist

- [ ] Remove old `RagdollController` components from NPCs
- [ ] Add `SimpleRagdollController` to NPCs
- [ ] Run Unity Ragdoll Builder on NPC prefabs
- [ ] Test ragdoll activation in play mode
- [ ] Verify combat and physics interactions
- [ ] Check debug logs for proper operation

## Unity 6.2 Ragdoll Builder Tips

### Best Practices
1. **Use consistent naming** for bones across characters
2. **Set appropriate mass distribution** (heavier torso, lighter extremities)
3. **Configure joint limits** realistically for human movement
4. **Test with different force values** to find optimal settings
5. **Use Physics Materials** for realistic surface interactions

### Common Settings
- **Drag**: 0.1-0.2 for air resistance
- **Angular Drag**: 0.05-0.1 for rotation damping
- **Mass**: Total 70-80, distributed by body part size
- **Joint Spring**: 0 for death ragdoll, higher for active ragdoll

This new system provides a cleaner, more maintainable solution that leverages Unity's proven ragdoll technology while maintaining all the combat and physics features your NPCs need.
