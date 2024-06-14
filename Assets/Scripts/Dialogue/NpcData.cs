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
    [SerializeField] public GameObject[] visualCue;
    [Tab("NPC Inform")]
    [SerializeField] private int npcId;
    [SerializeField] private string npcName;
    [SerializeField] private bool isNpc;
    [SerializeField] public Sprite[] npcPortrait;
    [SerializeField] private TextAsset dialogue;
    [SerializeField] private bool isMerchant;
    [SerializeField] public bool isGood=false;
    [Tab("Quest Inform")]
    [SerializeField] public int[] questId;
    [SerializeField] public int DialogueIndex;                     //여러퀘스트를 가지고있을때 지금 진행가능한 퀘스트번호 
    [SerializeField] public QuestState qs;
    [SerializeField] public PlayerController playerController;
    [SerializeField] public int questActionIndex;
    [Tab("Chapter Inform")]
    [SerializeField] private string D_path;
    private static string Chap_Good = "Dialogue/Good/Chap";
    private static string Chap_Bad = "Dialogue/Bad/Chap";
    private static string Chap_Other= "Dialogue/Other/Chap";
    private static string Chap_Normal= "Dialogue/Normal/Chap";
    [SerializeField] public int ChapNum;


    public bool playerInRange;
 
    public bool GetMerchant()
    {
        return this.isMerchant;
    }
    public void SetMerchant(bool value)
    {
        isMerchant=value;
    }

    private void Awake()
    {
        //questIndex = DialogueManager.GetInstance().GetQuestIndex(npcId);
       
        DialogueIndex = 0;
        playerInRange = false;
        npcName = this.name;
        foreach (GameObject cue in visualCue)
        {
            cue.SetActive(false);
        }
    }

    public int GetNpcId()
    {
        return this.npcId;
    }

    private void Update()
    {
        if (isMerchant)
        {
            D_path = Chap_Other + ChapNum.ToString() + "/" + npcName + "/" + npcName + "0";
        }
        else
        {
            switch (PlayerController.GetInstance().RState)
            {
                case ReputeState.Good:
                    D_path = Chap_Good + ChapNum.ToString() + "/" + npcName + "/" + npcName + (DialogueIndex * 5 + questActionIndex).ToString();
                    break;
                case ReputeState.Bad:
                    D_path = Chap_Bad + ChapNum.ToString() + "/" + npcName + "/" + npcName + (DialogueIndex * 5 + questActionIndex).ToString();
                    break;
                case ReputeState.Normal:
                    D_path = Chap_Normal + ChapNum.ToString() + "/" + npcName + "/" + npcName + (DialogueIndex * 5 + questActionIndex).ToString();
                    break;
                default:
                    D_path = Chap_Normal + ChapNum.ToString() + "/" + npcName + "/" + npcName + "0";
                    break;
            }
        }
        dialogue = Resources.Load(D_path) as TextAsset;

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
     
        QuestManager.GetInstance().CheckRequirement();
        if (collider.gameObject.tag == "Player") 
        {
            playerInRange =true;
            
            if (questId.Length > 0)          //퀘스트아이디가 있을 때
            {
                qs = QuestManager.GetInstance().CheckState(questId[DialogueIndex]);
                questActionIndex = Convert.ToInt32(QuestManager.GetInstance().CheckState(questId[DialogueIndex]));
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
