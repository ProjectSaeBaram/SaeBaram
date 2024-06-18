/// <summary>
/// 씬에 대한 UI를 관리하는 기본 클래스.
/// UI_Base를 상속받아, 씬 전반에 걸친 UI 요소의 초기화를 담당.
/// </summary>
public class UI_Scene : UI_Base
{
    /// <summary>
    /// UI 초기화 메서드.
    /// 씬에 속한 UI 요소들을 초기화하고, 씬의 캔버스 설정을 담당.
    /// </summary>
    public override void Init()
    {
        // Managers.UI.SetCanvas를 호출하여 이 UI가 속한 GameObject의 캔버스 설정을 조정.
        Managers.UI.SetCanvas(gameObject, false);
    }
}