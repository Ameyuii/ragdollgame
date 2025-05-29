HƯỚNG DẪN THIẾT LẬP ANIMATION CHO NPC
===============================

1. THIẾT LẬP ANIMATOR CONTROLLER
-------------------------------
A. Sử dụng Animator Controller có sẵn:
   - Trong project, bạn đã có sẵn "CharacterAnimationController.controller" trong thư mục "Assets/Animation"
   - Hoặc sử dụng "StarterAssetsThirdPerson.controller" từ thư mục "Assets/Military/StarterAssets/ThirdPersonController/Character/Animations"

B. Tạo Animator Controller mới (nếu cần):
   1. Trong Project window, nhấp chuột phải > Create > Animator Controller
   2. Đặt tên là "NPCAnimator"
   3. Mở Controller bằng cách double-click vào nó

2. THIẾT LẬP CÁC THAM SỐ (PARAMETERS)
------------------------------------
Mở Animator Controller và thêm các parameters sau:
   - IsWalking (Boolean): Điều khiển trạng thái đi bộ
   - IsRunning (Boolean): Điều khiển trạng thái chạy (tùy chọn)
   - Attack (Trigger): Kích hoạt animation tấn công
   - Hit (Trigger): Kích hoạt animation bị đánh
   - Die (Trigger): Kích hoạt animation chết

3. CÁC TRẠNG THÁI (STATES) CẦN THIẾT
-----------------------------------
A. Idle State:
   - Thêm animation "Stand--Idle.anim.fbx" từ thư mục "Assets/Military/StarterAssets/ThirdPersonController/Character/Animations"
   - Đặt làm trạng thái mặc định

B. Walking State:
   - Thêm animation "Locomotion--Walk_N.anim.fbx"
   - Tạo transition từ Idle sang Walking với điều kiện IsWalking = true
   - Tạo transition từ Walking sang Idle với điều kiện IsWalking = false
   
C. Attack State:
   - Thêm animation tấn công (nếu có) hoặc sử dụng animation khác thay thế tạm thời
   - Tạo transition từ bất kỳ trạng thái nào sang Attack với điều kiện Attack được kích hoạt
   - Tạo transition từ Attack trở về Idle sau khi animation hoàn tất
   
D. Hit State:
   - Thêm animation bị đánh (nếu có)
   - Tạo transition khi Hit được kích hoạt
   
E. Die State:
   - Thêm animation chết (nếu có)
   - Tạo transition khi Die được kích hoạt
   - Không cần tạo transition ra khỏi trạng thái này

4. GẮN ANIMATOR VÀO NPC
----------------------
A. Chọn GameObject NPC trong scene
B. Trong Inspector:
   1. Nếu chưa có, thêm component Animator (Add Component > Miscellaneous > Animator)
   2. Kéo và thả Controller vào trường "Controller" của component Animator
   3. Đảm bảo Avatar được thiết lập đúng (nếu sử dụng nhân vật humanoid)
   4. Đánh dấu "Apply Root Motion" nếu animation cần kiểm soát vị trí gốc của nhân vật

5. THIẾT LẬP COLLIDER VÀ NAVMESHAGENT
------------------------------------
A. Đảm bảo mỗi NPC có:
   - Collider (có thể là Capsule Collider) để phát hiện va chạm
   - NavMeshAgent để di chuyển trên NavMesh
   - Script NPCController đã được gắn

6. KIỂM TRA VÀ DEBUG
------------------
A. Đảm bảo các layer mask trong NPCController được thiết lập đúng:
   - enemyLayerMask: Layer của các NPC khác
   - obstacleLayerMask: Layer của các chướng ngại vật

B. Chạy game và kiểm tra:
   - Animation idle có hoạt động khi nhân vật đứng yên?
   - Animation đi bộ có hoạt động khi nhân vật di chuyển?
   - Animation tấn công có kích hoạt khi trong tầm đánh?

C. Debug Animation:
   - Trong Play mode, chọn một GameObject NPC
   - Trong Inspector, xem tab "Animator"
   - Theo dõi các tham số và trạng thái animation hiện tại

7. MẸO BỔ SUNG
------------
A. Điều chỉnh tốc độ animation:
   - Trong Animator Controller, chọn trạng thái và điều chỉnh "Speed" trong Inspector
   
B. Blending giữa các animation:
   - Điều chỉnh "Transition Duration" và "Has Exit Time" trong các transitions

C. Sử dụng Animation Events:
   - Mở animation clip và thêm các events để gọi hàm đặc biệt tại thời điểm nhất định
   - Ví dụ: Thêm event để phát âm thanh hoặc hiệu ứng tại thời điểm nhất định của animation tấn công

LƯU Ý QUAN TRỌNG
---------------
* Nếu các animation không được tương thích với model NPC của bạn, bạn cần điều chỉnh retargeting để chúng khớp với hệ thống xương của nhân vật.
* Đảm bảo rằng script NPCController.cs luôn tham chiếu đến các tham số animation đúng tên (IsWalking, Attack, Hit, Die) như đã thiết lập trong Animator.