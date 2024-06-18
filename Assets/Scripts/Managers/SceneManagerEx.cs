using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene을 관리하기 위한 Manager
/// </summary>
public class SceneManagerEx
{
    // 현재 Scene의 종류
    private Define.Scene _curSceneType = Define.Scene.Unknown;
    
    // 현재 Scene의 종류에 접근하는 프로퍼티
    public Define.Scene CurrentSceneType
    {
        get
        {
            if (_curSceneType != Define.Scene.Unknown)
                return _curSceneType;
            return CurrentScene.SceneType;
        }
        set => _curSceneType = value;
    }
    
    /// <summary>
    /// 현재 활성화된 Scene의 BaseScene타입의 인스턴스
    /// </summary>
    public BaseScene CurrentScene => GameObject.FindObjectOfType<BaseScene>();

    /// <summary>
    /// 지정한 Scene을 Load하는 기능.
    /// </summary>
    /// <param name="type">정해져 있는 enum으로 Scene을 지정</param>
    public void LoadScene(Define.Scene type)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    /// <summary>
    /// 매개변수로 받은 Scene type을 문자열로 변환해 반환하는 함수.
    /// </summary>
    /// <param name="type">원하는 Scene enum</param>
    /// <returns></returns>
    string GetSceneName(Define.Scene type)
    {
        // enum을 string으로 바꾼다. C#의 Reflection기능
        string name = System.Enum.GetName(typeof(Define.Scene), type);

        return name;
    }

    public void ChangeScene(Define.Scene type)
    {
        CurrentScene.Clear();

        _curSceneType = type;
        SceneManager.LoadScene(GetSceneName(type));
    }
    
    /// <summary>
    /// 현재 Scene의 Clear를 호출하는 함수. Scene 전환시 필요한 정리작업을 수행하는 함수.
    /// </summary>
    public void Clear()
    {
        CurrentScene.Clear();
    }
}