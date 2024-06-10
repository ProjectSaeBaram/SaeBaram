using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

/// <summary>
/// Json으로 읽어들이는 데이터의 포맷 클래스는 ILoader인터페이스를 구현해야 함.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public interface ILoader<TKey, TValue>
{
    /// <summary>
    /// 데이터를 Dictionary 형태로 변환하는 메서드. 
    /// Dictionary의 키와 값은 인터페이스의 제네릭 타입으로 지정.
    /// </summary>
    /// <returns></returns>
    Dictionary<TKey, TValue> MakeDict();
}

/// <summary>
/// 저장된 모든 아이템에 대한 로그를 저장하는 최상단 객체의 클래스
/// </summary>
[Serializable]
public class EntireLog
{
    public List<LogForOneItem> logs = new();
}

/// <summary>
/// 저장된 하나의 아이템에 대한 로그 정보
/// </summary>
[Serializable]
public class LogForOneItem
{
    public int index;
    public List<LogData> data = new();
}

/// <summary>
/// 저장된 최소 단위의 로그
/// </summary>
[Serializable]
public class LogData
{
    public string log;
    public LogData(string log)
    {
         this.log = log;
    }
}

/// <summary>
/// 세션 데이터 
/// </summary>
[Serializable]
public class SessionData
{
    public int Gold;
    public int Experience;
    public int QuestIndex;
    public Dictionary<int, int> TalkIndex;
    public int Repute;
}
/// <summary>
/// 퀘스트 데이터 
/// </summary>


/// <summary>
/// 데이터를 관리하는 매니저.
/// </summary>
public class DataManager
{
    /// <summary>
    /// 저장 경로 
    /// </summary>
    private string _defaultPath;
    /// <summary>
    /// 세션 데이터 저장 경로
    /// </summary>
    private string _defaultPath_session;

    /// <summary>
    /// 아이템 코드 데이터베이스 스크립터블 오브젝트
    /// </summary>
    private ItemCodeDatabase itemCodeDatabase;

    /// <summary>
    /// 인벤토리 칸의 갯수. 30칸
    /// </summary>
    private const int NumberOfInventorySlots = 30;

    /// <summary>
    /// 퀵 슬롯 칸의 갯수. 8칸
    /// </summary>
    private const int NumberOfQuickSlots = 8;

    /// <summary>
    /// 아이템 정보를 캐싱하는 ushort 배열
    /// </summary>
    private ushort[] InventoryTable;

    /// <summary>
    /// 퀵슬롯 아이템 정보를 캐싱하는 ushort 배열
    /// </summary>
    private ushort[] QuickSlotItems;

    /// <summary>
    /// 인벤토리 내 전체 아이템들의 로그를 저장하는 객체
    /// </summary>
    private EntireLog EntireLog_Inventory;

    /// <summary>
    /// 퀵슬롯 내 아이템들의 로그를 저장하는 객체
    /// </summary>
    private EntireLog EntireLog_Quick;

    /// <summary>
    /// 아이템 id에 해당하는 아이템의 이름을 저장하는 딕셔너리
    /// </summary>
    public Dictionary<int, string> itemCodeDict;

    /// <summary>
    /// 아이템 이름에 해당하는 아이템의 코드를 저장하는 딕셔너리
    /// </summary>
    public Dictionary<string, int> reverseItemCodeDict;

    /// <summary>
    /// Item을 Ingredient와 Tool로 구분하는 기준 Boundary.
    /// Item의 id가 BOUNDARY보다 크거나 같으면 Tool, 보다 작으면 Ingredient.
    /// </summary>
    public readonly int BOUNDARY = 128;

    /// <summary>
    /// 게임이 꺼질 때 + NotebookPopup이 꺼질 때 Invoke되는 UnityAction
    /// 인벤토리 아이템 저장용
    /// </summary>
    public UnityAction OnClose = null;

    /// <summary>
    /// 게임이 꺼질 때 Invoke되는 UnityAction
    /// 퀵슬롯 아이템 저장용
    /// </summary>
    public UnityAction OnCloseQ = null;



