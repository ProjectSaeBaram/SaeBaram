using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoincollectQuest : QuestData
{
    private int coinsCollected = 0;
    private int coinsToComplete = 5;


    public CoincollectQuest(string name, int npc, string npcn, int Index, QuestState qs, int gold, string location) : base(name, npc, npcn, Index, qs, gold, location)
    {
        base.questName = name;
        base.npcId = npc;
        base.npcname = npcn;
        base.Indexrequirment = Index;
        base.qs = qs;
        base.goldReward = gold;
        base.loc = location;
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
        return "";
    }

    public override void updateQuest()
    {
        if (coinsCollected == coinsToComplete)
        {
            qs++;
        }
    }
}
