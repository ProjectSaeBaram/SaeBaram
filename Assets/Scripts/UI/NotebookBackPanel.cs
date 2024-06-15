using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NotebookBackPanel : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private UI_NotebookPopup _uiNotebookPopup;
    
    public void OnPointerUp(PointerEventData eventData)
    {
        // 좌클릭인 경우에만
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        // 
        if ((Managers.UI.GetTopPopupUI() as UI_NotebookPopup).CatchedItem is UI_Inven_Item item)
            Managers.Data.RemoveItemFromInventory(item);
        
        _uiNotebookPopup.ClosePopupUI(eventData);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
