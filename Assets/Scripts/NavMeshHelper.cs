using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Script hỗ trợ cho NavMesh, giúp các nhân vật di chuyển tốt hơn và khắc phục các vấn đề thường gặp
/// </summary>
public class NavMeshHelper : MonoBehaviour
{
    [Header("Thiết lập chung")]
    [Tooltip("Tự động sửa vị trí của các nhân vật lên NavMesh khi khởi động")]
    public bool fixPositionsOnStart = true;
    
    [Tooltip("Kiểm tra định kỳ kết nối NavMesh giữa các nhân vật")]
    public bool checkConnectionsPeriodically = true;
    
    [Tooltip("Thời gian giữa các lần kiểm tra (giây)")]
    public float checkInterval = 5f;
    
    [Header("Thiết lập khắc phục")]
    [Tooltip("Khoảng cách tối đa để tìm vị trí trên NavMesh (m)")]
    public float maxPlacementDistance = 10f;
    
    [Tooltip("Tự động tăng bán kính tìm kiếm NavMesh nếu không tìm thấy")]
    public bool autoExpandSearchRadius = true;
    
    [Header("Debug")]
    [Tooltip("Hiển thị thông tin debug")]
    public bool showDebugInfo = true;
    
    [Tooltip("Hiển thị NavMesh trong Scene View")]
    public bool visualizeNavMesh = true;
    
    // Bắt đầu
    void Start()
    {
        if (fixPositionsOnStart)
        {
            StartCoroutine(DelayedNavMeshFix());
        }
        
        if (checkConnectionsPeriodically)
        {
            StartCoroutine(PeriodicConnectionCheck());
        }
    }
    
    // Khắc phục NavMesh với độ trễ để đảm bảo mọi thứ đã được khởi tạo
    IEnumerator DelayedNavMeshFix()
    {
        // Đợi 1 giây để đảm bảo scene đã được khởi tạo đầy đủ
        yield return new WaitForSeconds(1f);
        
        FixNavMeshAgentPositions();
    }
    
    // Kiểm tra kết nối NavMesh định kỳ
    IEnumerator PeriodicConnectionCheck()
    {
        while (true)
        {
            // Đợi theo khoảng thời gian đã cấu hình
            yield return new WaitForSeconds(checkInterval);
            
            // Kiểm tra kết nối
            CheckNavMeshConnections();
        }
    }
    
    /// <summary>
    /// Khắc phục vị trí của tất cả NavMeshAgent trong scene
    /// </summary>
    public void FixNavMeshAgentPositions()
    {
        // Tìm tất cả NavMeshAgent trong scene
        NavMeshAgent[] allAgents = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None);
        int fixedCount = 0;
        
        if (showDebugInfo)
        {
            Debug.Log($"Bắt đầu khắc phục vị trí NavMeshAgent: {allAgents.Length} agents được tìm thấy");
        }
        
