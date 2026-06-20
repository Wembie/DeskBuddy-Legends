using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Application.Interfaces;

public interface ISaveIntegrityGuard
{
    void Sign(string savePath, string signaturePath, string accountKey);
    Result Verify(string savePath, string signaturePath, string accountKey);
}
