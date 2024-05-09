using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemTooltip : UI_Base
{
    [SerializeField] private TextMeshProUGUI ItemName;
    [SerializeField] private TextMeshProUGUI ItemContext;
    [SerializeField] private List<TextMeshProUGUI> Logs;

    [SerializeField] private UI_Inven_Item targetItem;
    
    private RectTransform _rectTransform;
    private PointerEventData _eventData;
    
    public override void Init()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (_eventData != null)
        {
            _rectTransform.position = _eventData.position + new Vector2(15, 0);
        }
    }

    public void ShowTooltip(UI_Inven_Item item, PointerEventData eventData)
    {
        _eventData = eventData;
        targetItem = item;
        GetTextsFromTargetItem();
    }

    public void UnsetPointerEventData()
    {
        _eventData = null;
        targetItem = null;
        ClearTexts();
    }

    private void GetTextsFromTargetItem()
    {
        ItemName.text = targetItem.Name;
        ItemContext.text = targetItem.itemType.ToString();

        if (targetItem.Logs == null) return;
        for (int i = 0; i < targetItem.Logs.Count; i++)
        {
            Logs[i].gameObject.SetActive(true);
            Logs[i].text = targetItem.Logs[i];
        }
    }

    private void ClearTexts()
    {
        ItemName.text = "";
        ItemContext.text = "";
        for (int i = 0; i < Logs.Count; i++)
        {
            Logs[i].text = "";
            // 첫번째 Log Text만 활성화
            Logs[i].gameObject.SetActive(i == 0);
        }

    }
}
