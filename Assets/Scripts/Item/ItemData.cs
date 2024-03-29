/// <summary>
/// 아이템의 정보를 인벤토리 팝업에서 읽기 위한 클래스
/// </summary>
public abstract class ItemData
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public int Quality { get; set; }
    
    protected ItemData(int id, string name, int quality)
    {
        Id = id;
        Name = name;
        Quality = quality;
    }
}

public class Tool : ItemData, ITool
{
    public int Durability { get; set; }
    
    public int ReinforceCount { get; set; }
    
    public Tool(int id, string name, int quality, int durability, int reinforceCount) : base(id, name, quality)
    {
        Durability = durability;
        ReinforceCount = reinforceCount;
    }
}

public class Ingredient : ItemData , IStackable
{
    public int Amount { get; set; }
    
    public Ingredient(int id, string name, int quality, int amount) : base(id, name, quality)
    {
        Amount = amount;
    }
}