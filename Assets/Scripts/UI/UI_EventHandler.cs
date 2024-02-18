using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI 이벤트 처리를 위한 핸들러 클래스.
/// 이 클래스는 EventSystem으로부터 포인터 클릭과 드래그 이벤트를 캐치하여
/// 등록된 콜백 메서드로 전달합니다.
/// </summary>
public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    // 클릭 이벤트 발생 시 호출될 콜백을 저장하는 Action.
    public Action<PointerEventData> OnClickHandler = null;

    // 드래그 이벤트 발생 시 호출될 콜백을 저장하는 Action.
    public Action<PointerEventData> OnDragHandler = null;

    /// <summary>
    /// 포인터 클릭 이벤트를 처리합니다.
    /// 등록된 OnClickHandler가 있으면 이를 호출하여 클릭 이벤트 데이터를 전달합니다.
    /// </summary>
    /// <param name="eventData">클릭 이벤트에 대한 데이터.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickHandler?.Invoke(eventData);
    }
    
    /// <summary>
    /// 드래그 이벤트를 처리합니다.
    /// 등록된 OnDragHandler가 있으면 이를 호출하여 드래그 이벤트 데이터를 전달합니다.
    /// </summary>
    /// <param name="eventData">드래그 이벤트에 대한 데이터.</param>
    public void OnDrag(PointerEventData eventData)
    {
        OnDragHandler?.Invoke(eventData);
    }
}