        foreach (NavMeshAgent agent in allAgents)
        {
            // Kiểm tra xem agent có ở trên NavMesh không
            if (!agent.isOnNavMesh)
            {
                // Tìm vị trí gần nhất trên NavMesh
                if (PlaceAgentOnNavMesh(agent))
                {
                    fixedCount++;
                }
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"Đã khắc phục {fixedCount}/{allAgents.Length} NavMeshAgent");
        }
    }
    
    /// <summary>
    /// Đặt một NavMeshAgent lên vị trí hợp lệ trên NavMesh
    /// </summary>
    /// <param name="agent">NavMeshAgent cần đặt lên NavMesh</param>
    /// <returns>True nếu thành công, False nếu không tìm thấy vị trí hợp lệ</returns>
    bool PlaceAgentOnNavMesh(NavMeshAgent agent)
    {
        if (agent == null) return false;
        
        // Thử đặt agent lên NavMesh
        NavMeshHit hit;
        float searchRadius = maxPlacementDistance;
        bool found = false;
        
        // Thử với bán kính ban đầu
        found = NavMesh.SamplePosition(agent.transform.position, out hit, searchRadius, NavMesh.AllAreas);
        
        // Nếu không tìm thấy và cho phép tăng bán kính
        if (!found && autoExpandSearchRadius)
        {
            // Tăng bán kính tìm kiếm lên gấp đôi
            searchRadius *= 2;
            found = NavMesh.SamplePosition(agent.transform.position, out hit, searchRadius, NavMesh.AllAreas);
            
            // Thử lần nữa với bán kính lớn hơn nữa nếu cần
            if (!found)
            {
                searchRadius *= 2;
                found = NavMesh.SamplePosition(agent.transform.position, out hit, searchRadius, NavMesh.AllAreas);
            }
        }
        
        // Nếu tìm thấy vị trí phù hợp, di chuyển agent đến đó
        if (found)
        {
            agent.Warp(hit.position); // Warp để đảm bảo agent được đặt đúng vị trí
            
            if (showDebugInfo)
            {
                Debug.Log($"Đã đặt {agent.gameObject.name} lên NavMesh tại {hit.position}, khoảng cách di chuyển: {Vector3.Distance(agent.transform.position, hit.position):F2}m");
            }
            
            return true;
        }
        else
        {
            if (showDebugInfo)
            {
                Debug.LogWarning($"Không thể tìm thấy vị trí NavMesh cho {agent.gameObject.name} trong phạm vi {searchRadius}m");
            }
            
            return false;
        }
    }
    
    /// <summary>
    /// Kiểm tra kết nối NavMesh giữa các nhân vật
    /// </summary>
    public void CheckNavMeshConnections()
    {
        // Tìm tất cả NPCController trong scene
        NPCController[] allNPCs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
        Dictionary<NPCController, Dictionary<NPCController, bool>> connections = 
            new Dictionary<NPCController, Dictionary<NPCController, bool>>();
        
        if (showDebugInfo)
        {
            Debug.Log($"Kiểm tra kết nối NavMesh giữa {allNPCs.Length} NPCs");
        }
        
        // Kiểm tra từng cặp NPC
        for (int i = 0; i < allNPCs.Length; i++)
        {
            NPCController npc1 = allNPCs[i];
            if (npc1 == null || npc1.IsDead()) continue;
            
            NavMeshAgent agent1 = npc1.GetComponent<NavMeshAgent>();
            if (agent1 == null || !agent1.isOnNavMesh) continue;
            
            connections[npc1] = new Dictionary<NPCController, bool>();
            
            for (int j = 0; j < allNPCs.Length; j++)
            {
                if (i == j) continue;
                
                NPCController npc2 = allNPCs[j];
                if (npc2 == null || npc2.IsDead()) continue;
                
                NavMeshAgent agent2 = npc2.GetComponent<NavMeshAgent>();
                if (agent2 == null || !agent2.isOnNavMesh) continue;
                
                // Kiểm tra xem có đường đi từ npc1 đến npc2 không
                NavMeshPath path = new NavMeshPath();
                bool success = NavMesh.CalculatePath(
                    agent1.transform.position,
                    agent2.transform.position,
                    NavMesh.AllAreas,
                    path
                );
                
                bool hasPath = path.status == NavMeshPathStatus.PathComplete;
                connections[npc1][npc2] = hasPath;
                
                if (showDebugInfo && !hasPath && npc1.team != npc2.team)
                {
                    Debug.LogWarning($"Không có đường đi từ {npc1.gameObject.name} (Team {npc1.team}) đến {npc2.gameObject.name} (Team {npc2.team})!");
                }
            }
        }
        
        // Hiển thị thông tin tóm tắt
        if (showDebugInfo)
        {
            int totalConnections = 0;
            int validConnections = 0;
            
            foreach (var npc in connections.Keys)
            {
                foreach (var connected in connections[npc].Values)
                {
                    totalConnections++;
                    if (connected) validConnections++;
                }
            }
            
            Debug.Log($"Kết nối NavMesh: {validConnections}/{totalConnections} đường đi hợp lệ ({validConnections * 100f / totalConnections:F1}%)");
        }
    }
    
    // Hiển thị NavMesh và kết nối trong Scene View
    void OnDrawGizmos()
    {
        if (!visualizeNavMesh) return;
        
        // Hiển thị NavMesh dưới dạng grid
        float gridSize = 1.0f;
        float maxDistance = 50f;
        Vector3 center = transform.position;
        
        Gizmos.color = new Color(0, 0.7f, 1, 0.15f);
        
        // Vẽ một grid của các điểm để thể hiện NavMesh
        for (float x = -maxDistance; x <= maxDistance; x += gridSize)
        {
            for (float z = -maxDistance; z <= maxDistance; z += gridSize)
            {
                Vector3 worldPos = new Vector3(center.x + x, center.y, center.z + z);
                
                // Kiểm tra xem điểm này có nằm trên NavMesh không
                NavMeshHit hit;
                if (NavMesh.SamplePosition(worldPos, out hit, 0.1f, NavMesh.AllAreas))
                {
                    Gizmos.DrawCube(hit.position, new Vector3(gridSize * 0.9f, 0.01f, gridSize * 0.9f));
                }
            }
        }
    }
}