namespace AtlasOps.Modules.Geography.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Geography.Contracts;

public sealed record BoundarySearchAnalytics(int TotalCount, int ActiveCount, int OverdueCount, decimal EstimatedCost, double AverageRisk, IReadOnlyDictionary<BoundarySearchState, int> StateCounts, IReadOnlyDictionary<string, int> OwnerCounts);
public sealed record RankedBoundarySearch(BoundarySearchRecord Record, double Score, IReadOnlyList<string> Reasons);
public sealed record BoundarySearchHistorySnapshot(Guid RecordId, long Revision, DateTimeOffset CapturedAt, string Reason, BoundarySearchRecord Record);

public sealed class BoundarySearchAnalyticsEngine
{
    public BoundarySearchAnalytics Analyze(IEnumerable<BoundarySearchRecord> records, DateTimeOffset now)
    {
        BoundarySearchRecord[] snapshot = records.ToArray();
        Dictionary<BoundarySearchState, int> states = Enum.GetValues<BoundarySearchState>().ToDictionary(static state => state, state => snapshot.Count(record => record.State == state));
        Dictionary<string, int> owners = snapshot.GroupBy(static record => record.Owner, StringComparer.OrdinalIgnoreCase).OrderByDescending(static group => group.Count()).ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        return new(snapshot.Length, snapshot.Count(static record => record.State == BoundarySearchState.Active), snapshot.Count(record => record.DueAt.HasValue && record.DueAt.Value < now && record.State is not BoundarySearchState.Completed and not BoundarySearchState.Archived), snapshot.Sum(static record => record.EstimatedCost), snapshot.Length == 0 ? 0d : snapshot.Average(static record => record.RiskScore), states, owners);
    }
}
public sealed class BoundarySearchRankingEngine
{
    public IReadOnlyList<RankedBoundarySearch> Rank(IEnumerable<BoundarySearchRecord> records, DateTimeOffset now, int limit)
    {
        return records.Select(record => RankRecord(record, now)).OrderByDescending(static result => result.Score).ThenBy(static result => result.Record.Name, StringComparer.OrdinalIgnoreCase).Take(Math.Clamp(limit, 1, 500)).ToArray();
    }
    private static RankedBoundarySearch RankRecord(BoundarySearchRecord record, DateTimeOffset now)
    {
        double score = record.Priority * 0.45d + record.RiskScore * 40d; List<string> reasons = new();
        if (record.Priority >= 80) { reasons.Add("high-priority"); }
        if (record.RiskScore >= 0.7d) { reasons.Add("elevated-risk"); }
        if (record.DueAt.HasValue && record.DueAt.Value < now) { score += 30d; reasons.Add("overdue"); }
        else if (record.DueAt.HasValue && (record.DueAt.Value - now).TotalDays <= 7d) { score += 15d; reasons.Add("due-soon"); }
        if (record.State == BoundarySearchState.Paused) { score += 8d; reasons.Add("paused"); }
        return new(record, Math.Round(score, 2), reasons);
    }
}
public sealed class BoundarySearchHistoryJournal
{
    private readonly ConcurrentDictionary<Guid, List<BoundarySearchHistorySnapshot>> _history = new();
    public void Capture(BoundarySearchRecord record, DateTimeOffset capturedAt, string reason)
    {
        List<BoundarySearchHistorySnapshot> snapshots = _history.GetOrAdd(record.Id, static _ => new());
        lock (snapshots) { snapshots.Add(new(record.Id, record.Revision, capturedAt, reason, record)); }
    }
    public IReadOnlyList<BoundarySearchHistorySnapshot> Read(Guid recordId)
    {
        if (!_history.TryGetValue(recordId, out List<BoundarySearchHistorySnapshot>? snapshots)) { return Array.Empty<BoundarySearchHistorySnapshot>(); }
        lock (snapshots) { return snapshots.OrderByDescending(static snapshot => snapshot.Revision).ToArray(); }
    }
    public BoundarySearchRecord? Restore(Guid recordId, long revision, DateTimeOffset restoredAt)
    {
        BoundarySearchHistorySnapshot? snapshot = Read(recordId).FirstOrDefault(item => item.Revision == revision);
        return snapshot?.Record with { UpdatedAt = restoredAt, Revision = snapshot.Record.Revision + 1 };
    }
}