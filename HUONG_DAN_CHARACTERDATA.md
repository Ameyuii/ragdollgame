# 📋 Hướng Dẫn Sử Dụng CharacterData System

## 🎯 CharacterData là gì?
CharacterData là một ScriptableObject cho phép bạn tạo các file cấu hình (.asset) để thiết lập stats cho NPCs mà không cần viết code.

## 📝 Cách sử dụng từng bước:

### Bước 1: Tạo CharacterData Asset
1. **Trong Unity Editor:**
   - Right-click trong Project window
   - Chọn: `Create → NPC System → Character Data`
   - Đặt tên file (ví dụ: `EliteWarrior`, `FireMage`, `SpeedArcher`)

### Bước 2: Cấu hình Stats
2. **Chỉnh sửa asset vừa tạo:**
   ```
   Basic Stats:
   - Character Name: "Elite Warrior"
   - Max Health: 200
   - Move Speed: 2.5
   - Team Id: 1
   
   Combat Stats:
   - Base Damage: 35
   - Attack Range: 3
   - Attack Cooldown: 1.5
   - Detection Range: 20
   
   Visual & Audio:
   - Character Prefab: [Drag warrior model]
   - Animator Controller: [Drag animator]
   - Hit Effect: [Drag particle effect]
   - Death Effect: [Drag death effect]
   
   AI Behavior:
   - Patrol Speed: 1.5
   - Patrol Rest Time: (2, 5)
   - Can Attack While Patrolling: ✓
   ```

### Bước 3: Áp Dụng vào NPC
3. **Kết nối với NPCs:**
   - Chọn GameObject chứa **WarriorController/MageController/ArcherController** (KHÔNG phải NPCBaseController!)
   - NPCBaseController là abstract class nên không thể add trực tiếp
   - Trong Inspector, tìm section `Character Configuration`
   - Drag & drop asset CharacterData vào field `Character Data`
   - **KHÔNG cần làm gì thêm!** Hệ thống sẽ tự động áp dụng khi game start

### Bước 4: Tạo NPC trong scene
4. **Tạo NPCs với MCP Unity:**
   ```
   Sử dụng MCP commands để tạo NPCs:
   - Tạo GameObject mới
   - Add WarriorController/MageController/ArcherController
   - Add NavMeshAgent, Animator, Collider, Rigidbody
   - Drag CharacterData asset vào controller
   ```
### Bước 5: Kiểm Tra
5. **Verify hoạt động:**
   - Play game
   - Kiểm tra Console log: "✅ NPC_Name: Applied CharacterData 'Elite Warrior'"
   - Stats sẽ được tự động cập nhật

## 🎮 Ví dụ thực tế:

### Tạo các variant NPCs:
```
📁 CharacterDatas/
├── Warriors/
│   ├── BasicWarrior.asset     (HP: 100, DMG: 20)
│   ├── EliteWarrior.asset     (HP: 200, DMG: 35)
│   └── BossWarrior.asset      (HP: 500, DMG: 50)
├── Mages/
│   ├── FireMage.asset         (HP: 80, DMG: 40, Range: 10)
│   └── IceMage.asset          (HP: 90, DMG: 30, Slow effect)
└── Archers/
    ├── SpeedArcher.asset      (HP: 70, DMG: 25, Speed: 5)
    └── SniperArcher.asset     (HP: 60, DMG: 45, Range: 20)
```

### Kết nối với NPCs:
```
🏰 Scene Hierarchy:
├── Warriors/
│   ├── Warrior_01 → BasicWarrior.asset
│   ├── Warrior_02 → EliteWarrior.asset
│   └── Boss_Warrior → BossWarrior.asset
├── Mages/
│   ├── Fire_Mage_01 → FireMage.asset
│   └── Ice_Mage_01 → IceMage.asset
└── Archers/
    ├── Archer_01 → SpeedArcher.asset
    └── Sniper_01 → SniperArcher.asset
```

## ✅ Lợi ích:

1. **Không cần code:** Designer có thể tạo NPC variants
2. **Tái sử dụng:** 1 asset có thể dùng cho nhiều NPCs
3. **Dễ tuning:** Thay đổi stats mà không cần recompile
4. **Organized:** Tất cả data ở 1 chỗ, dễ quản lý
5. **Runtime flexibility:** Có thể thay đổi data trong game

## 🔧 Advanced Tips:

### Runtime thay đổi data:
```csharp
// Trong script game manager
public CharacterData bossData;
NPCBaseController npc = FindNPC("Boss");
npc.SetCharacterData(bossData); // Thay đổi data runtime
```

### Tạo random variants:
```csharp
public CharacterData[] warriorVariants;
CharacterData randomData = warriorVariants[Random.Range(0, warriorVariants.Length)];
npc.SetCharacterData(randomData);
```

## ❗ Lưu ý quan trọng:

1. **Asset phải được tạo từ menu Create:** Không thể tạo bằng cách copy file
2. **Drag & drop vào đúng field:** Field `Character Data` trong Inspector
3. **Auto-apply chỉ hoạt động khi Start():** Nếu cần áp dụng manual, dùng `SetCharacterData()`
4. **Backup assets:** CharacterData assets nên được version control

## 🐛 Troubleshooting:

**Không thấy menu Create:**
- Kiểm tra `CharacterData.cs` có attribute `[CreateAssetMenu]`
- Refresh Unity Project (Ctrl+R)

**Stats không áp dụng:**
- Kiểm tra Console logs xem có lỗi gì
- Verify field `Character Data` đã được assign
- Kiểm tra `InitializeCharacter()` có được gọi

**Effect không hiển thị:**
- Đảm bảo prefab effects hợp lệ
- Kiểm tra layer và culling mask

---

🎉 **Hoàn tất!** Bây giờ bạn có thể tạo vô số NPCs với stats khác nhau mà không cần viết code!
