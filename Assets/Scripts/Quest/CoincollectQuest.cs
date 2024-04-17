using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoincollectQuest : QuestData
{
    private int coinsCollected = 0;
    private int coinsToComplete = 5;

    public CoincollectQuest(string name, int npc, string npcn, int Index, QuestState qs, int gold, string location, QuestType t) : base(name, npc, npcn, Index, qs, gold, location, t)
    {
        base.questName = name;
        base.npcId = npc;
        base.npcname = npcn;
        base.Indexrequirment = Index;
        base.qs = qs;
        base.goldReward = gold;
        base.type = t;
    }

    //코인을 수집함에 따라 퀘스트 진행상황 체크

    public void Getcoin(int coin)
    {
        coinsCollected++;
        updateQuest();
    }


    public override QuestData getQuestData()
    {
        return this;
    }

    public override string getQuestInfo()
    {
        return "코인을 5개모아오거라";
    }

    public override void updateQuest()
    {
        if (coinsCollected == coinsToComplete)
        {
            this.qs++;
        }
    }
}
