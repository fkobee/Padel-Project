namespace RankingPadelAPI.Domain;

public class User
{
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool Authenticated { get; set; }
    public string Id { get; set; }
    public Role? Role { get; set; }
    public string? Token { get; set; }


    public User()
    {
        Id = Guid.NewGuid().ToString();
    }
}
