using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Inven_Slot : UI_Base, IDropHandler, IPointerClickHandler
{
    public UI_Inven_Item Item;
    
    public override void Init()
    {
        
    }
    
    // 마우스에서 손을 때면
    public void OnDrop(PointerEventData eventData) {
        // InventorySlot의 자식이 없을 때
        if(transform.childCount == 0) {
            try
            {
                // eventData가 들고있는 InventoryItem을 받고
                Item = eventData.pointerDrag.GetComponent<UI_Inven_Item>();

                // Inventory의 parentAfterDrag를 자신의 transform으로 저장. (복귀 위치가 변경됨.)
                Item.parentAfterDrag.GetComponent<UI_Inven_Slot>().Item = null;
                Item.parentAfterDrag = transform;
            }
            catch (Exception)
            {
                
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // InventorySlot의 자식이 없을 때
            if(transform.childCount == 0) {

                // eventData가 들고있는 InventoryItem을 받고
                Item = (Managers.UI.GetTopPopupUI() as UI_NotebookPopup)?.CatchedItem;
                Item.transform.SetParent(transform);
                Item.Released();
                // Inventory의 parentAfterDrag를 자신의 transform으로 저장. (복귀 위치가 변경됨.)
                (Managers.UI.GetTopPopupUI() as UI_NotebookPopup)!.CatchedItem = null;
            }
        }
    }
}
