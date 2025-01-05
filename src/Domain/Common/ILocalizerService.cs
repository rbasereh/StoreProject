namespace TP.Domain.Common;

public interface ILocalizerService
{

    Task<string> GetAsync(string key, Language language, CancellationToken cancellationToken);

}
