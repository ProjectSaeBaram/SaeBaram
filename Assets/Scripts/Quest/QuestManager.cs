 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int[] questId;               //전체 아이디를 보관하는게 좋을까?
    public int questActionIndex;
    public int questIndex;
    public QuestList QuestList;
    public Dictionary<int, QuestData> questList;
    private Dictionary<int, NpcData> questNpc;

    private static QuestManager instance;

    private void Awake()
    {
        instance = this;
        questList = new Dictionary<int, QuestData>();               //퀘스트 아이디: 퀘스트 정보
        questNpc = new Dictionary<int, NpcData>();
        questIndex = 1;
        GenerateData();
        CheckRequirement();
    }

    public static QuestManager GetInstance()
    {
        return instance;
    }

    void GenerateData()
    {
        questList.Add(1, new MeetPeopleQuest("대화하기", 1000,"할아버지",1,QuestState.REQUIREMENTS_NOT_MET,10,"튜토리얼 마을",QuestType.Corauge));

        questList.Add(2, new CoincollectQuest("코인 모으기", 1000, "할아버지", 1, QuestState.CAN_START, 10, "튜토리얼 마을", QuestType.Corauge));

    }

    public void AdvanceQuest(int id,NpcData npc)            //퀘스트 진행상황 업데이트
    {

        npc.questActionIndex++;
        questNpc.Add(id, npc);
        questList[npc.questId[npc.questIndex]].qs++;
        Debug.Log(questList[id].qs);
        if (questList[npc.questId[npc.questIndex]].qs == QuestState.FINISHED)
        {
            AdvanceIndex(id, npc);
            return;
        }  

    }

    public void AdvanceQuest(int id)            //퀘스트 진행상황 업데이트
    {

        questActionIndex++;
        questList[questIndex].qs++;
        Debug.Log(questList[id].qs);
        if (questList[questIndex].qs == QuestState.FINISHED)
        {
            AdvanceIndex(id);
            return;
        }

    }
    public void AdvanceNpcIndex(NpcData npc)
    {
        if(npc.questId.Length>1)
        {
            npc.questIndex++;
        }
    }


    public void CheckRequirement()             //진행 순서가 맞아진다면 시작가능한 퀘스트 체크
    {
        for (int i = 1; i <= questList.Count; i++){
            if (questIndex >= questList[i].Indexrequirment && questList[i].qs==QuestState.REQUIREMENTS_NOT_MET)
            {
                questList[i].qs = QuestState.CAN_START;
                Debug.Log(questList[i].qs);
            }
        }
    }

    public QuestState CheckState(int id)          //퀘스트 현재 상태 반환
    {
        return questList[id].qs;
    }
    
    public void updateState(int id)
    {
        questList[id].updateQuest();
    }

    public int getnpcId(int id)
    {
        return questList[id].npcId;
    }

    public void AdvanceIndex(int qid)      //스토리 진행에 따라 다음 퀘스트가 진행될 수 있게 인덱스 값 증가
    {
        questIndex++;
        if (questNpc[qid].questId.Length > 1)
        {
            questNpc[qid].questIndex++;
        }
        questActionIndex = 0;
    }

    public void AdvanceIndex(int qid,NpcData npc)      //스토리 진행에 따라 다음 퀘스트가 진행될 수 있게 인덱스 값 증가
    {
        questIndex ++;
        if (questNpc[qid].questId.Length>1)
        {
            questNpc[qid].questIndex++;
        }
        npc.questActionIndex = 0;
    }


    public NpcData GetNpcId(int questId)
    {
        return questNpc[questId];
    }



    public QuestData GetQuestData(int questId)
    {
        return questList[questId].getQuestData();
    }

    //public int GetcurrentActionIndex(int questId)
    //{
    //    return questActionIndex;
    //}
}
