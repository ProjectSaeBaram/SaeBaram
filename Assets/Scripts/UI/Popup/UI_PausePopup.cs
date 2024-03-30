using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_PausePopup : UI_Popup
{
    enum Images
    {
        Background,
    }

    enum Buttons
    {
        SettingsBtn,
        BacktoMainMenuBtn,
        ExitBtn,
    }

    public override void Init()
    {
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));
        
        GetImage((int)Images.Background).gameObject.AddUIEvent(ClosePopupUI);
        GetButton((int)Buttons.SettingsBtn).gameObject.AddUIEvent(Settings);
        GetButton((int)Buttons.BacktoMainMenuBtn).gameObject.AddUIEvent(BacktoMainMenu);
        GetButton((int)Buttons.ExitBtn).gameObject.AddUIEvent(ExitGame);

        Time.timeScale = 0.0f;     // 일시정지
    }

    public override void ClosePopupUI(PointerEventData action)
    {
        Managers.UI.ClosePopupUI(this);
        Time.timeScale = 1.0f;
    }
    
    void BacktoMainMenu(PointerEventData action)
    {
        // TODO 데이터 저장
        //Managers.Data.SaveData();
        Managers.Scene.ChangeScene(Define.Scene.LobbyScene);
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
    }
    
}
