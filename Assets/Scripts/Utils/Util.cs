using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 기능성 함수들을 넣어두는 클래스
/// </summary>
public class Util
{

    /// <summary>
    /// GameObject에게서 원하는 컴포넌트를 가져온다.
    /// 없다면, 새로 만들어서 붙이고 가져온다.
    /// </summary>
    /// <param name="go">컴포넌트를 가져올 GameObject</param>
    /// <typeparam name="T">컴포넌트의 타입</typeparam>
    /// <returns>컴포넌트</returns>
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    } 

    /// <summary>
    /// 부모 GameObject의 자식 GameObject 중에서 이름과 타입에 맞는 자식을 찾아 반환하는 함수
    /// </summary>
    /// <param name="go">부모 오브젝트</param>
    /// <param name="name">찾고자하는 대상 GameObject의 이름 (선택, 기본 null)</param>
    /// <param name="recursive"> 자식의 자식까지도 찾을 것인지 (선택, 기본 false)</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;
        if (recursive)
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) ||component.name == name)
                    // 이름이 비어있거나 찾던 이름과 동일하면
                    return component;
            }
        }
        else        // 재귀가 X (= 직속 자식에 한해서만 찾는 경우)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                // 이름이 비어있거나 찾던 이름과 동일하면
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        return null;
    }
    
    /// <summary>
    /// 부모 GameObject의 자식 중에서 GameObject를 찾아 반환하는 함수.
    /// </summary>
    /// <param name="go">부모 GameObject</param>
    /// <param name="name">찾고자 하는 GameObject의 이름 (선택, 기본값 null)</param>
    /// <param name="recursive">자식의 자식까지도 찾을 것인지 (선택, 기본값 false)</param>
    /// <returns></returns>
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {   // 모든 GameObject는 Transform을 갖고있다.
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;
        return transform.gameObject;
    }
    
}
