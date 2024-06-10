using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public int Gold { get; set; }
    public int Experience { get; set; }
    public int QuestIndex { get; set; }
    public Dictionary<int, int> TalkIndex { get; set; } = new Dictionary<int, int>();
    public int Repute { get; set; }

    private DataManager dataManager;
    private QuestManager questManager;
    private DialogueManager dialogueManager;

    void Start()
    {
        dataManager = new DataManager();
        questManager = QuestManager.GetInstance();
        dialogueManager = DialogueManager.GetInstance();
       // dataManager.InitializeSession(this, questManager, dialogueManager);
    }

    void OnApplicationQuit()
    {
       // dataManager.SaveSession(this, questManager, dialogueManager);
    }

    public void AddGold(int amount)
    {
        Gold += amount;
    }

    public void AddExperience(int amount)
    {
        Experience += amount;
    }
}
