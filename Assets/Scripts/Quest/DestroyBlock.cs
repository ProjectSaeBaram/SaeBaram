using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyBlock : QuestData
{
    public UnityAction QuestCompleted;
    private static DestroyBlock instance;

    public DestroyBlock(string name, int npc, string npcn, int Index, QuestState qs, int gold, string location, QuestType t = QuestType.None) : base(name, npc, npcn, Index, qs, gold, location, t)
    {
        base.questName = name;
        base.npcId = npc;
        base.npcname = npcn;
        base.Indexrequirment = Index;
        base.qs = qs;
        base.goldReward = gold;
        base.type = t;

        instance = this;
    }

    public static DestroyBlock GetInstance()
    {
        return instance;
    }



    public override QuestData getQuestData()
    {
        return this;
    }

    public override string getQuestInfo()
    {
        return "앞에 있는 벽을 부숴주세요";
    }

    public override void updateQuest()
    {
        qs++;
        QuestCompleted.Invoke(); // 퀘스트 완료 이벤트 호출
        Debug.Log("벽 부수기 퀘스트 완료!");
    }
}
