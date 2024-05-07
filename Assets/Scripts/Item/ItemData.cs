using System.Collections.Generic;

public interface ITool
{
    public int Durability { get; set; }
    
    public int ReinforceCount { get; set; }
}

public interface Iingredient 
{
    public int Amount { get; set; }
}

/// <summary>
/// 아이템의 정보를 인벤토리 팝업에서 읽기 위한 클래스
/// </summary>
public abstract class ItemData
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public int Quality { get; set; }
    
    public List<string> Logs { get; set; }
    
    protected ItemData(int id, string name, int quality, List<string> logs = null)
    {
        Id = id;
        Name = name;
        Quality = quality;
        Logs = logs ?? new List<string>();
    }

    public string GetName()
    {
        return Name;
    }

    public void SetLogFromLogDatas(List<LogData> log)
    {
        foreach (var i in log)
        {
            Logs.Add(i.log);
        }
    }
    
    public void SetLogFromLogString(List<string> log)
    {
        foreach (var i in log)
        {
            Logs.Add(i);
        }
    }
}

public class Tool : ItemData, ITool
{
    public int Durability { get; set; }
    
    public int ReinforceCount { get; set; }
    
    public Tool(int id, string name, int quality, int durability, int reinforceCount, List<string> logs = null) : base(id, name, quality, logs)
    {
        Durability = durability;
        ReinforceCount = reinforceCount;
    }
}

public class Ingredient : ItemData, Iingredient
{
    public int Amount { get; set; }
    
    public Ingredient(int id, string name, int quality, int amount, List<string> logs = null) : base(id, name, quality, logs)
    {
        Amount = amount;
    }
}

public class DummyItem : ItemData
{
    public DummyItem() : base(0, "NONE", 0)
    {
        // 더미 아이템을 위한 생성자, 필요한 경우 추가적인 속성 설정 가능
    }
}

