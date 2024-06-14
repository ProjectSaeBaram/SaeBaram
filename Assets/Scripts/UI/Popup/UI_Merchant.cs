using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VInspector;

public class UI_Merchant : UI_Popup, ICatcher, ITooltipHandler
{
    [Tab("Merchant UI")]
    [SerializeField] private UI_Merchant_MerchantInven merchantInventory;
    [SerializeField] public UI_Merchant_PlayerInven Player_panel;
    [SerializeField] private Button Ex_Button;
    [SerializeField] private UI_Inven_ItemTooltip tooltip;

    public UI_Inven_Item CatchedItem { get; set; }


    private void Start()
    {
        PlayerController _player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        _player.Disableall();
        Ex_Button.onClick.AddListener(() => CloseMerchant());
        //Player_panel.InitInventory(this,);
        Player_panel.merchant = this;
        Player_panel.InitInventory(Player_panel.VisualizedLayer, tooltip);
        

    }

    public void CloseMerchant()
    {
        Player_panel.ClosePopupUI(null);
        //Player_panel.ClosePopupUI();
        PlayerController player = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        player.Enableall();
    }

    public void ShowToolTip(UI_Inven_Item invenItem, PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(true);
        tooltip.ShowTooltip(invenItem, eventData);
    }

    public void HideTooltip()
    {
        tooltip.gameObject.SetActive(false);
        tooltip.UnsetPointerEventData();

    }
}
