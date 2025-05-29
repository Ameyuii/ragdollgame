===== HƯỚNG DẪN SỬ DỤNG NPCController =====

NPCController là script điều khiển nhân vật với các thuộc tính cơ bản như: máu, team, tốc độ di chuyển, tầm đánh, 
tốc độ đánh, và khoảng cách phát hiện kẻ địch. Nhân vật khác team sẽ tấn công nhau.

===== CÁCH SỬ DỤNG =====

1) Thêm component NPCController vào GameObject của nhân vật:
   - Chọn nhân vật trong Scene hoặc Hierarchy
   - Add Component > Scripts > NPCController

2) Thiết lập thông số:
   - maxHealth: Máu tối đa của nhân vật
   - team: Số ID của team (nhân vật cùng team ID sẽ không tấn công nhau)
   - moveSpeed: Tốc độ di chuyển
   - attackDamage: Sát thương gây ra khi tấn công
   - attackCooldown: Thời gian hồi chiêu (giây)
   - attackRange: Tầm đánh (m)
   - detectionRange: Khoảng cách phát hiện kẻ địch (m)

3) Đảm bảo NavMeshAgent được thiết lập:
   - Mỗi nhân vật cần có NavMeshAgent component
   - Bake NavMesh cho scene (Window > AI > Navigation > Bake)

===== TEAM SYSTEM =====

- Nhân vật chỉ tấn công những nhân vật khác team
- Nhân vật cùng team sẽ không tấn công nhau
- Đặt team ID trong Inspector để phân chia các nhân vật

Ví dụ:
- Team 0: Nhân vật của người chơi
- Team 1: Kẻ địch loại 1
- Team 2: Kẻ địch loại 2
- Team 3: NPC trung lập

===== TÌM KIẾM KẺ ĐỊCH =====

Nhân vật sẽ tự động:
1. Tìm kiếm kẻ địch trong phạm vi detectionRange
2. Di chuyển đến kẻ địch gần nhất
3. Tấn công khi đến trong tầm attackRange
4. Không tấn công đồng minh (nhân vật cùng team)

===== DEBUG =====

NPCController thêm các thông báo debug để theo dõi:
- Các thông tin khi Start()
- Trạng thái tìm kiếm kẻ địch
- Tấn công và nhận sát thương
- Kiểm tra đồng minh/kẻ địch dựa trên team

===== VẼ GIZMOS =====

Trong Scene View, NPCController hiển thị:
- Vòng tròn xanh: Phạm vi phát hiện (detectionRange)
- Vòng tròn đỏ: Tầm đánh (attackRange)
- Đường thẳng đỏ: Kết nối đến mục tiêu hiện tại