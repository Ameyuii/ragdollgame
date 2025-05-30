# HƯỚNG DẪN SỬ DỤNG HỆ THỐNG NPC MỚI

## TÓM TẮT
Đã tạo xong 2 script mới với toàn bộ chức năng từ NPCController cũ:
- `NPCBaseController_New.cs` - Base class chứa tất cả logic cơ bản
- `WarriorController_New.cs` - Warrior implementation kế thừa từ base class

## CÁC FILE ĐÃ TẠO

### 1. NPCBaseController_New.cs
**Vị trí:** `Assets/Scripts/Core/NPCBaseController_New.cs`

**Chức năng chính:**
- ✅ Hệ thống máu và team giống NPCController cũ
- ✅ AI tìm kiếm và tấn công kẻ địch
- ✅ Hệ thống di chuyển với NavMeshAgent 
- ✅ 3 loại attack (Attack, Attack1, Attack2) với random selection
- ✅ Attack cooldown system với variable timing
- ✅ Ragdoll integration khi máu thấp/chết
- ✅ Physics impact và knockback
- ✅ Patrol system khi không có mục tiêu
- ✅ Smooth animation transitions
- ✅ Debug logging system
- ✅ Line of sight checking
- ✅ Gizmos hiển thị phạm vi trong Scene View

### 2. WarriorController_New.cs
**Vị trí:** `Assets/Scripts/Characters/Warrior/WarriorController_New.cs`

**Chức năng chính:**
- ✅ Kế thừa toàn bộ từ NPCBaseController_New
- ✅ Override các animation events: OnFootstep(), OnAttackHit()
- ✅ Custom debug methods cho Warrior
- ✅ Test methods riêng

### 3. NPCSystemTester.cs
**Vị trí:** `Assets/Scripts/Testing/NPCSystemTester.cs`

**Chức năng chính:**
- ✅ Test script để kiểm tra chức năng NPCs
- ✅ GUI runtime để test các functions
- ✅ Auto-find Warrior NPCs trong scene

## HƯỚNG DẪN CÀI ĐẶT

### Bước 1: Compile Scripts
1. Mở Unity Editor
2. Chờ scripts compile xong (không có lỗi trong Console)

### Bước 2: Thêm Component vào GameObject
1. Chọn GameObject `Warrok W Kurniawan` trong Hierarchy
2. Trong Inspector, click "Add Component"
3. Tìm và thêm `WarriorController_New`
4. **LƯU Ý:** Có thể giữ lại component cũ để so sánh hoặc xóa đi

### Bước 3: Cấu hình Thông số
**Thiết lập cơ bản:**
- Max Health: 100
- Team: 0 (hoặc team khác để test combat)
- Move Speed: 3.5
- Attack Damage: 20
- Attack Range: 2
- Detection Range: 30

**Attack Variations:**
- Basic Attack Chance: 40%
- Attack1 Chance: 30% 
- Attack2 Chance: 30%

**Layer Masks:**
- Enemy Layer Mask: All layers (-1) hoặc cấu hình riêng
- Obstacle Layer Mask: Default layer

### Bước 4: Test Chức năng
1. Thêm `NPCSystemTester` component vào GameObject bất kỳ
2. Assign `testNPC` field = `Warrok W Kurniawan`
3. Play game để thấy GUI test ở góc trên bên trái
4. Sử dụng các button để test:
   - Test Attack
   - Take Damage  
   - Set Health
   - Debug Info

## CHỨC NĂNG CHÍNH

### ✅ Hệ thống Combat
- Tìm kẻ địch trong phạm vi detection
- Di chuyển đến mục tiêu
- Tấn công với 3 loại attack ngẫu nhiên
- Gây damage thực sự với physics impact
- Cooldown system cho mỗi loại attack

### ✅ Hệ thống AI  
- Patrol ngẫu nhiên khi không có mục tiêu
- Line of sight checking
- Obstacle avoidance
- Smooth movement transitions

### ✅ Hệ thống Animation
- IsWalking parameter cho di chuyển
- Attack, Attack1, Attack2 triggers
- Hit và Die animations
- OnFootstep animation events

### ✅ Ragdoll Integration
- Tự động kích hoạt khi máu < 30%
- Kích hoạt khi chết
- Physics impact khi bị tấn công

## TEST VÀ DEBUG

### Context Menu Actions
Click chuột phải vào component trong Inspector:
- "Debug Attack System Info" - Hiển thị thông tin attack system
- "Test Warrior Attack" - Test attack function
- "Debug Warrior Info" - Hiển thị thông tin tổng quan

### Debug Logs
Set `Show Debug Logs = true` để xem:
- AI decision making
- Attack system logs  
- Movement transitions
- Combat events

### Runtime GUI Testing
Khi có NPCSystemTester component:
- GUI hiển thị ở góc trên bên trái
- Real-time health/status display
- Quick action buttons

## SO SÁNH VỚI HỆ THỐNG CŨ

| Tính năng | NPCController (cũ) | NPCBaseController_New | 
|-----------|-------------------|----------------------|
| Combat System | ✅ | ✅ Sao chép y hệt |
| 3 Attack Types | ✅ | ✅ Sao chép y hệt |
| AI Behavior | ✅ | ✅ Sao chép y hệt |
| Ragdoll Integration | ✅ | ✅ Sao chép y hệt |
| Debug System | ✅ | ✅ Sao chép y hệt |
| Modular Design | ❌ | ✅ Base class + inheritance |
| Test Tools | ❌ | ✅ NPCSystemTester |

## LƯU Ý QUAN TRỌNG

1. **Không kết nối với NPCController cũ** - Đúng theo yêu cầu, hệ thống mới hoàn toàn độc lập
2. **Chức năng tương tự y hệt** - Toàn bộ logic từ NPCController đã được sao chép
3. **currentHealth field hiển thị trong Inspector** - Đã thêm [SerializeField]
4. **Debug system hoàn chỉnh** - Có thể monitor mọi hoạt động

## SỬ DỤNG

Sau khi setup xong, NPC sẽ:
1. Tự động tìm kẻ địch cùng scene khác team
2. Di chuyển đến và tấn công với 3 loại attack ngẫu nhiên  
3. Gây damage thực sự và có physics impact
4. Kích hoạt ragdoll khi máu thấp hoặc chết
5. Patrol ngẫu nhiên khi không có mục tiêu

**Hệ thống hoàn toàn hoạt động tương tự NPCController cũ nhưng với kiến trúc modular hơn!**
