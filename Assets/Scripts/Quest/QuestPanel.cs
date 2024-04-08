using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class QuestPanel : MonoBehaviour
{
    [Tab("QuestList")]
    [SerializeField] private QuestList questlist;
    [Tab("QuestInfo")]
    [SerializeField] private QuestInfoPanel infoPanel;

    private void Start()
    {
        questlist.DisplayProgessList();
    }
}
