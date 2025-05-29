# HƯỚNG DẪN HỆ THỐNG RAGDOLL HYBRID

## 📋 Tổng quan
Hệ thống Ragdoll Hybrid cho phép chuyển đổi mượt mà giữa animation và vật lý ragdoll. Hệ thống gồm 3 script chính:

1. **RagdollController** - Core controller quản lý trạng thái ragdoll
2. **RagdollSetupHelper** - Tiện ích tự động tạo ragdoll setup
3. **RagdollDemo** - Demo và test hệ thống

## 🔧 Thiết lập ban đầu

### Bước 1: Chuẩn bị nhân vật
- Nhân vật cần có **Animator** với **Humanoid Avatar**
- Đã có animation setup cơ bản
- Có **Rigidbody** và **Collider** chính

### Bước 2: Tạo Ragdoll tự động
1. Thêm script **RagdollSetupHelper** vào GameObject nhân vật
2. Trong Inspector, điều chỉnh các thông số:
   - **Khối lượng tổng**: 70kg (mặc định)
   - **Tự động tạo Collider**: true
   - **Tự động setup Joint**: true
   - **Vật liệu vật lý**: (tùy chọn)
3. Right-click trên script và chọn **"Tạo Ragdoll Tự Động"**
4. Script sẽ tự động tạo Rigidbody và Collider cho các bộ phận

### Bước 3: Thêm RagdollController
- Script **RagdollController** sẽ được tự động thêm sau khi tạo ragdoll
- Điều chỉnh các thông số trong Inspector:
  - **Thời gian chuyển sang ragdoll**: 0.5s
  - **Thời gian khôi phục animation**: 2s
  - **Lực kích hoạt ragdoll**: 10 units

### Bước 4: Test hệ thống
1. Thêm script **RagdollDemo** để test
2. Gán **RagdollController** vào Demo script
3. Chạy game và test bằng các phím:
   - **R**: Kích hoạt ragdoll
   - **T**: Khôi phục animation
   - **Y**: Chuyển chế độ hybrid
   - **↑↓**: Điều chỉnh blend trong hybrid mode

## 🎮 Sử dụng trong code

### Kích hoạt Ragdoll
```csharp
public class WeaponImpact : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        RagdollController ragdoll = other.GetComponent<RagdollController>();
        if (ragdoll != null)
        {
            Vector3 luc = transform.forward * 20f;
            Vector3 viTri = other.transform.position;
            ragdoll.KichHoatRagdoll(luc, viTri);
        }
    }
}
```

### Khôi phục Animation
```csharp
// Khôi phục sau khi ragdoll
if (ragdollController.DangLaRagdoll)
{
    ragdollController.KhoiPhucAnimation();
}
```

### Chế độ Hybrid
```csharp
// Chuyển sang chế độ hybrid với 50% physics
ragdollController.ChuyenSangCheDoHybrid(0.5f);

// Điều chỉnh tỷ lệ blend (0 = full animation, 1 = full physics)
ragdollController.DieuChinhTyLeBlend(0.7f);
```

## 🔄 Các trạng thái Ragdoll

### Animation Mode
- Chỉ sử dụng animation
- Ragdoll physics bị tắt
- Nhân vật di chuyển theo animation bình thường

### Ragdoll Mode  
- Chỉ sử dụng physics
- Animation bị tắt
- Nhân vật phản ứng theo vật lý

### Hybrid Mode
- Kết hợp animation và physics
- Có thể điều chỉnh tỷ lệ blend
- Cho phép tạo hiệu ứng "procedural animation"

### Transition States
- **ChuyenDoiSangRagdoll**: Đang chuyển từ animation sang ragdoll
- **KhoiPhucAnimation**: Đang khôi phục từ ragdoll về animation

## ⚙️ Tùy chỉnh nâng cao

### Điều chỉnh khối lượng bộ phận
Trong **RagdollSetupHelper**, điều chỉnh các tỷ lệ:
- **Đầu**: 15% tổng khối lượng
- **Thân**: 35% tổng khối lượng  
- **Chân**: 25% tổng khối lượng
- **Tay**: 10% tổng khối lượng

### Thiết lập Joint Limits
Script tự động tạo **CharacterJoint** với giới hạn:
- **Twist**: ±20°
- **Swing**: ±30°
- Có thể tùy chỉnh trong `SetupJoint()` method

### Tùy chỉnh Collider
Mỗi bộ phận có collider phù hợp:
- **Đầu**: SphereCollider
- **Thân**: BoxCollider
- **Tay/Chân**: CapsuleCollider
- **Bàn tay/chân**: BoxCollider nhỏ

## 🚀 Ứng dụng thực tế

### 1. Combat System
```csharp
public class CombatManager : MonoBehaviour
{
    void ApplyDamage(GameObject target, float damage, Vector3 force)
    {
        RagdollController ragdoll = target.GetComponent<RagdollController>();
        if (ragdoll != null && damage > 50f)
        {
            ragdoll.KichHoatRagdoll(force, target.transform.position);
            
            // Tự động khôi phục sau 3 giây
            StartCoroutine(DelayedRecovery(ragdoll, 3f));
        }
    }
    
    IEnumerator DelayedRecovery(RagdollController ragdoll, float delay)
    {
        yield return new WaitForSeconds(delay);
        ragdoll.KhoiPhucAnimation();
    }
}
```

### 2. Environmental Interaction
```csharp
public class ExplosionEffect : MonoBehaviour
{
    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        
        foreach (var col in colliders)
        {
            RagdollController ragdoll = col.GetComponent<RagdollController>();
            if (ragdoll != null)
            {
                Vector3 direction = (col.transform.position - transform.position).normalized;
                Vector3 force = direction * explosionForce;
                ragdoll.KichHoatRagdoll(force, col.transform.position);
            }
        }
    }
}
```

### 3. Character Death System
```csharp
public class HealthSystem : MonoBehaviour
{
    void Die()
    {
        RagdollController ragdoll = GetComponent<RagdollController>();
        if (ragdoll != null)
        {
            ragdoll.KichHoatRagdollNgayLapTuc();
            // Không khôi phục - nhân vật đã chết
        }
    }
}
```

## 🔍 Troubleshooting

### Ragdoll không hoạt động
- Kiểm tra nhân vật có Humanoid Avatar
- Đảm bảo đã chạy "Tạo Ragdoll Tự Động"
- Kiểm tra các Rigidbody đã được tạo

### Animation không mượt
- Tăng thời gian transition
- Kiểm tra Animation Controller settings
- Đảm bảo các animation clip đã được import đúng

### Physics không realistic
- Điều chỉnh khối lượng các bộ phận
- Thêm PhysicMaterial với friction/bounce phù hợp
- Điều chỉnh Joint limits

### Performance issues
- Giảm số lượng Rigidbody nếu không cần thiết
- Sử dụng LOD system cho ragdoll
- Disable ragdoll khi ở xa camera

## 📝 Notes

- Hệ thống tương thích với Unity 2022.3+
- Hỗ trợ Input System mới
- Có thể kết hợp với NavMesh Agent
- Hỗ trợ multiplayer (cần sync states)

## 🔧 Maintenance

### Xóa Ragdoll setup
1. Chọn nhân vật
2. Right-click trên **RagdollSetupHelper**
3. Chọn **"Xóa Ragdoll"**

### Tạo lại Ragdoll
1. Xóa setup cũ
2. Chạy lại **"Tạo Ragdoll Tự Động"**
3. Điều chỉnh lại các thông số

---

**Lưu ý**: Luôn backup scene trước khi thực hiện setup ragdoll tự động!
