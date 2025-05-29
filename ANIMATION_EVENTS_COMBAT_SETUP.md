# ⚔️ HƯỚNG DẪN SETUP ANIMATION EVENTS CHO COMBAT SYSTEM

## 🎯 Vấn đề đã sửa:
- **Trước**: Damage được gây theo thời gian cooldown (liên tục khi đứng gần)
- **Sau**: Damage chỉ được gây khi animation có hit frame thực sự

## 🔧 Thay đổi trong Code:

### 1. **Thêm biến mới:**
```csharp
private NPCController? currentAttackTarget; // Target hiện tại đang bị tấn công
```

### 2. **Sửa method Attack():**
- Chỉ trigger animation, KHÔNG gây damage ngay
- Lưu target vào `currentAttackTarget`
- Damage sẽ được gây trong `OnAttackHit()`

### 3. **Thêm method mới:**
- `OnAttackHit()`: Được gọi từ Animation Event
- `DealDamageToTarget()`: Gây damage thực sự với validation

## 🎬 SETUP ANIMATION EVENTS (QUAN TRỌNG):

### Bước 1: Mở Animation Window
1. Chọn NPC GameObject trong scene
2. Window > Animation > Animation
3. Chọn attack animation clip

### Bước 2: Thêm Animation Event
1. Tìm frame mà vũ khí chạm target (thường là frame 60-80% của animation)
2. Click chuột phải trên timeline → **Add Animation Event**
3. Trong Inspector của Animation Event:
   - **Function**: `OnAttackHit`
   - **Không cần parameter**

### Bước 3: Test
1. Chạy game
2. Kiểm tra Console logs:
   - `🎯 [NPC] bắt đầu animation tấn công [Target]` - Khi trigger
   - `⚔️ [NPC] gây [damage] sát thương cho [Target]!` - Khi hit frame

## 📋 KIỂM TRA HOẠT ĐỘNG:

### ✅ Đúng:
- Damage chỉ xảy ra 1 lần mỗi animation
- Damage xảy ra đúng timing với hit frame
- Có validation target vẫn trong tầm

### ❌ Sai:
- Damage liên tục khi đứng gần
- Damage ngay khi animation bắt đầu
- Damage khi target đã ra khỏi tầm

## 🐛 TROUBLESHOOTING:

### Nếu vẫn damage liên tục:
1. Kiểm tra Animation Event đã được thêm chưa
2. Kiểm tra Function name: `OnAttackHit` (đúng case)
3. Kiểm tra animation có loop không

### Nếu không damage:
1. Kiểm tra có Animation Event chưa
2. Kiểm tra Console có log "🎯 bắt đầu animation" không
3. Kiểm tra Console có log "⚔️ gây sát thương" không

## 📂 Files liên quan:
- `NPCController.cs` - Logic combat đã được sửa
- Animation clips của attack - Cần thêm Animation Events

---
**Lưu ý**: Animation Events là cách chính xác nhất để đồng bộ damage với animation timing!
