namespace AtlasOps.Adapters.Azure;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public sealed record AzureSourceRequest(string Operation, string Resource, string? ContinuationToken, int PageSize, IReadOnlyDictionary<string, string> Parameters);
public sealed record AzureSourceRecord(string Id, string Kind, string Region, decimal Cost, DateTimeOffset ObservedAt, string SourceHash, IReadOnlyDictionary<string, string> Properties);
public sealed record AzureSourcePage(IReadOnlyList<AzureSourceRecord> Items, string? ContinuationToken, bool HasMore);
public sealed record AzureSourceHealth(bool Healthy, string Code, TimeSpan Latency, DateTimeOffset CheckedAt, IReadOnlyDictionary<string, string> Diagnostics);
public interface IAzureSourceTransport { ValueTask<AzureSourcePage> ExecuteAsync(AzureSourceRequest request, CancellationToken cancellationToken); }
public sealed class AzureSourceNormalizer
{
    public AzureSourceRecord Normalize(string id, string kind, string region, decimal cost, DateTimeOffset observedAt, IReadOnlyDictionary<string, string> properties)
    {
        string normalizedId = id.Trim().ToUpperInvariant(); string normalizedKind = string.IsNullOrWhiteSpace(kind) ? "unknown" : kind.Trim().ToLowerInvariant(); string normalizedRegion = string.IsNullOrWhiteSpace(region) ? "global" : region.Trim().ToLowerInvariant();
        string canonical = string.Join("|", normalizedId, normalizedKind, normalizedRegion, cost.ToString(CultureInfo.InvariantCulture), observedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
        string hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
        return new(normalizedId, normalizedKind, normalizedRegion, Math.Max(0m, cost), observedAt.ToUniversalTime(), hash, new Dictionary<string, string>(properties, StringComparer.OrdinalIgnoreCase));
    }
}
public sealed class AzureSourcePolicy
{
    public IReadOnlyList<string> Validate(AzureSourceRequest request)
    {
        List<string> issues = new();
        if (string.IsNullOrWhiteSpace(request.Operation)) { issues.Add("Operation is required."); }
        if (string.IsNullOrWhiteSpace(request.Resource)) { issues.Add("Resource is required."); }
        if (request.PageSize is < 1 or > 1000) { issues.Add("Page size must be between 1 and 1000."); }
        return issues;
    }
}
public sealed class InMemoryAzureSourceTransport : IAzureSourceTransport
{
    private readonly ConcurrentDictionary<string, AzureSourceRecord> _records = new(StringComparer.OrdinalIgnoreCase);
    public void Seed(IEnumerable<AzureSourceRecord> records) { foreach (AzureSourceRecord record in records) { _records[record.Id] = record; } }
    public ValueTask<AzureSourcePage> ExecuteAsync(AzureSourceRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); int offset = int.TryParse(request.ContinuationToken, NumberStyles.None, CultureInfo.InvariantCulture, out int parsed) ? parsed : 0;
        AzureSourceRecord[] filtered = _records.Values.Where(record => record.Kind.Contains(request.Resource, StringComparison.OrdinalIgnoreCase) || record.Region.Contains(request.Resource, StringComparison.OrdinalIgnoreCase) || record.Id.Contains(request.Resource, StringComparison.OrdinalIgnoreCase)).OrderBy(static record => record.Id, StringComparer.OrdinalIgnoreCase).ToArray();
        AzureSourceRecord[] page = filtered.Skip(offset).Take(request.PageSize).ToArray(); int nextOffset = offset + page.Length; string? token = nextOffset < filtered.Length ? nextOffset.ToString(CultureInfo.InvariantCulture) : null;
        return ValueTask.FromResult(new AzureSourcePage(page, token, token is not null));
    }
}
public sealed class AzureSourceAdapter
{
    private readonly IAzureSourceTransport _transport; private readonly AzureSourcePolicy _policy = new();
    public AzureSourceAdapter(IAzureSourceTransport transport) { _transport = transport; }
    public async ValueTask<AzureSourcePage> QueryAsync(AzureSourceRequest request, CancellationToken cancellationToken)
    {
        IReadOnlyList<string> issues = _policy.Validate(request); if (issues.Count > 0) { return new(Array.Empty<AzureSourceRecord>(), null, false); }
        return await _transport.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
    }
    public async ValueTask<AzureSourceHealth> ProbeAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        try { await _transport.ExecuteAsync(new("health", string.Empty, null, 1, new Dictionary<string, string>()), cancellationToken).ConfigureAwait(false); return new(true, "ready", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), now, new Dictionary<string, string> { ["adapter"] = "Azure" }); }
        catch (OperationCanceledException) { return new(false, "cancelled", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), now, new Dictionary<string, string>()); }
    }
}