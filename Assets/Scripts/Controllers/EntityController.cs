using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    protected Define.ItemMainStatType MyType;
    protected int currentHp = 100;
    protected int maxHp = 100;
    
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Handled_Item target = other.GetComponent<Handled_Item>();
        if (target == null) return;
        
        // 데미지 맞는 로직
        try
        {
            GetHit(target, target._itemStats.GetStatValueForEntityType(MyType));
        }
        catch (UnityException)
        {
            
        }
    }
    
    protected virtual void GetHit(Handled_Item target, int damage)
    {
        // 내 체력을 깎는다
        // 체력이 0 밑으로 떨어지면, 죽는다
        
        DebugEx.LogWarning($"{damage} damage Dealt!");
        
        // 내가 아이템 떨굴 것들 떨구고
        currentHp -= damage;
        if (currentHp <= 0)
        {
            DropItems(target);
            Destroy(gameObject, 0.4f);
        }
    }

    protected virtual void DropItems(Handled_Item target)
    {
        DebugEx.LogWarning("Item Dropped!");
    }
}
