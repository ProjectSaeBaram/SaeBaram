using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoincollectQuest : QuestData
{
    private int coinsCollected = 0;
    private int coinsToComplete = 5;

    private string name;
    private int npc;
    private string npcn;
    private int index;
    private QuestState state;
    private int gold;
    private string location;
    private QuestType ty;

    public CoincollectQuest(string n, int npc, string npcn, int Index, QuestState qs, int gold, string location, QuestType t)
    {
        this.questName = n;
        this.npcname = npcn;
        this.Indexrequirment = Index;
        this.qs = qs;
        this.goldReward = gold;
        this.loc = location;
        this.ty = t;
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
