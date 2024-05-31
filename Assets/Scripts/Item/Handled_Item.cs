using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Handled_Item : MonoBehaviour
{
    [SerializeField] private ToolStatsDatabase _toolStatsDatabase;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private UI_Inven_Item _uiInvenItem;
    [SerializeField] private Collider2D _collider;
    [SerializeField] public int Index;
    
    public ToolStatsDatabase.ItemStats _itemStats;

    private HashSet<EntityController> triggeredEntities = new HashSet<EntityController>();
    
    public void OnEnable()
    {
        _spriteRenderer.sprite = _uiInvenItem?.image.sprite;
    }

    private void OnDisable()
    {
        _spriteRenderer.sprite = null;
    }

    public void ItemUIReferenceSetter(UI_Inven_Item item)
    {
        if (item == null)
        {
            _uiInvenItem = null;
            _itemStats = null;
        }
        else
        {
            _uiInvenItem = item;
            int t = Managers.Data.reverseItemCodeDict[_uiInvenItem.Name];
            _itemStats = _toolStatsDatabase.GetItemStats(t);
        }
        
        _spriteRenderer.sprite = _uiInvenItem?.image.sprite;
    }

    public void ColliderActivate()
    {
        if (_itemStats == null) return;
        _collider.enabled = true;
        triggeredEntities.Clear();
        
    }

    public void ColliderDeactivate()
    {
        _collider.enabled = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Entity 랑만 Trigger
        EntityController target = other.GetComponent<EntityController>();
        // 지금 HandledItem이 stat이 없는 아이템이면 기능 X
        if (target == null) return;

        // 이미 Trigger된 Entity라면 무시
        if (triggeredEntities.Contains(target)) return;
        
        // Trigger되지 않은 새로운 Entity라면 처리
        triggeredEntities.Add(target);
        
        // 대상 Entity가 HandledITem에게서 맞는다
        
        target.GetHit(this, _itemStats.GetStatValueForEntityType(target.EntityType));
    }
}
