using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestData 
{
    [Header("Quest Info")]
    public string questName;
    public int npcId;     //퀘스트를 가지고있는 npcId
    public string npcname;
    public int Indexrequirment;
    public QuestState qs;
    public string loc;              //퀘스트npc위치

    [Header("Reward Info")]
    public int goldReward;


    public QuestData(string name, int npc, string npcn,int Index,QuestState qs,int gold,string location)
    {
        questName = name;
        npcId = npc;
        npcname = npcn;
        Indexrequirment=Index;
        qs = QuestState.REQUIREMENTS_NOT_MET;
        goldReward = gold;
        loc=location;
}
    public abstract void updateQuest();

    public abstract QuestData getQuestData();

    public abstract string getQuestInfo();
}
