using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_InventoryPopup : UI_Popup
{
    enum GameObjects
    {
        ItemGrid,
    }

    private GridLayoutGroup itemGridLayoutGroup;

    [FormerlySerializedAs("_itemSlots")] 
    [Header("Items")] 
    private const int numberOfItemSlots = 10;       // 테스트용 기본 아이템 슬롯의 크기
    [SerializeField] private List<UI_Inven_Slot> itemSlots = new List<UI_Inven_Slot>();
    [SerializeField] private List<UI_Inven_Item> visualizedItems = new List<UI_Inven_Item>();
    private List<ItemData> _itemDataList = new List<ItemData>();
    
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        itemGridLayoutGroup = Get<GameObject>((int)GameObjects.ItemGrid).GetComponent<GridLayoutGroup>();

        VisualizeItemsInTheGrid();

        Managers.Data.OnClose += ExportInventoryData;
        Managers.Data.OnClose += Managers.Data.SaveInventoryData;
    }

    /// <summary>
    /// 인벤토리가 열릴 때, 아이템을 불러들여 시각화하는 기능
    /// 한번만 호출되어야 한다.
    /// </summary>
    private void VisualizeItemsInTheGrid()
    {
        GetItemInfos();

        for (int i = 0; i < numberOfItemSlots; i++)
        {
            // 아이템 슬롯
            itemSlots.Add(Managers.UI.MakeSubItem<UI_Inven_Slot>(itemGridLayoutGroup.transform));
            
            // 아이템
            ItemData item = _itemDataList[i];

            switch (item)       // 아이템의 종류에 따라 다르게 시각화
            {
                case Tool tool:
                {
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(itemSlots[i].transform));
                    visualizedItems[i].ToolInit(tool!.Name, tool!.Quality, tool!.Durability, tool!.ReinforceCount);
                    visualizedItems[i]._uiInventoryPopup = this;
                    break;
                }
                case Ingredient ingredient:
                {
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(itemSlots[i].transform));
                    visualizedItems[i].IngredientInit(ingredient!.Name, ingredient!.Quality, ingredient!.Amount);
                    visualizedItems[i]._uiInventoryPopup = this;
                    break;
                }
                case DummyItem:
                {
                    visualizedItems.Add(null);
                    break;
                }
            }

            itemSlots[i].Item = visualizedItems[i];
        }
    }

    // /// <summary>
    // /// 인벤토리가 열려있는 중에, 아이템 목록의 변화가 일어났을 때 갱신하고, 다시 시각화해주는 기능.
    // /// </summary>
    // private void RefreshItemUI()
    // {
    //     for (int i = 0; i < _itemDataList.Count; i++)
    //         visualizedItems[i].SetInfo(_itemDataList[i].Name, _itemDataList[i].Count);
    // }

    /// <summary>
    /// DataManager를 통해 캐싱된 데이터를 읽어오는 기능
    /// </summary>
    private void GetItemInfos()
    {
        _itemDataList = Managers.Data.ItemInfos();
    }

    /// <summary>
    /// 팝업이 닫힐 때는 인벤토리 정보를 다시 바이너리 파일로 저장
    /// DataManager에게 정보를 전달
    /// </summary>
    public void ExportInventoryData()
    {
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
                switch (itemUI.itemType)
                {
                    case UI_Inven_Item.ItemType.Tool:
                        _itemDataList[i] = 
                            new Tool(0, itemUI.Name, itemUI.Quality, itemUI.Durability, itemUI.ReinforceCount);
                        break;
                    case UI_Inven_Item.ItemType.Ingredient:
                        _itemDataList[i] = new Ingredient(0, itemUI.Name, itemUI.Quality, itemUI.Amount);
                        break;
                    case UI_Inven_Item.ItemType.Dummy:
                        _itemDataList[i] = new DummyItem();
                        break;
                }
                
            }
        }
    
        Managers.Data.TransDataListIntoArray(_itemDataList);
    }
}
