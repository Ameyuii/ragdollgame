using UnityEngine;
using AnimalRevolt.Characters;

namespace AnimalRevolt.Ragdoll
{
    /// <summary>
    /// Ragdoll Controller - Điều khiển chuyển đổi giữa animation và ragdoll
    /// </summary>
    public class RagdollController : MonoBehaviour
    {
        [Header("Ragdoll Settings")]
        [SerializeField] private float ragdollActivationForce = 10f;
        [SerializeField] private float recoveryTime = 2f;
        [SerializeField] private bool autoRecover = true;
        
        [Header("Physics")]
        [SerializeField] private float ragdollMass = 80f;
        [SerializeField] private float ragdollDrag = 0.5f;
        [SerializeField] private float ragdollAngularDrag = 5f;
        
        // Components
        private Animator animator;
        private Rigidbody[] ragdollRigidbodies;
        private CharacterJoint[] ragdollJoints;
        private Collider[] ragdollColliders;
        private Rigidbody mainRigidbody;
        private Collider mainCollider;
        
        // State
        private bool isRagdollActive = false;
        private float ragdollStartTime;
        private Vector3 ragdollPosition;
        private Quaternion ragdollRotation;
        
        // Events
        public System.Action OnRagdollActivated;
        public System.Action OnRagdollDeactivated;
        
        private void Awake()
        {
            InitializeComponents();
            SetupRagdoll();
        }
        
        private void Start()
        {
            DeactivateRagdoll();
        }
        
        private void Update()
        {
            if (isRagdollActive && autoRecover)
            {
                CheckForRecovery();
            }
        }
        
        /// <summary>
        /// Initialize components
        /// </summary>
        private void InitializeComponents()
        {
            animator = GetComponent<Animator>();
            mainRigidbody = GetComponent<Rigidbody>();
            mainCollider = GetComponent<Collider>();
            
            // Get all ragdoll components
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            ragdollJoints = GetComponentsInChildren<CharacterJoint>();
            ragdollColliders = GetComponentsInChildren<Collider>();
        }
        
