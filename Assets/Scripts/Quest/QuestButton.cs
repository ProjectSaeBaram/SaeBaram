using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour
{
    public QuestData qd;
    public Button questButton;
    public QuestInfoPanel questInfoPanel;
    public QuestData questData;

    private void Awake()
    {
        questButton = this.GetComponent<Button>();
        questButton.onClick.AddListener(() => QuestInfoPanel.GetInstance().setQuestInfo(qd));
    }
}
