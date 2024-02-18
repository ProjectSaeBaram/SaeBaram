using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI 버튼과 관련된 기능을 구현한 샘플 팝업 클래스.
/// UI_Popup 클래스를 상속받아, 버튼 클릭 및 기타 UI 요소와의 상호작용을 관리.
/// </summary>
public class UI_Button : UI_Popup
{
    // Buttons 열거형은 이 UI에서 관리할 버튼을 정의.
    enum Buttons
    {
        PointButton,
    }

    // Texts 열거형은 이 UI에서 관리할 텍스트를 정의.
    enum Texts
    {
        PointText,
        ScoreText,
    }

    // Images 열거형은 이 UI에서 관리할 이미지를 정의.
    enum Images
    {
        ItemIcon,
    }

    // GameObjects 열거형은 이 UI에서 관리할 기타 게임 오브젝트를 정의.
    enum GameObjects
    {
        TestObject,
    }

    /// <summary>
    /// UI 초기화 메서드. 상위 클래스의 초기화를 호출한 후, 이 클래스에서 관리할 UI 요소를 바인딩.
    /// </summary>
    public override void Init()
    {
        base.Init(); // 상위 클래스의 초기화 메서드 호출
        
        // 각 UI 요소를 바인딩.
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        // PointButton에 클릭 이벤트 핸들러를 추가.
        GetButton((int)Buttons.PointButton).gameObject.AddUIEvent(OnButtonClicked);

        // ItemIcon 이미지에 드래그 이벤트를 바인딩.
        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        BindEvent(go, (PointerEventData data) => { go.transform.position = data.position; }, Define.UIEvent.Drag);
    }
    
    private int _score = 0; // 점수를 저장하는 필드입니다.
    
    /// <summary>
    /// PointButton 클릭 시 호출되는 이벤트 핸들러.
    /// 클릭마다 _score를 증가시키고, ScoreText 텍스트를 업데이트.
    /// </summary>
    /// <param name="data">클릭 이벤트 데이터.</param>
    public void OnButtonClicked(PointerEventData data)
    {
        _score++; // 점수를 증가시킵니다.
        GetText((int)Texts.ScoreText).text = $"점수 : {_score}점"; // 점수 텍스트를 업데이트합니다.
    }
}