        /// <summary>
        /// Setup ragdoll physics
        /// </summary>
        private void SetupRagdoll()
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb != mainRigidbody)
                {
                    rb.mass = ragdollMass / ragdollRigidbodies.Length;
                    rb.linearDamping = ragdollDrag;
                    rb.angularDamping = ragdollAngularDrag;
                }
            }
        }
        
        /// <summary>
        /// Initialize with character stats
        /// </summary>
        public void Initialize(CharacterStats stats)
        {
            ragdollMass = stats.ragdollMass;
            ragdollDrag = stats.ragdollDrag;
            ragdollAngularDrag = stats.ragdollAngularDrag;
            recoveryTime = stats.recoveryTime;
            
            SetupRagdoll();
            
            Debug.Log($"[RagdollController] Initialized for {gameObject.name}");
        }
        
        /// <summary>
        /// Activate ragdoll physics
        /// </summary>
        public void ActivateRagdoll()
        {
            if (isRagdollActive) return;
            
            isRagdollActive = true;
            ragdollStartTime = Time.time;
            
            // Store current position and rotation
            ragdollPosition = transform.position;
            ragdollRotation = transform.rotation;
            
            // Disable animator
            if (animator != null)
                animator.enabled = false;
            
            // Disable main rigidbody and collider
            if (mainRigidbody != null)
                mainRigidbody.isKinematic = true;
            if (mainCollider != null)
                mainCollider.enabled = false;
            
            // Enable ragdoll rigidbodies and colliders
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb != mainRigidbody)
                {
                    rb.isKinematic = false;
                    rb.detectCollisions = true;
                }
            }
            
            foreach (Collider col in ragdollColliders)
            {
                if (col != mainCollider)
                {
                    col.enabled = true;
                }
            }
            
            OnRagdollActivated?.Invoke();
            
            Debug.Log($"[RagdollController] Ragdoll activated for {gameObject.name}");
        }
        
        /// <summary>
        /// Deactivate ragdoll and return to animation
        /// </summary>
        public void DeactivateRagdoll()
        {
            if (!isRagdollActive) return;
            
            isRagdollActive = false;
            
            // Get the main body position for recovery
            if (ragdollRigidbodies.Length > 0)
            {
                // Find the hip/spine rigidbody (usually the first one or has "hip"/"spine" in name)
                Rigidbody hipRigidbody = FindHipRigidbody();
                if (hipRigidbody != null)
                {
                    transform.position = hipRigidbody.position;
                    transform.rotation = Quaternion.LookRotation(hipRigidbody.transform.forward, Vector3.up);
                }
            }
            
            // Disable ragdoll rigidbodies and colliders
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb != mainRigidbody)
                {
                    rb.isKinematic = true;
                    rb.detectCollisions = false;
                }
            }
            
            foreach (Collider col in ragdollColliders)
            {
                if (col != mainCollider)
                {
                    col.enabled = false;
                }
            }
            
            // Enable main rigidbody and collider
            if (mainRigidbody != null)
                mainRigidbody.isKinematic = false;
            if (mainCollider != null)
                mainCollider.enabled = true;
            
            // Enable animator
            if (animator != null)
                animator.enabled = true;
            
            OnRagdollDeactivated?.Invoke();
            
            Debug.Log($"[RagdollController] Ragdoll deactivated for {gameObject.name}");
        }
        
        /// <summary>
        /// Apply knockback force to ragdoll
        /// </summary>
        public void ApplyKnockback(Vector3 direction, float force)
        {
            if (!isRagdollActive)
            {
                // Auto-activate ragdoll if force is strong enough
                if (force >= ragdollActivationForce)
                {
                    ActivateRagdoll();
                }
                else
                {
                    return;
                }
            }
            
            // Apply force to all ragdoll rigidbodies
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb != mainRigidbody && !rb.isKinematic)
                {
                    rb.AddForce(direction * force, ForceMode.Impulse);
                }
            }
            
            Debug.Log($"[RagdollController] Applied knockback force {force} to {gameObject.name}");
        }
        
        /// <summary>
        /// Apply explosion force to ragdoll
        /// </summary>
        public void ApplyExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius)
        {
            if (!isRagdollActive)
                ActivateRagdoll();
            
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb != mainRigidbody && !rb.isKinematic)
                {
                    rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
                }
            }
        }
        
        /// <summary>
        /// Check if should recover from ragdoll
        /// </summary>
        private void CheckForRecovery()
        {
            if (Time.time >= ragdollStartTime + recoveryTime)
            {
                // Check if ragdoll is stable (low velocity)
                bool isStable = true;
                foreach (Rigidbody rb in ragdollRigidbodies)
                {
                    if (rb != mainRigidbody && rb.linearVelocity.magnitude > 1f)
                    {
                        isStable = false;
                        break;
                    }
                }
                
                if (isStable)
                {
                    DeactivateRagdoll();
                }
            }
        }
        
        /// <summary>
        /// Find the main hip/spine rigidbody for recovery positioning
        /// </summary>
        private Rigidbody FindHipRigidbody()
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb != mainRigidbody)
                {
                    string name = rb.name.ToLower();
                    if (name.Contains("hip") || name.Contains("spine") || name.Contains("pelvis"))
                    {
                        return rb;
                    }
                }
            }
            
            // Fallback to first non-main rigidbody
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                if (rb != mainRigidbody)
                {
                    return rb;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Force immediate recovery
        /// </summary>
        public void ForceRecover()
        {
            if (isRagdollActive)
            {
                DeactivateRagdoll();
            }
        }
        
        /// <summary>
        /// Set auto recovery enabled/disabled
        /// </summary>
        public void SetAutoRecover(bool enabled)
        {
            autoRecover = enabled;
        }
        
        /// <summary>
        /// Set recovery time
        /// </summary>
        public void SetRecoveryTime(float time)
        {
            recoveryTime = time;
        }
        
        /// <summary>
        /// Check if ragdoll is currently active
        /// </summary>
        public bool IsRagdollActive()
        {
            return isRagdollActive;
        }
        
        /// <summary>
        /// Get time since ragdoll was activated
        /// </summary>
        public float GetRagdollTime()
        {
            return isRagdollActive ? Time.time - ragdollStartTime : 0f;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (isRagdollActive)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 1f);
            }
        }
    }
}