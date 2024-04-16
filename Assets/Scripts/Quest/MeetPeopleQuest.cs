using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeetPeopleQuest : QuestData
{
    public bool isMeet;
    private string name;
    private int npc;
    private string npcn;
    private int index;
    private QuestState state;
    private int gold;
    private string location;
    private QuestType ty;
    public MeetPeopleQuest(string name, int npc, string npcn, int Index, QuestState qs, int gold, string location, QuestType t)
    {
        this.questName = name;
        this.npcname = npcn;
        this.Indexrequirment = Index;
        this.qs = qs;
        this.goldReward = gold;
        this.loc = location;
        this.ty = t;
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
