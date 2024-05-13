using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager
{
    // 해시테이블 캐싱
    private Hashtable CraftingTable = new Hashtable();
    
    private Dictionary<int, string> itemCodeDict;
    private Dictionary<string, int> reverseItemCodeDict;

    private UI_Inven_Item[] _items = new UI_Inven_Item[3];
    
    public void Init()
    {
        CraftingTable = new Hashtable();
        itemCodeDict = Managers.Data.itemCodeDict;
        reverseItemCodeDict = Managers.Data.reverseItemCodeDict;

        InitCraftingTable();
        
        for (int i = 0; i < 3; i++)
        {
            _items[i] = null;
        }
    }

    private void InitCraftingTable()
    {
        CraftingTable.Add(001002003, 128);
    }

    public void RegisterIngredientItem(int index, UI_Inven_Item item)
    {
        _items[index] = item;

        // IsCreatable();
        DebugEx.LogWarning(IsCreatable().ToString());
    }
    
    public object IsCreatable()
    {
        int[] Elements = new int[3];
        
        for (int i = 0; i < 3; i++)
        {
            if (_items[i] == null)
            {
                Elements[i] = 0;
            }
            else
            {
                Elements[i] = reverseItemCodeDict[_items[i].Name];
            }
        }
        
        int find = (int)(Elements[0] * Mathf.Pow(10, 6) + Elements[1] * Mathf.Pow(10, 3) + Elements[2]);
        return (CraftingTable[find] != null) ? CraftingTable[find] : false;
    }
    
    
    
}
