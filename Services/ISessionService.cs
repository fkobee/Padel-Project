using RankingPadelAPI.Domain.Arguments;
using RankingPadelAPI.Domain;

namespace RankingPadelAPI.Services;

public interface ISessionService
{
    User? GetUserByToken(string? token);

    User GenerateNewToken(ObtainSessionArgs args);

    User GetValidatedUser(ObtainSessionArgs args);

    string GetUserId();
}
