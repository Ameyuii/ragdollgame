# 🎬 CHARACTER MANAGEMENT TOOL - VIDEO TUTORIAL SCRIPT

## 📹 VIDEO 1: GIỚI THIỆU & SETUP (5 phút)

### Scene 1: Intro (30s)
**[Screen: Unity Editor với project trống]**
- "Chào mọi người! Hôm nay mình sẽ hướng dẫn sử dụng Character Management Tool"
- "Tool này giúp bạn quản lý nhân vật game một cách chuyên nghiệp"
- "Không cần code, hoạt động 100% trong Editor mode"

### Scene 2: Vấn đề thường gặp (1 phút)
**[Screen: Hierarchy với nhiều prefabs lộn xộn]**
- "Bạn có gặp những vấn đề này không?"
- "Quên thêm script vào nhân vật → nhân vật không hoạt động"
- "Setup từng nhân vật một → mất thời gian"
- "Không nhớ hết components cần thiết"
- "Khó quản lý khi có nhiều nhân vật"

### Scene 3: Giải pháp (30s)
**[Screen: CharacterManager Inspector]**
- "Character Management Tool giải quyết tất cả!"
- "Quản lý trực quan trong Inspector"
- "Auto-setup components"
- "Template system cho các loại nhân vật"
- "Batch operations"

### Scene 4: Setup đầu tiên (3 phút)
**[Screen: Thực hành setup]**

**Bước 1: Thêm Manager (1 phút)**
- "Đầu tiên, chúng ta thêm Character Manager"
- Right-click GameObject → Character Management → Add Character Manager
- "Tool tự động tìm và gán UI references"
- "Rất đơn giản!"

**Bước 2: Khởi tạo Categories (1 phút)**
- "Tiếp theo, khởi tạo categories mặc định"
- Click "Initialize Default Categories"
- "Chúng ta có 4 categories: Chiến binh, Robot, Quái vật, Zombie"
- "Mỗi category có màu sắc riêng để dễ phân biệt"

**Bước 3: Kiểm tra kết quả (1 phút)**
- "Xem Inspector, chúng ta đã có categories"
- "Auto Refresh UI đã bật"
- "Hệ thống sẵn sàng sử dụng!"

---

## 📹 VIDEO 2: THÊM NHÂN VẬT CƠ BẢN (7 phút)

### Scene 1: Thêm nhân vật đơn lẻ (3 phút)
**[Screen: CharacterManager Inspector]**

**Method 1: Basic Add (1.5 phút)**
- "Cách đầu tiên: Basic Add"
- Expand "Add New Character"
- Drag prefab vào "Character Prefab"
- "Tên tự động điền từ prefab"
- Chọn category "Chiến binh"
- Click "Add Character"
- "Xem! Nhân vật đã được thêm vào category"

**Method 2: Advanced Setup (1.5 phút)**
- "Cách thứ hai: Advanced Setup"
- Click "Advanced Setup & Add"
- "Cửa sổ Advanced Setup mở ra"
- "Ở đây chúng ta có thể cấu hình chi tiết"
- "Mình sẽ demo chi tiết trong video sau"

### Scene 2: Batch Add (2 phút)
**[Screen: Project window với nhiều prefabs]**
- "Bây giờ thêm nhiều nhân vật cùng lúc"
- Select 5-6 prefabs trong Project
- "Quay lại CharacterManager Inspector"
- "Thấy 'Selected Objects: 6'"
- Click "Add All Selected Objects"
- "Boom! Tất cả đã được thêm vào category"

### Scene 3: Quản lý trong Inspector (2 phút)
**[Screen: CharacterManager Inspector với characters]**
- "Xem cách quản lý characters"
- Expand category "Chiến binh"
- "Thấy tất cả characters với icon và thông tin"
- Click "Edit" một character
- "Cửa sổ edit mở ra, có thể chỉnh sửa stats"
- "Health, Speed, Attack Damage, Attack Range"
- "Rất trực quan và dễ sử dụng"

---

## 📹 VIDEO 3: ADVANCED CHARACTER SETUP (10 phút)

### Scene 1: Mở Advanced Setup (1 phút)
**[Screen: Tools menu]**
- "Advanced Character Setup là tính năng mạnh nhất"
- Tools → Advanced Character Setup
- "Cửa sổ setup chi tiết mở ra"
- "Ở đây chúng ta có thể setup nhân vật hoàn chỉnh"

### Scene 2: Target Selection (1 phút)
**[Screen: Advanced Setup window]**
- "Đầu tiên chọn target"
- Drag prefab vào "Target Prefab"
- "Tên tự động điền"
- "Character Manager reference tự động tìm"
- Click "Auto-Detect Missing Components"
- "Tool tự động phát hiện components cần thêm"

