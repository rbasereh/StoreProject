using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace TP.Application.Common;

public class LocalizerService(IMemoryCache cache, IServiceScopeFactory serviceScope) : ILocalizerService
{
    public async Task<string> GetAsync(string key, Language language, CancellationToken cancellationToken)
    {
        var memoryKey = CacheService.GetLocalizeKey(language == Language.Fa ? "fa" : "en", key);

        var value = cache.Get<string>(memoryKey);

        if (string.IsNullOrEmpty(value))
        {
            using var scope = serviceScope.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            var localeStringResource =
                await dbContext.Set<LocaleStringResource>()
                               .AsNoTracking()
                               .FirstOrDefaultAsync(lo =>
                                   lo.ResourceName == key && lo.LanguageId == (int)language, cancellationToken);
            var lvalue = localeStringResource is null ? memoryKey : localeStringResource.ResourceValue;
            value = cache.Set(memoryKey, lvalue);
        }

        return value;
    }
}
