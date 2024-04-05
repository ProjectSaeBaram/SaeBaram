using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class QuestLog : MonoBehaviour
{
    public QuestManager quest;                //전체 패널에 붙이고 state를 채크해서 additem으로 지금 수행하고있는 퀘스트를 가져온다. 
    public Button[] QuestList;                //퀘스트 리스트 

    public TextMeshProUGUI QuestTitle;
    public TextMeshProUGUI QuestDescription;
    public ScrollRect QuestScrollRect;
    public QuestData qd;
    public GameObject questInfoPanel;
    //퀘스트 상세설명 -> 이름, 설명, 위치,관련된 npc이름,진행상황 textmeshpro 5개 필요?
    private void Start()
    {
        QuestScrollRect.normalizedPosition = new Vector2(1f, 1f);                   //사이즈 동적으로 바꿔주기
        Vector2 size = QuestScrollRect.content.sizeDelta;                   
        size.y = 5000f;
        QuestScrollRect.content.sizeDelta = size;

    }

    public void addQuest(int id)
    {
        GameObject qeust= Resources.Load("QuestButton") as GameObject;
        qd=QuestManager.GetInstance().GetQuestData(id);
        
        GameObject instance = PrefabUtility.InstantiatePrefab(qeust) as GameObject;     //프리팹추가해주기

        instance.transform.SetParent(QuestScrollRect.content.transform);
        instance.name = qd.questName;
        instance.AddComponent<QuestButton>();
        QuestButton qb=instance.GetComponent<QuestButton>();
        qb.questData = qd;
        //퀘스트에 따라 이름 및 퀘스트 스크립트 추가해주기 
    }

   
}
