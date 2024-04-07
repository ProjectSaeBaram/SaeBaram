using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class QuestLog : MonoBehaviour
{   
    //필요한 기능 1.새로고침기능 2.버튼추가기능 3.눌렀을 때 정보나오게하기

    public QuestManager quest;                //전체 패널에 붙이고 state를 채크해서 additem으로 지금 수행하고있는 퀘스트를 가져온다. 
    public List<Button> ButtonList;                //퀘스트 리스트 
    public ScrollView ScrollView;
    public TextMeshProUGUI QuestTitle;
    public TextMeshProUGUI QuestDescription;
    public ScrollRect QuestScrollRect;
    public List<QuestData> qd_progess=new List<QuestData>();
    public List<QuestData> qd_complete = new List<QuestData>();
    public GameObject questInfoPanel;
    //퀘스트 상세설명 -> 이름, 설명, 위치,관련된 npc이름,진행상황 textmeshpro 5개 필요?
    private void Start()
    {
           
    }

    public void RefreshquestList()                  //진행중인 퀘스트와 끝난 퀘스트를 찾아서 각 리스트에 넣어주고 지금 선택된 버튼에 따라 해당리스트로 버튼리스트를 초기화해준다.
    {
        for (int i = 0; i < quest.questList.Count; i++)
        {
            if (quest.questList[i].qs == QuestState.IN_PROGRESS)            //진행중인 퀘스트 받아오기 
            {
                qd_progess.Add(quest.questList[i].getQuestData());
            }
            else if (quest.questList[i].qs == QuestState.FINISHED)
            {
                qd_complete.Add(quest.questList[i].getQuestData());
            }
        }

        for(int i = 0;i< qd_progess.Count;i++)
        {
            ButtonList[i].GetComponent<QuestButton>().SetQuestInfo(qd_progess[i]);
            if(i>ButtonList.Count)                  //만약에 지금있는 버튼들보다 퀘스트표시해야될게 많다면 버튼을 추가해준다.
            {
                Managers.Resource.Instantiate("",this.transform);
                ButtonList.Add(this.transform.GetChild(i).GetComponent<Button>());
                ButtonList[i].GetComponent<QuestButton>().SetQuestInfo(qd_progess[i]);
            }
        }
    }



    public void AddQuest(QuestData data)
    {
        qd_progess.Add(data);
    }
}
