using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReputeManager : MonoBehaviour
{
    //평판관리 시스템

    //평판 점수
    [SerializeField] private int ReputeScore;               


    public void Init()          //제일 초기에는 평판이 50
    {
        ReputeScore = 50;
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

    public int GetRepute()
    {
       return this.ReputeScore;
    }


    public void SetRepute(int score)
    {
        ReputeScore = score;
    }

    public void addRepute(int score)
    {
        if((ReputeScore +score)<=100)
        {
            ReputeScore += score;
        }
        else
        {
            ReputeScore = 100;
        }
    }

    public void DecreaseRepute(int score)
    {
        if ((ReputeScore - score) >= 0)
        {
            ReputeScore -= score;
        }
        else
        {
            ReputeScore = 0;
        }
    }
}
