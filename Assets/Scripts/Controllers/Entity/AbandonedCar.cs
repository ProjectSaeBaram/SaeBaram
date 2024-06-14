using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class AbandonedCar : EntityController
{
    [SerializeField] private Collider2D _collider;
    
    [SerializeField] private AudioClip[] OnHitSound;
    [SerializeField] private AudioClip[] OnDestroySound;
    
    private void OnEnable()
    {
        EntityType = Define.ItemMainStatType.Mining;
        currentHp = 30;
        maxHp = 30;
    }

    public override void GetHit(Handled_Item target, int damage)
    {
        // 내 체력을 깎는다
        // 체력이 0 밑으로 떨어지면, 죽는다

        if (target._itemStats.GetMainStatType() == Define.ItemMainStatType.DismantlingDriver ||
            target._itemStats.GetMainStatType() == Define.ItemMainStatType.DismantlingHammer)
        {
            damage = target._itemStats.GetMainStatValue();
        }
        
        DebugEx.LogWarning($"{damage} damage Dealt!");
        
        // 내가 아이템 떨굴 것들 떨구고
        currentHp -= damage;
        if (currentHp <= 0)
        {
            WhenDestroy(target);
            _collider.enabled = false;
            Destroy(gameObject);
            return;
        }
        
        // 사운드 재생
        Managers.Sound.Play(OnHitSound[Random.Range(0, OnHitSound.Length)], volume: 0.3f);
    }
    
    protected override void WhenDestroy(Handled_Item target)
    {
        if (target._itemStats.GetMainStatType() == Define.ItemMainStatType.DismantlingHammer)
        {
            // 철판 드랍
            DropItem(9);
        }
        else if (target._itemStats.GetMainStatType() == Define.ItemMainStatType.DismantlingDriver)
        {
            // 회로 드랍
            DropItem(10);
        }
        else if (target._itemStats.GetMainStatType() == Define.ItemMainStatType.Mechanic)
        {
            // 철판, 회로 드랍
            DropItem(9);
            DropItem(10);
        }

        // 사운드 재생
        int randomSound = Random.Range(0, OnDestroySound.Length);
        Managers.Sound.Play(OnDestroySound[randomSound], volume: 0.2f);
    }
}