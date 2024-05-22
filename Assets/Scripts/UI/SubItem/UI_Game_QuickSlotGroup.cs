using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_Game_QuickSlotGroup : UI_Base
{
    [SerializeField] private List<UI_Game_QuickSlot> _quickSlots = new List<UI_Game_QuickSlot>();
    [SerializeField] private List<UI_Inven_Item> visualizedItems = new List<UI_Inven_Item>();
    private List<ItemData> _itemDataList = new List<ItemData>();
    
    public override void Init()
    {
        // 팝업을 끌 때, DataManager와 통신하여 인벤토리 데이터를 저장.
        Managers.Data.OnCloseQ -= ExportQuickSlotData;
        Managers.Data.OnCloseQ -= Managers.Data.SaveQuickSlotData;
        Managers.Data.OnCloseQ += ExportQuickSlotData;
        Managers.Data.OnCloseQ += Managers.Data.SaveQuickSlotData;

        // UI 요소 시각화 하는 과정 필요함

        VisualizeItemsInTheGrid();
    }

    /// <summary>
    /// 인벤토리가 열릴 때, 아이템을 불러들여 시각화하는 기능
    /// </summary>
    public void VisualizeItemsInTheGrid()
    {
        GetItemDataFromDataManager();
        
        for (int i = 0; i < 8; i++)
        {
            // 아이템
            ItemData item = _itemDataList[i];

            switch (item)       // 아이템의 종류에 따라 다르게 시각화
            {
                case Tool tool:
                {
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(_quickSlots[i].transform));
                    visualizedItems[i].parentPanel = gameObject;
                    visualizedItems[i].ToolInit(tool!.Name, tool!.Quality, tool!.Durability, tool!.ReinforceCount, tool!.Logs);
                    break;
                }
                case Ingredient ingredient:
                {
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(_quickSlots[i].transform));
                    visualizedItems[i].parentPanel = gameObject;
                    visualizedItems[i].IngredientInit(ingredient!.Name, ingredient!.Quality, ingredient!.Amount, ingredient!.Logs);
                    break;
                }
                case DummyItem:
                {
                    visualizedItems.Add(null);
                    break;
                }
            }

            _quickSlots[i].Item = visualizedItems[i];
        }
    }
    
    void GetItemDataFromDataManager()
    {
        Managers.Data.LoadQuickSlotData();
        _itemDataList = Managers.Data.QuickSlotItemInfos();
    }
    
    /// <summary>
    /// 슬롯들의 아이템들 레퍼런스 잡아주기
    /// </summary>
    /// <param name="notebookPopup"></param>
    public void InitQuickSlotsNotebookRef(UI_NotebookPopup notebookPopup)
    {
        foreach (var quickSlot in _quickSlots)
        {
            quickSlot.SetNotebookPopup(notebookPopup);
        }
    }
    
    /// <summary>
    /// GameScene이 전환될 때 퀵슬롯 아이템 데이터를 다시 바이너리 파일로 저장
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void ExportQuickSlotData()
    {
        for (int i = 0; i < 8; i++)
        {
            UI_Inven_Item itemUI = _quickSlots[i].Item;
            
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
    
        Managers.Data.TransDataListIntoArrayForQuickSlots(_itemDataList);
    }
}