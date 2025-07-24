using UnityEngine;
using UnityEngine.UI;

public class PrefabPreviewGenerator : MonoBehaviour
{
    public static Sprite GeneratePreviewSprite(GameObject prefab, int width = 128, int height = 128)
    {
        if (prefab == null) return null;
        
        // Create a temporary camera for rendering
        GameObject tempCameraObj = new GameObject("TempPreviewCamera");
        Camera tempCamera = tempCameraObj.AddComponent<Camera>();
        
        // Set up camera
        tempCamera.clearFlags = CameraClearFlags.SolidColor;
        tempCamera.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0f); // Transparent background
        tempCamera.orthographic = true;
        tempCamera.orthographicSize = 2f;
        tempCamera.nearClipPlane = 0.1f;
        tempCamera.farClipPlane = 10f;
        
        // Create render texture
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        tempCamera.targetTexture = renderTexture;
        
        // Instantiate prefab temporarily
        GameObject tempPrefab = Instantiate(prefab);
        
        // Position prefab in front of camera
        tempPrefab.transform.position = Vector3.zero;
        tempCamera.transform.position = new Vector3(0, 1, -3);
        tempCamera.transform.LookAt(tempPrefab.transform);
        
        // Get bounds to fit prefab in view
        Renderer[] renderers = tempPrefab.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }
            
            // Adjust camera to fit bounds
            float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            tempCamera.orthographicSize = maxSize * 0.6f;
            
            // Center the prefab
            tempPrefab.transform.position = -bounds.center;
        }
        
        // Render
        tempCamera.Render();
        
        // Create texture2D from render texture
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;
        
        // Create sprite
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        
        // Cleanup
        DestroyImmediate(tempPrefab);
        DestroyImmediate(tempCameraObj);
        DestroyImmediate(renderTexture);
        
        return sprite;
    }
    
    public static Texture2D GeneratePreviewTexture(GameObject prefab, int width = 128, int height = 128)
    {
        if (prefab == null) return null;
        
        // Create a simple colored texture as fallback
        Texture2D texture = new Texture2D(width, height);
        Color[] colors = new Color[width * height];
        
        // Generate a simple pattern based on prefab name
        Color baseColor = GetColorFromName(prefab.name);
        
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = baseColor;
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return texture;
    }
    
    static Color GetColorFromName(string name)
    {
        // Generate color based on name hash
        int hash = name.GetHashCode();
        Random.InitState(hash);
        
        return new Color(
            Random.Range(0.3f, 0.8f),
            Random.Range(0.3f, 0.8f),
            Random.Range(0.3f, 0.8f),
            1f
        );
    }
}