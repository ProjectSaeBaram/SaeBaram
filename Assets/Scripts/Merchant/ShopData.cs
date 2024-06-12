using System.Collections.Generic;
using UnityEngine;

public class ShopData : MonoBehaviour
{

    [SerializeField]public List<ItemData> itemsForSale=new List<ItemData>(); // 상인이 판매하는 아이템 목록

    [SerializeField]
    public void InitShopItems()
    {
        itemsForSale = new List<ItemData>
        {
            new Tool(129, "돌 도끼", 1, 5, 0),
        };
    }

    public ItemData GetMerchantItems(int idx)
    {
        return itemsForSale[idx];
    }
}
