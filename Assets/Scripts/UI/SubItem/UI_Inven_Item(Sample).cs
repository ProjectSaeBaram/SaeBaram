using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 아이템 UI를 관리하는 클래스.
/// UI_Base를 상속받아, 인벤토리 아이템의 개별 UI 요소를 초기화하고 관리합니다.
/// 이런식으로 관리 가능한 UI 오소로는 아이콘이 있다.
/// </summary>
public class UI_Inven_Item : UI_Base, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // GameObjects 열거형은 인벤토리 아이템 UI 내에서 관리할 게임 오브젝트를 정의합니다.
    enum GameObjects
    {
        ItemNameText,   // 아이템 이름 표시 텍스트
        ItemNumberText, // 아이템 갯수 표시 텍스트
    }

    enum Images
    {
        ItemIcon,       // 아이템 아이콘 이미지
    }

    [Header("UI")] [SerializeField] public UI_InventoryPopup _uiInventoryPopup;
    [SerializeField] private Image image;
    [SerializeField] public string Name;   // 아이템의 이름을 저장하는 필드
    [SerializeField] public int Count;    // 아이템의 갯수를 저장하는 필드
    
    // 드래그 이후 부모 Transform을 저장하기 위함
    [SerializeField] public Transform parentAfterDrag;
    
    /// <summary>
    /// UI 요소들을 초기화하는 메서드
    /// </summary>
    public override void Init()
    {
        // UI_Base의 Bind 메서드를 사용하여 UI 요소들을 바인딩.
        Bind<GameObject>(typeof(GameObjects)); 
        Bind<Image>(typeof(Images));
        
        // 아이템 이름 텍스트 UI에 아이템 이름을 설정.
        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<TextMeshProUGUI>().text = Name;
        Get<GameObject>((int)GameObjects.ItemNumberText).GetComponent<TextMeshProUGUI>().text = Count.ToString();

        image = Get<Image>((int)Images.ItemIcon);
    }

    /// <summary>
    /// 아이템 정보를 설정하는 메서드.
    /// </summary>
    /// <param name="name">설정할 아이템의 이름.</param>
    /// <param name="number">설정할 아이템의 갯수</param>
    public void SetInfo(string name, int number)
    {
        Name = name;           // 아이템 이름을 저장.
        Count = number;       // 아이템 갯수를 저장.
    }

    #region Drag and Drop

    private Vector3 DragOffset;
    
    public void OnBeginDrag(PointerEventData eventData) {
        
        // image.raycastTarget = true 면, 최상위가 자기 자신이라서 InventorySlot을 지정할 수 없음. 
        image.raycastTarget = false;
        // 정확한 위치에 옮기지 않았을 경우 원래 자리로 되돌아가기 위함.
        parentAfterDrag = transform.parent;
        // 하이어러키에서 제일 밑에 가도록 해서 가장 위에 보이도록 하기위함.
        transform.SetParent(_uiInventoryPopup.transform);

        // 마우스 커서 클릭 위치 오프셋 적용
        DragOffset = transform.position - Input.mousePosition;
    }
    
    public void OnDrag(PointerEventData eventData) {
        
        // InvntoryItem의 위치를 마우스 위치로 이동
        transform.position = Input.mousePosition + DragOffset;
    }
    
    public void OnEndDrag(PointerEventData eventData) {
        
        image.raycastTarget = true;
        // 중간에 parentAfterDrag가 변경되지 않았으면 원래 위치로 복귀. 중간에 바뀌었으면 다른 위치로 이동.
        transform.SetParent(parentAfterDrag);
        
        transform.localPosition = Vector3.zero;
    }

    #endregion
}