using DeskBuddyLegends.Core.SharedKernel;

namespace DeskBuddyLegends.Core.Application.Interfaces;

public sealed record CloudQuotaInfo(long TotalBytes, long AvailableBytes);

public interface ISteamCloud
{
    Task<Result<byte[]>> ReadAsync(string filename, CancellationToken ct = default);
    Task<Result> WriteAsync(string filename, byte[] data, CancellationToken ct = default);
    Task<bool> FileExistsAsync(string filename, CancellationToken ct = default);
    Task<CloudQuotaInfo> GetQuotaInfoAsync(CancellationToken ct = default);
    Task<DateTime> GetFileTimestampAsync(string filename, CancellationToken ct = default);
}
