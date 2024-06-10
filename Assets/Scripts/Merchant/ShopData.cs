using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "ShopData", menuName = "Merchant/ShopData", order = 1)]
public class ShopData : ScriptableObject
{
    public List<Item> itemsForSale; // 상인이 판매하는 아이템 목록
}
