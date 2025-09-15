namespace RankingPadelAPI.Domain;

public class Permission
{
    public string Id { get; set; }
    public string Value { get; set; }

    public Permission(string value)
    {
        Value = value;
        Id = Guid.NewGuid().ToString();
    }

    public override string ToString()
    {
        return Value;
    }
}
