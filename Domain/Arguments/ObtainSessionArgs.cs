namespace RankingPadelAPI.Domain;
public sealed record class ObtainSessionArgs
{
    public readonly string Email;

    public readonly string Password;

    public ObtainSessionArgs(
        string email,
        string password)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new DomainException("Email is required");
        }

        Email = email;

        if (string.IsNullOrEmpty(password))
        {
            throw new DomainException("Password is required");
        }

        Password = password;
    }
}
