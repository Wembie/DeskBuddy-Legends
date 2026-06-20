namespace DeskBuddyLegends.Core.Application.Interfaces;

public interface ISteamRichPresence
{
    void SetCompanion(string companionName, int level, string affinityKey);
    void SetActivity(string activityKey);
    void SetAchievementCount(int total, int unlocked);
    void Clear();
}
