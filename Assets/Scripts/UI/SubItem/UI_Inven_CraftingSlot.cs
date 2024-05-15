using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_Inven_CraftingSlot : UI_Inven_Slot, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] public int CraftingSlotIndex;
    
    public override void Init()
    {
    }
    
    /// <summary>
    /// 이 슬롯에 아이템을 넣을 때
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDrop(PointerEventData eventData) {
        // InventorySlot의 자식이 없을 때
        if(transform.childCount == 0) {
            try
            {
                // eventData가 들고있는 InventoryItem을 받고
                Item = eventData.pointerDrag.GetComponent<UI_Inven_Item>();

                // Item의 parentAfterDrag를 자신의 transform으로 저장. (복귀 위치가 변경됨.)
                Item.parentAfterDrag.GetComponent<UI_Inven_Slot>().Item = null;
                Item.parentAfterDrag = transform;
                
                Managers.Crafting.OnItemForCraftingChanged.Invoke();
            }
            catch (Exception)
            {
                
            }
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // InventorySlot의 자식이 없을 때
            if(transform.childCount == 0) {

                // eventData가 들고있는 InventoryItem을 받고
                Item = (Managers.UI.GetTopPopupUI() as UI_NotebookPopup)?.CatchedItem;
                Item?.transform.SetParent(transform);
                Item?.Released();
                // Inventory의 parentAfterDrag를 자신의 transform으로 저장. (복귀 위치가 변경됨.)
                (Managers.UI.GetTopPopupUI() as UI_NotebookPopup)!.CatchedItem = null;
                
                Managers.Crafting.OnItemForCraftingChanged.Invoke();
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        try
        {
            
        }
        catch (Exception)
        {
            
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        try
        {
        }
        catch (Exception)
        {
            
        }
    }

    
    
}
