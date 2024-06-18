using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Merchant_MerchantInven : UI_Popup
{
    [SerializeField] public TextMeshProUGUI Merchant_value;
    [SerializeField] public GameObject value_Button;

   public void showValue(bool value)
    {
        if (value)
        {
            Merchant_value.text = "우호 상점";
            value_Button.gameObject.SetActive(true);
        }
        else
        {
            Merchant_value.text = "단풍 상점";
            value_Button.gameObject.SetActive(true);
        }
    }

}
