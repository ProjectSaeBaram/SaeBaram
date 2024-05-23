using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class TreeController : EntityController
{
    [SerializeField] private BoxCollider2D _collider;
    
    [SerializeField]
    private AudioSource _audioSource;
    
    [SerializeField] private AudioClip[] OnHitSound;
    [SerializeField] private AudioClip[] OnDestroySound;
    [SerializeField] private GameObject EmptyAudioSource;
    
    private void OnEnable()
    {
        MyType = Define.ItemMainStatType.Lumbering;
        currentHp = 20;
        maxHp = 20;
    }

    protected override void GetHit(Handled_Item target, int damage)
    {
        // 내 체력을 깎는다
        // 체력이 0 밑으로 떨어지면, 죽는다
        
        DebugEx.LogWarning($"{damage} damage Dealt!");
        
        // 내가 아이템 떨굴 것들 떨구고
        currentHp -= damage;
        if (currentHp <= 0)
        {
            DropItems(target);
            _collider.enabled = false;
            Destroy(gameObject, 0.3f);
            return;
        }
        _audioSource.PlayOneShot(OnHitSound[Random.Range(0, OnHitSound.Length)]);
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
            
            droppedItem.InitInfoByValue(1, quality);
        }

        MakeDestroySound();
    }

    private void MakeDestroySound()
    {
        AudioSource audioSource = Instantiate(EmptyAudioSource, transform.position, quaternion.identity).GetComponent<AudioSource>();
        int rand = Random.Range(0, OnDestroySound.Length);
        audioSource.PlayOneShot(OnDestroySound[rand]);
        Destroy(audioSource.gameObject, OnDestroySound[rand].length);
    }
    
}
