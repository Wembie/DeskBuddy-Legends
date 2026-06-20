namespace DeskBuddyLegends.Core.Application.Interfaces;

public interface ISteamOverlay
{
    event EventHandler<bool>? OverlayActivated;
    void OpenAchievements();
    bool IsOverlayEnabled { get; }
}
