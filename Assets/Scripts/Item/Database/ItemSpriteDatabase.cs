using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 아이템 스프라이트 데이터베이스를 관리하는 스크립터블 오브젝트 클래스
/// </summary>
[CreateAssetMenu(fileName = "ItemSpriteDatabase", menuName = "Inventory/ItemSpriteDatabase")]
public class ItemSpriteDatabase : ScriptableObject
{
    /// <summary>
    /// 아이템 데이터를 저장하는 클래스
    /// </summary>
    [System.Serializable]
    public class ItemData
    {
        [Tooltip("아이템 ID")]
        public int id; // 아이템의 고유 ID

        [Tooltip("아이템 이미지")]
        public Sprite itemImage; // 아이템의 스프라이트 이미지
    }

    /// <summary>
    /// 모든 아이템 데이터를 저장하는 리스트
    /// </summary>
    public List<ItemData> items = new List<ItemData>();

    /// <summary>
    /// 주어진 아이템 ID에 해당하는 아이템 이미지를 반환하는 함수
    /// </summary>
    /// <param name="id">아이템 ID</param>
    /// <returns>아이템 이미지 (없으면 null 반환)</returns>
    public Sprite GetItemImageById(int id)
    {
        foreach (var item in items)
        {
            if (item.id == id)
            {
                return item.itemImage;
            }
        }
        return null; // 또는 기본 스프라이트 반환
    }
}