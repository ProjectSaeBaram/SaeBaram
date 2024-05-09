using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

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
/// 데이터를 관리하는 매니저.
/// </summary>
public class DataManager
{
    /// <summary>
    /// 저장 경로 
    /// </summary>
    private string _defaultPath;
    
    // /// <summary>
    // /// 게임 내 정보를 담는 Dictionary. 
    // /// 처음 게임이 시작될 때, Init() 함수를 통해 필요한 정보를 여기에 캐싱하고,
    // /// 이후에 필요할 때는 여기를 참조해 정보를 활용한다.
    // /// </summary>
    //public Dictionary<int, Data.Stat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>();

    /// <summary>
    /// 인벤토리 칸의 갯수. 현재는 테스트용으로 30칸만 존재한다.
    /// </summary>
    public const int NumberOfInventorySlots = 30;

    /// <summary>
    /// 아이템 정보를 캐싱하는 ushort 배열
    /// </summary>
    public ushort[] InventoryTable;

    /// <summary>
    /// 인벤토리 내 전체 아이템들의 로그를 저장하는 객체
    /// </summary>
    public EntireLog EntireLog;
    
    /// <summary>
    /// 아이템 id에 해당하는 아이템의 이름을 저장하는 딕셔너리
    /// </summary>
    Dictionary<int, string> itemCodeDict = new Dictionary<int, string>()
    {
        { 0, "NONE"},
        { 1, "나무" },
        { 2, "돌" },
        { 3, "철" },
        { 4, "나무 막대기" },
        { 5, "돌 막대기" },
        { 6, "철 막대기" },
        { 7, "나무 판" },
        { 8, "돌 판" },
        { 9, "철 판" },
        { 10, "회로" },
        // BOUNDARY
        { 128, "돌 곡괭이" },
        { 129, "돌 도끼" },
        { 130, "돌 망치" },
        { 131, "돌 드라이버" },
        { 132, "돌 단검" },
        { 133, "활" },
        { 134, "나무 몽둥이" },
        { 135, "철 곡괭이" },
        { 136, "철 도끼" },
        { 137, "철 망치" },
        { 138, "철 드라이버" },
        { 139, "철 단검" },
        { 140, "드릴" }
    };
    
    /// <summary>
    /// 아이템 이름에 해당하는 아이템의 코드를 저장하는 딕셔너리
    /// </summary>
    Dictionary<string, int> reverseItemCodeDict = new Dictionary<string, int>()
    {
        { "NONE", 0},
        { "나무", 1 },
        { "돌", 2 },
        { "철", 3 },
        { "나무 막대기", 4 },
        { "돌 막대기", 5 },
        { "철 막대기", 6 },
        { "나무 판", 7 },
        { "돌 판", 8 },
        { "철 판", 9 },
        { "회로", 10 },
        // BOUNDARY
        { "돌 곡괭이", 128 },
        { "돌 도끼", 129 },
        { "돌 망치", 130 },
        { "돌 드라이버", 131 },
        { "돌 단검", 132 },
        { "활", 133 },
        { "나무 몽둥이", 134 },
        { "철 곡괭이", 135 },
        { "철 도끼", 136 },
        { "철 망치", 137 },
        { "철 드라이버", 138 },
        { "철 단검", 139 },
        { "드릴", 140 }
    };

    /// <summary>
    /// Item을 Ingredient와 Tool로 구분하는 기준 Boundary.
    /// Item의 id가 BOUNDARY보다 크거나 같으면 Tool, 보다 작으면 Ingredient.
    /// </summary>
    public const int BOUNDARY = 128;
    
    /// <summary>
    ///  게임이 꺼질 때 Invoke되는 UnityAction
    /// </summary>
    public UnityAction OnClose = null;
    
