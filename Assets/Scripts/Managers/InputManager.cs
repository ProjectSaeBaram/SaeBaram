using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 입력 관리를 담당하는 매니저.
/// 키보드 및 마우스 입력을 감지하고, 정의된 액션에 따라 이벤트를 발생시킴.
/// 특히, 마우스 입력이 중요한 게임(예: 디아블로, 롤 등등...)에 유용하게 사용됨.
/// </summary>
public class InputManager
{
    // 키보드 입력이 감지될 때 호출될 액션.
    public Action KeyAction = null;
    // 마우스 이벤트가 감지될 때 호출될 액션.
    public Action<Define.MouseEvent> MouseAction = null;

    // 마우스 버튼이 눌려있는지 여부를 추적하는 변수.
    private bool _pressed = false;
    
    // 마우스 버튼을 누르고 있는 시간을 기록하기 위한 변수.
    // Click과 PointerUp 이벤트를 구분하기 위해 사용됨.
    private float _pressedTime = 0;
    
    /// <summary>
    /// 매 프레임마다 입력을 감지하고 액션을 호출하는 메서드.
    /// </summary>
    public void OnUpdate()
    {
        // UI 요소가 클릭된 경우 입력 처리를 하지 않는다.
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        // 키보드 입력이 있고, KeyAction이 설정되어 있다면 액션을 호출.
        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        // 마우스 액션이 설정되어 있다면 마우스 입력을 처리.
        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0)) // 마우스 왼쪽 버튼이 눌려있는 경우
            {
                if (!_pressed) // 마우스 버튼이 처음 눌렸다면 PointerDown 이벤트를 발생시킴.
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _pressedTime = Time.time; // 누르고 있는 시간을 기록 시작.
                }
                MouseAction.Invoke(Define.MouseEvent.Press); // Press 이벤트를 계속 발생시킴.
                _pressed = true;
            }
            else // 마우스 왼쪽 버튼에서 손을 뗐을 경우
            {
                if (_pressed) // 마우스 버튼이 눌렸다가 떼어졌다면
                {
                    if(Time.time < _pressedTime + 0.2f) // 짧은 시간 내에 떼었다면 Click 이벤트를 발생시킴.
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    
                    MouseAction.Invoke(Define.MouseEvent.PointerUp); // PointerUp 이벤트를 발생시킴.
                }

                _pressed = false;
                _pressedTime = 0; // 시간 기록을 초기화.
            }
        }
    }

    /// <summary>
    /// 입력 관리자를 초기화하는 메서드.
    /// 등록된 모든 액션을 제거.
    /// </summary>
    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
}
