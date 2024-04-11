using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VInspector;

public class QuestInfoPanel : MonoBehaviour
{
    //퀘스트 상세설명 -> 이름, 설명, 위치,관련된 npc이름,진행상황 textmeshpro 5개 필요?
    [Tab("QuestInfo")]
    [SerializeField] private TextMeshProUGUI questTitle;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI questLoc;
    [SerializeField] private TextMeshProUGUI npcName;
    [SerializeField] private TextMeshProUGUI questState;
    [SerializeField] private TextMeshProUGUI questReward;
    private static QuestInfoPanel instance;
    private void Awake()
    {
        questTitle.text = "";
        questDescription.text = "";
        questLoc.text = "";
        npcName.text = "";
        questState.text = "";
        questReward.text = "";
    }

    public void Resetting()
    {
        questTitle.text = "";
        questDescription.text = "";
        questLoc.text = "";
        npcName.text = "";
        questState.text = "";
        questReward.text = "";
    }

    public static QuestInfoPanel GetInstance()
    {
        return instance;
    }
    public void setQuestInfo(QuestData questData)
    {
        questTitle.text = questData.questName;
        questDescription.text = questData.getQuestInfo();
        questLoc.text = questData.loc;
        npcName.text = questData.npcname;
        switch (questData.qs)
        {
            case QuestState.IN_PROGRESS:
                questState.text = "진행 중";
                break;
            case QuestState.CAN_FINISH:
                questState.text = "완료 가능";
                break;
            case QuestState.FINISHED:
                questState.text = "완료";
                break;
        }
        questReward.text = "골드 보상 :"+questData.goldReward.ToString();
    }
}
