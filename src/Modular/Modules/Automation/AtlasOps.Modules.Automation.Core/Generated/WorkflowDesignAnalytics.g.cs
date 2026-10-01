namespace AtlasOps.Modules.Automation.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Automation.Contracts;

public sealed record WorkflowDesignAnalytics(int TotalCount, int ActiveCount, int OverdueCount, decimal EstimatedCost, double AverageRisk, IReadOnlyDictionary<WorkflowDesignState, int> StateCounts, IReadOnlyDictionary<string, int> OwnerCounts);
public sealed record RankedWorkflowDesign(WorkflowDesignRecord Record, double Score, IReadOnlyList<string> Reasons);
public sealed record WorkflowDesignHistorySnapshot(Guid RecordId, long Revision, DateTimeOffset CapturedAt, string Reason, WorkflowDesignRecord Record);

public sealed class WorkflowDesignAnalyticsEngine
{
    public WorkflowDesignAnalytics Analyze(IEnumerable<WorkflowDesignRecord> records, DateTimeOffset now)
    {
        WorkflowDesignRecord[] snapshot = records.ToArray();
        Dictionary<WorkflowDesignState, int> states = Enum.GetValues<WorkflowDesignState>().ToDictionary(static state => state, state => snapshot.Count(record => record.State == state));
        Dictionary<string, int> owners = snapshot.GroupBy(static record => record.Owner, StringComparer.OrdinalIgnoreCase).OrderByDescending(static group => group.Count()).ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        return new(snapshot.Length, snapshot.Count(static record => record.State == WorkflowDesignState.Active), snapshot.Count(record => record.DueAt.HasValue && record.DueAt.Value < now && record.State is not WorkflowDesignState.Completed and not WorkflowDesignState.Archived), snapshot.Sum(static record => record.EstimatedCost), snapshot.Length == 0 ? 0d : snapshot.Average(static record => record.RiskScore), states, owners);
    }
}
public sealed class WorkflowDesignRankingEngine
{
    public IReadOnlyList<RankedWorkflowDesign> Rank(IEnumerable<WorkflowDesignRecord> records, DateTimeOffset now, int limit)
    {
        return records.Select(record => RankRecord(record, now)).OrderByDescending(static result => result.Score).ThenBy(static result => result.Record.Name, StringComparer.OrdinalIgnoreCase).Take(Math.Clamp(limit, 1, 500)).ToArray();
    }
    private static RankedWorkflowDesign RankRecord(WorkflowDesignRecord record, DateTimeOffset now)
    {
        double score = record.Priority * 0.45d + record.RiskScore * 40d; List<string> reasons = new();
        if (record.Priority >= 80) { reasons.Add("high-priority"); }
        if (record.RiskScore >= 0.7d) { reasons.Add("elevated-risk"); }
        if (record.DueAt.HasValue && record.DueAt.Value < now) { score += 30d; reasons.Add("overdue"); }
        else if (record.DueAt.HasValue && (record.DueAt.Value - now).TotalDays <= 7d) { score += 15d; reasons.Add("due-soon"); }
        if (record.State == WorkflowDesignState.Paused) { score += 8d; reasons.Add("paused"); }
        return new(record, Math.Round(score, 2), reasons);
    }
}
public sealed class WorkflowDesignHistoryJournal
{
    private readonly ConcurrentDictionary<Guid, List<WorkflowDesignHistorySnapshot>> _history = new();
    public void Capture(WorkflowDesignRecord record, DateTimeOffset capturedAt, string reason)
    {
        List<WorkflowDesignHistorySnapshot> snapshots = _history.GetOrAdd(record.Id, static _ => new());
        lock (snapshots) { snapshots.Add(new(record.Id, record.Revision, capturedAt, reason, record)); }
    }
    public IReadOnlyList<WorkflowDesignHistorySnapshot> Read(Guid recordId)
    {
        if (!_history.TryGetValue(recordId, out List<WorkflowDesignHistorySnapshot>? snapshots)) { return Array.Empty<WorkflowDesignHistorySnapshot>(); }
        lock (snapshots) { return snapshots.OrderByDescending(static snapshot => snapshot.Revision).ToArray(); }
    }
    public WorkflowDesignRecord? Restore(Guid recordId, long revision, DateTimeOffset restoredAt)
    {
        WorkflowDesignHistorySnapshot? snapshot = Read(recordId).FirstOrDefault(item => item.Revision == revision);
        return snapshot?.Record with { UpdatedAt = restoredAt, Revision = snapshot.Record.Revision + 1 };
    }
}