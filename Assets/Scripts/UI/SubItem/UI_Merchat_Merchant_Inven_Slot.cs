using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Merchat_Merchant_Inven_Slot : UI_Inven_Slot
{
    public UI_Merchant UIMerchant;
    public void SetUIMerchant(UI_Merchant uiMerchant)
    {
        UIMerchant = uiMerchant;
        if (Item != null)
        {
            Item.ToolTipHandler = uiMerchant;
        }
    }
}
