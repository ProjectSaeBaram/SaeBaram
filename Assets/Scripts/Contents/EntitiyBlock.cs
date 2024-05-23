using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntitiyBlock : EntityInfo
{
    [Header("EntityInfo")]
    [SerializeField] private int EHp;

    public bool playerInRange;
    public UnityEvent BlockDead;
    public static EntitiyBlock Instance;
    private void Awake()
    {
        EHp = 100;
        Instance = this;
        playerInRange = false;
        if (BlockDead == null)
        {
            BlockDead = new UnityEvent();
        }
    }

    private void FixedUpdate()
    {
        if (playerInRange )
        {
            if (PlayerController.GetInstance().GetInteractPressed())
            {
                Damaged(50);
            }
        }
    }

    public static EntitiyBlock GetInstance()
    {
        return Instance;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        playerInRange = true;
    }

    public override void Damaged(int damage)
    {
        if (EHp - damage > 0)
        {
            EHp -= damage;
            DebugEx.Log("Damage : "+damage+"!");
        }
        else
        {
            EHp= 0;
            DebugEx.Log(EHp);
            BlockDead.Invoke(); // 엔티티가 죽을 때 이벤트 호출
        }
    }

}
