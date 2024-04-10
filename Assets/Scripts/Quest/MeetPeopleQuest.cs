using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeetPeopleQuest : QuestData
{
    public bool isMeet;

    public MeetPeopleQuest(string name, int npc, string npcn, int Index, QuestState qs, int gold, string location) : base(name, npc, npcn, Index, qs, gold, location)
    {
        base.questName = name;
        base.npcId = npc;
        base.npcname= npcn;
        base.Indexrequirment = Index;
        base.qs = qs;
        base.goldReward = gold;
        base.loc = location;
    }


    public override QuestData getQuestData()
    {
        return this;
    }

    public override string getQuestInfo()
    {
        return "퀘스트설명1"; //퀘스트 설명
    }

    public override void updateQuest()
    {
        qs++;
    }
}
