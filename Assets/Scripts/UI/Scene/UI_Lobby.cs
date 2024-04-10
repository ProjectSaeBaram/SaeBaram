using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Lobby : UI_Scene
{
    enum Buttons
    {
        LoadGameBtn,
        NewGameBtn,
        SettingsBtn,
        ExitGameBtn,
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        
        GetButton((int)Buttons.LoadGameBtn).gameObject.AddUIEvent(LoadGame);
        GetButton((int)Buttons.NewGameBtn).gameObject.AddUIEvent(NewGame);
        GetButton((int)Buttons.SettingsBtn).gameObject.AddUIEvent(Settings);
        GetButton((int)Buttons.ExitGameBtn).gameObject.AddUIEvent(ExitGame);
    }

    void LoadGame(PointerEventData data)
    {
        Managers.Scene.ChangeScene(Define.Scene.GameScene);
        Managers.Game.thisGameis = Define.ThisGameis.LoadedGame;
    }

    void NewGame(PointerEventData data)
    {
        Managers.Scene.ChangeScene(Define.Scene.GameScene);
        Managers.Game.thisGameis = Define.ThisGameis.NewGame;
    }

    void Settings(PointerEventData data)
    {
        // Managers.UI.ShowPopupUI<UI_SettingsPopup>();
    }

    public void ExitGame(PointerEventData data)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

}