    /// <summary>
    /// 게임이 시작할 때 데이터를 로드하는 초기화 메서드. 
    /// LoadJson 메서드를 사용해 지정된 경로의 Json 파일로부터 데이터를 로드하고,
    /// ILoader 인터페이스를 구현하는 객체의 MakeDict 함수를 호출해 정보를 캐싱한다.
    /// </summary>
    public void Init()
    {
        // 데이터 저장 기본 경로
        _defaultPath = Application.persistentDataPath + "/";
        // 세션 데이터 저장 경로
        _defaultPath_session= Application.persistentDataPath + "/SessionData.json";




        // 아이템 코드 딕셔너리 초기화
        itemCodeDatabase = Managers.Resource.Load<ItemCodeDatabase>("Contents/ItemCodeDatabase");
        itemCodeDict = itemCodeDatabase.GetItemCodeDict();
        reverseItemCodeDict = itemCodeDatabase.GetReverseItemCodeDict();

        // 인벤토리 데이터 초기화
        InventoryTable = new ushort[NumberOfInventorySlots];
        for (int i = 0; i < NumberOfInventorySlots; i++)
            InventoryTable[i] = (ushort)0;

        // 퀵슬롯 데이터 초기화
        QuickSlotItems = new ushort[NumberOfQuickSlots];
        for (int i = 0; i < NumberOfQuickSlots; i++)
            QuickSlotItems[i] = (ushort)0;

        if (!LoadInventoryData())
            // 인벤토리를 테스트 데이터로 채우는 함수
            MakeItemTest();

        // EntireLog를 Json으로부터 불러오기 
        EntireLog_Inventory = LoadEntireLogFromJson(_defaultPath + "/LogsForItems.json");

        // EntireLog를 Json으로부터 불러오기 
        EntireLog_Quick = LoadEntireLogFromJson(_defaultPath + "/qLogsForItems.json");
    }

