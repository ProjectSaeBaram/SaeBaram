using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_Item_CraftingSlot : UI_Inven_Slot
{
    [SerializeField] private int CraftingSlotIndex;
    
    public override void Init()
    {
        OnRegister -= Managers.Crafting.RegisterIngredientItem;
        OnRegister += Managers.Crafting.RegisterIngredientItem;
    }
    
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
                
                OnRegister.Invoke(CraftingSlotIndex, Item);
            }
            catch (Exception)
            {
                
            }
        }
    }
}
