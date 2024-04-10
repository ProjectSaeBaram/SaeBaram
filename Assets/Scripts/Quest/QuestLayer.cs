using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class QuestLayer : UI_Popup
{
    [Tab("QuestList")]
    [SerializeField] public QuestList questlist;
    [SerializeField] public Button Tab_Todo;
    [SerializeField] public Button Tab_Finished;
    [Tab("QuestInfo")]
    [SerializeField] private QuestInfoPanel infoPanel;

    private static QuestLayer instance;

    public static QuestLayer GetInstance()
    {
        return instance;
    }


    private void Start()
    {
        instance = this;
        Tab_Todo.onClick.AddListener(() => DisplayTodo());
        Tab_Finished.onClick.AddListener(() => DisplayFinished());
        questlist.gameObject.SetActive(true);
        infoPanel.gameObject.SetActive(true);
    }

    public void DisplayTodo()
    {
        Debug.Log("Todo!");
        questlist.DisplayProgessList();
        infoPanel.Resetting();
    }

    public void DisplayFinished()
    {
        Debug.Log("Finished!");
        questlist.DisplayFinishedList();
        infoPanel.Resetting();
    }

    public void DisplayQuest()
    {
        questlist.gameObject.SetActive(true);
        infoPanel.gameObject.SetActive(true);
    }
}
