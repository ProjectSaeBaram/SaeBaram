using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 아이템 UI를 관리하는 클래스.
/// UI_Base를 상속받아, 인벤토리 아이템의 개별 UI 요소를 초기화하고 관리합니다.
/// 이런식으로 관리 가능한 UI 오소로는 아이콘이 있다.
/// </summary>
public class UI_Inven_Item : UI_Base, IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerClickHandler, IPointerUpHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private readonly int Max_Amount = 63;
    
    [FormerlySerializedAs("itemDatabase")] public ItemSpriteDatabase itemSpriteDatabase;       // 참조할 아이템 데이터베이스

    private RectTransform _rectTransform;

    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI ItemNameText;              // 아이템 이름 표시 텍스트
    [SerializeField] private TextMeshProUGUI ItemAmountText;            // 아이템 갯수 표시 텍스트
    [SerializeField] private TextMeshProUGUI ItemReinforceCount;        // 아이템 강화 횟수 표시 텍스트
    [SerializeField] public Image image;                                // 아이템 이미지
    [SerializeField] private Slider DurabilitySlider;                   // 아이템 내구도 표시 슬라이더
    
    [SerializeField] public GameObject parentPanel;
    [SerializeField] public string Name;                // 아이템의 이름을 저장하는 필드
    [SerializeField] public int Quality;                // 아이템의 퀄리티를 저장하는 필드 (0~3)
                                                        // {0 : 비정상 값}, {1 : 하}, {2 : 중}, {3 : 상}
    [SerializeField] public int Amount = 1;             // 아이템의 갯수를 저장하는 필드 (0~63)
    [SerializeField] public int Durability = 1;         // 아이템의 내구도를 저장하는 필드 (0~15)
    public float maxDurability = 15f;
    [SerializeField] public int ReinforceCount = 0;     // 아이템의 강화 횟수를 저장하는 필드 (0~3)
    [SerializeField] public Define.ItemType itemType;          // 아이템의 타입을 저장하기 위한 필드

    public UI_NotebookPopup UINotebookPopup;
    
    [Header("Logs")] [SerializeField] public List<string> Logs;
    
    // 드래그 이후 부모 Transform을 저장하기 위함
    [SerializeField] public Transform parentAfterDrag;

    [SerializeField] private bool isCatched = false;

    public Action OnValueChange = null;
    
    /// <summary>
    /// UI 요소들을 초기화하는 메서드
    /// </summary>
    public override void Init()
    {
        OnValueChange -= RefreshUI;
        OnValueChange += RefreshUI;
        
        UINotebookPopup = Managers.UI.GetTopPopupUI() as UI_NotebookPopup;

        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// 이 아이템이 도구인 경우는 갯수를 표시할 필요가 없다.
    /// </summary>
    public void ToolInit(string name, int quality, int durability, int reinforceCount, List<string> logs)
    {
        Name = name;                        // 아이템 이름을 저장.
        Quality = quality;                  // 아이템 품질을 저장.
        Durability = durability;            // 아이템 내구도를 저장.
        ReinforceCount = reinforceCount;    // 아이템의 강화 횟수를 저장.
        
        // 이미지 설정
        if (itemSpriteDatabase != null && image != null)
        {
            int itemId = Managers.Data.reverseItemCodeDict[Name];
            Sprite itemSprite = itemSpriteDatabase.GetItemImageById(itemId);
            if (itemSprite != null) image.sprite = itemSprite;
        }
        
        // 아이템 이름 텍스트 UI에 아이템 이름을 설정.
        DurabilitySlider.value = Durability / maxDurability;
        ItemReinforceCount.text = ReinforceCount.ToString();
        ItemAmountText.gameObject.SetActive(false);

        // 로그 받아오기
        Logs = logs;
        
        itemType = Define.ItemType.Tool;
        parentAfterDrag = transform.parent;
    }

    /// <summary>
    /// 이 아이템이 재료인 경우에는, 내구도와 강화횟수를 표시할 필요가 없고, 갯수를 표시해야 한다.
    /// </summary>
    public void IngredientInit(string name, int quality, int amount, List<string> logs)
    {
        Name = name;                        // 아이템 이름을 저장.
        Quality = quality;                  // 아이템 품질을 저장.
        Amount = amount;                    // 아이템 갯수를 저장.
        
        // 이미지 설정
        if (itemSpriteDatabase != null && image != null)
        {
            int itemId = Managers.Data.reverseItemCodeDict[Name];
            Sprite itemSprite = itemSpriteDatabase.GetItemImageById(itemId);
            if (itemSprite != null) image.sprite = itemSprite;
        }
        
        // 아이템 이름 텍스트 UI에 아이템 이름을 설정.
        // Get<TextMeshProUGUI>((int)Texts.ItemNameText).text = Name;
        DurabilitySlider.gameObject.SetActive(false);
        ItemReinforceCount.gameObject.SetActive(false);
        ItemAmountText.text = Amount.ToString();

        // 로그 받아오기
        Logs = logs;
        
        itemType = Define.ItemType.Ingredient;
        parentAfterDrag = transform.parent;
    }

    private void Update()
    {
        if (!isCatched) return;

        transform.position = Input.mousePosition;
    }

    private void RefreshUI()
    {
        // 아이템 이름 텍스트 UI에 아이템 이름을 설정.
        // Get<TextMeshProUGUI>((int)Texts.ItemNameText).text = Name;
        ItemAmountText.text = Amount.ToString();
        ItemReinforceCount.text = ReinforceCount.ToString();
        DurabilitySlider.value = Durability / maxDurability;
    }

    #region Drag and Drop

    private Vector3 _dragOffset;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isCatched) return;
        
        // image.raycastTarget = true 면, 최상위가 자기 자신이라서 InventorySlot을 지정할 수 없음. 
        image.raycastTarget = false;
        // 정확한 위치에 옮기지 않았을 경우 원래 자리로 되돌아가기 위함.
        parentAfterDrag = transform.parent;
        // 하이어러키에서 제일 밑에 가도록 해서 가장 위에 보이도록 하기위함.
        transform.SetParent(parentPanel.transform);

        // 마우스 커서 클릭 위치 오프셋 적용
        _dragOffset = transform.position - Input.mousePosition;
    }
    
    public void OnDrag(PointerEventData eventData) {
        
        // InvntoryItem의 위치를 마우스 위치로 이동
        transform.position = Input.mousePosition + _dragOffset;
    }
    
    public void OnEndDrag(PointerEventData eventData) {
        
        if (isCatched) return;
        
        image.raycastTarget = true;
        // 중간에 parentAfterDrag가 변경되지 않았으면 원래 위치로 복귀. 중간에 바뀌었으면 다른 위치로 이동.
        transform.SetParent(parentAfterDrag);
        
        _rectTransform.anchoredPosition = Vector2.zero;
        _rectTransform.localScale = Vector2.one;

        foreach (var hoverd in eventData.hovered)
        {
            if(hoverd.GetComponent<NotebookBackPanel>() != null)
            {
                Managers.Data.RemoveItemFromInventory(this);
                UINotebookPopup.ClosePopupUI(eventData);
            }
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UI_Inven_Item catchedItem = UINotebookPopup.CatchedItem;
            if (catchedItem != null && catchedItem?.itemType == Define.ItemType.Ingredient && this.itemType == Define.ItemType.Ingredient && catchedItem.Name == this.Name
                && catchedItem?.Quality == this.Quality)
            {
                if (catchedItem.Amount + this.Amount > Max_Amount)
                {
                    catchedItem.Amount = catchedItem.Amount + this.Amount - Max_Amount;
                    this.Amount = Max_Amount;
                
                    catchedItem.transform.position = Vector3.zero;
                    //RefreshUI();
                    OnValueChange.Invoke();
                    // catchedItem.RefreshUI();
                    catchedItem.OnValueChange.Invoke();
                }
                else
                {
                    // (+ 만약 이 아이템이 CraftingSlot에서 꺼내어졌다면, 이 아이템이 없는 버전으로 검색해야한다)
                    if (catchedItem.parentAfterDrag.GetComponent<UI_Inven_Slot>() is UI_Inven_CraftingSlot)
                    {
                        this.Amount += catchedItem.Amount;
                        DestroyImmediate(catchedItem.gameObject);
                        catchedItem = null;
                        OnValueChange.Invoke();
                    
                        // 검색
                        Managers.Crafting.OnItemForCraftingChanged.Invoke();
                    }
                    else
                    {
                        this.Amount += catchedItem.Amount;
                        DestroyImmediate(catchedItem.gameObject);
                        catchedItem = null;
                        OnValueChange.Invoke();
                    }
                }
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var item = eventData.pointerDrag.GetComponent<UI_Inven_Item>();
        if (item != null && item?.itemType == Define.ItemType.Ingredient && this.itemType == Define.ItemType.Ingredient && item?.Name == this.Name 
            && item?.Quality == this.Quality)
        {
            if (item.Amount + this.Amount > Max_Amount)
            {
                item.Amount = item.Amount + this.Amount - Max_Amount;
                this.Amount = Max_Amount;
                
                item.transform.position = Vector3.zero;
                //RefreshUI();
                OnValueChange.Invoke();
                // item.RefreshUI();
                item.OnValueChange.Invoke();
            }
            else
            {
                // (+ 만약 이 아이템이 CraftingSlot에서 꺼내어졌다면, 이 아이템이 없는 버전으로 검색해야한다)
                if (item.parentAfterDrag.GetComponent<UI_Inven_Slot>() is UI_Inven_CraftingSlot)
                {
                    this.Amount += item.Amount;
                    DestroyImmediate(item.gameObject);
                    item = null;
                    OnValueChange.Invoke();
                    
                    // 검색
                    Managers.Crafting.OnItemForCraftingChanged.Invoke();
                }
                else
                {
                    this.Amount += item.Amount;
                    DestroyImmediate(item.gameObject);
                    item = null;
                    OnValueChange.Invoke();
                }
            }
        }
    }
    
    #endregion
    
    #region Click

    public void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭인 경우에 동작
        if (eventData.button == PointerEventData.InputButton.Right && !isCatched && itemType != Define.ItemType.Tool && UINotebookPopup.CatchedItem == null)
        {
            UI_SeparateIngredientPopup popup = Managers.UI.ShowPopupUI<UI_SeparateIngredientPopup>();
            popup.InitItemReference(this);
        }
    }

    public void Catched()
    {
        isCatched = true;
        image.color = new Color(1,1,1,0.8f);
        UINotebookPopup.CatchedItem = this;
        image.raycastTarget = false;
    }

    public void Released()
    {
        isCatched = false;
        image.color = new Color(1,1,1,1);
        _rectTransform.anchoredPosition = Vector2.zero;
        _rectTransform.localScale = Vector2.one;
        image.raycastTarget = true;
    }
    
    #endregion
    
    #region Tooltip

    public void OnPointerEnter(PointerEventData eventData)
    {
        UINotebookPopup.ShowToolTip(this, eventData);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UINotebookPopup.HideTooltip();
    }
    
    #endregion
    
}