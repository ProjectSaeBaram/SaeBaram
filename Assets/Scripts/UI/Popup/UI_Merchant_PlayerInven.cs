using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Merchant_PlayerInven : UI_Popup, ICatcher
{
    [Header("Items")]
    private const int numberOfItemSlots = 30;  // 기본 아이템 슬롯의 크기
    [SerializeField] private List<UI_Merchat_Player_Inven_Slot> itemSlots;
    [SerializeField] private List<UI_Inven_Item> visualizedItems = new List<UI_Inven_Item>();
    private List<ItemData> _itemDataList = new List<ItemData>();
    [SerializeField] public GameObject VisualizedLayer;
    [SerializeField] public Confirm_Player ConfirmBuy;
    private UI_Inven_ItemTooltip tooltip;
    public UI_Merchant merchant;
    public bool isBuyActive=false;
    public UI_Inven_Item CatchedItem { get; set; } // CatchedItem 필드 추가
    

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
            slot.SetUIMerchant(merchant);
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
        if (Managers.UI.GetTopPopupUI() != merchant) return;

        // 인벤토리의 데이터 저장
        Managers.Data.OnClose?.Invoke();

        Time.timeScale = 1;
        Managers.Data.OnClose -= ExportInventoryData;
        Managers.Data.OnClose -= Managers.Data.SaveInventoryData;

        // 아이템 제거
        for (int i = 0; i < numberOfItemSlots; i++)
        {
            DestroyImmediate(itemSlots[i].gameObject);
        }

        itemSlots.Clear();
        visualizedItems.Clear();

        // 플레이어 아이템 줍기 기능 활성화 
        EnablePickupItem();

        Managers.UI.ClosePopupUI();
    }

    // Confirm_Merchant를 활성화하는 메서드
    public void ActivateConfirmBuy()
    {
        if (ConfirmBuy != null)
        {
            ConfirmBuy.gameObject.SetActive(true);
        }
    }

    // Confirm_Merchant를 비활성화하는 메서드
    public void DeactivateConfirmBuy()
    {
        if (ConfirmBuy != null)
        {
            ConfirmBuy.gameObject.SetActive(false);
        }
    }
}
