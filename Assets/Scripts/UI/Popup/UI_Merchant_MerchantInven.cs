using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Merchant_MerchantInven : UI_Popup, ICatcher
{
    [SerializeField] public Confirm_Merchant ConfirmSell;
    [Header("Items")]
    private const int numberOfItemSlots = 30;  // 기본 아이템 슬롯의 크기
    [SerializeField] private List<UI_Merchat_Merchant_Inven_Slot> itemSlots;
    [SerializeField] private List<UI_Inven_Item> visualizedItems = new List<UI_Inven_Item>();
    private List<ItemData> _shopItemDataList;
    [SerializeField] public GameObject VisualizedLayer;
    [SerializeField] private UI_Inven_ItemTooltip tooltip;
    [SerializeField] public ShopData shopData;
    public UI_Merchant merchant;
    public UI_Inven_Item CatchedItem { get; set; } // CatchedItem 필드 추가

    public override void Init()
    {
        // 상점 아이템 초기화
        shopData.InitShopItems();

       

    }

    public void InitShopInventory(GameObject visualizedLayer, UI_Inven_ItemTooltip itemTooltip)
    {
        VisualizedLayer = visualizedLayer;
        tooltip = itemTooltip;
        _shopItemDataList = new List<ItemData>() {
            new Tool(129, "돌 도끼", 1, 5, 0),
        };
        VisualizeShopItems(true);
    }

    public void VisualizeShopItems(bool initialize = false)
    {
        Transform content = Util.FindChild<Transform>(VisualizedLayer, "InventoryContent", true);

        if (!initialize)
        {
            foreach (var slot in itemSlots)
            {
                slot.transform.SetParent(content);
                if (slot.Item != null) slot.Item.parentPanel = VisualizedLayer;
            }
            return;
        }

       


        for (int i = 0; i < itemSlots.Count; i++)
        {
            UI_Merchat_Merchant_Inven_Slot slot = itemSlots[i];
            slot.SetUIMerchant(merchant);
            slot.SlotIndex = i;
            ItemData item;
            if (i<_shopItemDataList.Count)
            {
                item = _shopItemDataList[i];
            }
            else
            {
                item= new DummyItem();
            }
         
            switch (item)
            {
                case Tool tool:
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(itemSlots[i].transform));
                    visualizedItems[i].parentPanel = VisualizedLayer;
                    visualizedItems[i].ToolInit(tool.Name, tool.Quality, tool.Durability, tool.ReinforceCount, tool.Logs);
                    break;
                case Ingredient ingredient:
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(itemSlots[i].transform));
                    visualizedItems[i].parentPanel = VisualizedLayer;
                    visualizedItems[i].IngredientInit(ingredient.Name, ingredient.Quality, ingredient.Amount, ingredient.Logs);
                    break;
                case DummyItem:
                    visualizedItems.Add(null);
                    break;
            }
            itemSlots[i].Item = visualizedItems[i];
        }
    }

    // Confirm_Merchant를 활성화하는 메서드
    public void ActivateConfirmSell()
    {
        if (ConfirmSell != null)
        {
            ConfirmSell.gameObject.SetActive(true);
        }
    }

    // Confirm_Merchant를 비활성화하는 메서드
    public void DeactivateConfirmSell()
    {
        if (ConfirmSell != null)
        { 
            ConfirmSell.gameObject.SetActive(false);
        }
    }
}
