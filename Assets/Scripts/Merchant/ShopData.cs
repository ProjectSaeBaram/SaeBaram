using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "Merchant/ShopData", order = 1)]
public class ShopData : ScriptableObject
{
    public List<ItemData> itemsForSale; // 상인이 판매하는 아이템 목록

    public void InitShopItems()
    {
        itemsForSale = new List<ItemData>
        {
            new Tool(2, "똥 활", 1, 5, 0),
            new Ingredient(3, "나무 재료", 2, 63),
            new Ingredient(4, "돌 재료", 2, 63),
            new Ingredient(5, "철 재료", 2, 63)
            // 여기에 추가적으로 아이템을 추가합니다.
        };
    }
}
