using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ITooltipHandler 
{
    public void ShowToolTip(UI_Inven_Item invenItem, PointerEventData eventData);



    public void HideTooltip();
 
}
