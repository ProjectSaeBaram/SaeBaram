using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CraftingManager
{
    // 조합식을 저장한 스크립터블 오브젝트 참조
    public CraftingRecipeDatabase craftingRecipeDatabase; 
    
    /// <summary>
    /// 아이템 id에 해당하는 아이템의 이름을 저장하는 딕셔너리
    /// </summary>
    private Dictionary<int, string> itemCodeDict;
    
    /// <summary>
    /// 아이템 이름에 해당하는 아이템의 코드를 저장하는 딕셔너리
    /// </summary>
    private Dictionary<string, int> reverseItemCodeDict;

    private const int SizeOfCraftingSlot = 3;
    
    /// <summary>
    /// 아이템 조합 슬롯
    /// </summary>
    private UI_Inven_CraftingSlot[] _craftingSlots = new UI_Inven_CraftingSlot[SizeOfCraftingSlot];
    
    /// <summary>
    /// 아이템 조합 버튼
    /// </summary>
    private Button CraftingButton;
    
    /// <summary>
    /// 조합 결과물 아이템 슬롯
    /// </summary>
    private UI_Inven_ProductionSlot ProductionSlot;
    
    /// <summary>
    /// 조합식을 검색할 때 Invoke 될 UnityAction
    /// 조합식은 슬롯에 아이템을 넣을 때와, 뺄 때 모두 검색한다.
    /// </summary>
    public UnityAction OnItemForCraftingChanged;
    
    public void Init()
    {
        craftingRecipeDatabase = Managers.Resource.Load<CraftingRecipeDatabase>("Contents/CraftingRecipeDatabase");
        
        itemCodeDict = Managers.Data.itemCodeDict;
        reverseItemCodeDict = Managers.Data.reverseItemCodeDict;
        
        OnItemForCraftingChanged -= CachingUIElements;
        OnItemForCraftingChanged -= CheckCreatable;
        OnItemForCraftingChanged += CachingUIElements;
        OnItemForCraftingChanged += CheckCreatable;
    }
    
    /// <summary>
    /// 조합에 필요한 UI 요소를 찾아서 캐싱하는 함수
    /// </summary>
    private void CachingUIElements()
    {
        // CraftingSlot들 찾아서 캐싱
        foreach (var slot in Object.FindObjectsOfType<UI_Inven_CraftingSlot>())
            _craftingSlots[slot.CraftingSlotIndex] = slot;
        
        // CraftingButton 찾아서 캐싱 (+버튼에 이벤트 바인드)
        CraftingButton = Util.FindChild<Button>(Managers.UI.GetTopPopupUI().gameObject, "Crafting_Button", true);
        CraftingButton.onClick.RemoveAllListeners();
        CraftingButton.onClick.AddListener(VisualizeProductionItem);

        // 아이템 결과물 슬롯
        ProductionSlot = Util.FindChild<UI_Inven_ProductionSlot>(Managers.UI.GetTopPopupUI().gameObject,
            "Crafting_ProductionSlot", true);
    }
    
    private void CheckCreatable()
    {
        IsCreatable();
    }
    
    /// <summary>
    /// 현재 조합 칸에 등록된 아이템으로 조합이 가능한지 여부를 검사하는 함수
    /// 조합이 가능하다면, 조합 결과 아이템의 id를 반환
    /// 조합이 불가능하다면, false를 반환
    /// </summary>
    /// <returns></returns>
    public bool IsCreatable()
    {
        int[] subKeys = new int[SizeOfCraftingSlot];

        for (var i = 0; i < SizeOfCraftingSlot; i++)
        {
            if (_craftingSlots[i].Item != null)
                subKeys[i] = reverseItemCodeDict[_craftingSlots[i].Item.Name];
            else
                subKeys[i] = 0;
        }

        int? outputItemId = craftingRecipeDatabase.GetOutputItemId(subKeys);
        DebugEx.LogWarning($" current Items on Table : {subKeys[0]},{subKeys[1]},{subKeys[2]} | result : {outputItemId}");

        return outputItemId != null;
    }
    
    /// <summary>
    /// 조합 결과 아이템의 퀄리티를 산출하는 함수
    /// </summary>
    /// <returns></returns>
    private int GetQualityOfProductionItem()
    {
        int qualityPoint = 0;
    
        // qualityPoint는 재료로 들어간 아이템들의 (퀄리티 * 갯수)의 합
        foreach (var craftingSlot in _craftingSlots)
            if (craftingSlot.Item is not null) 
                qualityPoint += craftingSlot.Item.Quality * craftingSlot.Item.Amount;
        
        const int ensureHighQuality = 22;            // 무조건 상 품질을 보장하는 경계
        const int betweenHighAndMedium = 12;         // 상 품질과 중 품질을 구분짓는 경계
        const int betweenMediumAndLow = 6;           // 중 품질과 하 품질을 구분짓는 경계

        int result;
        switch (qualityPoint)
        {
            case >= ensureHighQuality:          // 무조건 상 품질을 보장하는 경계
                result = 3;  // 무조건 상 품질
                break;
            case >= betweenHighAndMedium:       // 상 품질과 중 품질을 구분짓는 경계
            {
                float randValue = Random.value;
                result = randValue switch
                {
                    < 0.70f => 3,           // 70% 확률로 상 품질
                    < 0.90f => 2,           // 20% 확률로 중 품질
                    _ => 1                  // 10% 확률로 하 품질 
                };
                break;
            }
            case >= betweenMediumAndLow:        // 중 품질과 하 품질을 구분짓는 경계
            {
                float randValue = Random.value;
                result = randValue switch
                {
                    < 0.70f => 2,           // 70% 확률로 중 품질
                    < 0.90f => 3,           // 20% 확률로 상 품질
                    _ => 1                  // 10% 확률로 하 품질
                };
                break;
            }
            default:
            {
                float randValue = Random.value;
                result = randValue switch
                {
                    < 0.70f => 1,           // 70% 확률로 하 품질
                    < 0.90f => 2,           // 20% 확률로 중 품질
                    _ => 3                  // 10% 확률로 상 품질
                };
                break;
            }
        }

        DebugEx.LogWarning($"total qualityPoint of Items : {qualityPoint} | result : {result}");
        return result;
    }
    
    /// <summary>
    /// 조합 결과 아이템을 실제로 만드는 함수
    /// </summary>
    public void VisualizeProductionItem()
    {
        if (!IsCreatable() || ProductionSlot.Item != null) return;
    
        int[] subKeys = new int[SizeOfCraftingSlot];
        for (var i = 0; i < SizeOfCraftingSlot; i++)
        {
            if (_craftingSlots[i].Item != null)
                subKeys[i] = reverseItemCodeDict[_craftingSlots[i].Item.Name];
            else
                subKeys[i] = 0;
        }
        int? outputItemId = craftingRecipeDatabase.GetOutputItemId(subKeys);
        if (outputItemId == null) return;

        int productionItemId = outputItemId.Value;

        UI_Inven_Item productionItem = Managers.UI.MakeSubItem<UI_Inven_Item>(ProductionSlot.transform);
        ProductionSlot.Item = productionItem;
        string name = itemCodeDict[productionItemId];
        int quality = GetQualityOfProductionItem();

        UI_Inven_Item.ItemType type = (productionItemId >= Managers.Data.BOUNDARY)
            ? UI_Inven_Item.ItemType.Tool
            : UI_Inven_Item.ItemType.Ingredient;

        switch (type)
        {
            case UI_Inven_Item.ItemType.Ingredient:
                productionItem.parentPanel = (Managers.UI.GetTopPopupUI() as UI_NotebookPopup)?.VisualizedLayer;
                productionItem.IngredientInit(name, quality, 1, null);
                break;
            case UI_Inven_Item.ItemType.Tool:
                productionItem.parentPanel = (Managers.UI.GetTopPopupUI() as UI_NotebookPopup)?.VisualizedLayer;
                productionItem.ToolInit(name, quality, 15, 0, null);
                break;
        }

        foreach (var craftingSlot in _craftingSlots)
        {
            if (craftingSlot.Item != null)
            {
                Object.DestroyImmediate(craftingSlot.Item.gameObject);
                craftingSlot.Item = null;
            }
        }
    }
    
    /// <summary>
    /// 조합식을 추가하는 메서드
    /// </summary>
    /// <param name="inputItemIds">재료로 사용되는 아이템의 id를 담은 배열 (size:3)</param>
    /// <param name="outputItemId">조합 결과로 나올 아이템</param>
    public void AddCraftingRecipe(int[] inputItemIds, int outputItemId)
    {
        craftingRecipeDatabase.AddRecipe(inputItemIds, outputItemId);
    }

    /// <summary>
    /// 조합식을 제거하는 메서드
    /// </summary>
    /// <param name="inputItemIds">재료로 사용되는 아이템의 id를 담은 배열 (size:3)</param>
    public void RemoveCraftingRecipe(int[] inputItemIds)
    {
        craftingRecipeDatabase.RemoveRecipe(inputItemIds);
    }
}
