using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReputeManager : MonoBehaviour
{
    //평판관리 시스템

    //평판 점수
    [SerializeField] private int ReputeScore;
    [SerializeField] private ReputeState ReputeState;


    public void Init()          //제일 초기에는 평판이 50
    {
        ReputeScore = 50;
        ReputeState=SetReputeState(ReputeScore);
        DebugEx.Log("현재 평판 점수 : " + ReputeScore+" 평판도 "+GetReputeState(ReputeState));
       
    }

    public ReputeState SetReputeState(int score)
    {
        if (score >= 60)
        {

            return ReputeState.Good;
        }
        else if(score<=60&&score>=40)
        {
            return ReputeState.Normal;
        }
        else
        {
            return ReputeState.Bad;
        }
 
    }
    public string GetReputeState(ReputeState reputeState)
    {
        switch(reputeState)
        {
            case ReputeState.Good:
                return "선함";
            case ReputeState.Normal:
                return "보통";
            case ReputeState.Bad:
                return "악명";
            default:
                return "";

        }

    }

    public int GetRepute()
    {
       return this.ReputeScore;
    }


    public void SetRepute(int score)
    {
        ReputeScore = score;
    }

    public void AddRepute(int score)
    {
        ReputeScore = Mathf.Min(ReputeScore + score, 100);
        ReputeState = SetReputeState(ReputeScore);
        DebugEx.Log("현재 평판 점수 : " + ReputeScore + "평판도 " + GetReputeState(ReputeState));
    }

    public void DecreaseRepute(int score)
    {
        ReputeScore = Mathf.Max(ReputeScore - score, 0);
        ReputeState = SetReputeState(ReputeScore);
        DebugEx.Log("현재 평판 점수 : " + ReputeScore + "평판도 " + GetReputeState(ReputeState));
    }
}
