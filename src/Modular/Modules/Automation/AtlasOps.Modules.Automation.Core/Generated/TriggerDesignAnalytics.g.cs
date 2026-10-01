namespace AtlasOps.Modules.Automation.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Automation.Contracts;

public sealed record TriggerDesignAnalytics(int TotalCount, int ActiveCount, int OverdueCount, decimal EstimatedCost, double AverageRisk, IReadOnlyDictionary<TriggerDesignState, int> StateCounts, IReadOnlyDictionary<string, int> OwnerCounts);
public sealed record RankedTriggerDesign(TriggerDesignRecord Record, double Score, IReadOnlyList<string> Reasons);
public sealed record TriggerDesignHistorySnapshot(Guid RecordId, long Revision, DateTimeOffset CapturedAt, string Reason, TriggerDesignRecord Record);

public sealed class TriggerDesignAnalyticsEngine
{
    public TriggerDesignAnalytics Analyze(IEnumerable<TriggerDesignRecord> records, DateTimeOffset now)
    {
        TriggerDesignRecord[] snapshot = records.ToArray();
        Dictionary<TriggerDesignState, int> states = Enum.GetValues<TriggerDesignState>().ToDictionary(static state => state, state => snapshot.Count(record => record.State == state));
        Dictionary<string, int> owners = snapshot.GroupBy(static record => record.Owner, StringComparer.OrdinalIgnoreCase).OrderByDescending(static group => group.Count()).ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        return new(snapshot.Length, snapshot.Count(static record => record.State == TriggerDesignState.Active), snapshot.Count(record => record.DueAt.HasValue && record.DueAt.Value < now && record.State is not TriggerDesignState.Completed and not TriggerDesignState.Archived), snapshot.Sum(static record => record.EstimatedCost), snapshot.Length == 0 ? 0d : snapshot.Average(static record => record.RiskScore), states, owners);
    }
}
public sealed class TriggerDesignRankingEngine
{
    public IReadOnlyList<RankedTriggerDesign> Rank(IEnumerable<TriggerDesignRecord> records, DateTimeOffset now, int limit)
    {
        return records.Select(record => RankRecord(record, now)).OrderByDescending(static result => result.Score).ThenBy(static result => result.Record.Name, StringComparer.OrdinalIgnoreCase).Take(Math.Clamp(limit, 1, 500)).ToArray();
    }
    private static RankedTriggerDesign RankRecord(TriggerDesignRecord record, DateTimeOffset now)
    {
        double score = record.Priority * 0.45d + record.RiskScore * 40d; List<string> reasons = new();
        if (record.Priority >= 80) { reasons.Add("high-priority"); }
        if (record.RiskScore >= 0.7d) { reasons.Add("elevated-risk"); }
        if (record.DueAt.HasValue && record.DueAt.Value < now) { score += 30d; reasons.Add("overdue"); }
        else if (record.DueAt.HasValue && (record.DueAt.Value - now).TotalDays <= 7d) { score += 15d; reasons.Add("due-soon"); }
        if (record.State == TriggerDesignState.Paused) { score += 8d; reasons.Add("paused"); }
        return new(record, Math.Round(score, 2), reasons);
    }
}
public sealed class TriggerDesignHistoryJournal
{
    private readonly ConcurrentDictionary<Guid, List<TriggerDesignHistorySnapshot>> _history = new();
    public void Capture(TriggerDesignRecord record, DateTimeOffset capturedAt, string reason)
    {
        List<TriggerDesignHistorySnapshot> snapshots = _history.GetOrAdd(record.Id, static _ => new());
        lock (snapshots) { snapshots.Add(new(record.Id, record.Revision, capturedAt, reason, record)); }
    }
    public IReadOnlyList<TriggerDesignHistorySnapshot> Read(Guid recordId)
    {
        if (!_history.TryGetValue(recordId, out List<TriggerDesignHistorySnapshot>? snapshots)) { return Array.Empty<TriggerDesignHistorySnapshot>(); }
        lock (snapshots) { return snapshots.OrderByDescending(static snapshot => snapshot.Revision).ToArray(); }
    }
    public TriggerDesignRecord? Restore(Guid recordId, long revision, DateTimeOffset restoredAt)
    {
        TriggerDesignHistorySnapshot? snapshot = Read(recordId).FirstOrDefault(item => item.Revision == revision);
        return snapshot?.Record with { UpdatedAt = restoredAt, Revision = snapshot.Record.Revision + 1 };
    }
}