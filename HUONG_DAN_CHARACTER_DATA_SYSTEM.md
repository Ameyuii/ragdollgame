# HƯỚNG DẪN HỆ THỐNG CHARACTER DATA MỚI

## Tổng quan
Hệ thống CharacterData mới cho phép bạn tạo nhiều loại nhân vật khác nhau một cách dễ dàng mà không cần viết code. Bạn chỉ cần tạo các file ScriptableObject và gán vào GameObject.

## Cách tạo nhân vật mới

### Bước 1: Tạo CharacterData ScriptableObject
1. **Chuột phải trong Project Window** → `Create` → `NPC System` → `Character Data`
2. **Đặt tên** cho file (ví dụ: `Warrior_Heavy.asset`, `Mage_Fire.asset`)
3. **Cấu hình stats** trong Inspector:
   - `Character Name`: Tên nhân vật
   - `Max Health`: Máu tối đa  
   - `Move Speed`: Tốc độ di chuyển
   - `Team Id`: ID team (0, 1, 2...)
   - `Base Damage`: Sát thương cơ bản
   - `Attack Range`: Tầm đánh
   - `Attack Cooldown`: Thời gian hồi chiêu
   - `Detection Range`: Tầm phát hiện kẻ địch

### Bước 2: Tạo GameObject nhân vật
1. **Tạo GameObject mới** hoặc sử dụng prefab có sẵn
2. **Thêm component** phù hợp:
   - `WarriorController` cho chiến binh
   - `MageController` cho pháp sư  
   - `ArcherController` cho cung thủ
3. **Kéo thả CharacterData** vào slot `Character Data` trong Inspector

### Bước 3: Thiết lập visual (model, animation)
1. **Gán Model** và Animator vào GameObject
2. **Thêm NavMeshAgent** component
3. **Thiết lập Layer** và Tag nếu cần

## Ví dụ tạo các loại nhân vật

### Warrior Tank (Chiến binh phòng thủ)
```
Character Name: "Heavy Warrior"
Max Health: 200
Move Speed: 2.0
Base Damage: 25
Attack Range: 2.5
Attack Cooldown: 1.5
```

### Warrior DPS (Chiến binh tấn công)
```
Character Name: "Berserker"
Max Health: 120
Move Speed: 4.0
Base Damage: 35
Attack Range: 2.0
Attack Cooldown: 0.8
```

### Fire Mage (Pháp sư lửa)
```
Character Name: "Fire Wizard"
Max Health: 80
Move Speed: 3.0
Base Damage: 40
Attack Range: 6.0
Attack Cooldown: 2.0
```

### Ice Mage (Pháp sư băng)
```
Character Name: "Frost Mage"
Max Health: 90
Move Speed: 2.5
Base Damage: 30
Attack Range: 7.0
Attack Cooldown: 1.8
```

### Sniper Archer (Cung thủ bắn tỉa)
```
Character Name: "Sniper"
Max Health: 100
Move Speed: 3.5
Base Damage: 50
Attack Range: 12.0
Attack Cooldown: 3.0
```

### Rapid Archer (Cung thủ bắn nhanh)
```
Character Name: "Rapid Shooter"
Max Health: 90
Move Speed: 4.0
Base Damage: 20
Attack Range: 8.0
Attack Cooldown: 0.6
```

## Lợi ích của hệ thống mới

### 1. Dễ dàng tạo variant mới
- Không cần viết code cho mỗi loại nhân vật
- Chỉ cần tạo CharacterData asset mới
- Designer có thể tự tạo nhân vật

### 2. Dễ dàng balance game
- Chỉnh stats trực tiếp trong Inspector
- Không cần compile lại code
- Test nhanh các thay đổi

### 3. Tái sử dụng code
- 1 script WarriorController có thể dùng cho nhiều loại warrior
- 1 script MageController có thể dùng cho nhiều loại mage
- Giảm duplicate code

### 4. Dễ dàng manage
- Tất cả stats ở 1 nơi (CharacterData)
- Dễ copy/paste stats giữa các nhân vật
- Dễ backup và version control

## Debug và Test

### Debug thông tin nhân vật
1. **Chọn GameObject** trong Scene
2. **Chuột phải** → Context Menu → `Debug Warrior Info` (hoặc Mage/Archer)
3. **Xem Console** để kiểm tra stats đã được load đúng chưa

### Test tính năng
1. **Đặt 2 nhân vật** khác team trong Scene  
2. **Chạy game** và quan sát họ chiến đấu
3. **Kiểm tra damage** bằng cách xem Console logs

## Mở rộng trong tương lai

### Thêm stats mới
1. **Thêm field** vào `CharacterData.cs`
2. **Update method** `ApplyCharacterData()` trong `NPCBaseController.cs`
3. **Recompile** và cập nhật các asset

### Thêm loại nhân vật mới
1. **Tạo class mới** kế thừa từ `NPCBaseController`
2. **Override** các method cần thiết
3. **Tạo CharacterData** asset cho loại nhân vật mới

---

**🎯 Mục tiêu:** Với hệ thống này, bạn có thể tạo một đội quân gồm hàng chục loại nhân vật khác nhau mà không cần viết thêm code nào!
