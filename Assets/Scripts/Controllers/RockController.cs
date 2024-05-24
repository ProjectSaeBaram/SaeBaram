using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RockController : EntityController
{
    [SerializeField] private AudioClip[] OnHitSound;
    [SerializeField] private AudioClip[] OnDestroySound;
    
    private void OnEnable()
    {
        MyType = Define.ItemMainStatType.Mining;
        currentHp = 20;
        maxHp = 20;
    }

    protected override void DropItems(Handled_Item target)
    {
        if (target._itemStats.GetMainStatType() == MyType)
        {
            DroppedItem droppedItem = Managers.Game.Spawn(Define.WorldObject.DroppedItem, "Item/DroppedItem").GetComponent<DroppedItem>();
            droppedItem.transform.position = transform.position + Vector3.up * 30;

            int quality = 1;
            float rand = Random.Range(0, 1);
            
            if (rand <= 0.7f)
                quality = 1;
            else if (rand <= 0.9f) 
                quality = 2;
            else if (rand <= 1.0f)
                quality = 3;
            
            droppedItem.InitInfoByValue(2, quality);
        }
    }

}
