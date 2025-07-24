using UnityEngine;

public class SetupDragDropSystem : MonoBehaviour
{
    public static void Execute()
    {
        // Tìm tất cả các nhân vật trong scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            // Kiểm tra nếu là nhân vật (có chứa "Character" hoặc "npc" trong tên)
            if (obj.name.Contains("Character") || obj.name.Contains("npc"))
            {
                // Thêm component CharacterDragDrop nếu chưa có
                CharacterDragDrop dragDrop = obj.GetComponent<CharacterDragDrop>();
                if (dragDrop == null)
                {
                    dragDrop = obj.AddComponent<CharacterDragDrop>();
                    Debug.Log($"Added CharacterDragDrop to {obj.name}");
                }
                
                // Thêm collider nếu chưa có
                Collider collider = obj.GetComponent<Collider>();
                if (collider == null)
                {
                    CapsuleCollider capsule = obj.AddComponent<CapsuleCollider>();
                    capsule.height = 2f;
                    capsule.radius = 0.5f;
                    capsule.center = new Vector3(0, 1f, 0);
                    Debug.Log($"Added CapsuleCollider to {obj.name}");
                }
            }
        }
        
        // Thiết lập layer cho Ground
        GameObject ground = GameObject.Find("Ground");
        if (ground != null)
        {
            // Tạo layer "Ground" nếu chưa có
            CreateLayerIfNotExists("Ground", 8);
            ground.layer = 8; // Ground layer
            Debug.Log("Set Ground layer to 8");
        }
        
        // Thiết lập layer cho các obstacle
        SetObstacleLayer("Obstacle_Cube1");
        SetObstacleLayer("Obstacle_Cube2");
        SetObstacleLayer("Obstacle_Cube3");
        SetObstacleLayer("Obstacle_Sphere1");
        SetObstacleLayer("Obstacle_Sphere2");
        SetObstacleLayer("Obstacle_Cylinder1");
        
        Debug.Log("Drag & Drop system setup completed!");
    }
    
    static void SetObstacleLayer(string objectName)
    {
        GameObject obstacle = GameObject.Find(objectName);
        if (obstacle != null)
        {
            CreateLayerIfNotExists("Ground", 8);
            obstacle.layer = 8; // Cũng đặt vào Ground layer để có thể thả nhân vật lên
        }
    }
    
    static void CreateLayerIfNotExists(string layerName, int layerIndex)
    {
        // Unity sẽ tự động tạo layer khi cần thiết
        // Chúng ta chỉ cần đảm bảo sử dụng đúng index
    }
}