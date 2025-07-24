using UnityEngine;
using UnityEngine.EventSystems;

public class MapDropZone : MonoBehaviour, IDropHandler
{
    public BattleGameManager gameManager;
    
    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<BattleGameManager>();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop detected on map");
        
        // The actual drop handling is done in CharacterDragSource.OnEndDrag
        // This is just for additional drop zone functionality if needed
    }
    
    void OnDrawGizmos()
    {
        // Draw the drop zone bounds in editor
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}