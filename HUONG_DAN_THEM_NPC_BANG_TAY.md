# 🎯 HƯỚNG DẪN THÊM NPC BẰNG TAY

## Cách thêm NPC mới vào scene

### 1. Tạo GameObject NPC
- Tạo GameObject mới trong scene
- Đặt tên theo loại NPC (VD: "Warrior_01", "Mage_02", "Archer_03")

### 2. Thêm các Component cần thiết
Thêm lần lượt các component sau:

#### A. Components cơ bản:
- `Animator` (nếu chưa có)
- `NavMeshAgent` (cho AI movement)
- `Rigidbody` 
- `Collider` (Capsule hoặc Box)

#### B. NPC Scripts theo loại:

##### 🗡️ WARRIOR:
1. Thêm script `WarriorController`
2. Gán `CharacterData` trong Inspector
3. Script sẽ tự tạo `WarriorAttackSystem`

##### 🔮 MAGE:
1. Thêm script `MageController`  
2. Gán `CharacterData` trong Inspector
3. Script sẽ tự tạo `MageAttackSystem`

##### 🏹 ARCHER:
1. Thêm script `ArcherController`
2. Gán `CharacterData` trong Inspector  
3. Script sẽ tự tạo `ArcherAttackSystem`

### 3. Cấu hình CharacterData
Tạo CharacterData asset:
- Right-click trong Project → Create → NPC System → Character Data
- Điều chỉnh stats: Health, Damage, Speed, Range...
- Gán vào NPC Controller

### 4. Cấu hình Animator
- Gán Animator Controller có các trigger cần thiết
- Đảm bảo có các animation: Idle, Walk, Attack, Hit, Death

### 5. Cấu hình NavMeshAgent
- Radius: 0.5
- Height: 2.0  
- Speed: 3.5
- Stopping Distance: 1.5

## ✅ Checklist sau khi thêm NPC:

- [ ] NPC có Animator với Controller
- [ ] NPC có NavMeshAgent được cấu hình
- [ ] NPC có CharacterData được gán
- [ ] NPC có Collider và Rigidbody
- [ ] Test NPC hoạt động trong Play mode

## 🚀 Lợi ích của cách thủ công:
- ✅ Kiểm soát hoàn toàn từng NPC
- ✅ Dễ debug và customize
- ✅ Không cần migration tool phức tạp
- ✅ Hiểu rõ architecture của system

## 💡 Tips:
- Đặt tên NPC có ý nghĩa: "Guard_Warrior_01", "Boss_Mage", etc.
- Group NPCs trong Empty GameObjects theo loại
- Sử dụng Prefabs để tái sử dụng NPC setup
