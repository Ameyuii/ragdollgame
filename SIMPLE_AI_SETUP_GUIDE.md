# 🤖 HƯỚNG DẪN SETUP AI MANUAL ĐỔN GIẢN

## 📋 TỔNG QUAN

Hướng dẫn này giúp bạn setup AI character **HOÀN TOÀN MANUAL** mà không cần dựa vào context menu hay features phức tạp. Mọi thứ đều thực hiện qua Inspector và manual steps.

---

## 🚨 KHẮC PHỤC VẤN ĐỀ CONTEXT MENU

### Vấn đề: Context Menu "Setup AI Character" không xuất hiện

**Nguyên nhân có thể:**
- Unity scripts chưa compile xong
- Có compilation errors trong project
- Unity đang refresh hoặc importing assets

**Giải pháp:**
1. Kiểm tra Console Window (`Window > General > Console`)
2. Đảm bảo không có error messages (đỏ)
3. Chờ Unity compile xong (loading bar biến mất)
4. Nếu vẫn không có context menu → Dùng phương pháp manual bên dưới

---

## 🎯 PHƯƠNG PHÁP 1: SỬ DỤNG SIMPLE AI SETUP SCRIPT

### Bước 1: Add SimpleAISetup Script
1. Chọn character GameObject trong scene
2. Click **Add Component** trong Inspector
3. Gõ "**SimpleAISetup**" và chọn script
4. Script sẽ hiện ra với interface đơn giản

### Bước 2: Cấu hình Settings
Trong Inspector của **SimpleAISetup** component:

```
📋 SIMPLE AI SETUP GUIDE
✅ Show Help: checked

🎯 Team Configuration
- Team Type: AI_Team1 (Blue) hoặc AI_Team2 (Red)  
- Character Name: "AI Character 1" (tùy chỉnh)
- Team Color: Blue/Red (tự động cập nhật)

⚙️ AI Settings  
- Walk Speed: 3
- Run Speed: 6
- Attack Damage: 25
- Attack Range: 2
- Detection Radius: 10
```

### Bước 3: Thực hiện Setup
1. Right-click vào **SimpleAISetup** component title
2. Chọn **"🚀 Setup AI Character"**
3. Xem Console để theo dõi quá trình setup
4. Khi thấy "✅ AI Character setup complete" → hoàn thành!

### Bước 4: Validate Setup
1. Right-click **SimpleAISetup** component  
2. Chọn **"🔍 Validate Setup"**
3. Kiểm tra Console - phải thấy "✅ setup is valid!"

---

## 🛠️ PHƯƠNG PHÁP 2: SETUP HOÀN TOÀN MANUAL

Nếu script không hoạt động, làm theo từng bước manual:

### Bước 1: Setup Unity Components Cơ Bản

#### A. NavMesh Agent
1. Chọn character GameObject
2. **Add Component** > **AI** > **Nav Mesh Agent**
3. Cấu hình:
   - Speed: 3
   - Angular Speed: 120
   - Acceleration: 8
   - Stopping Distance: 1.5
   - Auto Braking: ✅
   - Auto Repath: ✅

#### B. Collider (nếu chưa có)
1. **Add Component** > **Physics** > **Capsule Collider**
2. Cấu hình:
   - Height: 2
   - Radius: 0.5
   - Center: (0, 1, 0)

#### C. Tag Setup
1. Trong Inspector, thay đổi **Tag**:
   - Team 1 (Blue): "BlueTeam"
   - Team 2 (Red): "RedTeam"
   - Hoặc tạo tag mới nếu cần

### Bước 2: Setup AI Components

#### A. Team Member
1. **Add Component** > gõ "**TeamMember**"
2. Trong Inspector:
   - Team Type: AI_Team1 hoặc AI_Team2
   - Team Name: "Blue Team" hoặc "Red Team"  
   - Team Color: Blue hoặc Red
   - Health: 100
   - Max Health: 100

#### B. Enemy Detector  
1. **Add Component** > gõ "**EnemyDetector**"
2. Cấu hình:
   - Detection Radius: 10
   - Detection Angle: 90
   - Debug Mode: ✅

#### C. Combat Controller
1. **Add Component** > gõ "**CombatController**"
2. Cấu hình:
   - Attack Damage: 25
   - Attack Range: 2
   - Attack Cooldown: 1
   - Behavior Type: Aggressive
   - Engage Distance: 8
   - Debug Mode: ✅

#### D. AI Movement Controller
1. **Add Component** > gõ "**AIMovementController**"
2. Cấu hình:
   - Walk Speed: 3
   - Run Speed: 6
   - Rotation Speed: 5
   - Behavior Type: Aggressive
   - Seek Radius: 15
   - Debug Mode: ✅

#### E. AI State Machine
1. **Add Component** > gõ "**AIStateMachine**"
2. Cấu hình:
   - Debug Mode: ✅

### Bước 3: Kiểm Tra Setup
Trong Inspector, character phải có **tất cả** components sau:
- ✅ NavMesh Agent
- ✅ TeamMember  
- ✅ EnemyDetector
- ✅ CombatController
- ✅ AIMovementController
- ✅ AIStateMachine
- ✅ Collider (Capsule/Box/etc.)

---

