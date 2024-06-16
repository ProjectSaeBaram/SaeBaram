using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        
        GetButton((int)Buttons.EscapeButton).gameObject.AddUIEvent((eventData) => Managers.UI.ShowPopupUI<UI_PausePopup>());
    }

}
