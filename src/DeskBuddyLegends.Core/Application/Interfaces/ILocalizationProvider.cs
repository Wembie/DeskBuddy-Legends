namespace DeskBuddyLegends.Core.Application.Interfaces;

public interface ILocalizationProvider
{
    string Get(string key, string locale);
    bool TryGet(string key, string locale, out string value);
    IReadOnlyList<string> SupportedLocales { get; }
    string FallbackLocale { get; }
}
