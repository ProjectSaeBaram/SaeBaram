using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Merchat_Player_Inven_Slot : UI_Inven_Slot
{
    public UI_Merchant UIMerchant;


    // 마우스에서 손을 때면
    public override void OnDrop(PointerEventData eventData)
    {
        // InventorySlot의 자식이 없을 때
        if (transform.childCount == 0)
        {
            try
            {
                // eventData가 들고있는 InventoryItem을 받고
                Item = eventData.pointerDrag.GetComponent<UI_Inven_Item>();

                // parentAfterDrag를 자신의 transform으로 저장. (복귀 위치가 변경됨.)
                Item.parentAfterDrag.GetComponent<UI_Inven_Slot>().Item = null;

                // (+ 만약 이 아이템이 CraftingSlot에서 꺼내어졌다면, 이 아이템이 없는 버전으로 검색해야한다)
                if (Item.parentAfterDrag.GetComponent<UI_Inven_Slot>() is UI_Inven_CraftingSlot)
                {
                    Managers.Crafting.OnItemForCraftingChanged.Invoke();
                }

                Item.parentAfterDrag = transform;
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
            // 분리된 아이템을 커서에 든 채, Slot의 자식이 없을 때
            if (UIMerchant.CatchedItem is not null && transform.childCount == 0)
            {

                // eventData가 들고있는 InventoryItem을 받고
                Item = UIMerchant.CatchedItem;
                Item?.transform.SetParent(transform);
                Item?.Released();

                // parentAfterDrag를 자신의 transform으로 저장. (복귀 위치가 변경됨.)
                UIMerchant.CatchedItem = null;
                Item.parentAfterDrag = transform;
            }
        }
    }

    public void SetUIMerchant(UI_Merchant uiMerchant)
    {
        UIMerchant = uiMerchant;
        if (Item != null)
        {
            Item.ToolTipHandler = uiMerchant;
        }
    }
}
