namespace AtlasOps.Modules.Lifecycle.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Lifecycle.Contracts;

public sealed record ProductRiskAnalytics(int TotalCount, int ActiveCount, int OverdueCount, decimal EstimatedCost, double AverageRisk, IReadOnlyDictionary<ProductRiskState, int> StateCounts, IReadOnlyDictionary<string, int> OwnerCounts);
public sealed record RankedProductRisk(ProductRiskRecord Record, double Score, IReadOnlyList<string> Reasons);
public sealed record ProductRiskHistorySnapshot(Guid RecordId, long Revision, DateTimeOffset CapturedAt, string Reason, ProductRiskRecord Record);

public sealed class ProductRiskAnalyticsEngine
{
    public ProductRiskAnalytics Analyze(IEnumerable<ProductRiskRecord> records, DateTimeOffset now)
    {
        ProductRiskRecord[] snapshot = records.ToArray();
        Dictionary<ProductRiskState, int> states = Enum.GetValues<ProductRiskState>().ToDictionary(static state => state, state => snapshot.Count(record => record.State == state));
        Dictionary<string, int> owners = snapshot.GroupBy(static record => record.Owner, StringComparer.OrdinalIgnoreCase).OrderByDescending(static group => group.Count()).ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        return new(snapshot.Length, snapshot.Count(static record => record.State == ProductRiskState.Active), snapshot.Count(record => record.DueAt.HasValue && record.DueAt.Value < now && record.State is not ProductRiskState.Completed and not ProductRiskState.Archived), snapshot.Sum(static record => record.EstimatedCost), snapshot.Length == 0 ? 0d : snapshot.Average(static record => record.RiskScore), states, owners);
    }
}
public sealed class ProductRiskRankingEngine
{
    public IReadOnlyList<RankedProductRisk> Rank(IEnumerable<ProductRiskRecord> records, DateTimeOffset now, int limit)
    {
        return records.Select(record => RankRecord(record, now)).OrderByDescending(static result => result.Score).ThenBy(static result => result.Record.Name, StringComparer.OrdinalIgnoreCase).Take(Math.Clamp(limit, 1, 500)).ToArray();
    }
    private static RankedProductRisk RankRecord(ProductRiskRecord record, DateTimeOffset now)
    {
        double score = record.Priority * 0.45d + record.RiskScore * 40d; List<string> reasons = new();
        if (record.Priority >= 80) { reasons.Add("high-priority"); }
        if (record.RiskScore >= 0.7d) { reasons.Add("elevated-risk"); }
        if (record.DueAt.HasValue && record.DueAt.Value < now) { score += 30d; reasons.Add("overdue"); }
        else if (record.DueAt.HasValue && (record.DueAt.Value - now).TotalDays <= 7d) { score += 15d; reasons.Add("due-soon"); }
        if (record.State == ProductRiskState.Paused) { score += 8d; reasons.Add("paused"); }
        return new(record, Math.Round(score, 2), reasons);
    }
}
public sealed class ProductRiskHistoryJournal
{
    private readonly ConcurrentDictionary<Guid, List<ProductRiskHistorySnapshot>> _history = new();
    public void Capture(ProductRiskRecord record, DateTimeOffset capturedAt, string reason)
    {
        List<ProductRiskHistorySnapshot> snapshots = _history.GetOrAdd(record.Id, static _ => new());
        lock (snapshots) { snapshots.Add(new(record.Id, record.Revision, capturedAt, reason, record)); }
    }
    public IReadOnlyList<ProductRiskHistorySnapshot> Read(Guid recordId)
    {
        if (!_history.TryGetValue(recordId, out List<ProductRiskHistorySnapshot>? snapshots)) { return Array.Empty<ProductRiskHistorySnapshot>(); }
        lock (snapshots) { return snapshots.OrderByDescending(static snapshot => snapshot.Revision).ToArray(); }
    }
    public ProductRiskRecord? Restore(Guid recordId, long revision, DateTimeOffset restoredAt)
    {
        ProductRiskHistorySnapshot? snapshot = Read(recordId).FirstOrDefault(item => item.Revision == revision);
        return snapshot?.Record with { UpdatedAt = restoredAt, Revision = snapshot.Record.Revision + 1 };
    }
}