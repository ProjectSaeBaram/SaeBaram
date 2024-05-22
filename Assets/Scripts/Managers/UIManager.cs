using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI 관리를 담당하는 매니저.
/// 모든 팝업 및 씬 UI를 관리하며, UI 요소의 생성, 표시, 삭제를 담당.
/// </summary>
public class UIManager
{
    // Popup UI의 순서를 관리하는 변수. 팝업의 렌더링 순서를 결정.
    private int _order = 10;

    // 현재 활성화된 Popup UI를 추적하는 스택.
    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

    // 현재 Scene UI를 저장하는 변수.
    private UI_Scene sceneUI = null;

    // UI의 루트 게임 오브젝트를 반환하거나 생성.
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }
    
    /// <summary>
    /// 게임 오브젝트에 Canvas 컴포넌트를 설정하고, 필요한 경우 순서를 지정.
    /// </summary>
    /// <param name="go">Canvas 컴포넌트를 추가할 게임 오브젝트.</param>
    /// <param name="sort">순서 지정 여부. true이면 _order 값을 사용하여 순서를 지정.</param>
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    /// <summary>
    /// 지정된 부모 Transform에 서브 아이템을 생성하고 자식으로 추가하는 함수.
    /// 주로 인벤토리 안의 아이템과 같이, 개별 아이콘을 시각화할 때 사용됨.
    /// </summary>
    /// <param name="parent">새로 생성된 서브 아이템을 추가할 부모 Transform. null일 경우 부모 없이 생성됨.</param>
    /// <param name="name">인스턴스화할 Prefab의 이름. null이나 빈 문자열일 경우 T의 클래스 이름을 사용.</param>
    /// <typeparam name="T">서브 아이템의 타입으로, UI_Base을 확장해야 함.</typeparam>
    /// <returns>생성된 서브 아이템의 T 타입 컴포넌트.</returns>
    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}", parent);
        
        return Util.GetOrAddComponent<T>(go);
    }
    
    /// <summary>
    /// WorldSpace UI 생성용 함수.
    /// </summary>
    /// <param name="parent">새로 생성된 서브 아이템을 추가할 부모 Transform. null일 경우 부모 없이 생성됨.</param>
    /// <param name="name">인스턴스화할 Prefab의 이름. null이나 빈 문자열일 경우 T의 클래스 이름을 사용.</param>
    /// <typeparam name="T">서브 아이템의 타입으로, UI_Base을 확장해야 함.</typeparam>
    /// <returns>생성된 서브 아이템의 T 타입 컴포넌트.</returns>
    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");
        if(parent != null)
            go.transform.SetParent(parent);
        
        // WorldSpace 설정
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        
        return Util.GetOrAddComponent<T>(go);
    }

    /// <summary>
    /// Scene UI를 생성하고, UI_Root 아래에 배치합니다.
    /// </summary>
    /// <param name="name">생성할 Scene UI Prefab의 이름. null이나 빈 문자열일 경우 T의 클래스 이름을 사용함.</param>
    /// <typeparam name="T">Scene UI의 타입으로, UI_Scene을 확장해야 함.</typeparam>
    /// <returns>생성된 Scene UI의 T 타입 컴포넌트.</returns>
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T _sceneUI = Util.GetOrAddComponent<T>(go);
        sceneUI = _sceneUI;
    
        go.transform.SetParent(Root.transform);
    
        return _sceneUI;
    }

    /// <summary>
    /// Popup UI를 생성하고 표시하는 함수.
    /// </summary>
    /// <param name="name">생성할 Popup UI Prefab의 이름. null이나 빈 문자열일 경우 T의 클래스 이름을 사용함.</param>
    /// <typeparam name="T">Popup UI의 타입으로, UI_Popup을 상속해야 함.</typeparam>
    /// <returns>생성된 Popup UI의 T 타입 컴포넌트.</returns>
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);
    
        go.transform.SetParent(Root.transform);
    
        return popup;
    }
    
    /// <summary>
    /// Popup 스택의 최상단 팝업을 닫는 함수.
    /// </summary>
    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);

        _order--;
    }
    
    /// <summary>
    /// Popup 스택의 최상단 팝업을 닫는 함수.
    /// </summary>
    public UI_Popup GetTopPopupUI()
    {
        return _popupStack.Peek();
    }

    /// <summary>
    /// 특정 팝업을 닫는 함수. 스택의 최상단 Popup이 아니면 닫지 않는다.
    /// </summary>
    /// <param name="popup">닫고자 하는 Popup UI</param>
    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0 || _popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }
        
        ClosePopupUI();
    }

    /// <summary>
    /// 모든 Popup UI를 닫는 함수
    /// </summary>
    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }
    
    /// <summary>
    /// 현재 Scene에서 활성화된 SceneUI에 대한 Getter
    /// </summary>
    /// <returns></returns>
    public UI_Scene GetCurrentSceneUI()
    {
        return sceneUI;
    }
    
    /// <summary>
    /// 모든 UI를 비우는 함수.
    /// </summary>
    public void Clear()
    {
        CloseAllPopupUI();
        sceneUI = null;
    }
}
