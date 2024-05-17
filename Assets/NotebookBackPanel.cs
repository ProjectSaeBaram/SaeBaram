using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NotebookBackPanel : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private UI_NotebookPopup _uiNotebookPopup;
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        if ((Managers.UI.GetTopPopupUI() as UI_NotebookPopup).CatchedItem is UI_Inven_Item item2)
            Managers.Data.RemoveItemFromInventory(item2);
        
        _uiNotebookPopup.ClosePopupUI(eventData);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