    /// <summary>
    /// 게임이 시작할 때 데이터를 로드하는 초기화 메서드. 
    /// LoadJson 메서드를 사용해 지정된 경로의 Json 파일로부터 데이터를 로드하고,
    /// ILoader 인터페이스를 구현하는 객체의 MakeDict 함수를 호출해 정보를 캐싱한다.
    /// </summary>
    public void Init()
    {
        // 데이터 저장 기본 경로
        _defaultPath = Application.persistentDataPath + "/";
        
        // 다음과 같은 형태로 사용 가능
        // StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
        
        // 인벤토리 데이터 초기화
        InventoryTable = new ushort[NumberOfInventorySlots];
        for(int i = 0; i < NumberOfInventorySlots; i++)
            InventoryTable[i] = (ushort)0;

        if(!LoadInventoryData())
            // 인벤토리를 테스트 데이터로 채우는 함수
            MakeItemTest();
        
        // EntireLog를 Json으로부터 불러오기 
        EntireLog = LoadEntireLogFromJson(_defaultPath + "/LogsForItems.json");
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
        InventoryTable[3] = (ushort)16417;            // 나무 재료 (중간 퀄리티, 1개)
        InventoryTable[4] = (ushort)16575;            // 돌 재료 (중간 퀄리티, 63개)
        InventoryTable[5] = (ushort)16575;            // 돌 재료 (중간 퀄리티, 63개)

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

        SaveEntireLogIntoJson(_defaultPath + entireLogFileName, EntireLog);
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
        catch(Exception e)
        {
            DebugEx.LogWarning("There are no any save files.");
            DebugEx.Log($"Exception message : {e}");
            return false;
        }

        DebugEx.Log("LoadInventoryData ============== ");
        for (int i = 0; i < NumberOfInventorySlots; i++)
        {
            DebugEx.Log(InventoryTable[i]);
        }
        DebugEx.Log("LoadInventoryData ============== ");
    
        return true;
    }

    /// <summary>
    /// 아이템 정보를 인벤토리 팝업에서 읽어갈 수 있도록 변환하는 함수
    /// </summary>
    public List<ItemData> ItemInfos(bool printConsole = false)
    {
        List<ItemData> itemDatas = new List<ItemData>();
        
        if(printConsole)
            DebugEx.Log("############# ItemDescription ##############");
        
        for(int i = 0; i < NumberOfInventorySlots; i++)
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
            foreach (var logForOneItem in EntireLog.logs)
            {
                if (logForOneItem.index == i)
                {
                    itemData.SetLogFromLogDatas(logForOneItem.data);
                }
            }
            
            itemDatas.Add(itemData);
        }
        if(printConsole)
            DebugEx.Log("############# ItemDescription ##############");

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
            ushort id = (ushort) reverseItemCodeDict[inventory[i].GetName()];

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
        EntireLog = _entireLog;
        DebugEx.Log("Exported from InventoryPopup to DataManager");
    }

    #region About ItemLog

    public EntireLog LoadEntireLogFromJson(string path)
    {
        try{
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                return JsonUtility.FromJson<EntireLog>(json);
            }
        } catch(Exception e)
        {
            DebugEx.LogWarning("There are no saved EntireLog json file.");
            DebugEx.Log($"Exception message : {e}");

            // 테스트 데이터 생성용
            string testData =
                "{\n    \"logs\": [\n        {\n            \"index\": 1,\n            \"data\": [\n                {\n                    \"log\": \"첫번째 챕터 보스를 잡은 무기\"\n                },\n                {\n                    \"log\": \"첫번째로 만든 무기\"\n                }\n            ]\n        },\n        {\n            \"index\": 2,\n            \"data\": [\n                {\n                    \"log\": \"테스트 성공!\"\n                }\n            ]\n        }\n    ]\n}";
            //return new EntireLog();
            return JsonUtility.FromJson<EntireLog>(testData);
        }
    }
    
    public void SaveEntireLogIntoJson(string path, EntireLog entireLog)
    {
        string json = JsonUtility.ToJson(entireLog, true);
        File.WriteAllText(path, json);
        DebugEx.Log($"EntireLog saved into Json at : {path}");
    }

    #endregion
    
}