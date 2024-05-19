using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemCodeDatabase", menuName = "Inventory/ItemCodeDatabase")]
public class ItemCodeDatabase : ScriptableObject
{
    [System.Serializable]
    public class ItemCodeEntry
    {
        public int id;
        public string name;
    }

    [SerializeField]
    private List<ItemCodeEntry> itemCodes = new List<ItemCodeEntry>();

    private Dictionary<int, string> itemCodeDict = new Dictionary<int, string>();
    private Dictionary<string, int> reverseItemCodeDict = new Dictionary<string, int>();

    private void OnEnable()
    {
        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        itemCodeDict = new Dictionary<int, string>();
        reverseItemCodeDict = new Dictionary<string, int>();

        foreach (var itemCode in itemCodes)
        {
            itemCodeDict[itemCode.id] = itemCode.name;
            reverseItemCodeDict[itemCode.name] = itemCode.id;
        }
    }

    public Dictionary<int, string> GetItemCodeDict()
    {
        if (itemCodeDict == null)
            InitializeDictionaries();
        return itemCodeDict;
    }

    public Dictionary<string, int> GetReverseItemCodeDict()
    {
        if (reverseItemCodeDict == null)
            InitializeDictionaries();
        return reverseItemCodeDict;
    }
}