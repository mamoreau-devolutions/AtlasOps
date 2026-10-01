namespace AtlasOps.Modules.Inventory.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Inventory.Contracts;

public sealed record AssetClassificationAnalytics(int TotalCount, int ActiveCount, int OverdueCount, decimal EstimatedCost, double AverageRisk, IReadOnlyDictionary<AssetClassificationState, int> StateCounts, IReadOnlyDictionary<string, int> OwnerCounts);
public sealed record RankedAssetClassification(AssetClassificationRecord Record, double Score, IReadOnlyList<string> Reasons);
public sealed record AssetClassificationHistorySnapshot(Guid RecordId, long Revision, DateTimeOffset CapturedAt, string Reason, AssetClassificationRecord Record);

public sealed class AssetClassificationAnalyticsEngine
{
    public AssetClassificationAnalytics Analyze(IEnumerable<AssetClassificationRecord> records, DateTimeOffset now)
    {
        AssetClassificationRecord[] snapshot = records.ToArray();
        Dictionary<AssetClassificationState, int> states = Enum.GetValues<AssetClassificationState>().ToDictionary(static state => state, state => snapshot.Count(record => record.State == state));
        Dictionary<string, int> owners = snapshot.GroupBy(static record => record.Owner, StringComparer.OrdinalIgnoreCase).OrderByDescending(static group => group.Count()).ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        return new(snapshot.Length, snapshot.Count(static record => record.State == AssetClassificationState.Active), snapshot.Count(record => record.DueAt.HasValue && record.DueAt.Value < now && record.State is not AssetClassificationState.Completed and not AssetClassificationState.Archived), snapshot.Sum(static record => record.EstimatedCost), snapshot.Length == 0 ? 0d : snapshot.Average(static record => record.RiskScore), states, owners);
    }
}
public sealed class AssetClassificationRankingEngine
{
    public IReadOnlyList<RankedAssetClassification> Rank(IEnumerable<AssetClassificationRecord> records, DateTimeOffset now, int limit)
    {
        return records.Select(record => RankRecord(record, now)).OrderByDescending(static result => result.Score).ThenBy(static result => result.Record.Name, StringComparer.OrdinalIgnoreCase).Take(Math.Clamp(limit, 1, 500)).ToArray();
    }
    private static RankedAssetClassification RankRecord(AssetClassificationRecord record, DateTimeOffset now)
    {
        double score = record.Priority * 0.45d + record.RiskScore * 40d; List<string> reasons = new();
        if (record.Priority >= 80) { reasons.Add("high-priority"); }
        if (record.RiskScore >= 0.7d) { reasons.Add("elevated-risk"); }
        if (record.DueAt.HasValue && record.DueAt.Value < now) { score += 30d; reasons.Add("overdue"); }
        else if (record.DueAt.HasValue && (record.DueAt.Value - now).TotalDays <= 7d) { score += 15d; reasons.Add("due-soon"); }
        if (record.State == AssetClassificationState.Paused) { score += 8d; reasons.Add("paused"); }
        return new(record, Math.Round(score, 2), reasons);
    }
}
public sealed class AssetClassificationHistoryJournal
{
    private readonly ConcurrentDictionary<Guid, List<AssetClassificationHistorySnapshot>> _history = new();
    public void Capture(AssetClassificationRecord record, DateTimeOffset capturedAt, string reason)
    {
        List<AssetClassificationHistorySnapshot> snapshots = _history.GetOrAdd(record.Id, static _ => new());
        lock (snapshots) { snapshots.Add(new(record.Id, record.Revision, capturedAt, reason, record)); }
    }
    public IReadOnlyList<AssetClassificationHistorySnapshot> Read(Guid recordId)
    {
        if (!_history.TryGetValue(recordId, out List<AssetClassificationHistorySnapshot>? snapshots)) { return Array.Empty<AssetClassificationHistorySnapshot>(); }
        lock (snapshots) { return snapshots.OrderByDescending(static snapshot => snapshot.Revision).ToArray(); }
    }
    public AssetClassificationRecord? Restore(Guid recordId, long revision, DateTimeOffset restoredAt)
    {
        AssetClassificationHistorySnapshot? snapshot = Read(recordId).FirstOrDefault(item => item.Revision == revision);
        return snapshot?.Record with { UpdatedAt = restoredAt, Revision = snapshot.Record.Revision + 1 };
    }
}