using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MerchantInven : MonoBehaviour
{
    [SerializeField] public Confirm_Merchant ConfirmBuy;

    // Confirm_Merchant를 활성화하는 메서드
    public void ActivateConfirmBuy()
    {
        if (ConfirmBuy != null)
        {
            ConfirmBuy.gameObject.SetActive(true);
        }
    }

    // Confirm_Merchant를 비활성화하는 메서드
    public void DeactivateConfirmBuy()
    {
        if (ConfirmBuy != null)
        { 
            ConfirmBuy.gameObject.SetActive(false);
        }
    }
}
