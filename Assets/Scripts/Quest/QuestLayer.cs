using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class QuestLayer : UI_Popup
{
    [Tab("QuestList")]
    [SerializeField] public QuestList questlist;
    [SerializeField] public Button Tab_Todo;
    [SerializeField] public TextMeshProUGUI Tab_Todo_Text;
    [SerializeField] public Button Tab_Finished;
    [SerializeField] public TextMeshProUGUI Tab_Finished_Text;
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
        Tab_Todo.GetComponent<Button>().onClick.AddListener(() => DisplayTodo());
        Tab_Todo_Text.color = Color.black;
        Tab_Finished_Text.color = Color.gray;
        Tab_Finished.GetComponent<Button>().onClick.AddListener(() => DisplayFinished());
        questlist.gameObject.SetActive(true);
        infoPanel.gameObject.SetActive(true);
        questlist.RefreshquestList();
        DisplayTodo();
        DisplayQuest();
        DisplayTodo();
    }

    public void DisplayTodo()
    {
        Debug.Log("Todo!");
        Tab_Todo_Text.color = Color.black;
        Tab_Finished_Text.color = Color.gray;
        questlist.DisplayProgessList();
        infoPanel.Resetting();
    }

    public void DisplayFinished()
    {
        Debug.Log("Finished!");
        Tab_Todo_Text.color = Color.gray;
        Tab_Finished_Text.color = Color.black;
        questlist.DisplayFinishedList();
        infoPanel.Resetting();
    }

    public void DisplayQuest()
    {
        questlist.gameObject.SetActive(true);
        infoPanel.gameObject.SetActive(true);
    }
}
