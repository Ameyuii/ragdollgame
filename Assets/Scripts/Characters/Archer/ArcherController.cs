using UnityEngine;

namespace NPCSystem.Characters.Archer
{
    /// <summary>
    /// Controller cho NPC Archer - chuyên về tấn công tầm xa với cung tên
    /// </summary>
    public class ArcherController : NPCBaseController
    {
        [Header("Archer Specific Settings")]
        [SerializeField] private float shootRange = 15f;
        [SerializeField] private Transform bowTransform; // Vị trí cung
        [SerializeField] private Transform arrowSpawnPoint; // Điểm spawn arrow
        [SerializeField] private GameObject arrowPrefab; // Prefab mũi tên
        [SerializeField] private int maxArrows = 30; // Số mũi tên tối đa
        
        private int currentArrows;
          protected override void Start()
        {
            base.Start();
            // Khởi tạo số mũi tên
            currentArrows = maxArrows;
            
            // Set attack range cho Archer (tầm xa nhất)
            shootRange = attackRange; // Sử dụng attackRange từ NPCBaseController
        }
            protected void RotateTowardsTarget()
        {
            if (targetEnemy == null) return;
            
            // Archer cần quay chính xác để bắn trúng
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            direction.y = 0; // Chỉ quay theo trục Y
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4f);
            }
        }
        
        protected bool IsInAttackRange(Transform target)
        {
            if (target == null || currentArrows <= 0) return false;
            
            float distance = Vector3.Distance(transform.position, target.position);
            return distance <= shootRange;
        }
          /// <summary>
        /// Bắn mũi tên
        /// </summary>
        public void ShootArrow()
        {
            if (currentArrows <= 0 || arrowPrefab == null || arrowSpawnPoint == null) return;
            
            // Tạo mũi tên
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            
            // Hướng mũi tên về phía target
            if (targetEnemy != null)
            {
                Vector3 direction = (targetEnemy.transform.position - arrowSpawnPoint.position).normalized;
                arrow.transform.LookAt(targetEnemy.transform.position);
                
                // Thêm velocity cho arrow (nếu có Rigidbody)
                Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
                if (arrowRb != null)
                {
                    arrowRb.linearVelocity = direction * 20f; // Tốc độ mũi tên
                }
            }
            
            // Giảm số mũi tên
            currentArrows--;
            Debug.Log($"Archer bắn mũi tên! Còn lại: {currentArrows}");
        }
        
        /// <summary>
        /// Animation Event - được gọi khi bắn mũi tên
        /// </summary>
        public void OnArrowShot()
        {
            // Được gọi từ animation event
            ShootArrow();
        }
        
        /// <summary>
        /// Nạp thêm mũi tên
        /// </summary>
        public void ReloadArrows(int amount)
        {
            currentArrows = Mathf.Min(currentArrows + amount, maxArrows);
            Debug.Log($"Archer nạp thêm {amount} mũi tên. Tổng: {currentArrows}");
        }
          /// <summary>
        /// Kiểm tra có thể bắn không
        /// </summary>
        public bool CanShoot()
        {
            return targetEnemy != null && IsInAttackRange(targetEnemy.transform) && currentArrows > 0;
        }
        
        /// <summary>
        /// Get số mũi tên hiện tại
        /// </summary>
        public int GetCurrentArrows()
        {
            return currentArrows;
        }
        
        /// <summary>
        /// Tính khoảng cách đến target
        /// </summary>
        public float GetDistanceToTarget()
        {
            if (targetEnemy == null) return float.MaxValue;
            return Vector3.Distance(transform.position, targetEnemy.transform.position);
        }
          public override void TakeDamage(float damage, NPCBaseController? attacker = null)
        {
            if (attacker != null)
            {
                base.TakeDamage(damage, attacker);
            }
            
            // Archer thường tránh combat tầm gần
            if (currentHealth > 0 && targetEnemy != null)
            {
                float distance = GetDistanceToTarget();
                if (distance < 3f) // Quá gần
                {
                    // Có thể thêm logic tránh né hoặc step back
                    Debug.Log($"Archer {gameObject.name} bị tấn công gần! Cần tránh né!");
                }
            }
        }
        protected override void Update()
        {
            base.Update();
            
            // Auto reload khi hết mũi tên và không có target
            if (currentArrows <= 0 && targetEnemy == null)
            {
                // Có thể thêm animation reload
                ReloadArrows(maxArrows);
            }
        }
        
        /// <summary>
        /// Animation Event methods cho Archer
        /// </summary>
        public override void OnFootstep()
        {
            Debug.Log($"🦶 {gameObject.name}: Archer footstep");
        }
          public override void OnAttackHit()
        {
            Debug.Log($"🏹 {gameObject.name}: Archer arrow hit!");
            
            // Logic damage target
            if (targetEnemy != null && Vector3.Distance(transform.position, targetEnemy.transform.position) <= shootRange)
            {
                targetEnemy.TakeDamage(attackDamage, this);
                Debug.Log($"🎯 {gameObject.name}: Archer gây {attackDamage} damage cho {targetEnemy.gameObject.name}!");
            }
        }
    }
}
