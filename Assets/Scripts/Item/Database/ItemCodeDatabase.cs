using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 코드 데이터베이스를 관리하는 스크립터블 오브젝트 클래스
/// </summary>
[CreateAssetMenu(fileName = "ItemCodeDatabase", menuName = "Inventory/ItemCodeDatabase")]
public class ItemCodeDatabase : ScriptableObject
{
    /// <summary>
    /// 아이템 코드와 이름을 저장하는 클래스
    /// </summary>
    [System.Serializable]
    public class ItemCodeEntry
    {
        [Tooltip("아이템 ID")]
        public int id; // 아이템의 고유 ID
        
        [Tooltip("아이템 이름")]
        public string name; // 아이템의 이름
    }

    /// <summary>
    /// 아이템 코드 엔트리 리스트
    /// </summary>
    [SerializeField]
    private List<ItemCodeEntry> itemCodes = new List<ItemCodeEntry>();

    /// <summary>
    /// 아이템 ID를 키로, 아이템 이름을 값으로 가지는 딕셔너리
    /// </summary>
    private Dictionary<int, string> itemCodeDict = new Dictionary<int, string>();

    /// <summary>
    /// 아이템 이름을 키로, 아이템 ID를 값으로 가지는 딕셔너리
    /// </summary>
    private Dictionary<string, int> reverseItemCodeDict = new Dictionary<string, int>();

    /// <summary>
    /// 스크립터블 오브젝트가 활성화될 때 호출되는 함수
    /// 딕셔너리를 초기화함
    /// </summary>
    private void OnEnable()
    {
        InitializeDictionaries();
    }

    /// <summary>
    /// 아이템 코드 딕셔너리를 초기화하는 함수
    /// 아이템 코드 리스트를 순회하며 딕셔너리에 추가함
    /// </summary>
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

    /// <summary>
    /// 아이템 ID를 키로 가지는 아이템 코드 딕셔너리를 반환하는 함수
    /// </summary>
    /// <returns>아이템 코드 딕셔너리</returns>
    public Dictionary<int, string> GetItemCodeDict()
    {
        if (itemCodeDict == null)
            InitializeDictionaries();
        return itemCodeDict;
    }

    /// <summary>
    /// 아이템 이름을 키로 가지는 역방향 아이템 코드 딕셔너리를 반환하는 함수
    /// </summary>
    /// <returns>역방향 아이템 코드 딕셔너리</returns>
    public Dictionary<string, int> GetReverseItemCodeDict()
    {
        if (reverseItemCodeDict == null)
            InitializeDictionaries();
        return reverseItemCodeDict;
    }
}
