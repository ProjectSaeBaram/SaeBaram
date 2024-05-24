using System;
using UnityEngine.EventSystems;

public class UI_Game_QuickSlot : UI_Inven_Slot
{
    public override void Init()
    {
        
    }
    
    // 마우스에서 손을 때면
    public override void OnDrop(PointerEventData eventData)
    {
        // InventorySlot의 자식이 없을 때
        if(transform.childCount == 0) {
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
                
                // 추가
                Item.parentPanel = transform.parent.gameObject;
            }
            catch (Exception)
            {
                
            }
        }
    }
}
