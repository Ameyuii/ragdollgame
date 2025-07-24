using UnityEngine;
using UnityEngine.UI;

public class FixCharacterButtonRaycast : MonoBehaviour
{
    public static void Execute()
    {
        Debug.Log("=== FIXING CHARACTER BUTTON RAYCAST ISSUES ===");
        
        // Find all character buttons
        CharacterDragSource[] dragSources = Object.FindObjectsOfType<CharacterDragSource>();
        
        foreach (CharacterDragSource source in dragSources)
        {
            FixButtonRaycast(source);
        }
        
        Debug.Log("=== CHARACTER BUTTON RAYCAST FIXES COMPLETED ===");
    }
    
    static void FixButtonRaycast(CharacterDragSource source)
    {
        Debug.Log($"Fixing raycast for {source.name}");
        
        // Get all Text components in children
        Text[] childTexts = source.GetComponentsInChildren<Text>();
        
        foreach (Text text in childTexts)
        {
            if (text.gameObject != source.gameObject)
            {
                // Disable raycast target for child text elements
                if (text.raycastTarget)
                {
                    text.raycastTarget = false;
                    Debug.Log($"  Disabled raycastTarget for {text.name}");
                }
            }
        }
        
        // Get all Image components in children (except the main button image)
        Image[] childImages = source.GetComponentsInChildren<Image>();
        Image mainImage = source.GetComponent<Image>();
        
        foreach (Image image in childImages)
        {
            if (image != mainImage && image.gameObject != source.gameObject)
            {
                // Check if this is a decorative image that should not block raycasts
                if (image.name.Contains("Preview") || image.name.Contains("Border") || image.name.Contains("Icon"))
                {
                    if (image.raycastTarget)
                    {
                        image.raycastTarget = false;
                        Debug.Log($"  Disabled raycastTarget for image {image.name}");
                    }
                }
            }
        }
        
        // Ensure the main button image can receive raycasts
        if (mainImage != null)
        {
            mainImage.raycastTarget = true;
            Debug.Log($"  Ensured raycastTarget enabled for main image");
        }
        
        // Ensure the button component exists and is properly configured
        Button button = source.GetComponent<Button>();
        if (button != null)
        {
            button.interactable = true;
            if (button.targetGraphic == null && mainImage != null)
            {
                button.targetGraphic = mainImage;
                Debug.Log($"  Set button targetGraphic to main image");
            }
        }
    }
}