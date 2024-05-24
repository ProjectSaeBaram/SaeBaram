using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Handled_Item : MonoBehaviour
{
    [SerializeField] private ToolStatsDatabase _toolStatsDatabase;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private UI_Inven_Item _uiInvenItem;
    [SerializeField] private Collider2D _collider;

    public ToolStatsDatabase.ItemStats _itemStats;
    
    private void OnEnable()
    {
        _spriteRenderer.sprite = _uiInvenItem?.image.sprite;
    }

    private void OnDisable()
    {
        _spriteRenderer.sprite = null;
    }

    public void ItemUIReferenceSetter(UI_Inven_Item item)
    {
        _uiInvenItem = item;
        _itemStats = _toolStatsDatabase.GetItemStats(Managers.Data.reverseItemCodeDict[_uiInvenItem.Name]);
    }

    public void Activate()
    {
        _collider.enabled = true;
        
    }

    public void Deactivate()
    {
        _collider.enabled = false;
    }
}
