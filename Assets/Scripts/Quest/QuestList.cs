using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using VInspector;
using Button = UnityEngine.UI.Button;

public class QuestList : UI_Popup
{
    //필요한 기능 1.새로고침기능 2.버튼추가기능 3.눌렀을 때 정보나오게하기

    [Tab("QuestList Info")]
    [SerializeField] public List<Button> ButtonList;                //퀘스트 리스트 
    [SerializeField] public ScrollView ScrollView;
    private ScrollRect QuestScrollRect;
    private List<QuestData> qd_progess;
    private List<QuestData> qd_complete;
    //퀘스트 상세설명 -> 이름, 설명, 위치,관련된 npc이름,진행상황 textmeshpro 5개 필요?
    private void Start()        //진행중인 퀘스트 보여주기
    {
        qd_progess = new List<QuestData>();
        qd_complete = new List<QuestData>();
        int idx = 0;
        if (qd_progess.Count > ButtonList.Count)                  //만약에 지금있는 버튼들보다 퀘스트표시해야될게 많다면 버튼을 추가해준다.
        {
            while (qd_progess.Count > ButtonList.Count)
            {
                Managers.Resource.Instantiate("", this.transform);                   //버튼을 생성하고
                ButtonList.Add(this.transform.GetChild(ButtonList.Count - 1).GetComponent<Button>());      //버튼에 스크립트를 붙여준다.
                ButtonList[ButtonList.Count - 1].GetComponent<QuestButton>().SetQuestInfo(qd_progess[ButtonList.Count]);
            }
        }
        for (int i = 0; i < qd_progess.Count; i++)
        {
            ButtonList[i].GetComponent<QuestButton>().SetQuestInfo(qd_progess[i]);
            ButtonList[i].gameObject.SetActive(true);
            idx++;
        }
        for (int i = idx; i < ButtonList.Count; i++)
        {
            ButtonList[i].gameObject.SetActive(false);
        }
    }

    public void RefreshquestList()                  //진행중인 퀘스트와 끝난 퀘스트를 찾아서 각 리스트에 넣어주고 지금 선택된 버튼에 따라 해당리스트로 버튼리스트를 초기화해준다.
    {
        qd_progess = new List<QuestData>();
        qd_complete = new List<QuestData>();
        for (int i = 0; i < QuestManager.GetInstance().questList.Count; i++)
        {
            if (QuestManager.GetInstance().questList[i].qs == QuestState.IN_PROGRESS)            //진행중인 퀘스트 받아오기 
            {
                qd_progess.Add(QuestManager.GetInstance().questList[i].getQuestData());
            }
            else if (QuestManager.GetInstance().questList[i].qs == QuestState.FINISHED)
            {
                qd_complete.Add(QuestManager.GetInstance().questList[i].getQuestData());
            }
        }
    }

    public void DisplayProgessList()        //진행중인 퀘스트 보여주기
    {
        qd_progess = new List<QuestData>();
        int idx = 0;
        if (qd_progess.Count > ButtonList.Count)                  //만약에 지금있는 버튼들보다 퀘스트표시해야될게 많다면 버튼을 추가해준다.
        {
            while(qd_progess.Count > ButtonList.Count) 
            {
                Managers.Resource.Instantiate("", this.transform);                   //버튼을 생성하고
                ButtonList.Add(this.transform.GetChild(ButtonList.Count - 1).GetComponent<Button>());      //버튼에 스크립트를 붙여준다.
                ButtonList[ButtonList.Count - 1].GetComponent<QuestButton>().SetQuestInfo(qd_progess[ButtonList.Count]);
            }  
        }
        for (int i = 0; i < qd_progess.Count; i++)
        {
            ButtonList[i].GetComponent<QuestButton>().SetQuestInfo(qd_progess[i]);
            ButtonList[i].gameObject.SetActive(true);      
            idx++;
        }
        for (int i = idx; i < ButtonList.Count; i++)
        {
            ButtonList[i].gameObject.SetActive(false);
        }
    }

    public void DisplayFinishedList()           //끝난 퀘스트 보여주기
    {
        qd_complete = new List<QuestData>();
        int idx = 0;
        if (qd_complete.Count > ButtonList.Count)                  //만약에 지금있는 버튼들보다 퀘스트표시해야될게 많다면 버튼을 추가해준다.
        {
            while (qd_complete.Count > ButtonList.Count)
            {
                Managers.Resource.Instantiate("", this.transform);                   //버튼을 생성하고
                ButtonList.Add(this.transform.GetChild(ButtonList.Count - 1).GetComponent<Button>());      //버튼에 스크립트를 붙여준다.
                ButtonList[ButtonList.Count - 1].GetComponent<QuestButton>().SetQuestInfo(qd_progess[ButtonList.Count]);
            }
        }
        for (int i = 0; i < qd_complete.Count; i++)
        {
            ButtonList[i].GetComponent<QuestButton>().SetQuestInfo(qd_progess[i]);
            ButtonList[i].gameObject.SetActive(true);
            idx++;
        }
        for(int i = idx; i < ButtonList.Count; i++)
        {
            ButtonList[i].gameObject.SetActive(false);
        }
    }


    public void AddQuest(QuestData data)
    {
        qd_progess.Add(data);
    }
}
