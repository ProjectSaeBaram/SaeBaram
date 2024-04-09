using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NpcData : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject[] visualCue;
    [SerializeField] private bool isQuest;
    [Header("NPC Inform")]
    [SerializeField] public int npcId;
    [SerializeField] public string npcName;
    [SerializeField] public bool isNpc;
    [SerializeField] public Sprite[] npcPortrait;
    [SerializeField] public string loc;
    [Header("Quest Inform")]
    [SerializeField] public int[] questId;
    [SerializeField] public int questIndex;
    [SerializeField] public QuestState qs;

    private static NpcData instance;

    public bool playerInRange;

    public static NpcData GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        playerInRange = false;
        isQuest = false;
        foreach (var c in visualCue)
        {
            c.gameObject.SetActive(false);
        }
        npcName = this.name;
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player") 
        {
            playerInRange=true;
            //qs = collider.transform.GetComponent<Player>().questManager.CheckState(questId[questIndex]);
            //if (questId.Length > 0)          //퀘스트아이디가 있을 때
            //{
            //    if (qs == QuestState.CAN_START)
            //    {
            //        visualCue[0].SetActive(true);
            //    }
            //    else if (qs == QuestState.CAN_FINISH)
            //    {
            //        visualCue[1].SetActive(true);
            //    }
            //}  
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange=false;
            foreach(var c in visualCue)
            {
                c.gameObject.SetActive(false);
            }
        }
    }

    public NpcData getNpcdata(int npcId)
    {
        return this;
    }
}
