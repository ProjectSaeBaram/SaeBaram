using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VInspector;

public class QuestPanel : UI_Popup
{
    [Tab("QuestList")]
    [SerializeField] public QuestList questlist;
    [SerializeField] GameObject Questpop;
    [Tab("QuestInfo")]
    [SerializeField] private QuestInfoPanel infoPanel;

    private static QuestPanel instance;

    public static QuestPanel GetInstance()
    {
        return instance;
    }



    private void Start()
    {
        instance = this;
        questlist.gameObject.SetActive(true);
        infoPanel.gameObject.SetActive(true);
        //questlist.RefreshquestList();
        
    }

    public void DisplayQuest()
    {
        questlist.gameObject.SetActive(true);
        infoPanel.gameObject.SetActive(true);
    }
}
