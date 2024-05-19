using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 아이템 스텟 데이터베이스를 관리하는 스크립터블 오브젝트 클래스
/// </summary>
[CreateAssetMenu(fileName = "ItemStatsDatabase", menuName = "Inventory/ItemStatsDatabase")]
public class ItemStatsDatabase : ScriptableObject
{
    /// <summary>
    /// 아이템의 스탯을 저장하는 클래스
    /// </summary>
    [System.Serializable]
    public class ItemStats
    {
        [Tooltip("아이템 ID")] 
        [SerializeField] public int id; // 아이템의 고유 ID

        [Tooltip("벌목 능력치")] 
        [SerializeField] private int lumbering; // 벌목 능력치

        [Tooltip("분해 능력치")] 
        [SerializeField] private int dismantling; // 분해 능력치

        [Tooltip("채광 능력치")] 
        [SerializeField] private int mining; // 채광 능력치

        [Tooltip("공격 능력치")] 
        [SerializeField] private int attack; // 공격 능력치

        [FormerlySerializedAs("mainStat")]
        [Tooltip("주 능력치")] 
        [SerializeField] private Define.ItemMainStatType mainStatType; // 주 능력치 (Enum 타입)

        [Tooltip("전체능력치")]
        private Dictionary<Define.ItemMainStatType, int> Stats = new Dictionary<Define.ItemMainStatType, int>(); // 모든 능력치를 저장하는 딕셔너리

        /// <summary>
        /// 아이템의 능력치 딕셔너리를 초기화하는 함수
        /// </summary>
        public void InitializeStats()
        {
            Stats.Clear();
            Stats[Define.ItemMainStatType.Lumbering] = lumbering;
            Stats[Define.ItemMainStatType.Dismantling] = dismantling;
            Stats[Define.ItemMainStatType.Mining] = mining;
            Stats[Define.ItemMainStatType.Attack] = attack;
        }
        
        /// <summary>
        /// 아이템의 주 능력치 값을 반환하는 함수
        /// </summary>
        /// <returns>주 능력치 값</returns>
        public int GetMainStatValue()
        {
            return Stats[mainStatType];
        }

        /// <summary>
        /// 아이템의 주 능력치 타입을 반환하는 함수
        /// </summary>
        /// <returns>주 능력치 타입</returns>
        public Define.ItemMainStatType GetMainStatType()
        {
            return mainStatType;
        }
    }

    /// <summary>
    /// 모든 아이템의 스탯을 저장하는 리스트
    /// </summary>
    [SerializeField]
    private List<ItemStats> itemStatsList = new List<ItemStats>();

    /// <summary>
    /// 아이템 ID로 아이템 스탯을 검색할 수 있는 딕셔너리
    /// </summary>
    private Dictionary<int, ItemStats> itemStatsDict;

    /// <summary>
    /// 스크립터블 오브젝트가 활성화될 때 호출되는 함수
    /// 딕셔너리를 초기화함
    /// </summary>
    private void OnEnable()
    {
        InitializeDictionary();
    }

    /// <summary>
    /// 아이템 스탯 딕셔너리를 초기화하는 함수
    /// 아이템 리스트를 순회하며 딕셔너리에 추가함
    /// </summary>
    private void InitializeDictionary()
    {
        itemStatsDict = new Dictionary<int, ItemStats>();

        foreach (var itemStats in itemStatsList)
        {
            itemStats.InitializeStats(); // 각 아이템의 능력치 딕셔너리 초기화
            itemStatsDict[itemStats.id] = itemStats; // 딕셔너리에 추가
        }
    }

    /// <summary>
    /// 주어진 ID에 해당하는 아이템 스탯을 반환하는 함수
    /// </summary>
    /// <param name="id">아이템 ID</param>
    /// <returns>아이템 스탯 객체</returns>
    public ItemStats GetItemStats(int id)
    {
        if (itemStatsDict == null)
            InitializeDictionary(); // 딕셔너리가 초기화되지 않았다면 초기화

        itemStatsDict.TryGetValue(id, out var stats); // 딕셔너리에서 아이템 스탯을 검색
        return stats; // 검색된 아이템 스탯 반환 (없다면 null 반환)
    }
}
