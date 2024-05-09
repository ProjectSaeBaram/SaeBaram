using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_NotebookPopup : UI_Popup
{
    public enum GameObjects
    {
        CraftingLayer,
        ReinforcingLayer,
        QuestLayer,
        DiaryLayer,
        MapLayer,
        LastPage,
        ItemToolTip,
    }

    enum Images
    {
        BackPanel,
        NoteBook_Background,
    }
    
    enum Buttons
    {
        Bookmark_CraftingLayer,
        Bookmark_ReinforcingLayer,
        Bookmark_QuestLayer,
        Bookmark_DiaryLayer,
        Bookmark_MapLayer,
        Bookmark_LastPage,
        
        LastPage_Bookmark_CraftingLayer,
        LastPage_Bookmark_ReinforcingLayer,
        LastPage_Bookmark_QuestLayer,
        LastPage_Bookmark_DiaryLayer,
        LastPage_Bookmark_MapLayer,
    }

    // Key : Bookmark, Value : Layer
    private Dictionary<Buttons, GameObjects> BookmarksAndLayers = new();
    
    // 현재 시각화된 Layer
    [SerializeField] private GameObject VisualizedLayer;

    [SerializeField] public UI_Inven_Item CatchedItem = null;

    [SerializeField] private UI_ItemTooltip uiItemTooltip;
    
    void Start()
    {
        Init();
    }
    
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        Get<Image>((int)Images.BackPanel).gameObject.AddUIEvent(ClosePopupUI);

        uiItemTooltip = Get<GameObject>((int)GameObjects.ItemToolTip).GetComponent<UI_ItemTooltip>();
        Get<GameObject>((int)GameObjects.ItemToolTip).SetActive(false);
        
        // 여기 안에서 동작하는 함수들의 순서는 서로 매우 긴밀하게 연결되어있음. 조작 시 주의할 것.
        
        // 북마크들을 각각 자신에 해당하는 레이어들과 연결
        ConnectBookmarksIntoLayers();
        
        WhenOpened();
        
        // 팝업을 끌 때, DataManager와 통신하여 인벤토리 데이터를 저장.
        // Managers.Data.OnClose -= ExportInventoryData;
        // Managers.Data.OnClose -= Managers.Data.SaveInventoryData;
        Managers.Data.OnClose += ExportInventoryData;
        Managers.Data.OnClose += Managers.Data.SaveInventoryData;
        
        // 노트북 팝업이 열릴 때는 시간 정지
        Time.timeScale = 0;
    }

    /// <summary>
    /// 팝업이 열릴 때, CraftingLayer를 시각화. (기본값)
    /// 인벤토리 데이터도 함께 로드
    /// </summary>
    void WhenOpened()
    {
        CloseAllLayer();

        Get<Image>((int)Images.NoteBook_Background).gameObject.SetActive(true);
        
        VisualizedLayer = Get<GameObject>((int)GameObjects.CraftingLayer);
        VisualizedLayer.SetActive(true);
        
        VisualizeItemsInTheGrid(true);
    }
    
    #region Inventory
    
    [Header("Items")] 
    private const int numberOfItemSlots = 30;       // 테스트용 기본 아이템 슬롯의 크기
    [SerializeField] private List<UI_Inven_Slot> itemSlots = new List<UI_Inven_Slot>();
    [SerializeField] private List<UI_Inven_Item> visualizedItems = new List<UI_Inven_Item>();
    private List<ItemData> _itemDataList = new List<ItemData>();

    /// <summary>
    /// 인벤토리가 열릴 때, 아이템을 불러들여 시각화하는 기능
    /// </summary>
    public void VisualizeItemsInTheGrid(bool initialize = false)
    {
        // 아이템 슬롯들이 생성되어야하는 곳
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
            // 아이템 슬롯
            UI_Inven_Slot slot = Managers.UI.MakeSubItem<UI_Inven_Slot>(content);
            slot.SetNotebookPopup(this);
            itemSlots.Add(slot);
            
            // 아이템
            ItemData item = _itemDataList[i];

            switch (item)       // 아이템의 종류에 따라 다르게 시각화
            {
                case Tool tool:
                {
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(itemSlots[i].transform));
                    visualizedItems[i].parentPanel = VisualizedLayer;
                    visualizedItems[i].Init();
                    visualizedItems[i].ToolInit(tool!.Name, tool!.Quality, tool!.Durability, tool!.ReinforceCount, tool!.Logs);
                    break;
                }
                case Ingredient ingredient:
                {
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(itemSlots[i].transform));
                    visualizedItems[i].parentPanel = VisualizedLayer;
                    visualizedItems[i].Init();
                    visualizedItems[i].IngredientInit(ingredient!.Name, ingredient!.Quality, ingredient!.Amount, ingredient!.Logs);
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

    /// <summary>
    /// DataManager를 통해 캐싱된 데이터를 읽어오는 기능
    /// </summary>
    private void GetItemDataFromDataManager()
    {
        _itemDataList = Managers.Data.ItemInfos();
    }

    /// <summary>
    /// 팝업이 닫힐 때는 인벤토리 정보를 다시 바이너리 파일로 저장
    /// DataManager에게 정보를 전달
    /// </summary>
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
                    case UI_Inven_Item.ItemType.Tool:
                        itemData = new Tool(0, itemUI.Name, itemUI.Quality, itemUI.Durability, itemUI.ReinforceCount);
                        break;
                    case UI_Inven_Item.ItemType.Ingredient:
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

    public void ShowToolTip(UI_Inven_Item invenItem, PointerEventData eventData)
    {
        uiItemTooltip.gameObject.SetActive(true);
        uiItemTooltip.ShowTooltip(invenItem, eventData);
    }

    public void HideTooltip()
    {
        uiItemTooltip.gameObject.SetActive(false);
        uiItemTooltip.UnsetPointerEventData();
    }
    
    #endregion
    
    /// <summary>
    /// 모든 북마크들에게 레이어들을 매칭시키는 함수
    /// </summary>
    void ConnectBookmarksIntoLayers()
    {
        BookmarksAndLayers.Add(Buttons.Bookmark_CraftingLayer, GameObjects.CraftingLayer);
        BookmarksAndLayers.Add(Buttons.Bookmark_ReinforcingLayer, GameObjects.ReinforcingLayer);
        BookmarksAndLayers.Add(Buttons.Bookmark_QuestLayer, GameObjects.QuestLayer);
        BookmarksAndLayers.Add(Buttons.Bookmark_DiaryLayer, GameObjects.DiaryLayer);
        BookmarksAndLayers.Add(Buttons.Bookmark_MapLayer, GameObjects.MapLayer);
        BookmarksAndLayers.Add(Buttons.Bookmark_LastPage, GameObjects.LastPage);
        BookmarksAndLayers.Add(Buttons.LastPage_Bookmark_CraftingLayer, GameObjects.CraftingLayer);
        BookmarksAndLayers.Add(Buttons.LastPage_Bookmark_ReinforcingLayer, GameObjects.ReinforcingLayer);
        BookmarksAndLayers.Add(Buttons.LastPage_Bookmark_QuestLayer, GameObjects.QuestLayer);
        BookmarksAndLayers.Add(Buttons.LastPage_Bookmark_DiaryLayer, GameObjects.DiaryLayer);
        BookmarksAndLayers.Add(Buttons.LastPage_Bookmark_MapLayer, GameObjects.MapLayer);

        foreach (var pair in BookmarksAndLayers)
            Get<Button>((int)pair.Key).onClick.AddListener(() => CloseAllLayerWithException(pair.Value));
    }
    
    void CloseAllLayer()
    {
        foreach (var pair in BookmarksAndLayers)
            Get<GameObject>((int)pair.Value).SetActive(false);
    }
    
    private void CloseAllLayerWithException(GameObjects exceptLayer)
    {
        CloseAllLayer();

        Get<Image>((int)Images.NoteBook_Background).gameObject
            .SetActive(exceptLayer != GameObjects.LastPage);
        
        VisualizedLayer = Get<GameObject>((int)exceptLayer);
        VisualizedLayer.SetActive(true);
        
        if (exceptLayer is GameObjects.CraftingLayer or GameObjects.ReinforcingLayer)
            VisualizeItemsInTheGrid();
    }
    
    public override void ClosePopupUI(PointerEventData action)
    {
        
        // 인벤토리의 데이터 저장
        Managers.Data.OnClose?.Invoke();    // Test할 때 발생하는 오류를 막기 위해 ? (Nullable) 추가.
        
        Time.timeScale = 1;
        Managers.Data.OnClose -= ExportInventoryData;
        Managers.Data.OnClose -= Managers.Data.SaveInventoryData;
        
        base.ClosePopupUI(action);
    }
}
