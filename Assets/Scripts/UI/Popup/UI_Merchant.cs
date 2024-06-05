using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class UI_Merchant : UI_Popup
{
    [Tab("Merchant UI")]
    [SerializeField] private UI_MerchantInven Merchant_panel;
    [SerializeField] private UI_Merchant_PlayerInven Player_panel;
    [SerializeField] private Button Ex_Button;

    private void Start()
    {
        PlayerController player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        player.DisableInteract();
        Ex_Button.onClick.AddListener(() => CloseMerchant());
        //Player_panel.InitInventory(this,);
    }

    public void CloseMerchant()
    {
        Managers.UI.ClosePopupUI();
        PlayerController player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        player.EnableInteract();
    }
}
