using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_Inven_CraftingSlot : UI_Inven_Slot, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] public int CraftingSlotIndex;
    
    /// <summary>
    /// 조합식을 검색할 때 Invoke 될 UnityAction
    /// 조합식은 슬롯에 아이템을 넣을 때와, 뺄 때 모두 검색한다.
    /// </summary>
    private UnityAction<int, UI_Inven_Item> OnRegister;
    
    public override void Init()
    {
        OnRegister -= Managers.Crafting.RegisterIngredientItem;
        OnRegister += Managers.Crafting.RegisterIngredientItem;
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
                
                OnRegister.Invoke(CraftingSlotIndex, Item);
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
                
                OnRegister.Invoke(CraftingSlotIndex, Item);
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        try
        {
            if (transform.childCount == 0 || (transform.childCount == 1 && Item.itemType == UI_Inven_Item.ItemType.Ingredient && eventData.pointerDrag.GetComponent<UI_Inven_Item>().itemType == UI_Inven_Item.ItemType.Ingredient))
            {
                if (eventData.pointerDrag.GetComponent<UI_Inven_Item>().parentAfterDrag == null)
                {
                    OnRegister.Invoke(CraftingSlotIndex, null);
                }
            }
        }
        catch (Exception)
        {
            
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        try
        {
            if (eventData.pointerDrag.GetComponent<UI_Inven_Item>().parentAfterDrag == Item.parentAfterDrag)
            {
                OnRegister.Invoke(CraftingSlotIndex, null);
            }
        }
        catch (Exception)
        {
            
        }
    }

    
    
}
