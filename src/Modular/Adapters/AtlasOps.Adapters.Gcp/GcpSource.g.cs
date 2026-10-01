namespace AtlasOps.Adapters.Gcp;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public sealed record GcpSourceRequest(string Operation, string Resource, string? ContinuationToken, int PageSize, IReadOnlyDictionary<string, string> Parameters);
public sealed record GcpSourceRecord(string Id, string Kind, string Region, decimal Cost, DateTimeOffset ObservedAt, string SourceHash, IReadOnlyDictionary<string, string> Properties);
public sealed record GcpSourcePage(IReadOnlyList<GcpSourceRecord> Items, string? ContinuationToken, bool HasMore);
public sealed record GcpSourceHealth(bool Healthy, string Code, TimeSpan Latency, DateTimeOffset CheckedAt, IReadOnlyDictionary<string, string> Diagnostics);
public interface IGcpSourceTransport { ValueTask<GcpSourcePage> ExecuteAsync(GcpSourceRequest request, CancellationToken cancellationToken); }
public sealed class GcpSourceNormalizer
{
    public GcpSourceRecord Normalize(string id, string kind, string region, decimal cost, DateTimeOffset observedAt, IReadOnlyDictionary<string, string> properties)
    {
        string normalizedId = id.Trim().ToUpperInvariant(); string normalizedKind = string.IsNullOrWhiteSpace(kind) ? "unknown" : kind.Trim().ToLowerInvariant(); string normalizedRegion = string.IsNullOrWhiteSpace(region) ? "global" : region.Trim().ToLowerInvariant();
        string canonical = string.Join("|", normalizedId, normalizedKind, normalizedRegion, cost.ToString(CultureInfo.InvariantCulture), observedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
        string hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
        return new(normalizedId, normalizedKind, normalizedRegion, Math.Max(0m, cost), observedAt.ToUniversalTime(), hash, new Dictionary<string, string>(properties, StringComparer.OrdinalIgnoreCase));
    }
}
public sealed class GcpSourcePolicy
{
    public IReadOnlyList<string> Validate(GcpSourceRequest request)
    {
        List<string> issues = new();
        if (string.IsNullOrWhiteSpace(request.Operation)) { issues.Add("Operation is required."); }
        if (string.IsNullOrWhiteSpace(request.Resource)) { issues.Add("Resource is required."); }
        if (request.PageSize is < 1 or > 1000) { issues.Add("Page size must be between 1 and 1000."); }
        return issues;
    }
}
public sealed class InMemoryGcpSourceTransport : IGcpSourceTransport
{
    private readonly ConcurrentDictionary<string, GcpSourceRecord> _records = new(StringComparer.OrdinalIgnoreCase);
    public void Seed(IEnumerable<GcpSourceRecord> records) { foreach (GcpSourceRecord record in records) { _records[record.Id] = record; } }
    public ValueTask<GcpSourcePage> ExecuteAsync(GcpSourceRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); int offset = int.TryParse(request.ContinuationToken, NumberStyles.None, CultureInfo.InvariantCulture, out int parsed) ? parsed : 0;
        GcpSourceRecord[] filtered = _records.Values.Where(record => record.Kind.Contains(request.Resource, StringComparison.OrdinalIgnoreCase) || record.Region.Contains(request.Resource, StringComparison.OrdinalIgnoreCase) || record.Id.Contains(request.Resource, StringComparison.OrdinalIgnoreCase)).OrderBy(static record => record.Id, StringComparer.OrdinalIgnoreCase).ToArray();
        GcpSourceRecord[] page = filtered.Skip(offset).Take(request.PageSize).ToArray(); int nextOffset = offset + page.Length; string? token = nextOffset < filtered.Length ? nextOffset.ToString(CultureInfo.InvariantCulture) : null;
        return ValueTask.FromResult(new GcpSourcePage(page, token, token is not null));
    }
}
public sealed class GcpSourceAdapter
{
    private readonly IGcpSourceTransport _transport; private readonly GcpSourcePolicy _policy = new();
    public GcpSourceAdapter(IGcpSourceTransport transport) { _transport = transport; }
    public async ValueTask<GcpSourcePage> QueryAsync(GcpSourceRequest request, CancellationToken cancellationToken)
    {
        IReadOnlyList<string> issues = _policy.Validate(request); if (issues.Count > 0) { return new(Array.Empty<GcpSourceRecord>(), null, false); }
        return await _transport.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
    }
    public async ValueTask<GcpSourceHealth> ProbeAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        try { await _transport.ExecuteAsync(new("health", string.Empty, null, 1, new Dictionary<string, string>()), cancellationToken).ConfigureAwait(false); return new(true, "ready", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), now, new Dictionary<string, string> { ["adapter"] = "Gcp" }); }
        catch (OperationCanceledException) { return new(false, "cancelled", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), now, new Dictionary<string, string>()); }
    }
}