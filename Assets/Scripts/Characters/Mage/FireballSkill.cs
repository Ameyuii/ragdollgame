using UnityEngine;
using NPCSystem.Core.Skills;

namespace NPCSystem.Characters.Mage
{
    /// <summary>
    /// Fireball skill cho Mage - bắn fireball về phía target
    /// </summary>
    public class FireballSkill : SkillBase
    {
        [Header("Fireball Skill Settings")]
        [SerializeField] private GameObject fireballPrefab;
        [SerializeField] private Transform castPoint;
        [SerializeField] private float fireballSpeed = 20f;
        [SerializeField] private float explosionRadius = 3f;
        
        protected override void Start()
        {
            base.Start();
            skillName = "Fireball";
            cooldown = 6f;
            range = 12f;
            damage = 40f;
        }
        
        protected override void ExecuteSkill(Transform target)
        {
            if (!IsTargetInRange(target)) return;
            
            Debug.Log($"{owner.name} đang cast Fireball về phía {target.name}!");
            
            // Trigger animation
            if (owner.GetComponent<Animator>() != null)
            {
                owner.GetComponent<Animator>().SetTrigger("fireball");
            }
        }        /// <summary>
        /// Animation Event - tạo fireball khi cast
        /// </summary>
        public void OnFireballCast()
        {
            if (fireballPrefab == null || castPoint == null) return;
            
            // Tìm target gần nhất (vì targetEnemy là protected)
            Transform? currentTarget = FindNearestTarget();
            if (currentTarget == null) return;
            
            // Tạo fireball
            GameObject fireball = Instantiate(fireballPrefab, castPoint.position, castPoint.rotation);
            
            // Hướng fireball về target
            Vector3 direction = (currentTarget.position - castPoint.position).normalized;
            fireball.transform.LookAt(currentTarget.position);
            
            // Thêm movement cho fireball
            Rigidbody fireballRb = fireball.GetComponent<Rigidbody>();
            if (fireballRb != null)
            {
                fireballRb.linearVelocity = direction * fireballSpeed;
            }
            
            // Thêm script xử lý collision cho fireball
            FireballProjectile projectile = fireball.GetComponent<FireballProjectile>();
            if (projectile == null)
            {
                projectile = fireball.AddComponent<FireballProjectile>();
            }
            
            projectile.Initialize(damage, explosionRadius, owner);
        }
          private Transform? FindNearestTarget()
        {
            // Tìm target gần nhất với tag "Player"
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            Transform? nearestTarget = null;
            float minDistance = float.MaxValue;
            
            foreach (GameObject player in players)
            {
                if (player == owner.gameObject) continue; // Bỏ qua chính mình
                
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < minDistance && distance <= range)
                {
                    minDistance = distance;
                    nearestTarget = player.transform;
                }
            }
            
            return nearestTarget;
        }
    }
    
    /// <summary>
    /// Component xử lý fireball projectile
    /// </summary>
    public class FireballProjectile : MonoBehaviour
    {
        private float damage;
        private float explosionRadius;
        private NPCBaseController? caster;
        private bool hasExploded = false;
        
        public void Initialize(float dmg, float radius, NPCBaseController casterRef)
        {
            damage = dmg;
            explosionRadius = radius;
            caster = casterRef;
            
            // Tự hủy sau 5 giây
            Destroy(gameObject, 5f);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (hasExploded) return;
            
            // Explode khi chạm target hoặc obstacle
            if (other.CompareTag("Player") || other.CompareTag("Environment"))
            {
                Explode();
            }
        }
        
        private void Explode()
        {
            if (hasExploded) return;
            hasExploded = true;
            
            Debug.Log($"Fireball nổ tại {transform.position}!");
            
            // Gây damage cho tất cả targets trong radius
            Collider[] hitTargets = Physics.OverlapSphere(transform.position, explosionRadius);            foreach (var collider in hitTargets)
            {
                if (collider.CompareTag("Player"))
                {
                    var targetController = collider.GetComponent<NPCBaseController>();
                    if (targetController != null && targetController != caster && caster != null)
                    {
                        targetController.TakeDamage(damage, caster);
                        Debug.Log($"Fireball gây {damage} damage cho {collider.name}!");
                    }
                }
            }
            
            // TODO: Thêm explosion effect
            
            // Hủy fireball
            Destroy(gameObject);
        }
    }
}
