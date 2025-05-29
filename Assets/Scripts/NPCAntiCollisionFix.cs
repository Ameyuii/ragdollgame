using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Script fix toàn diện vấn đề NPCs bay ra xa khi va chạm
/// Attach vào mỗi NPC để tự động khắc phục
/// </summary>
public class NPCAntiCollisionFix : MonoBehaviour
{
    [Header("Anti-Collision Settings")]
    [SerializeField, Tooltip("Khoảng cách tối thiểu giữa các NPCs")]
    private float minDistanceBetweenNPCs = 1.2f;
    
    [SerializeField, Tooltip("Lực đẩy nhẹ để tách NPCs")]
    private float separationForce = 50f;
    
    [SerializeField, Tooltip("Giới hạn tốc độ tối đa")]
    private float maxVelocity = 3f;
    
    [SerializeField, Tooltip("Damping factor để giảm tốc")]
    private float velocityDamping = 0.95f;
    
    private Rigidbody rb;
    private NavMeshAgent navAgent;
    private NPCController npcController;
    private LayerMask npcLayerMask = -1; // Tất cả layers
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        navAgent = GetComponent<NavMeshAgent>();
        npcController = GetComponent<NPCController>();
        
        // Cài đặt Rigidbody an toàn
        if (rb != null)
        {
            rb.mass = 60f; // Khối lượng vừa phải
            rb.linearDamping = 3f; // Drag cao để giảm tốc nhanh
            rb.angularDamping = 8f; // Giảm xoay
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth movement
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Tránh clipping
        }
        
        // Cài đặt NavMeshAgent an toàn
        if (navAgent != null)
        {
            navAgent.radius = 0.3f; // Radius nhỏ
            navAgent.height = 1.8f;
            navAgent.stoppingDistance = 2f; // Dừng xa hơn
            navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
            navAgent.avoidancePriority = Random.Range(40, 60); // Priority khác nhau
        }
        
        Debug.Log($"🛡️ {gameObject.name}: NPCAntiCollisionFix đã được kích hoạt");
    }
      void FixedUpdate()
    {
        if (rb == null || npcController == null || npcController.IsDead()) return;
        
        // 1. GIỚI HẠN TỐC ĐỘ
        LimitVelocity();
        
        // 2. TÁCH NPCs KHI QUÁ GẦN
        SeparateFromOtherNPCs();
        
        // 3. NGĂN BAY RA XA
        PreventFlyingAway();
    }
    
    void LimitVelocity()
    {
        if (rb.linearVelocity.magnitude > maxVelocity)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
            Debug.Log($"⚡ {gameObject.name}: Giới hạn tốc độ ({maxVelocity})");
        }
        
        // Damping tự nhiên
        rb.linearVelocity *= velocityDamping;
    }
    
    void SeparateFromOtherNPCs()
    {
        // Tìm NPCs gần
        Collider[] nearbyNPCs = Physics.OverlapSphere(transform.position, minDistanceBetweenNPCs, npcLayerMask);
        
        Vector3 separationVector = Vector3.zero;
        int npcCount = 0;
        
        foreach (Collider col in nearbyNPCs)
        {
            if (col.gameObject == gameObject) continue; // Bỏ qua chính mình
              NPCController otherNPC = col.GetComponent<NPCController>();
            if (otherNPC == null || otherNPC.IsDead()) continue;
            
            // Tính vector tách
            Vector3 directionAway = transform.position - col.transform.position;
            float distance = directionAway.magnitude;
            
            if (distance < minDistanceBetweenNPCs && distance > 0.1f)
            {
                separationVector += directionAway.normalized / distance; // Lực tỉ lệ nghịch với khoảng cách
                npcCount++;
            }
        }
        
        // Áp dụng lực tách nhẹ
        if (npcCount > 0 && rb != null)
        {
            Vector3 separationForceVector = separationVector.normalized * separationForce;
            separationForceVector.y = 0; // Chỉ tách theo trục X, Z
            
            rb.AddForce(separationForceVector, ForceMode.Acceleration);
            
            Debug.Log($"🚫 {gameObject.name}: Tách khỏi {npcCount} NPCs gần ({separationForceVector.magnitude:F1})");
        }
    }
    
    void PreventFlyingAway()
    {
        // Nếu NPC bay quá cao hoặc xa
        Vector3 groundPos = new Vector3(transform.position.x, 0, transform.position.z);
        float distanceFromGround = Vector3.Distance(transform.position, groundPos);
        
        if (distanceFromGround > 5f || transform.position.y > 10f)
        {
            // Reset về mặt đất
            Vector3 safePosition = new Vector3(transform.position.x, 1f, transform.position.z);
            transform.position = safePosition;
            
            // Reset velocity
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            
            Debug.LogWarning($"🚁 {gameObject.name}: RESET VỊ TRÍ do bay quá xa!");
        }
    }
    
    // Tự động thêm component này vào NPC nếu chưa có
    [System.Obsolete("Use QuickNPCAntiCollisionFix instead")]
    public static void AddToNPC(GameObject npcGameObject)
    {
        if (npcGameObject.GetComponent<NPCAntiCollisionFix>() == null)
        {
            npcGameObject.AddComponent<NPCAntiCollisionFix>();
            Debug.Log($"➕ Đã thêm NPCAntiCollisionFix vào {npcGameObject.name}");
        }
    }
}
