using System;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

public class UI_Game_QuickSlot : UI_Inven_Slot
{
    public new event Action OnItemInitialized;
    
    public override UI_Inven_Item Item
    {
        get => base.Item;
        set
        {
            base.Item = value;
            OnItemInitialized?.Invoke();
        }
    }
    
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
                
                // 손에 들고있는 아이템 칸에 아이템을 넣었다면 
                PlayerController player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
                if (player._handledItem.Index == SlotIndex)
                    player._handledItem.ItemUIReferenceSetter(Item);
                
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
            if(UINotebookPopup.CatchedItem is not null && transform.childCount == 0) {

                // eventData가 들고있는 InventoryItem을 받고
                Item = UINotebookPopup.CatchedItem;
                Item?.transform.SetParent(transform);
                Item?.Released();
                
                // parentAfterDrag를 자신의 transform으로 저장. (복귀 위치가 변경됨.)
                UINotebookPopup.CatchedItem = null;
                Item.parentAfterDrag = transform;
                
                // 손에 들고있는 아이템 칸에 아이템을 넣었다면 
                PlayerController player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
                if (player._handledItem.Index == SlotIndex)
                    player._handledItem.ItemUIReferenceSetter(Item);
            }
        }
    }
}
