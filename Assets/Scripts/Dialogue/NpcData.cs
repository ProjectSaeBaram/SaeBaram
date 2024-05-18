using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VInspector;

public class NpcData : MonoBehaviour
{
    [Tab("Visual Cue")]
    [SerializeField] private GameObject[] visualCue;
    [Tab("NPC Inform")]
    [SerializeField] public int npcId;
    [SerializeField] public string npcName;
    [SerializeField] public bool isNpc;
    [SerializeField] public Sprite[] npcPortrait;
    [SerializeField] public List<TextAsset> dialogue;
    [SerializeField] public string loc;
    [Tab("Quest Inform")]
    [SerializeField] public int[] questId;
    [SerializeField] public int questIndex;                     //여러퀘스트를 가지고있을때 지금 진행가능한 퀘스트번호 
    [SerializeField] public QuestState qs;
    [SerializeField] public PlayerController playerController;
    [SerializeField] public int questActionIndex;
    [Tab("Chapter Inform")]
    [SerializeField] public static string Chap_Good = "Dialogue/Good/Chap";
    [SerializeField] public static string Chap_Bad = "Dialogue/Bad/Chap";
    [SerializeField] public int ChapNum;

    private static NpcData instance;

    public bool playerInRange;

    public static NpcData GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        questIndex = 0;
        playerInRange = false;
        dialogue = new List<TextAsset>();
        foreach (var c in visualCue)
        {
            c.gameObject.SetActive(false);
        }
        if(questId.Length >= 1 ) 
        {
            for (int i = 0; i < questId.Length*3; i++)
            {
                //dialogue[i] = Resources.Load("Dialogue/Chap0/Ruda/Ruda0") as TextAsset;
                //TextAsset t = Resources.Load(Chap_Good + ChapNum.ToString() + "/" + npcName + "/" + npcName + i.ToString()) as TextAsset;
                //Debug.Log(Resources.Load(Chap_Good + ChapNum.ToString() + "/" + npcName + "/" + npcName + i.ToString()) as TextAsset);
                dialogue.Add(Resources.Load(Chap_Good + ChapNum.ToString() + "/" + npcName + "/" + npcName + i.ToString()) as TextAsset);
            }
        }
        else
        {
            dialogue.Add(Resources.Load(Chap_Bad + ChapNum.ToString() + "/" + npcName + "/" + npcName + "0") as TextAsset);
        }
       
        npcName = this.name;
    }

    private void Update()
    {
        if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (PlayerController.GetInstance().GetInteractPressed())
            {
                DialogueManager.GetInstance().GetTalk2(this);
            }
        }
        else
        {

        }

    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player") 
        {
           
            playerInRange =true;
            qs = QuestManager.GetInstance().CheckState(questId[questIndex]);
            if (questId.Length > 0)          //퀘스트아이디가 있을 때
            {
                if (qs == QuestState.CAN_START)
                {
                    visualCue[0].SetActive(true);
                }
                else if (qs == QuestState.CAN_FINISH)
                {
                    visualCue[1].SetActive(true);
                }
            }
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
