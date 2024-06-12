using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 조합 레시피 데이터베이스를 관리하는 스크립터블 오브젝트 클래스
/// </summary>
[CreateAssetMenu(fileName = "CraftingRecipeDatabase", menuName = "Inventory/CraftingRecipeDatabase")]
public class CraftingRecipeDatabase : ScriptableObject
{
    /// <summary>
    /// 단일 조합 레시피를 나타내는 클래스
    /// </summary>
    [System.Serializable]
    public class CraftingRecipe
    {
        /// <summary>
        /// 입력 아이템 ID 배열
        /// </summary>
        public int[] inputItemIds = new int[3];

        /// <summary>
        /// 출력 아이템 ID
        /// </summary>
        public int outputItemId;
    }

    /// <summary>
    /// 모든 조합 레시피를 저장하는 리스트
    /// </summary>
    [SerializeField]
    private List<CraftingRecipe> recipes = new List<CraftingRecipe>();

    /// <summary>
    /// 조합 레시피를 빠르게 검색하기 위한 해시테이블
    /// </summary>
    private Hashtable craftingTable = new Hashtable();

    /// <summary>
    /// 스크립터블 오브젝트가 활성화될 때 호출되어 해시테이블을 초기화합니다.
    /// </summary>
    private void OnEnable()
    {
        InitializeCraftingTable();
    }

    /// <summary>
    /// 해시테이블을 초기화하여 모든 조합 레시피를 추가합니다.
    /// </summary>
    private void InitializeCraftingTable()
    {
        craftingTable.Clear();
        foreach (var recipe in recipes)
        {
            int key = GenerateKey(recipe.inputItemIds);
            if (!craftingTable.ContainsKey(key))
            {
                craftingTable.Add(key, recipe.outputItemId);
            }
        }
    }

    /// <summary>
    /// 입력 아이템 ID 배열로부터 고유한 키를 생성합니다.
    /// </summary>
    /// <param name="inputItemIds">입력 아이템 ID 배열</param>
    /// <returns>고유한 정수 키</returns>
    private int GenerateKey(int[] inputItemIds)
    {
        int key = 0;
        for (int i = 0; i < inputItemIds.Length; i++)
            key += inputItemIds[i] * (int)Mathf.Pow(10, 3 * (inputItemIds.Length - 1 - i));
        
        return key;
    }

    /// <summary>
    /// 입력 아이템 ID 배열에 해당하는 출력 아이템 ID를 반환합니다.
    /// </summary>
    /// <param name="inputItemIds">입력 아이템 ID 배열</param>
    /// <returns>출력 아이템 ID, 해당 레시피가 없으면 null</returns>
    public int? GetOutputItemId(int[] inputItemIds)
    {
        int key = GenerateKey(inputItemIds);
        if (craftingTable.ContainsKey(key))
        {
            return (int)craftingTable[key];
        }
        return null;
    }

    /// <summary>
    /// 새로운 조합 레시피를 추가.
    /// </summary>
    /// <param name="inputItemIds">입력 아이템 ID 배열</param>
    /// <param name="outputItemId">출력 아이템 ID</param>
    public void AddRecipe(int[] inputItemIds, int outputItemId)
    {
        CraftingRecipe newRecipe = new CraftingRecipe { inputItemIds = inputItemIds, outputItemId = outputItemId };
        recipes.Add(newRecipe);
        int key = GenerateKey(inputItemIds);
        if (!craftingTable.ContainsKey(key))
        {
            craftingTable.Add(key, outputItemId);
        }
    }

    /// <summary>
    /// 기존의 조합 레시피를 제거.
    /// </summary>
    /// <param name="inputItemIds">입력 아이템 ID 배열</param>
    public void RemoveRecipe(int[] inputItemIds)
    {
        int key = GenerateKey(inputItemIds);
        if (craftingTable.ContainsKey(key))
        {
            craftingTable.Remove(key);
        }
        recipes.RemoveAll(recipe => GenerateKey(recipe.inputItemIds) == key);
    }
}
