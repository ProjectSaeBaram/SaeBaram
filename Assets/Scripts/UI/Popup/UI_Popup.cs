using UnityEngine.EventSystems;

/// <summary>
/// 팝업 UI를 관리하는 기본 클래스.
/// UI_Base를 상속받아, 팝업 UI의 기본적인 초기화와 닫기 동작을 구현.
/// </summary>
public class UI_Popup : UI_Base
{
    /// <summary>
    /// UI 요소들을 초기화하는 메서드.
    /// </summary>
    public override void Init()
    {
        // Managers.UI.SetCanvas를 호출하여 이 UI가 속한 GameObject의 캔버스 설정을 조정.
        Managers.UI.SetCanvas(gameObject, true);
    }

    /// <summary>
    /// 팝업 UI를 닫는 메서드.
    /// </summary>
    public virtual void ClosePopupUI(PointerEventData action)
    {
        // Managers.UI.ClosePopupUI를 호출하여 현재 팝업 UI를 닫음.
        // 이 메서드는 상속받는 팝업 UI 클래스에서 오버라이드하여
        // 특정 닫기 동작을 추가하여 구현 가능.
        //Managers.UI.ClosePopupUI(this);
        
        // UIManager를 통해 팝업 제거를 요청.
        Managers.UI.RemovePopup(this);
    }
}