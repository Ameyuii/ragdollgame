# 🗂️ LEGACY SCRIPTS BACKUP

## 📋 **Purpose**
This folder contains backup copies of legacy scripts that will be replaced by the new Unified Character System.

## 🔄 **Migration Status**
- **Created**: 2025-07-28
- **Status**: Backup Phase
- **Target**: Complete migration to unified system

## 📁 **Backed Up Scripts**

### **🔴 Scripts to be Replaced**
- `CategoryButtonHandler.cs` → Will be replaced by `AutoUIGenerator`
- `CharacterManager.cs` → Will be replaced by `UnifiedGameManager`
- `CharacterDatabase.cs` → Will be replaced by `CharacterRegistry`
- `SimpleCharacterSelection.cs` → Will be replaced by `AutoUIGenerator`
- `CharacterDragSource.cs` → Will be replaced by `SimpleCharacterDrag`

### **🧹 Cleanup Scripts (No longer needed)**
- `CleanupConflictingObjects.cs`
- `FixMissingReferences.cs`
- `ProjectCrashFixExecutor.cs`
- `FixUnityProjectCrash.cs`

## ⚠️ **Important Notes**

### **DO NOT DELETE**
These scripts are kept as backup in case emergency rollback is needed during migration.

### **Deprecation Timeline**
1. **Week 1**: Scripts moved to backup, deprecation warnings added
2. **Week 2**: New system implemented alongside legacy
3. **Week 3**: Gradual migration with feature flags
4. **Week 4**: Complete migration, legacy scripts can be safely deleted

## 🚨 **Emergency Rollback**
If issues occur during migration, these backup scripts can be restored to their original locations.

## 📞 **Contact**
For questions about migration process, refer to `Assets/CoplayPlan.md`
