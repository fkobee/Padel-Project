namespace RankingPadelAPI.Domain;

public class Role
{
    public string Id { get; set; }
    public string Name { get; set; } = null!;
    public List<RolePermission> RolePermissions { get; set; } = null!;
    public bool HasPermission(Permission permission)
    {
        return RolePermissions.Any(rp => rp.Permission.Value == permission.Value);
    }

    public override string ToString()
    {
        return Name;
    }

    public Role()
    {
        Id = Guid.NewGuid().ToString();
    }
}
