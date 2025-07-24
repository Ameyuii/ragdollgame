using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public Color normalColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color hoverColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Image backgroundImage;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = hoverColor;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
    }
}