    /// <summary>
    /// Json 파일에서 데이터를 로드하고, 해당 데이터를 처리하기 위한 ILoader 인터페이스를 구현하는 타입의 인스턴스를 생성한다.
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="TLoader"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    TLoader LoadJson<TLoader, TKey, TValue>(string path) where TLoader : ILoader<TKey, TValue>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<TLoader>(textAsset.text);
    }

    /// <summary>
    /// 인벤토리를 테스트 데이터로 채우는 함수
    /// </summary>
    public void MakeItemTest()
    {
        // 테스트용 더미 데이터 
        InventoryTable[0] = (ushort)0;
        InventoryTable[1] = (ushort)57663;            // 종결옵 돌 단검 (퀄리티 최상, 풀내구도, 풀강)
        InventoryTable[2] = (ushort)8544;             // 똥 활 (퀄리티 최하, 내구도 절반, 0강)
        InventoryTable[3] = (ushort)16511;            // 나무 재료 (중간 퀄리티, 63개)
        InventoryTable[4] = (ushort)16575;            // 돌 재료 (중간 퀄리티, 63개)
        InventoryTable[5] = (ushort)16636;            // 철 재료 (중간 퀄리티, 63개)

        // 나머지는 빈값으로 채우기
        for (int i = 6; i < NumberOfInventorySlots; i++)
            InventoryTable[i] = (ushort)0;

        DebugEx.Log("makeItemTest ============== ");
        for (int i = 0; i < NumberOfInventorySlots; i++)
            DebugEx.Log(InventoryTable[i]);
        DebugEx.Log("makeItemTest ============== ");
    }

    /// <summary>
    /// 인벤토리 데이터를 바이너리 파일로 저장하는 함수
    /// </summary>
    public void SaveInventoryData()
    {
        const string dataFileName = "save.bin";
        const string entireLogFileName = "LogsForItems.json";

        FileStream fs = File.Open(_defaultPath + dataFileName, FileMode.Create);

        using (BinaryWriter wr = new BinaryWriter(fs))
        {
            foreach (ushort inventoryItem in InventoryTable)
            {
                wr.Write((ushort)inventoryItem);
            }
            DebugEx.Log("Inventory Save Success");
        }

        SaveEntireLogIntoJson(_defaultPath + entireLogFileName, EntireLog_Inventory);
    }

    /// <summary>
    /// 인벤토리 데이터를 바이너리 파일로 읽어오는 함수
    /// </summary>
    /// <returns> 데이터가 존재한다면 읽어오고 나서 true를 반환, 그렇지 않다면 false를 반환.</returns>
    public bool LoadInventoryData()
    {
        const string dataFileName = "save.bin";
        try
        {
            using (BinaryReader rdr = new BinaryReader(File.Open(_defaultPath + dataFileName, FileMode.Open)))
            {
                InventoryTable = new ushort[NumberOfInventorySlots];
                for (int i = 0; i < NumberOfInventorySlots; i++)
                {
                    if (rdr.BaseStream.Position < rdr.BaseStream.Length)
                    {
                        InventoryTable[i] = (ushort)rdr.ReadUInt16();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            DebugEx.LogWarning("There are no any save files.");
            DebugEx.Log($"Exception message : {e}");
            return false;
        }

        // DebugEx.Log("LoadInventoryData ============== ");
        // for (int i = 0; i < NumberOfInventorySlots; i++)
        // {
        //     DebugEx.Log(InventoryTable[i]);
        // }
        // DebugEx.Log("LoadInventoryData ============== ");

        return true;
    }

    /// <summary>
    /// 퀵슬롯 데이터를 바이너리 데이터로 저장하는 함수
    /// </summary>
    public void SaveQuickSlotData()
    {
        const string dataFileName = "qsave.bin";
        const string entireLogFileName = "qLogsForItems.json";

        FileStream fs = File.Open(_defaultPath + dataFileName, FileMode.Create);

        using (BinaryWriter wr = new BinaryWriter(fs))
        {
            foreach (ushort quickSlotItem in QuickSlotItems)
            {
                wr.Write((ushort)quickSlotItem);
            }
            DebugEx.Log("QuickSlot Save Success");
        }

        SaveEntireLogIntoJson(_defaultPath + entireLogFileName, EntireLog_Quick);
    }

    /// <summary>
    /// 퀵슬롯 데이터를 바이너리 파일로 읽어오는 함수
    /// </summary>
    public void LoadQuickSlotData()
    {
        const string dataFileName = "qsave.bin";
        try
        {
            using (BinaryReader rdr = new BinaryReader(File.Open(_defaultPath + dataFileName, FileMode.Open)))
            {
                QuickSlotItems = new ushort[NumberOfQuickSlots];
                for (int i = 0; i < NumberOfQuickSlots; i++)
                {
                    if (rdr.BaseStream.Position < rdr.BaseStream.Length)
                    {
                        QuickSlotItems[i] = (ushort)rdr.ReadUInt16();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            DebugEx.LogWarning("There are no any qsave files.");
            DebugEx.Log($"Exception message : {e}");
        }
    }


    /// <summary>
    /// 아이템 정보를 인벤토리 팝업에서 읽어갈 수 있도록 변환하는 함수
    /// </summary>
    public List<ItemData> ItemInfos(bool printConsole = false)
    {
        List<ItemData> itemDatas = new List<ItemData>();

        if (printConsole)
            DebugEx.Log("############# ItemDescription ##############");

        for (int i = 0; i < NumberOfInventorySlots; i++)
        {
            // 1. 아이템 퀄리티 구하기 (상위 2bit)
            int quality = (InventoryTable[i] >> 14);

            // 2. 아이템 종류 구하기 (퀄리티 이후 8bit)
            int id = ((int)InventoryTable[i] >> 6) & 255;
            string name = itemCodeDict[id];

            // 3. 아이템 내구도, 강화도 (갯수) 구하기
            // (도구인 경우 4bit가 내구도, 2bit가 강화도)
            // (재료인경우 6bit가 갯수)
            int durability = (InventoryTable[i] & 63) >> 2;
            int numOfReinforce = InventoryTable[i] & 3;

            // amount = durability * 4 + numOfReinforce              // 뒤 6비트로 갯수를 파악 (숫자를 1~64로 받기위해 + 1)

            if (printConsole)
            {
                string data = $"{name} {durability * 4 + numOfReinforce} 개";
                DebugEx.Log(data);
            }

            // 지금 읽어온 아이템이 Tool인지, Material인지 구분해야한다.

            ItemData itemData = null;
            if (name == "NONE")
                itemData = new DummyItem();
            else if (id >= BOUNDARY)
                itemData = new Tool(id, name, quality, durability, numOfReinforce);
            else
                itemData = new Ingredient(id, name, quality, durability * 4 + numOfReinforce);

            // 4. 현재 아이템이 로그를 가지고 있다면, 로그를 추가해준다.
            foreach (var logForOneItem in EntireLog_Inventory.logs)
            {
                if (logForOneItem.index == i)
                {
                    itemData.SetLogFromLogDatas(logForOneItem.data);
                }
            }

            itemDatas.Add(itemData);
        }
        if (printConsole)
            DebugEx.Log("############# ItemDescription ##############");

        return itemDatas;
    }

    /// <summary>
    /// 퀵슬롯 아이템 정보를 QuickSlotGroup에서 읽어갈 수 있도록 변환하는 함수
    /// </summary>
    public List<ItemData> QuickSlotItemInfos()
    {
        List<ItemData> itemDatas = new List<ItemData>();

        for (int i = 0; i < NumberOfQuickSlots; i++)
        {
            // 1. 아이템 퀄리티 구하기 (상위 2bit)
            int quality = (QuickSlotItems[i] >> 14);

            // 2. 아이템 종류 구하기 (퀄리티 이후 8bit)
            int id = ((int)QuickSlotItems[i] >> 6) & 255;
            string name = itemCodeDict[id];

            // 3. 아이템 내구도, 강화도 (갯수) 구하기
            // (도구인 경우 4bit가 내구도, 2bit가 강화도)
            // (재료인경우 6bit가 갯수)
            int durability = (QuickSlotItems[i] & 63) >> 2;
            int numOfReinforce = QuickSlotItems[i] & 3;

            // amount = durability * 4 + numOfReinforce              // 뒤 6비트로 갯수를 파악 (숫자를 1~64로 받기위해 + 1)


            // 지금 읽어온 아이템이 Tool인지, Material인지 구분해야한다.

            ItemData itemData = null;
            if (name == "NONE")
                itemData = new DummyItem();
            else if (id >= BOUNDARY)
                itemData = new Tool(id, name, quality, durability, numOfReinforce);
            else
                itemData = new Ingredient(id, name, quality, durability * 4 + numOfReinforce);

            // 4. 현재 아이템이 로그를 가지고 있다면, 로그를 추가해준다.
            foreach (var logForOneItem in EntireLog_Quick.logs)
            {
                if (logForOneItem.index == i)
                {
                    itemData.SetLogFromLogDatas(logForOneItem.data);
                }
            }
            itemDatas.Add(itemData);
        }

        return itemDatas;
    }

    /// <summary>
    /// 리스트로 입력받은 인벤토리 데이터를 다시 배열로 바꾸는 함수
    /// 이 이후에, SaveInventoryData함수가 호출되어 실제 Disk에 저장된다.
    /// </summary>
    /// <param name="inventory"></param>
    public void TransDataListIntoArray(List<ItemData> inventory)
    {
        ushort[] _inventoryTable = new ushort[NumberOfInventorySlots];
        EntireLog _entireLog = new EntireLog();

        for (int i = 0; i < inventory.Count; i++)
        {
            ushort data = 0;

            ushort quality = (ushort)inventory[i].Quality;
            // item 이름으로 코드 찾기
            ushort id = (ushort)reverseItemCodeDict[inventory[i].GetName()];

            // 아이템 품질 (상위 2bit)
            data |= (ushort)(quality << 14);

            // 아이템 ID (다음 8bit)
            data |= (ushort)(id << 6);

            if (inventory[i] is Tool)
            {
                Tool tool = (Tool)inventory[i];
                // 도구의 경우, 내구도 (4bit)와 강화도 (2bit)
                data |= (ushort)((tool.Durability & 0xF) << 2); // 4bit
                data |= (ushort)(tool.ReinforceCount & 0x3); // 마지막 2bit
            }
            else if (inventory[i] is Ingredient)
            {
                Ingredient ingredient = (Ingredient)inventory[i];
                // 재료의 경우, 갯수 (6bit)
                data |= (ushort)(ingredient.Amount & 0x3F); // 마지막 6bit
            }
            // 변환된 데이터 저장
            _inventoryTable[i] = data;

            // 로그 저장 (있는 경우에만)
            if (inventory[i].Logs.Count > 0)
            {
                LogForOneItem logForOneItem = new LogForOneItem();
                logForOneItem.index = i;
                foreach (var logString in inventory[i].Logs)
                {
                    logForOneItem.data.Add(new LogData(logString));
                }
                _entireLog.logs.Add(logForOneItem);
            }
        }
        InventoryTable = _inventoryTable;
        EntireLog_Inventory = _entireLog;
        DebugEx.Log("Exported from InventoryPopup to DataManager");
    }

    /// <summary>
    /// 리스트로 입력받은 퀵슬롯 데이터를 다시 배열로 바꾸는 함수
    /// 이 이후에, SaveInventoryData함수가 호출되어 실제 Disk에 저장된다.
    /// </summary>
    /// <param name="quickSlots"></param>
    public void TransDataListIntoArrayForQuickSlots(List<ItemData> quickSlots)
    {
        ushort[] _quickSlotTable = new ushort[NumberOfQuickSlots];
        EntireLog _entireLog = new EntireLog();

        for (int i = 0; i < quickSlots.Count; i++)
        {
            ushort data = 0;

            ushort quality = (ushort)quickSlots[i].Quality;
            // item 이름으로 코드 찾기
            ushort id = (ushort)reverseItemCodeDict[quickSlots[i].GetName()];

            // 아이템 품질 (상위 2bit)
            data |= (ushort)(quality << 14);

            // 아이템 ID (다음 8bit)
            data |= (ushort)(id << 6);

            if (quickSlots[i] is Tool)
            {
                Tool tool = (Tool)quickSlots[i];
                // 도구의 경우, 내구도 (4bit)와 강화도 (2bit)
                data |= (ushort)((tool.Durability & 0xF) << 2); // 4bit
                data |= (ushort)(tool.ReinforceCount & 0x3); // 마지막 2bit
            }
            else if (quickSlots[i] is Ingredient)
            {
                Ingredient ingredient = (Ingredient)quickSlots[i];
                // 재료의 경우, 갯수 (6bit)
                data |= (ushort)(ingredient.Amount & 0x3F); // 마지막 6bit
            }
            // 변환된 데이터 저장
            _quickSlotTable[i] = data;

            // 로그 저장 (있는 경우에만)
            if (quickSlots[i].Logs.Count > 0)
            {
                LogForOneItem logForOneItem = new LogForOneItem();
                logForOneItem.index = i;
                foreach (var logString in quickSlots[i].Logs)
                {
                    logForOneItem.data.Add(new LogData(logString));
                }
                _entireLog.logs.Add(logForOneItem);
            }
        }
        QuickSlotItems = _quickSlotTable;
        EntireLog_Quick = _entireLog;
        DebugEx.Log("Exported from QuickSlots to DataManager");
    }

    /// <summary>
    /// 인벤토리에 아이템을 추가하는 함수
    /// </summary>
    public void AddItemInInventory(DroppedItem droppedItem)
    {
        // 인벤토리 내 넣을 수 있는 공간 찾기

        int availableSlotIndex = 0;
        for (; availableSlotIndex < NumberOfInventorySlots; availableSlotIndex++)
            if (InventoryTable[availableSlotIndex] == 0) break;

        #region 인벤토리에 새로만들어서 넣기
        ushort newItem = 0;
        // TODO : id, itemType, quality, amount, durability, reinforceCount, logs

        ushort id = (ushort)droppedItem.ItemId;
        ushort quality = (ushort)droppedItem.Quality;

        // 아이템 품질 (상위 2bit)
        newItem |= (ushort)(quality << 14);

        // 아이템 ID (다음 8bit)
        newItem |= (ushort)(id << 6);

        if (droppedItem.itemType == Define.ItemType.Tool)
        {
            // 도구의 경우, 내구도 (4bit)와 강화도 (2bit)
            newItem |= (ushort)((droppedItem.Durability & 0xF) << 2); // 4bit
            newItem |= (ushort)(droppedItem.ReinforceCount & 0x3); // 마지막 2bit
        }
        else if (droppedItem.itemType == Define.ItemType.Ingredient)
        {
            // 재료의 경우, 갯수 (6bit)
            newItem |= (ushort)(droppedItem.Amount & 0x3F); // 마지막 6bit
        }

        // 인벤토리 내 비어있는 칸에 아이템 저장 
        InventoryTable[availableSlotIndex] = newItem;

        // 로그 저장 (있는 경우에만)
        if (droppedItem.Logs.Count > 0)
        {
            LogForOneItem logForOneItem = new LogForOneItem();
            logForOneItem.index = availableSlotIndex;
            foreach (var logString in droppedItem.Logs)
            {
                logForOneItem.data.Add(new LogData(logString));
            }
            EntireLog_Inventory.logs.Add(logForOneItem);
        }
        #endregion

        SaveInventoryData();
    }

    /// <summary>
    /// 인벤토리에서 아이템을 꺼내어 DroppedItem으로 만드는 함수
    /// </summary>
    public void RemoveItemFromInventory(UI_Inven_Item item)
    {
        // TODO : 아이템 제거
        if (item.parentAfterDrag.GetComponent<UI_Inven_Slot>() is not UI_Inven_CraftingSlot)
        {
            int fromIndex = item.parentAfterDrag.GetComponent<UI_Inven_Slot>().SlotIndex;
            InventoryTable[fromIndex] = 0;
        }

        DroppedItem droppedItem = Managers.Game.Spawn(Define.WorldObject.DroppedItem, "Item/DroppedItem").GetComponent<DroppedItem>();
        droppedItem.transform.position = Managers.Game.GetPlayer().transform.position + Vector3.up * 30;

        droppedItem.InitInfoByUI(item);
        Object.DestroyImmediate(item.gameObject);
    }

    #region About ItemLog

    public EntireLog LoadEntireLogFromJson(string path)
    {
        try
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                return JsonUtility.FromJson<EntireLog>(json);
            }
        }
        catch (Exception e)
        {
            if (path == _defaultPath + "/LogsForItems.json")
            {
                DebugEx.LogWarning("There are no saved EntireLog_Inventory json file.");
                DebugEx.Log($"Exception message : {e}");

                // 테스트 데이터 생성용
                string testData =
                    "{\n    \"logs\": [\n        {\n            \"index\": 1,\n            \"data\": [\n                {\n                    \"log\": \"첫번째 챕터 보스를 잡은 무기\"\n                },\n                {\n                    \"log\": \"첫번째로 만든 무기\"\n                }\n            ]\n        },\n        {\n            \"index\": 2,\n            \"data\": [\n                {\n                    \"log\": \"테스트 성공!\"\n                }\n            ]\n        }\n    ]\n}";
                //return new EntireLog_Inventory();
                return JsonUtility.FromJson<EntireLog>(testData);
            }
            else
                return new EntireLog();
        }
    }

    public void SaveEntireLogIntoJson(string path, EntireLog entireLog)
    {
        string json = JsonUtility.ToJson(entireLog, true);
        File.WriteAllText(path, json);
        DebugEx.Log($"EntireLog_Inventory saved into Json at : {path}");
    }

    #endregion

    /// <summary>
    /// 아이템 id로 아이템 타입을 찾는 함수
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Define.ItemType ItemTypeById(int id)
    {
        return (id >= BOUNDARY) ? Define.ItemType.Tool : Define.ItemType.Ingredient;
    }


    public void InitializeSession(SessionManager sessionManager, DialogueManager dialogueManager, ReputeManager reputeManager)
    {
        SessionData sessionData = LoadSessionData();

        if (sessionData != null)
        {
            sessionManager.Gold = sessionData.Gold;
            sessionManager.Experience = sessionData.Experience;
            sessionManager.QuestIndex = sessionData.QuestIndex;
            sessionManager.Repute = sessionData.Repute;
            sessionManager.TalkIndex = sessionData.TalkIndex;

            dialogueManager.LoadTalkIndex(sessionData.TalkIndex);
            reputeManager.SetRepute(sessionData.Repute);
        }
        else
        {
            sessionManager.Gold = 0;
            sessionManager.Experience = 0;
            sessionManager.QuestIndex = 0;
            sessionManager.Repute = 50;

            sessionManager.TalkIndex.Clear();
            for (int i = 0; i <= 10; i++)
            {
                sessionManager.TalkIndex[i] = 0;
            }

            reputeManager.Init();
        }
    }

    public void SaveSession(SessionManager sessionManager, DialogueManager dialogueManager, ReputeManager reputeManager)
    {
        SessionData sessionData = new SessionData
        {
            Gold = sessionManager.Gold,
            Experience = sessionManager.Experience,
            QuestIndex = sessionManager.QuestIndex,
            TalkIndex = new Dictionary<int, int>(sessionManager.TalkIndex),
            Repute = sessionManager.Repute
        };

        string jsonData = JsonUtility.ToJson(sessionData, true);
        File.WriteAllText(_defaultPath_session, jsonData);
        Debug.Log("Session data saved successfully.");
    }

    private SessionData LoadSessionData()
    {
        if (File.Exists(_defaultPath_session))
        {
            string jsonData = File.ReadAllText(_defaultPath_session);
            return JsonUtility.FromJson<SessionData>(jsonData);
        }
        else
        {
            Debug.LogWarning("Session data file not found.");
            return null;
        }
    }
}
