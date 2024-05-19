using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [System.Serializable]
    public class ItemData
    {
        public int id;
        public Sprite itemImage;
    }

    public List<ItemData> items = new List<ItemData>();

    public Sprite GetItemImageById(int id)
    {
        foreach (var item in items)
        {
            if (item.id == id)
            {
                return item.itemImage;
            }
        }
        return null; // or a default sprite
    }
}
