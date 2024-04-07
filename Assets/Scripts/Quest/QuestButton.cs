using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour
{
    public TextMeshPro QuestName;
    public QuestData questData;

    private void Awake()
    {
        this.name = QuestName.text;
        //this.GetComponent<Button>().onClick.AddListener(() => QuestInfoPanel.GetInstance().setQuestInfo(qd));
    }

    public void SetQuestInfo(QuestData data)
    {
        QuestName.text = data.questName;
        questData=data;
    }
}
