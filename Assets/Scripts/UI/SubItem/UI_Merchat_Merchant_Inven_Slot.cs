using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class UI_Merchat_Merchant_Inven_Slot : UI_Inven_Slot
{
    
    public UI_Merchant UIMerchant;
  

  

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
