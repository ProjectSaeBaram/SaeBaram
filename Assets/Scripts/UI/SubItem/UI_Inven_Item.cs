using TMPro;
using UnityEngine;

/// <summary>
/// 인벤토리 아이템 UI를 관리하는 샘플 클래스.
/// UI_Base를 상속받아, 인벤토리 아이템의 개별 UI 요소를 초기화하고 관리합니다.
/// 이런식으로 관리 가능한 UI 오소로는 아이콘이 있습니다.
/// </summary>
public class UI_Inven_Item : UI_Base
{
    // GameObjects 열거형은 인벤토리 아이템 UI 내에서 관리할 게임 오브젝트를 정의합니다.
    enum GameObjects
    {
        ItemIcon,      // 아이템 아이콘 이미지
        ItemNameText,  // 아이템 이름 표시 텍스트
    }

    private string _name; // 아이템의 이름을 저장하는 필드입니다.
    
    /// <summary>
    /// UI 요소들을 초기화하는 메서드
    /// </summary>
    public override void Init()
    {
        // UI_Base의 Bind 메서드를 사용하여 UI 요소들을 바인딩.
        Bind<GameObject>(typeof(GameObjects)); 

        // 아이템 이름 텍스트 UI에 아이템 이름을 설정합니다.
        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<TextMeshProUGUI>().text = _name;
        
        // 아이템 아이콘에 클릭 이벤트 리스너를 추가합니다.
        Get<GameObject>((int)GameObjects.ItemIcon).AddUIEvent((pointerEventData) => {Debug.Log($"Item Clicked! {_name}");});
    }

    /// <summary>
    /// 아이템 정보를 설정하는 메서드.
    /// </summary>
    /// <param name="name">설정할 아이템의 이름.</param>
    public void SetInfo(string name)
    {
        _name = name; // 아이템 이름을 저장.
    }

}