namespace TP.Application.Common.Interfaces;
public interface IUser
{
    string? Id { get; }
    string Code { get; }
    int UserId { get; }
    string UserName { get; }

    Task<string> GetTokenAsync();

    Language Language { get; }

    string UserAgent { get; }

}
