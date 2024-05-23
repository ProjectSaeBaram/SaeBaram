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
    [SerializeField] public TextAsset dialogue;
    [SerializeField] public string loc;
    [Tab("Quest Inform")]
    [SerializeField] public int[] questId;
    [SerializeField] public int DialogueIndex;                     //여러퀘스트를 가지고있을때 지금 진행가능한 퀘스트번호 
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
        //questIndex = DialogueManager.GetInstance().GetQuestIndex(npcId);
        DialogueIndex = 0;
        playerInRange = false;
        npcName = this.name;
        foreach(GameObject cue in visualCue)
        {
            cue.SetActive(false);
        }
        dialogue = Resources.Load(Chap_Good + ChapNum.ToString() + "/" + npcName + "/" + npcName + (DialogueIndex * 4 + questActionIndex).ToString()) as TextAsset;
    }

    private void FixedUpdate()
    {
        if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (PlayerController.GetInstance().GetInteractPressed())
            {
                DialogueManager.GetInstance().GetTalk2(dialogue,this);
            }
        }
        else
        {

        }
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        questActionIndex = Convert.ToInt32(QuestManager.GetInstance().CheckState(questId[DialogueIndex]));
        Debug.Log(Chap_Good + ChapNum.ToString() + "/" + npcName + "/" + npcName + (DialogueIndex * 4 + questActionIndex).ToString());
        
        if (collider.gameObject.tag == "Player") 
        {
           
            playerInRange =true;
            qs = QuestManager.GetInstance().CheckState(questId[DialogueIndex]);
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
