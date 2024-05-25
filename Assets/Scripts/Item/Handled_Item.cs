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

    public ToolStatsDatabase.ItemStats _itemStats;

    private HashSet<EntityController> triggeredEntities = new HashSet<EntityController>();
    
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

    public void ColliderActivate()
    {
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
        if (target == null) return;

        // 이미 Trigger된 Entity라면 무시
        if (triggeredEntities.Contains(target)) return;
        
        // Trigger되지 않은 새로운 Entity라면 처리
        triggeredEntities.Add(target);
        
        // 대상 Entity의 맞는 함수 동작
        target.GetHit(this, _itemStats.GetStatValueForEntityType(target.EntityType));
    }
}
