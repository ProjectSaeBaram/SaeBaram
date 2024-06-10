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
    [Tab("NPC Inform")]
    [SerializeField] private GameObject[] visualCue;
    [SerializeField] private int npcId;
    [SerializeField] private string npcName;
    [SerializeField] private bool isNpc;
    [SerializeField] public Sprite[] npcPortrait;
    [SerializeField] private TextAsset dialogue;
    [Tab("Merchant Info")]
    [SerializeField] private bool isMerchant;
    public ShopData shopData; // 스크립터블 오브젝트 참조
    [Tab("Quest Inform")]
    [SerializeField] public int[] questId;
    [SerializeField] public int TalkIndex;                     //여러퀘스트를 가지고있을때 지금 진행가능한 퀘스트번호 
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

   

    [Tab("Merchant")]
    //[SerializeField] private GameObject

    private static NpcData instance;

    public bool playerInRange;

    public static NpcData GetInstance()
    {
        return instance;
    }
    
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
        
        TalkIndex = 0;
        playerInRange = false;
        npcName = this.name;
        foreach(GameObject cue in visualCue)
        {
            cue.SetActive(false);
        }
       

        
       
    }

    public int GetNpcId()
    {
        return this.npcId;
    }

    private void FixedUpdate()
    {
        if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (PlayerController.GetInstance().GetInteractPressed())
            {
                Managers.UI.ShowPopupUI<UI_DialoguePopup>();
                DialogueManager.GetInstance().GetTalk2(dialogue,this);
            }
        }
        else
        {

        }
    }


    private void OnTriggerEnter2D(Collider2D collider)
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
                    D_path = Chap_Good + ChapNum.ToString() + "/" + npcName + "/" + npcName + (TalkIndex * 5 + questActionIndex).ToString();
                    break;
                case ReputeState.Bad:
                    D_path = Chap_Bad + ChapNum.ToString() + "/" + npcName + "/" + npcName + (TalkIndex * 5 + questActionIndex).ToString();
                    break;
                case ReputeState.Normal:
                    D_path = Chap_Normal + ChapNum.ToString() + "/" + npcName + "/" + npcName + (TalkIndex * 5 + questActionIndex).ToString();
                    break;
                default:
                    D_path = Chap_Normal + ChapNum.ToString() + "/" + npcName + "/" + npcName + "0";
                    break;
            }
        }
        dialogue = Resources.Load(D_path) as TextAsset;

        QuestManager.GetInstance().CheckRequirement();
        if (collider.gameObject.tag == "Player") 
        {
            playerInRange =true;
            
            if (questId.Length > 0)          //퀘스트아이디가 있을 때
            {
                qs = QuestManager.GetInstance().CheckState(questId[TalkIndex]);
                questActionIndex = Convert.ToInt32(QuestManager.GetInstance().CheckState(questId[TalkIndex]));
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
        Debug.Log(D_path);
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
