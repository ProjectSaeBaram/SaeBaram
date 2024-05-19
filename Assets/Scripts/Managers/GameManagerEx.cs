using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 컨텐츠에서 몬스터와 플레이어를 관리하기 위한 매니저
/// </summary>
public class GameManagerEx
{
    public Define.ThisGameis thisGameis { get; set; }
    
    // 플레이어는 하나뿐이니까
    private GameObject _player;
    
    // 중복을 방지하면서 몬스터를 저장하기 위한 HashSet
    private HashSet<GameObject> _monsters = new HashSet<GameObject>();

    // 몬스터가 추가되거나 삭제될 때 SpawningPool에게도 전달하기 위한 Action
    // 매개변수는 늘거나, 줄어들은 숫자
    public Action<int> OnSpawnEvent;

    /// <summary>
    /// 플레이어를 찾기 위한 함수
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayer()
    {
        return _player;
    }
    
    /// <summary>
    /// GameObject 생성을 담당하는 함수.
    /// </summary>
    /// <returns></returns>
    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Monster:
                _monsters.Add(go);
                OnSpawnEvent?.Invoke(1);
                break;
            case Define.WorldObject.Player:
                _player = go;
                break;
            case Define.WorldObject.DroppedItem:
                break;
        }
        return go;
    }

    /// <summary>
    /// 인자로 받은 GameObject의 타입을 반환하는 함수.
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        // BaseController는 플레이어와 몬스터를 제어하는 클래스의 상위 클래스.
        // State Pattern을 기반으로 매 프레임단위의 움직임을 제어하는 클래스였는데, 단순히 참고만 할 것.
        // BaseController bc = go.GetComponent<BaseController>();
        // if (bc == null)
        //     return Define.WorldObject.Unknown;
        //
        // return bc.WorldObjectType;

        // 다음 줄 코드도 수정해야 함!
        return Define.WorldObject.Unknown;
    }
    
    /// <summary>
    /// GameObject를 삭제하는 함수.
    /// 삭제하고자 하는 GameObject를 관리목록에서도 지우고, Destroy한다.
    /// </summary>
    /// <param name="go">삭제하고자 하는 GameObject</param>
    public void Despawn(GameObject go)
    {
        Define.WorldObject type = GetWorldObjectType(go);

        switch (type)
        {
            case Define.WorldObject.Monster:
                if (_monsters.Contains(go))
                {
                    _monsters.Remove(go);
                    OnSpawnEvent?.Invoke(-1);
                }
                break;
            case Define.WorldObject.Player:
                if (_player == go)
                    _player = null;
                break;
        }
        
        Managers.Resource.Destroy(go);
    }
}
