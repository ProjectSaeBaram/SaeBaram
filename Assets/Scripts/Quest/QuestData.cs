using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestData
{
    [Header("Quest Info")]
    public string ClassName;
    public string questName;
    public int npcId; // 퀘스트를 가지고 있는 NPC의 ID
    public string npcname;
    public int Indexrequirment;
    public QuestState qs;
    public QuestType type;
    public string loc; // 퀘스트 NPC 위치

    [Header("Reward Info")]
    public int goldReward;

    public QuestData(string name, int npc, string npcn, int index, QuestState questState, int gold, string location, QuestType questType = QuestType.None)
    {
        ClassName = this.GetType().Name; // 클래스 이름을 자동으로 설정
        questName = name;
        npcId = npc;
        npcname = npcn;
        Indexrequirment = index;
        qs = questState;
        goldReward = gold;
        loc = location;
        type = questType;
    }

    public abstract void updateQuest();
    public abstract QuestData getQuestData();
    public abstract string getQuestInfo();
}
