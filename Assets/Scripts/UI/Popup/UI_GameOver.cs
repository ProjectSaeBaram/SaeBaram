using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GameOver : UI_Popup
{
    
    enum Buttons
    {
        SettingsBtn,
        BacktoMainMenuBtn,
        ExitBtn,
    }
    
    
    public override void Init()
    {
        base.Init();
        
        Bind<Button>(typeof(Buttons));
        
        GetButton((int)Buttons.SettingsBtn).gameObject.AddUIEvent(Settings);
        GetButton((int)Buttons.BacktoMainMenuBtn).gameObject.AddUIEvent(BacktoMainMenu);
        GetButton((int)Buttons.ExitBtn).gameObject.AddUIEvent(ExitGame);
        
        Time.timeScale = 0.0f;
    }
    
    void BacktoMainMenu(PointerEventData action)
    {
        // TODO 데이터 저장
        //Managers.Data.SaveData();
        Managers.Scene.ChangeScene(Define.Scene.LobbyScene);
        Time.timeScale = 1.0f;
    }
    
    void Settings(PointerEventData action)
    {
        Managers.UI.ShowPopupUI<UI_SettingsPopup>();
    }
    
    public void ExitGame(PointerEventData action)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
        Time.timeScale = 1.0f;
    }

}
