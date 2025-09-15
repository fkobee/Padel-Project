namespace RankingPadelAPI.Domain;

public class RolePermission
{
    public string RoleId { get; set; } = null!;
    public Role Role { get; set; } = null!;

    public string PermissionId { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
