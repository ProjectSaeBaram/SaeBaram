using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class UI_Merchant : UI_Popup
{
    [Tab("Merchant UI")]
    [SerializeField] private GameObject Merchant_panel;
    [SerializeField] private Button Ex_Button;

    private void Start()
    {
        Ex_Button.onClick.AddListener(() => Managers.UI.ClosePopupUI());
    }
}
