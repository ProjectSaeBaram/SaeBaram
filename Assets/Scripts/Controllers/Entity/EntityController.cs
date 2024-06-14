using UnityEngine;
using UnityEngine.Serialization;

public abstract class EntityController : MonoBehaviour
{
    public Define.ItemMainStatType EntityType;
    public int currentHp = 100;
    public int maxHp = 100;
    
    public virtual void GetHit(Handled_Item target, int damage)
    {
        // 내 체력을 깎는다
        // 체력이 0 밑으로 떨어지면, 죽는다
        
        DebugEx.LogWarning($"{damage} damage Dealt!");
        
        // 내가 아이템 떨굴 것들 떨구고
        currentHp -= damage;
        if (currentHp <= 0)
        {
            WhenDestroy(target);
            Destroy(gameObject, 0.4f);
        }
    }

    protected virtual void WhenDestroy(Handled_Item target)
    {
        DebugEx.LogWarning("Item Dropped!");
    }
    
    /// <summary>
    /// id에 해당하는 아이템을 생성하는 함수
    /// 이때, 퀄리티는 무작위로 설정
    /// </summary>
    /// <param name="itemId"></param>
    protected void DropItem(int itemId)
    {
        int quality = 1;
        float rand = Random.Range(0, 1);
            
        if (rand <= 0.7f)
            quality = 1;
        else if (rand <= 0.9f) 
            quality = 2;
        else if (rand <= 1.0f)
            quality = 3;
        
        DroppedItem droppedItem = Managers.Game.Spawn(Define.WorldObject.DroppedItem, "Item/DroppedItem").GetComponent<DroppedItem>();
        droppedItem.transform.position = transform.position + Vector3.up * 30;
        droppedItem.InitInfoByValue(itemId, quality);
    }
}
