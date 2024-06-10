using UnityEngine;
using UnityEngine.Events;

public class DestroyBlock : QuestData
{
    public UnityEvent QuestCompleted;
    private static DestroyBlock instance;

    public DestroyBlock(string name, int npc, string npcn, int index, QuestState questState, int gold, string location, QuestType questType = QuestType.None)
        : base(name, npc, npcn, index, questState, gold, location, questType)
    {
        if (QuestCompleted == null)
        {
            QuestCompleted = new UnityEvent();
        }
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