### Scene 3: Components Selection (3 phút)
**[Screen: Components section]**

**Core Components (1.5 phút)**
- "Phần Core Components"
- "RagdollCharacter: Script chính điều khiển nhân vật"
- "NavMeshAgent: AI navigation"
- "Animator: Animation controller"
- "Rigidbody: Physics simulation"
- "CapsuleCollider: Collision detection"
- "AudioSource: Sound effects"

**AI Components (1.5 phút)**
- "Phần AI Components"
- "Character AI: Trí tuệ nhân tạo"
- "Health System: Hệ thống máu"
- "Weapon System: Hệ thống vũ khí"
- "Tick vào những gì cần thiết"

### Scene 4: Animation & Physics Setup (2 phút)
**[Screen: Animation và Physics sections]**
- "Animation Setup"
- Drag Animator Controller
- "Avatar cho humanoid characters"
- "Physics Setup"
- "Mass: Khối lượng"
- "Use Gravity, Is Kinematic"
- "Physic Material cho collision"

### Scene 5: Stats & Setup (2 phút)
**[Screen: Stats section và Setup button]**
- "Character Stats"
- "Health: 100, Speed: 5, Attack Damage: 20"
- "Có thể điều chỉnh theo gameplay"
- Click "Setup Character with All Components"
- "Tool bắt đầu setup..."
- "Xem Setup Log: Components được thêm từng cái"
- "Prefab được save tự động"
- "Character được thêm vào Manager"
- "Hoàn thành!"

### Scene 6: Batch Setup (1 phút)
**[Screen: Multiple selection]**
- "Setup nhiều characters cùng lúc"
- Select nhiều prefabs
- Click "Setup Multiple Selected"
- "Tất cả được setup với cùng settings"
- "Rất tiết kiệm thời gian!"

---

## 📹 VIDEO 4: QUICK TEMPLATE SYSTEM (6 phút)

### Scene 1: Giới thiệu Templates (1 phút)
**[Screen: Template window]**
- "Quick Template System cho setup nhanh"
- Tools → Character Component Template
- "5 templates có sẵn cho các loại nhân vật khác nhau"
- "Mỗi template có combo components phù hợp"

### Scene 2: 5 Templates chi tiết (3 phút)
**[Screen: Template selection]**

**Basic Character (30s)**
- Select "Basic Character"
- "RagdollCharacter + Rigidbody + CapsuleCollider"
- "Dùng cho prototype nhanh"

**AI Character (30s)**
- Select "AI Character"
- "Thêm NavMeshAgent + Animator"
- "Dùng cho bot, NPC, enemy AI"

**Player Character (30s)**
- Select "Player Character"
- "Thêm AudioSource, bỏ NavMeshAgent"
- "Dùng cho nhân vật người chơi"

**Combat Character (30s)**
- Select "Combat Character"
- "Full components: All-in-one"
- "Dùng cho nhân vật chiến đấu hoàn chỉnh"

**Vehicle Character (30s)**
- Select "Vehicle Character"
- "BoxCollider thay vì CapsuleCollider"
- "Dùng cho xe tăng, robot lớn"

### Scene 3: Apply Template (1 phút)
**[Screen: Applying template]**
- Select một GameObject (Cube)
- Chọn "AI Character" template
- Click "Apply Template"
- "Xem Setup Log: Components được thêm"
- "Cube giờ đã thành AI character!"

### Scene 4: Batch Template (1 phút)
**[Screen: Multiple objects]**
- Select 10 GameObjects
- Chọn "Combat Character" template
- Click "Apply Template to All Selected"
- "10 objects được setup cùng lúc!"
- "Rất mạnh mẽ cho batch processing"

---

## 📹 VIDEO 5: TIPS & TRICKS (8 phút)

### Scene 1: Menu Shortcuts (2 phút)
**[Screen: Various menus]**
- "Các shortcuts hữu ích"
- Right-click GameObject → Character Management
- Right-click Assets → Character Management
- Tools → Character Manager menu
- "Context menu: Right-click component"

### Scene 2: Workflow Tips (3 phút)
**[Screen: Practical workflow]**

**Tip 1: Prototype Workflow (1 phút)**
- "Tạo Cube/Capsule"
- "Apply Basic Character template"
- "Test gameplay ngay lập tức"

**Tip 2: Production Workflow (1 phút)**
- "Import models"
- "Use Advanced Setup cho detail"
- "Batch operations cho efficiency"

