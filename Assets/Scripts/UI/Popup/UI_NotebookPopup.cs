using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_NotebookPopup : UI_Popup, ITooltipHandler,ICatcher
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
    [SerializeField] public GameObject VisualizedLayer;

    //[SerializeField] public UI_Inven_Item CatchedItem = null;

    // ICatcher 구현
    public UI_Inven_Item CatchedItem { get; set; } // ICatcher에서 요구하는 선택된 아이템 필드

    [FormerlySerializedAs("uiItemTooltip")] [SerializeField] private UI_Inven_ItemTooltip uiInvenItemTooltip;

    [SerializeField] private UI_Game_QuickSlotGroup _quickSlotGroup;
    
    private bool initialized = false;
    
    void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        // 팝업을 끌 때, DataManager와 통신하여 인벤토리 데이터를 저장.
        Managers.Data.OnClose -= ExportInventoryData;
        Managers.Data.OnClose -= Managers.Data.SaveInventoryData;
        Managers.Data.OnClose += ExportInventoryData;
        Managers.Data.OnClose += Managers.Data.SaveInventoryData;
        
        if(initialized)
            WhenOpened();
    }

    public override void Init()
    {
        base.Init();
        int currentOrder = GetComponent<Canvas>().sortingOrder;
        Managers.UI.GetCurrentSceneUI().GetComponent<Canvas>().sortingOrder = currentOrder;
        
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        
        uiInvenItemTooltip = Get<GameObject>((int)GameObjects.ItemToolTip).GetComponent<UI_Inven_ItemTooltip>();
        Get<GameObject>((int)GameObjects.ItemToolTip).SetActive(false);
        
        // 여기 안에서 동작하는 함수들의 순서는 서로 매우 긴밀하게 연결되어있음. 조작 시 주의할 것.
        
        // 북마크들을 각각 자신에 해당하는 레이어들과 연결
        ConnectBookmarksIntoLayers();
        
        WhenOpened();

        UI_Game_QuickSlotGroup quickSlotGroup = FindObjectOfType<UI_Game_QuickSlotGroup>();
        quickSlotGroup.InitQuickSlotsNotebookRef(this);
        
        initialized = true;
    }

    /// <summary>
    /// 팝업이 열릴 때, CraftingLayer를 시각화. (기본값)
    /// 인벤토리 데이터도 함께 로드
    /// </summary>
    void WhenOpened()
    {
        //DebugEx.LogWarning("WhenOpened!");
        CloseAllLayer();

        Get<Image>((int)Images.NoteBook_Background).gameObject.SetActive(true);
        
        VisualizedLayer = Get<GameObject>((int)GameObjects.CraftingLayer);
        VisualizedLayer.SetActive(true);
        
        VisualizeItemsInTheGrid(true);
        
        // 노트북 팝업이 열릴 때는 시간 정지
        Time.timeScale = 0;

        // 플레이어 액션 일부 비활성화 
        DisablePickupItem();
    }

    /// <summary>
    /// 플레이어 아이템 줍기 기능 & 공격 기능 비활성화 
    /// </summary>
    void DisablePickupItem()
    {
        PlayerController player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        player.DisablePickupItem();
        player.DisableClick();
    }
    
    /// <summary>
    /// 플레이어 아이템 줍기 기능 & 공격 기능 활성화 
    /// </summary>
    void EnablePickupItem()
    {
        PlayerController player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        player.EnablePickupItem();
        player.EnableClick();
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
        visualizedItems = new List<UI_Inven_Item>();
        
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
            //UI_Inven_Slot slot = Managers.UI.MakeSubItem<UI_Inven_Slot>(content);
            //itemSlots.Add(slot);
            //slot.SetNotebookPopup(this);
            UI_Inven_Slot slot = itemSlots[i];
            slot.SlotIndex = i;
            
            // 아이템
            ItemData item = _itemDataList[i];

            switch (item)       // 아이템의 종류에 따라 다르게 시각화
            {
                case Tool tool:
                {
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(itemSlots[i].transform));
                    visualizedItems[i].parentPanel = VisualizedLayer;
                    visualizedItems[i].ToolInit(tool!.Name, tool!.Quality, tool!.Durability, tool!.ReinforceCount, tool!.Logs);
                    break;
                }
                case Ingredient ingredient:
                {
                    visualizedItems.Add(Managers.UI.MakeSubItem<UI_Inven_Item>(itemSlots[i].transform));
                    visualizedItems[i].parentPanel = VisualizedLayer;
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
        Managers.Data.LoadInventoryData();
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
        if (Managers.UI.GetTopPopupUI() != this) return;
        
        // 인벤토리의 데이터 저장
        Managers.Data.OnClose?.Invoke();    // Test할 때 발생하는 오류를 막기 위해 ? (Nullable) 추가.
        
        // 시간은 다시 흘러간다
        Time.timeScale = 1;
        
        // 데이터 저장 UnityAction 관리
        Managers.Data.OnClose -= ExportInventoryData;
        Managers.Data.OnClose -= Managers.Data.SaveInventoryData;

        if (CatchedItem != null)
            Managers.Data.RemoveItemFromInventory(CatchedItem);
        
        // 아이템 제거 & 손에 들고있던 아이템은 드랍
        // for (int i = 0; i < numberOfItemSlots; i++)
        // {
        //     // // 인벤 슬롯의 아이템이 슬롯 안에 없으면,
        //     // if (itemSlots[i]?.item.transform.parent != itemSlots[i].transform)
        //     // {
        //     //     Managers.Data.RemoveItemFromInventory(itemSlots[i].item);
        //     // }
        //     DestroyImmediate(itemSlots[i].gameObject);
        // }

        foreach (var slot in itemSlots)
        {
            if(slot.Item != null)
                DestroyImmediate(slot.Item.gameObject);
        }
        visualizedItems.Clear();
        
        // 플레이어 아이템 줍기 기능 활성화 
        EnablePickupItem();
        
        base.ClosePopupUI(action);
        
        DebugEx.LogWarning("Notebook Popup Closed!");
    }
}
