using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Color = UnityEngine.Color;

public class UI_Inven_ItemTooltip : UI_Base
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
        // 툴팁 타이틀 색을 아이템 퀄리티에 비례하게
        ItemName.text = targetItem.Name;
        if (targetItem.ReinforceCount != 0)
        {
            ItemName.text += $"+{targetItem.ReinforceCount}";
        }
        Color NameColor = new Color();
        switch (targetItem.Quality)
        {
            case 1 or 0:
                NameColor = Color.white;
                break;
            case 2:
                NameColor = Color.green;
                break;
            case 3:
                NameColor = new Color(0, 0.749f, 1f);  
                break;
        }
    
        ItemName.color = NameColor;
        
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
