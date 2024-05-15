using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingManager
{
    /// <summary>
    /// 조합법을 들고 있는 해시테이블 캐싱
    /// </summary>
    private Hashtable CraftingTable = new Hashtable();
    
    /// <summary>
    /// 아이템 id에 해당하는 아이템의 이름을 저장하는 딕셔너리
    /// </summary>
    private Dictionary<int, string> itemCodeDict;
    
    /// <summary>
    /// 아이템 이름에 해당하는 아이템의 코드를 저장하는 딕셔너리
    /// </summary>
    private Dictionary<string, int> reverseItemCodeDict;

    /// <summary>
    /// 아이템 조합 슬롯에 넣은 아이템
    /// </summary>
    private UI_Inven_Item[] _items = new UI_Inven_Item[3];

    private Button CraftingButton;

    private int currentKey = 0;
    
    public void Init()
    {
        CraftingTable = new Hashtable();
        itemCodeDict = Managers.Data.itemCodeDict;
        reverseItemCodeDict = Managers.Data.reverseItemCodeDict;

        InitCraftingTable();
        
        for (int i = 0; i < 3; i++)
            _items[i] = null;
    }

    private void InitCraftingTable()
    {
        // 기본 조합식
        CraftingTable.Add(002004002, 128);
        CraftingTable.Add(004002000, 129);
        CraftingTable.Add(002004000, 130);
        CraftingTable.Add(002002004, 131);
        CraftingTable.Add(004002002, 132);
        CraftingTable.Add(004004012, 133);
        CraftingTable.Add(004001001, 134);
        CraftingTable.Add(003004003, 135);
        CraftingTable.Add(004003000, 136);
        CraftingTable.Add(003004000, 137);
        CraftingTable.Add(003003004, 138);
        CraftingTable.Add(004003003, 139);
        CraftingTable.Add(011010135, 140);
        CraftingTable.Add(001001000, 007);
        CraftingTable.Add(001001001, 012);
        CraftingTable.Add(001000001, 004);

        CraftingTable.Add(002002000, 008);
        CraftingTable.Add(002000002, 005);
        
        CraftingTable.Add(003003000, 009);
        CraftingTable.Add(003000003, 006);
    }

    /// <summary>
    /// 재료 아이템을 조합 칸에 등록하는 함수
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public void RegisterIngredientItem(int index, UI_Inven_Item item)
    {
        _items[index] = item;
        
        CraftingButton = Util.FindChild<Button>(Managers.UI.GetTopPopupUI().gameObject, "Crafting_Button", true);

        CraftingButton.interactable = IsCreatable();
        CraftingButton.gameObject.AddUIEvent(VisualizeProductionItem);
    }
    
    /// <summary>
    /// 현재 조합 칸에 등록된 아이템으로 조합이 가능한지 여부를 검사하는 함수
    /// 조합이 가능하다면, 조합 결과 아이템의 id를 반환
    /// 조합이 불가능하다면, false를 반환
    /// </summary>
    /// <returns></returns>
    public bool IsCreatable()
    {
        int[] Elements = new int[3];
        
        for (int i = 0; i < 3; i++)
        {
            if (_items[i] == null)
                Elements[i] = 0;
            else
                Elements[i] = reverseItemCodeDict[_items[i].Name];
        }
        
        currentKey = (int)(Elements[0] * Mathf.Pow(10, 6) + Elements[1] * Mathf.Pow(10, 3) + Elements[2]);
        
        DebugEx.LogWarning($" current Items on Table : {Elements[0]},{Elements[1]},{Elements[2]} | result : {CraftingTable[currentKey]}");
        
        return CraftingTable[currentKey] != null;
    }

    /// <summary>
    /// 조합 결과 아이템을 실제로 만드는 함수
    /// </summary>
    public void VisualizeProductionItem(PointerEventData eventData)
    {
        if (!IsCreatable()) return;
        
        UI_Inven_Slot productionSlot = Util.FindChild<UI_Inven_Slot>(Managers.UI.GetTopPopupUI().gameObject, "Crafting_ProductionSlot", true);
        
        int productionItemId = int.Parse(CraftingTable[currentKey].ToString());

        UI_Inven_Item productionItem = Managers.UI.MakeSubItem<UI_Inven_Item>(productionSlot.transform);
        string name = itemCodeDict[productionItemId];
        UI_Inven_Item.ItemType type = (productionItemId >= Managers.Data.BOUNDARY)
            ? UI_Inven_Item.ItemType.Tool
            : UI_Inven_Item.ItemType.Ingredient;

        switch (type)
        {
            case UI_Inven_Item.ItemType.Ingredient:
                productionItem.parentPanel = (Managers.UI.GetTopPopupUI() as UI_NotebookPopup)!.VisualizedLayer;
                productionItem.IngredientInit(name, 0, 1, null);
                break;
            case UI_Inven_Item.ItemType.Tool:
                productionItem.parentPanel = (Managers.UI.GetTopPopupUI() as UI_NotebookPopup)!.VisualizedLayer;
                productionItem.ToolInit(name, 0, 15, 0, null);
                break;
        }
    }
}
