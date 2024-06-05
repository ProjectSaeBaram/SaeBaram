using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UI_NotebookPopup;

public class UI_Merchant_PlayerInven : UI_Popup
{

   
    [Header("Items")]
    private const int numberOfItemSlots = 30;  // 기본 아이템 슬롯의 크기
    [SerializeField] private List<UI_Merchat_Player_Inven_Slot> itemSlots;
    [SerializeField] private List<UI_Inven_Item> visualizedItems = new List<UI_Inven_Item>();
    private List<ItemData> _itemDataList = new List<ItemData>();
    [SerializeField] public GameObject VisualizedLayer;
    [SerializeField] private UI_Inven_ItemTooltip uiInvenItemTooltip;
    [SerializeField] private UI_Inven_ItemTooltip tooltip;

    public override void Init()
    {

    }

    public void InitInventory(GameObject visualizedLayer, UI_Inven_ItemTooltip itemTooltip)
    {
        VisualizedLayer = visualizedLayer;
        uiInvenItemTooltip = itemTooltip;

        VisualizeItemsInTheGrid(true);
    }

    public void VisualizeItemsInTheGrid(bool initialize = false)
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

        GetItemDataFromDataManager();

        for (int i = 0; i < numberOfItemSlots; i++)
        {
            UI_Merchat_Player_Inven_Slot slot = Managers.UI.MakeSubItem<UI_Merchat_Player_Inven_Slot>(content);
            //slot.SetNotebookPopup();
            slot.SlotIndex = i;

            ItemData item = _itemDataList[i];
            switch (item)
            {
                case Tool tool:
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(itemSlots[i].transform));
                    visualizedItems[i].parentPanel = VisualizedLayer;
                    visualizedItems[i].ToolInit(tool!.Name, tool!.Quality, tool!.Durability, tool!.ReinforceCount, tool!.Logs);
                    break;
                case Ingredient ingredient:
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(itemSlots[i].transform));
                    visualizedItems[i].parentPanel = VisualizedLayer;
                    visualizedItems[i].IngredientInit(ingredient!.Name, ingredient!.Quality, ingredient!.Amount, ingredient!.Logs);
                    break;
                case DummyItem:
                    visualizedItems.Add(null);
                    break;
            }
            itemSlots[i].Item = visualizedItems[i];
        }
    }

    private void GetItemDataFromDataManager()
    {
        Managers.Data.LoadInventoryData();
        _itemDataList = Managers.Data.ItemInfos();
    }

    public void ShowToolTip(UI_Inven_Item invenItem, PointerEventData eventData)
    {
        uiInvenItemTooltip.gameObject.SetActive(true);
        uiInvenItemTooltip.ShowTooltip(invenItem, eventData);
    }

    public void HideTooltip()
    {
        uiInvenItemTooltip.gameObject.SetActive(false);
        uiInvenItemTooltip.UnsetPointerEventData();
    }
}