## 🏗️ PHƯƠNG PHÁP 3: TẠO TEST SCENE NHANH

### Bước 1: Setup Scene Cơ Bản
1. Tạo **Empty GameObject** → đặt tên "**Ground**"
2. **Add Component** > **Mesh Renderer** + **Mesh Filter**
3. Trong **Mesh Filter** → chọn **Cube** mesh
4. Scale Ground: (20, 0.1, 20)
5. Position: (0, 0, 0)

### Bước 2: Bake NavMesh
1. Mở **Window** > **AI** > **Navigation**
2. Chọn **Ground** object
3. Trong **Navigation** window > **Object** tab
4. Check **Navigation Static**
5. Chuyển sang **Bake** tab
6. Click **Bake** button
7. Đợi baking xong (Ground sẽ có màu xanh lá trong Scene view)

### Bước 3: Tạo AI Characters  
1. Tạo **Cube** đầu tiên:
   - Position: (-5, 1, 0)
   - Đặt tên: "**Blue_AI_1**"
   - Setup theo hướng dẫn trên với **Team Type = AI_Team1**

2. Tạo **Cube** thứ hai:
   - Position: (5, 1, 0)  
   - Đặt tên: "**Red_AI_1**"
   - Setup theo hướng dẫn trên với **Team Type = AI_Team2**

### Bước 4: Test
1. **Play** scene
2. Xem Console log - phải thấy:
   ```
   Blue_AI_1 joined team: Blue Team (AI_Team1)
   Red_AI_1 joined team: Red Team (AI_Team2)
   Blue_AI_1 detected enemy: Red_AI_1
   Red_AI_1 detected enemy: Blue_AI_1
   ```
3. Characters sẽ di chuyển và combat với nhau

---

## 🔧 TROUBLESHOOTING

### ❌ Character không di chuyển
**Kiểm tra:**
- NavMesh Agent có enabled không?
- Ground có bake NavMesh chưa? (phải có màu xanh lá)
- Console có error "not on NavMesh" không?

**Giải pháp:**
- Bake lại NavMesh
- Đảm bảo character đứng trên Ground
- Restart Play mode

### ❌ Character không tấn công nhau
**Kiểm tra:**
- Team Type có khác nhau không? (Team1 vs Team2)
- Detection Radius đủ lớn không?
- Console có thấy "detected enemy" messages không?

**Giải pháp:**
- Đặt characters gần nhau hơn (< 10 units)
- Tăng Detection Radius lên 15
- Check Debug Mode = true trong tất cả components

### ❌ Console hiển thị errors
**Các lỗi thường gặp:**

1. **"Missing component"**
   - Add đúng component bị thiếu
   - Restart Play mode

2. **"NavMesh not found"**  
   - Bake NavMesh cho scene
   - Đảm bảo Ground có Navigation Static

3. **"Compilation error"**
   - Mở Console, xem error chi tiết
   - Fix code errors trước khi test

### ❌ Components không tìm thấy
**Nếu không tìm được script khi Add Component:**
- Chờ Unity compile xong
- Check Console có errors không
- Restart Unity nếu cần
- Dùng phương pháp alternative bên dưới

---

## 🚀 ALTERNATIVE: SETUP VỚI EXISTING SCRIPTS

Nếu SimpleAISetup không hoạt động, dùng **AICharacterSetup** (script có sẵn):

1. **Add Component** > "**AICharacterSetup**"
2. Cấu hình settings trong Inspector
3. Right-click component title
4. Chọn **"Setup AI Character"**
5. Nếu không có context menu → dùng Manual method ở trên

---

## 📊 KIỂM TRA KẾT QUẢ

### Console Messages Thành Công:
```
✅ AIMovementController initialized for Blue_AI_1
✅ EnemyDetector initialized for Blue_AI_1 (Team: Blue Team)  
✅ Blue_AI_1 joined team: Blue Team (AI_Team1)
✅ Blue_AI_1 detected enemy: Red_AI_1
✅ Blue_AI_1 AI State: Idle -> Seeking
```

### Scene View:
- Characters có **colored team indicators** (wireframe spheres)
- NavMesh hiển thị màu **xanh lá** trên Ground
- Characters di chuyển smooth trên NavMesh
- Debug lines hiển thị detection radius và targets

### Game View:
- Characters tự động tìm và combat nhau
- Health bars giảm khi bị attack  
- Ragdoll effect khi chết (nếu có)

---

## 🎮 NEXT STEPS

Sau khi setup thành công:

1. **Thêm nhiều characters** bằng cách Duplicate existing
2. **Tạo teams lớn hơn** với nhiều members
3. **Test với các prefabs character** có sẵn
4. **Add animations** cho movement và combat
5. **Customize AI behaviors** qua Inspector settings

---

## 📞 SUPPORT

Nếu vẫn gặp vấn đề:
1. Copy **toàn bộ Console messages** 
2. Screenshot **Inspector của character** đã setup
3. Mô tả **từng bước** đã làm
4. Kiểm tra **Unity version** và **project settings**

**Lưu ý quan trọng:** 
- Setup theo **từng bước một**, đừng skip
- **Validate** sau mỗi bước
- **Check Console** thường xuyên để catch errors sớm
- **Save scene** sau khi setup thành công