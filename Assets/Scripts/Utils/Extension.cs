using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 확장 기능을 담는 클래스
/// </summary>
public static class Extension
{
    /// <summary>
    /// GameObject에게서 원하는 컴포넌트를 가져온다.
    /// 없다면, 새로 만들어서 붙이고 가져온다.
    /// 내부적으로 Util.GetOrAddComponent를 사용한다.
    /// </summary>
    /// <param name="go">입력 X</param>
    /// <typeparam name="T">컴포넌트의 타입</typeparam>
    /// <returns>컴포넌트</returns>
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    } 
    
    /// <summary>
    /// 대상 UI의 Event에 함수를 지정하여 바인딩한다.
    /// </summary>
    /// <param name="go">입력 X</param>
    /// <param name="action">바인딩하고자 하는 함수</param>
    /// <param name="type">이벤트가 Call되는 타입</param>
    public static void AddUIEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(go,action, type);
    }
    
    /// <summary>
    /// <see cref="Poolable"/>한 GameObject가 유효한지 검사하는 함수.
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static bool IsValid(this GameObject go)
    {
        return go != null && go.activeSelf == true;
    }
}
