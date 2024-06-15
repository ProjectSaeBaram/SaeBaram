using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WallController : EntityController
{
    public UnityEvent BlockDead  = new UnityEvent();
    
    [SerializeField] private Collider2D _collider;
    
    [SerializeField] private AudioClip[] OnHitSound;
    [SerializeField] private AudioClip[] OnDestroySound;
    
    private void OnEnable()
    {
        EntityType = Define.ItemMainStatType.Mining;
        currentHp = 100;
        maxHp = 100;
        
        BlockDead.AddListener(() => QuestManager.GetInstance().UpdateQuestState(0));
    }

    public override void GetHit(Handled_Item target, int damage)
    {
        DebugEx.LogWarning($"{damage} damage Dealt!");
        
        // 만약 나를 때린 아이템이 품질 상(3)의 드릴이면, 바로 깨진다.
        if (target._itemStats.id == 140 && target.GetOriginItemUI().Quality == 3)
        {
            damage = currentHp;
        }
        
        // 내 체력을 깎는다
        // 체력이 0 밑으로 떨어지면, 죽는다
        currentHp -= damage;
        if (currentHp <= 0)
        {
            // 퀘스트 관련 이벤트 수행
            Debug.Log(QuestManager.GetInstance().CheckState(0));
            if (QuestManager.GetInstance().CheckState(0) == QuestState.IN_PROGRESS)
            {
                BlockDead.Invoke(); // 엔티티가 죽을 때 이벤트 호출
                
                // 내가 아이템 떨굴 것들 떨구고
                WhenDestroy(target);
                _collider.enabled = false;
                Destroy(gameObject);
            }
            
            return;
        }
        
        // 사운드 재생
        Managers.Sound.Play(OnHitSound[Random.Range(0, OnHitSound.Length)], volume: 0.2f);
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
