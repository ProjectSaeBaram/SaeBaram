using System;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
   [SerializeField] private ItemDatabase _itemDatabase;
   
   [SerializeField] private SpriteRenderer _itemAppearance;
   
   [Header("Item Infos")]
   [SerializeField] public int ItemId;
   [SerializeField] public int Quality;
   [SerializeField] public int Amount = 1;    
   [SerializeField] public int Durability = 1;
   [SerializeField] public int ReinforceCount = 0;
   [SerializeField] public Define.ItemType itemType; 
   
   [SerializeField] private bool isEatable = false;
   
   [Header("Logs")] [SerializeField] public List<string> Logs = new List<string>();
   
   private void OnEnable()
   {
      _itemAppearance.sprite = _itemDatabase.GetItemImageById(ItemId);
   }

   public void InitInfo(UI_Inven_Item uiInvenItem)
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
   
   /// <summary>
   /// 플레이어가 획득할 때 Call되는 함수
   /// </summary>
   public void Fed()
   {
      Managers.Data.AddItemInInventory(this);
      
      DestroyImmediate(gameObject);
   }
}
