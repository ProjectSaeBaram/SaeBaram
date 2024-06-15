using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DialogueManager : MonoBehaviour
{
    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;
    [Header("NPC Data")]
    [SerializeField] private Dictionary<int, int> npcDindex;                //npc 별 대화인덱스 저장할 변수
    private NpcData npcdata;
    private Story currentStory;                                     //Ink 로 생성된 텍스트를 받아올 Class변수
    [SerializeField] public GameObject[] npcList;

    private const string SPEAKER_TAG = "speaker";                   //테그값들 테그값 : 변수
    private const string PORTRAIT_TAG = "portrait";
    private const string PLAYER_TAG = "player";
    private const string LAYOUT_TAG = "layout";
    private const string GOOD_TAG = "good";
    private const string BAD_TAG = "bad";
    private const string DIALOGUE_TAG = "Dialogue";
    public UI_DialoguePopup popup;
    public QuestLayer qpanel;
    public PlayerController playerController;
    [SerializeField] public bool isGood = false;

    public bool dialogueIsPlaying { get; private set; }             //현재 대화창에 진입했는지 확인할 변수
                                                                    //퀘스트 진행상황은 퀘스트 메니저에서 관리
    private DialogueVariables dialogueVariables;

    public static DialogueManager instance;

    private void Awake()
    {
        instance = this;
        //TODO : Json으로 저장된 챕터별 npc 아이디와 대화인덱스 변수 가져와서 변수 초기화
        //dialogueVariables = new DialogueVariables(loadGlobalsJSON);
    }


    public static DialogueManager GetInstance()
    {
        return instance;
    }

    public int GetQuestIndex(int id)
    {
        return npcDindex[id];
    }
    
    public void setQuestIndex(int id,int idx)
    {
        npcDindex[id] = idx;
    }
    

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }
        if (currentStory.currentChoices.Count==0 && playerController.GetInteractPressed())
        {
            ContinueStory();
        }
    }


    public void GetTalk2(TextAsset dialogue,NpcData npc)
    {
        PlayerController player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        player.DisableExceptInteract();
        npcdata = npc;
        npcdata.SetMerchant(false);
        isGood = npc.isGood;
        currentStory = new Story(dialogue.text);
        dialogueIsPlaying = true;
        Managers.UI.ShowPopupUI<UI_DialoguePopup>();
        popup.dialoguePanel.SetActive(true);
        //dialogueVariables.StartListening(currentStory);
        foreach (GameObject cue in npc.visualCue)
        {
            cue.SetActive(false);
        }
        //태그 초기화
        popup.displayNameText.text = "???";
        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        //dialogueVariables.StopListening(currentStory);
        dialogueIsPlaying = false;
        popup.dialogueText.text = "";
        popup.dialoguePanel.SetActive(false);
        PlayerController player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        player.EnableExceptInter();
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue) //더 보여줄 이야기가 있다면
        {
            popup.dialogueText.text = currentStory.Continue();  // 한줄 출력
            DisplayChoices();                                   // 선택이 있으면 선택출력
            //태그관리
            HandleTags(currentStory.currentTags);
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag parsed error : " + tag+splitTag);
            }
            string tagkey = splitTag[0].Trim();
            string tagvalue = splitTag[1].Trim();   

            switch (tagkey)
            {
                case SPEAKER_TAG:
                    popup.displayNameText.text = tagvalue;
                    break;
                case PORTRAIT_TAG:
                    popup.portraitImage.sprite = npcdata.npcPortrait[int.Parse(tagvalue)];
                    break;
                case PLAYER_TAG:
                    popup.portraitImage.sprite = playerController.getplayerPortrait(int.Parse(tagvalue));

                    break;
                case LAYOUT_TAG:
                    popup.layoutAnimator.Play(tagvalue);
                    break;
                case GOOD_TAG:
                    //Debug.Log("Good+"+tagvalue);
                    break;
                case BAD_TAG:
                    //Debug.Log("Bad+"+tagvalue);
                    break;
                case DIALOGUE_TAG:              //퀘스트, 상점 구분
                    if (tagvalue == "Merchant")
                    {
                        npcdata.SetMerchant(true);
                    }
                    break;
                default:
                    Debug.LogWarning("Tag exists but not handled");
                    break;
            }

        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        if (currentChoices.Count > popup.choices.Length)           //현재 선택지의 개수가 버튼의 개수보다 많으면 오류 
        {
            Debug.LogError("More choices than ever");
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            popup.choices[index].gameObject.SetActive(true);
            popup.choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < popup.choices.Length; i++)
        {
            popup.choices[i].gameObject.SetActive(false);
        }
        popup.choicep.SetActive(true);

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(popup.choices[0].gameObject);
    }

    public void makeChoice(int choice)
    {
        currentStory.ChooseChoiceIndex(choice);
        if (choice == 0)
        {
            if (npcdata.questId.Length > 0)
            {
                QuestState qs = QuestManager.GetInstance().CheckState(npcdata.questId[npcdata.DialogueIndex]);
                if (qs == QuestState.CAN_START)
                {
                    QuestManager.GetInstance().AdvanceQuest(npcdata.questId[npcdata.DialogueIndex], npcdata);
                    //qpanel.questlist.AddQuest(QuestManager.GetInstance().GetQuestData(npcdata.questId[npcdata.questIndex]));
                }else if (qs == QuestState.CAN_FINISH)
                {
                    QuestManager.GetInstance().AdvanceQuest(npcdata.questId[npcdata.DialogueIndex], npcdata);
                }
            }

            if(npcdata.GetMerchant())
            {
                Managers.UI.CloseAllPopupUI();
                Managers.UI.ShowPopupUI<UI_Merchant>();
            }
        }
        ContinueStory();
        DebugEx.Log(choice);
    }

    public void setvisibleNpc()
    {
        foreach (var npc in npcList)
        {
            if (npc.GetComponent<NpcData>().isGood && Managers.Game.GetPlayer().GetComponent<PlayerController>().RState == ReputeState.Good)
            {
                npc.gameObject.SetActive(true);
            }
            else
            {
                npc.gameObject.SetActive(false);
            }
        }
    }
}
