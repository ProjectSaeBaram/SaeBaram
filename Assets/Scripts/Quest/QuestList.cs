using System;
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
    [SerializeField] public List<GameObject> ButtonList;                //퀘스트 리스트 
    [SerializeField] public ScrollView ScrollView;
    private ScrollRect QuestScrollRect;
    private List<QuestData> qd_progess;
    private List<QuestData> qd_complete;
    private static QuestList instance;
    //퀘스트 상세설명 -> 이름, 설명, 위치,관련된 npc이름,진행상황 textmeshpro 5개 필요?

    public static QuestList GetInstance()
    {
        return instance;
    }

    private void Start()        //진행중인 퀘스트 보여주기
    {  
        qd_progess = new List<QuestData>();
        qd_complete = new List<QuestData>();
        for (int i = 0; i < QuestManager.GetInstance().questList.Count; i++)
        {
            if (QuestManager.GetInstance().questList[i].qs == QuestState.IN_PROGRESS|| QuestManager.GetInstance().questList[i].qs == QuestState.CAN_FINISH)
            {
                qd_progess.Add(QuestManager.GetInstance().questList[i]);
            }
            else if (QuestManager.GetInstance().questList[i].qs == QuestState.FINISHED)
            {
                qd_complete.Add(QuestManager.GetInstance().questList[i]);
            }
        }
        int idx = 0;
        if (qd_progess.Count > ButtonList.Count)                  //만약에 지금있는 버튼들보다 퀘스트표시해야될게 많다면 버튼을 추가해준다.
        {
            while (qd_progess.Count > ButtonList.Count)
            {
                var newButton = Managers.Resource.Instantiate("UI/PopUp/QuestList", this.transform);
                var buttonComponent = newButton.GetComponent<Button>();
                ButtonList.Add(buttonComponent.gameObject);

                int newButtonIndex = ButtonList.Count - 1;
                buttonComponent.GetComponent<QuestButton>().SetQuestInfo(qd_progess[newButtonIndex]);
                buttonComponent.GetComponent<QuestButton>().SetQuestCheck(qd_progess[newButtonIndex]);
            }

        }

        for (int i = 0; i < qd_progess.Count; i++)
        {
            ButtonList[i].GetComponent<QuestButton>().SetQuestInfo(qd_progess[i]);
            ButtonList[i].GetComponent<QuestButton>().SetQuestCheck(qd_progess[i]);
            ButtonList[i].SetActive(true);
            Debug.Log($"Button {i} set active: {ButtonList[i].activeSelf} with quest: {qd_progess[i].questName}");
            idx++;
        }

        for (int i = idx; i < ButtonList.Count; i++)
        {
            ButtonList[i].gameObject.SetActive(false);
        }

        Debug.Log($"Button 0 set inactive.:{ButtonList[0].activeSelf}");

    }

    #region Refresh
    public void RefreshquestList()                  //진행중인 퀘스트와 끝난 퀘스트를 찾아서 각 리스트에 넣어주고 지금 선택된 버튼에 따라 해당리스트로 버튼리스트를 초기화해준다.
    {

        qd_progess = new List<QuestData>();
        qd_complete = new List<QuestData>();
        for (int i = 0; i < QuestManager.GetInstance().questList.Count; i++)
        {
            if (QuestManager.GetInstance().questList[i].qs == QuestState.IN_PROGRESS || QuestManager.GetInstance().questList[i].qs == QuestState.CAN_FINISH)
            {
                qd_progess.Add(QuestManager.GetInstance().questList[i]);
            }
            else if (QuestManager.GetInstance().questList[i].qs == QuestState.FINISHED)
            {
                qd_complete.Add(QuestManager.GetInstance().questList[i]);
            }
        }
        int idx = 0;
        DebugEx.Log(qd_progess.Count + "vs" + ButtonList.Count);
        DebugEx.Log(qd_progess.Count> ButtonList.Count);
        if (qd_progess.Count > ButtonList.Count)                  //만약에 지금있는 버튼들보다 퀘스트표시해야될게 많다면 버튼을 추가해준다.
        {
            while (qd_progess.Count > ButtonList.Count)
            {
                Managers.Resource.Instantiate("UI/PopUp/QuestButton", this.transform);                   //버튼을 생성하고
                ButtonList.Add(this.transform.GetChild(ButtonList.Count - 1).GetComponent<Button>().gameObject);      //버튼에 스크립트를 붙여준다.
                ButtonList[ButtonList.Count - 1].GetComponent<QuestButton>().SetQuestInfo(qd_progess[ButtonList.Count-1]);
                ButtonList[ButtonList.Count - 1].GetComponent<QuestButton>().SetQuestCheck(qd_progess[ButtonList.Count - 1]);
            }
        }
        for (int i = 0; i < qd_progess.Count; i++)
        {
            ButtonList[i].GetComponent<QuestButton>().SetQuestInfo(qd_progess[i]);
            ButtonList[i].GetComponent<QuestButton>().SetQuestCheck(qd_progess[i]);
            ButtonList[i].SetActive(true);
            Debug.Log($"Button {i} set active: {ButtonList[i].activeSelf} with quest: {qd_progess[i].questName}");
            idx++;
        }

        for (int i = idx; i < ButtonList.Count; i++)
        {
            ButtonList[i].gameObject.SetActive(false);
        }
        Debug.Log($"Button 0 set inactive.:{ButtonList[0].activeSelf}");
    }
    #endregion

    #region DisplayProgress
    public void DisplayProgessList()        //진행중인 퀘스트 보여주기
    {
        qd_progess = new List<QuestData>();
        for (int i = 0; i < QuestManager.GetInstance().questList.Count; i++)
        {
            if (QuestManager.GetInstance().questList[i].qs == QuestState.IN_PROGRESS || QuestManager.GetInstance().questList[i].qs == QuestState.CAN_FINISH)
            {
                qd_progess.Add(QuestManager.GetInstance().questList[i]);
            }
        }
        int idx = 0;
        if (qd_progess.Count > ButtonList.Count)                  //만약에 지금있는 버튼들보다 퀘스트표시해야될게 많다면 버튼을 추가해준다.
        {
            while (qd_progess.Count > ButtonList.Count)
            {
                Managers.Resource.Instantiate("UI/Popup/QuestButton.prefab", this.transform);                   //버튼을 생성하고
                ButtonList.Add(this.transform.GetChild(ButtonList.Count - 1).GetComponent<Button>().gameObject);      //버튼에 스크립트를 붙여준다.
                ButtonList[ButtonList.Count - 1].GetComponent<QuestButton>().SetQuestInfo(qd_progess[ButtonList.Count]);
                ButtonList[ButtonList.Count - 1].GetComponent<QuestButton>().SetQuestCheck(qd_progess[ButtonList.Count]);
            }
        }
        for (int i = 0; i < qd_progess.Count; i++)
        {
            ButtonList[i].GetComponent<QuestButton>().SetQuestInfo(qd_progess[i]);
            ButtonList[i].GetComponent<QuestButton>().SetQuestCheck(qd_progess[i]);
            ButtonList[i].SetActive(true);
            Debug.Log($"Button {i} set active: {ButtonList[i].activeSelf} with quest: {qd_progess[i].questName}");
            idx++;
        }

        for (int i = idx; i < ButtonList.Count; i++)
        {
            ButtonList[i].gameObject.SetActive(false);
          
        }

        Debug.Log($"Button 0 set inactive.:{ButtonList[0].activeSelf}");
    }
    #endregion

    #region DisplayFinished
    public void DisplayFinishedList()           //끝난 퀘스트 보여주기
    {
        qd_complete = new List<QuestData>();
        for (int i = 0; i < QuestManager.GetInstance().questList.Count; i++)
        {
            if (QuestManager.GetInstance().questList[i].qs == QuestState.FINISHED)
            {
                qd_complete.Add(QuestManager.GetInstance().questList[i]);
            }
        }
        int idx = 0;
        if (qd_complete.Count > ButtonList.Count)                  //만약에 지금있는 버튼들보다 퀘스트표시해야될게 많다면 버튼을 추가해준다.
        {
            while (qd_complete.Count > ButtonList.Count)
            {
                Managers.Resource.Instantiate("UI/Popup/QuestButton.prefab", this.transform);                   //버튼을 생성하고
                ButtonList.Add(this.transform.GetChild(ButtonList.Count - 1).GetComponent<Button>().gameObject);      //버튼에 스크립트를 붙여준다.
                ButtonList[ButtonList.Count - 1].GetComponent<QuestButton>().SetQuestInfo(qd_complete[ButtonList.Count]);
                ButtonList[ButtonList.Count - 1].GetComponent<QuestButton>().SetQuestCheck(qd_complete[ButtonList.Count]);
            }
        }
        for (int i = 0; i < qd_complete.Count; i++)
        {
            ButtonList[i].GetComponent<QuestButton>().SetQuestInfo(qd_complete[i]);
            ButtonList[i].GetComponent<QuestButton>().SetQuestCheck(qd_complete[i]);
            ButtonList[i].SetActive(true);
            Debug.Log($"Button {i} set active: {ButtonList[i].activeSelf} with quest: {qd_complete[i].questName}");
            idx++;
        }

        for (int i = idx; i < ButtonList.Count; i++)
        {
            ButtonList[i].gameObject.SetActive(false);
        }
        Debug.Log($"Button 0 set inactive.:{ButtonList[0].activeSelf}");
    }
    #endregion
}
