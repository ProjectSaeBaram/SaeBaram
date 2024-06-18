using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DroppedItem : MonoBehaviour
{
   [FormerlySerializedAs("_itemDatabase")] [SerializeField] private ItemSpriteDatabase itemSpriteDatabase;
   
   [SerializeField] private SpriteRenderer _itemAppearance;
   
   [Header("Item Infos")]
   [SerializeField] public int ItemId;
   [SerializeField] public int Quality;
   [SerializeField] public int Amount = 1;    
   [SerializeField] public int Durability = 0;
   [SerializeField] public int ReinforceCount = 0;
   [SerializeField] public Define.ItemType itemType; 
   
   [Header("Logs")] [SerializeField] public List<string> Logs = new List<string>();
   
   private void OnEnable()
   {
      _itemAppearance.sprite = itemSpriteDatabase.GetItemImageById(ItemId);
   }

   public void InitInfoByUI(UI_Inven_Item uiInvenItem)
   {
      ItemId = Managers.Data.reverseItemCodeDict[uiInvenItem.Name];
      Quality = uiInvenItem.Quality;
      Amount = uiInvenItem.Amount;
      Durability = uiInvenItem.Durability;
      ReinforceCount = uiInvenItem.ReinforceCount;

      itemType = Managers.Data.ItemTypeById(ItemId);

      if (uiInvenItem.Logs != null)
      {
         foreach (var log in uiInvenItem.Logs)
            Logs.Add(log);
      }
      
      OnEnable();
   }

   public void InitInfoByValue(int id, int quality)
   {
       ItemId = id;
       Quality = quality;
       
       itemType = Managers.Data.ItemTypeById(ItemId);

       OnEnable();
   }
   
   public void InitInfoByValue(int id, int quality, string log)
   {
       ItemId = id;
       Quality = quality;
       Logs.Add(log); 
       itemType = Managers.Data.ItemTypeById(ItemId);

       OnEnable();
   }
   
   public void InitInfoByValue(int id, int quality, List<string> log)
   {
       ItemId = id;
       Quality = quality;
       Logs = log;
       itemType = Managers.Data.ItemTypeById(ItemId);

       OnEnable();
   }
   
   
   /// <summary>
   /// 플레이어가 획득할 때 Call되는 함수
   /// </summary>
   public void Fed()
   {
      Managers.Data.AddItemInInventory(this);
      
      DestroyImmediate(gameObject);
   }
}
