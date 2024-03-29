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

        Managers.Data.OnClose = null;
        //Managers.Data.OnClose += ExportInventoryData; TODO
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
            if (item.Name == "NONE")                    // 이름이 NONE 이라는 것은 빈칸이라는 의미
            {
                visualizedItems.Add(null);
                continue;
            }

            switch (item)
            {
                case ITool:
                {
                    Tool tool = item as Tool;
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(itemSlots[i].transform));
                    visualizedItems[i].ToolInit(tool!.Name, tool!.Durability, tool!.ReinforceCount);
                    visualizedItems[i]._uiInventoryPopup = this;
                    break;
                }
                case IStackable:
                {
                    Ingredient ingredient = item as Ingredient;
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(itemSlots[i].transform));
                    visualizedItems[i].IngredientInit(ingredient!.Name, ingredient!.Amount);
                    visualizedItems[i]._uiInventoryPopup = this;
                    break;
                }
            }
            

            itemSlots[i].Item = visualizedItems[i];
        }
    }

    /// <summary>
    /// 인벤토리가 열려있는 중에, 아이템 목록의 변화가 일어났을 때 갱신하고, 다시 시각화해주는 기능.
    /// </summary>
    // private void RefreshItemUI() TODO
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

    /// <summary> TODO
    /// 팝업이 닫힐 때는 인벤토리 정보를 다시 바이너리 파일로 저장
    /// </summary>
    // public void ExportInventoryData()
    // {
    //     for (int i = 0; i < numberOfItemSlots; i++)
    //     {
    //         if (itemSlots[i].Item == null)
    //         {
    //             // 여기서 갯수를 1로 지정하지 않으면 바이너리 파일에 0만 찍혀서 저장되지 않는다.
    //             // 빈 공간으로도 인식되지 못한다
    //             _itemDataList[i] = new ItemData("NONE", 1);
    //         }
    //         else
    //         {
    //             UI_Inven_Slot slot = itemSlots[i];
    //             _itemDataList[i] = new ItemData(slot.Item.Name, slot.Item.Count);
    //         }
    //     }
    //
    //     Managers.Data.TransDataListIntoArray(_itemDataList);
    // }
}
