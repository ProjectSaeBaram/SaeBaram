using System;
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
    private UI_Inven_ItemTooltip tooltip;


    public override void Init()
    {
       
    }

    private void OnEnable()
    {
        // 팝업을 끌 때, DataManager와 통신하여 인벤토리 데이터를 저장.
        Managers.Data.OnClose -= ExportInventoryData;
        Managers.Data.OnClose -= Managers.Data.SaveInventoryData;
        Managers.Data.OnClose += ExportInventoryData;
        Managers.Data.OnClose += Managers.Data.SaveInventoryData;


    }

    public void InitInventory(GameObject visualizedLayer, UI_Inven_ItemTooltip itemTooltip)
    {
        VisualizedLayer = visualizedLayer;
        tooltip = itemTooltip;

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
            UI_Merchat_Player_Inven_Slot slot = itemSlots[i];
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
    void EnablePickupItem()
    {
        PlayerController player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        player.EnablePickupItem();
        player.EnableClick();
    }

   

    public void ExportInventoryData()
    {
        try
        {
            bool proove = this.gameObject.activeSelf;
        }
        catch (Exception)
        {
            DebugEx.Log("Skipped Exporting from InventoryPopup to DataManager");
            return;
        }

        for (int i = 0; i < numberOfItemSlots; i++)
        {
            UI_Inven_Item itemUI = itemSlots[i].Item;

            if (itemUI == null)
            {
                // 빈 공간으로 인식되기 위해 DummyItem을 저장 
                _itemDataList[i] = new DummyItem();
            }
            else
            {
                ItemData itemData = null;

                switch (itemUI.itemType)
                {
                    case Define.ItemType.Tool:
                        itemData = new Tool(0, itemUI.Name, itemUI.Quality, itemUI.Durability, itemUI.ReinforceCount);
                        break;
                    case Define.ItemType.Ingredient:
                        itemData = new Ingredient(0, itemUI.Name, itemUI.Quality, itemUI.Amount);
                        break;
                }

                if (itemUI.Logs != null)
                {
                    itemData.SetLogFromLogString(itemUI.Logs);
                }

                _itemDataList[i] = itemData;
            }
        }

        Managers.Data.TransDataListIntoArray(_itemDataList);
    }

    public override void ClosePopupUI(PointerEventData action)
    {
        if (Managers.UI.GetTopPopupUI() != this) return;

      

        // 인벤토리의 데이터 저장
        Managers.Data.OnClose?.Invoke();    // Test할 때 발생하는 오류를 막기 위해 ? (Nullable) 추가.

        Time.timeScale = 1;
        Managers.Data.OnClose -= ExportInventoryData;
        Managers.Data.OnClose -= Managers.Data.SaveInventoryData;

        //if (CatchedItem != null)
        //   Managers.Data.RemoveItemFromInventory(CatchedItem);

        // 아이템 제거
        for (int i = 0; i < numberOfItemSlots; i++)
        {
            DestroyImmediate(itemSlots[i].gameObject);
            // itemSlots[i] = null;
            // visualizedItems[i] = null;
        }

        itemSlots.Clear();
        visualizedItems.Clear();

        // 플레이어 아이템 줍기 기능 활성화 
        EnablePickupItem();

        base.ClosePopupUI(action);
    }


}
