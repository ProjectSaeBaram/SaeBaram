using UnityEngine;
using Random = UnityEngine.Random;

public class RockController : EntityController
{
    [SerializeField] private Collider2D _collider;
    
    [SerializeField] private AudioClip[] OnHitSound;
    [SerializeField] private AudioClip[] OnDestroySound;
    
    private void OnEnable()
    {
        EntityType = Define.ItemMainStatType.Mining;
        currentHp = 20;
        maxHp = 20;
    }

    public override void GetHit(Handled_Item target, int damage)
    {
        // 내 체력을 깎는다
        // 체력이 0 밑으로 떨어지면, 죽는다
        
        DebugEx.LogWarning($"{damage} damage Dealt!");
        
        // 내가 아이템 떨굴 것들 떨구고
        currentHp -= damage;
        if (currentHp <= 0)
        {
            WhenDestroy(target);
            _collider.enabled = false;
            Destroy(gameObject, 0.3f);
            return;
        }
        
        // 사운드 재생
        Managers.Sound.Play(OnHitSound[Random.Range(0, OnHitSound.Length)], volume: 0.3f);
    }
    
    protected override void WhenDestroy(Handled_Item target)
    {
        if (target._itemStats.GetMainStatType() == EntityType || target._itemStats.GetMainStatType() == Define.ItemMainStatType.Mechanic)
        {
            // 돌 드랍
            DropItem(2);
        }
        
        // 사운드 재생
        int randomSound = Random.Range(0, OnDestroySound.Length);
        Managers.Sound.Play(OnDestroySound[randomSound], volume: 0.3f);
    }
}