**Tip 3: Organization (1 phút)**
- "Đặt tên categories rõ ràng"
- "Sử dụng màu sắc"
- "Group theo gameplay role"

### Scene 3: Troubleshooting (2 phút)
**[Screen: Common issues]**

**UI không cập nhật (30s)**
- "Click Refresh UI"
- "Check Auto Refresh UI setting"
- "Tools → Force Refresh All"

**Performance chậm (30s)**
- "Tắt Auto Refresh UI"
- "Sử dụng manual refresh"
- "Chia nhỏ categories"

**Components không tìm thấy (1 phút)**
- "Check script tồn tại"
- "Check compilation errors"
- "Xem Setup Log để debug"

### Scene 4: Advanced Tips (1 phút)
**[Screen: Advanced usage]**
- "Programmatic access"
- "Custom templates"
- "Integration với systems khác"
- "Backup trước khi batch operations"

---

## 📹 VIDEO 6: REAL PROJECT DEMO (12 phút)

### Scene 1: Project Setup (2 phút)
**[Screen: Empty project]**
- "Demo với project thực tế"
- "Import asset pack với 20 character models"
- "Setup Character Manager"
- "Initialize categories"

### Scene 2: Batch Import & Setup (4 phút)
**[Screen: Mass import]**
- "Select tất cả 20 models"
- "Phân loại: 5 soldiers, 5 robots, 5 monsters, 5 zombies"
- "Sử dụng templates khác nhau cho từng loại"
- "Soldiers: AI Character template"
- "Robots: Combat Character template"
- "Monsters: Basic Character template"
- "Zombies: AI Character template"
- "Batch apply templates"
- "Add all to Character Manager"

### Scene 3: Customization (3 phút)
**[Screen: Character customization]**
- "Customize stats cho từng loại"
- "Soldiers: Health 100, Speed 5"
- "Robots: Health 150, Speed 4"
- "Monsters: Health 80, Speed 6"
- "Zombies: Health 60, Speed 3"
- "Set team colors"
- "Add descriptions"

### Scene 4: Testing & Validation (2 phút)
**[Screen: Testing]**
- "Validate Character Data"
- "Print Statistics"
- "Test in game"
- "Characters hoạt động perfect!"

### Scene 5: Final Result (1 phút)
**[Screen: Final project]**
- "20 characters setup trong 10 phút"
- "Tất cả có đầy đủ components"
- "Organized trong categories"
- "Ready for gameplay!"

---

## 📹 VIDEO 7: Q&A & WRAP UP (5 phút)

### Scene 1: Common Questions (3 phút)
**[Screen: FAQ]**

**Q: Tool có hoạt động với multiplayer không?**
- "Có! Characters data có thể sync qua network"

**Q: Có thể tạo custom templates không?**
- "Có! Modify CharacterComponentTemplate.cs"

**Q: Performance với 100+ characters?**
- "Tắt Auto Refresh, sử dụng manual refresh"

**Q: Backup và restore?**
- "Data lưu trong scene, backup scene file"

### Scene 2: Best Practices Summary (1 phút)
**[Screen: Best practices list]**
- "Sử dụng templates cho consistency"
- "Batch operations cho efficiency"
- "Validate data định kỳ"
- "Backup trước major changes"
- "Organization với categories"

### Scene 3: Wrap Up (1 phút)
**[Screen: Tool overview]**
- "Character Management Tool giúp:"
- "✅ Tiết kiệm 80% thời gian setup"
- "✅ Đảm bảo consistency"
- "✅ Tránh errors và bugs"
- "✅ Scale với hundreds of characters"
- "Thanks for watching!"
- "Like và subscribe để support!"

---

## 🎬 PRODUCTION NOTES

### Camera Angles
- **Wide shot**: Toàn bộ Unity Editor
- **Medium shot**: Inspector panel
- **Close-up**: Specific buttons/fields
- **Screen recording**: 1920x1080, 60fps

### Audio
- **Voice over**: Clear, moderate pace
- **Background music**: Soft, non-distracting
- **Sound effects**: Click sounds, success chimes

### Graphics
- **Arrows**: Point to important UI elements
- **Highlights**: Yellow boxes around key areas
- **Text overlays**: Key points and tips
- **Transitions**: Smooth cuts between scenes

### Timing
- **Total series**: ~53 minutes
- **Individual videos**: 5-12 minutes each
- **Pace**: Allow time for viewers to follow
- **Pauses**: After each major step

### Call-to-Actions
- **Like** if helpful
- **Subscribe** for more Unity tutorials
- **Comment** with questions
- **Share** with fellow developers

---

*Video Tutorial Script v1.0 - Character Management Tool*