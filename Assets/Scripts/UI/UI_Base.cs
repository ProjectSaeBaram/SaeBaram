using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI 시스템의 기본을 구성하는 추상 클래스.
/// UI 요소와 enum을 매핑하고, UI 요소에 대한 참조를 관리.
/// </summary>
public abstract class UI_Base : MonoBehaviour
{
    // UI 요소와 enum의 매핑을 저장하는 딕셔너리.
    private Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    /// <summary>
    /// UI 초기화 메서드. UI_Base를 상속하는 모든 클래스는 이 메서드를 구현해야 합니다.
    /// </summary>
    public abstract void Init();

    /// <summary>   
    /// UI_Base를 상속하는 클래스들은, Init()함수를 Start() 함수 내에서 호출시킬 필요없이, 자동으로 호출되게 만든다.
    /// Init이 Start를 대신함
    /// </summary>
    private void Start()
    {
        Init();
    }

    /// <summary>
    /// UI 요소를 바인딩하는 메서드. 지정된 타입의 UI 요소를 enum 이름과 매핑합니다.
    /// </summary>
    /// <typeparam name="T">바인딩할 UI 요소의 타입</typeparam>
    /// <param name="type">enum 타입</param>
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type); // enum의 이름을 가져옵니다.
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects); // 매핑 딕셔너리에 추가합니다.

        for (int i = 0; i < names.Length; i++)
        {
            // GameObject 타입의 경우와 그 외의 경우를 구분하여 바인딩합니다.
            if(typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true); // GameObject는 직접 찾습니다.
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true); // 그 외 타입은 제네릭 메서드를 사용합니다.
            
            if(objects[i] == null)
                DebugEx.Log($"Failed to Bind! ({names[i]})"); // 바인딩 실패 시 로그 출력
        }
    }

    /// <summary>
    /// 지정된 인덱스의 UI 요소를 가져옵니다.
    /// </summary>
    /// <typeparam name="T">가져올 UI 요소의 타입</typeparam>
    /// <param name="index">가져올 UI 요소의 인덱스</param>
    /// <returns>바인딩된 UI 요소</returns>
    protected T Get<T>(int index) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (!_objects.TryGetValue(typeof(T), out objects)) 
            return null; // 매핑된 객체가 없으면 null 반환

        return objects[index] as T;
    }

    // 특정 UI 요소 타입을 위한 편의 메서드들
    protected GameObject GetObject(int index)
    {
        return Get<GameObject>(index);
    }
    protected TextMeshProUGUI GetText(int index)
    {
        return Get<TextMeshProUGUI>(index);
    }
    
    protected Button GetButton(int index)
    {
        return Get<Button>(index);
    }
    
    protected Image GetImage(int index)
    {
        return Get<Image>(index);
    }

    /// <summary>
    /// UI 요소에 이벤트를 바인딩하는 정적 메서드.
    /// </summary>
    /// <param name="go">이벤트를 바인딩할 GameObject</param>
    /// <param name="action">바인딩할 액션</param>
    /// <param name="type">UI 이벤트 타입</param>
    public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler _event = Util.GetOrAddComponent<UI_EventHandler>(go); // 이벤트 핸들러 컴포넌트를 가져오거나 추가합니다.

        // 지정된 타입에 따라 액션을 이벤트 핸들러에 바인딩합니다.
        switch (type)
        {
            case Define.UIEvent.Click:
                _event.OnClickHandler -= action; // 중복 바인딩 방지를 위해 먼저 제거
                _event.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                _event.OnDragHandler -= action; // 중복 바인딩 방지를 위해 먼저 제거
                _event.OnDragHandler += action;
                break;
        }
    }
}
