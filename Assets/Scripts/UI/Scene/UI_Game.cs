using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Game : UI_Scene
{
    enum Buttons
    {
        EscapeButton,
    }
    
    public override void Init()
    {
        base.Init(); // 상위 클래스의 초기화 메서드 호출

        Bind<Button>(typeof(Buttons));
        
        GetButton((int)Buttons.EscapeButton).gameObject.AddUIEvent(PauseOrResume);
    }

    void PauseOrResume(PointerEventData eventData)
    {
        // 1. 뭐든지 열려있으면 다 닫기
        // 2. 아무것도 없으면 열기

        if (Managers.UI.GetStackSize() > 0)
            Managers.UI.CloseAllPopupUI();
        else
            Managers.UI.ShowPopupUI<UI_PausePopup>();
    }
}


