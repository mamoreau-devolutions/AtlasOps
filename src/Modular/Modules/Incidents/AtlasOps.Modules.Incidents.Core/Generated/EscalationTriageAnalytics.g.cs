namespace AtlasOps.Modules.Incidents.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Incidents.Contracts;

public sealed record EscalationTriageAnalytics(int TotalCount, int ActiveCount, int OverdueCount, decimal EstimatedCost, double AverageRisk, IReadOnlyDictionary<EscalationTriageState, int> StateCounts, IReadOnlyDictionary<string, int> OwnerCounts);
public sealed record RankedEscalationTriage(EscalationTriageRecord Record, double Score, IReadOnlyList<string> Reasons);
public sealed record EscalationTriageHistorySnapshot(Guid RecordId, long Revision, DateTimeOffset CapturedAt, string Reason, EscalationTriageRecord Record);

public sealed class EscalationTriageAnalyticsEngine
{
    public EscalationTriageAnalytics Analyze(IEnumerable<EscalationTriageRecord> records, DateTimeOffset now)
    {
        EscalationTriageRecord[] snapshot = records.ToArray();
        Dictionary<EscalationTriageState, int> states = Enum.GetValues<EscalationTriageState>().ToDictionary(static state => state, state => snapshot.Count(record => record.State == state));
        Dictionary<string, int> owners = snapshot.GroupBy(static record => record.Owner, StringComparer.OrdinalIgnoreCase).OrderByDescending(static group => group.Count()).ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        return new(snapshot.Length, snapshot.Count(static record => record.State == EscalationTriageState.Active), snapshot.Count(record => record.DueAt.HasValue && record.DueAt.Value < now && record.State is not EscalationTriageState.Completed and not EscalationTriageState.Archived), snapshot.Sum(static record => record.EstimatedCost), snapshot.Length == 0 ? 0d : snapshot.Average(static record => record.RiskScore), states, owners);
    }
}
public sealed class EscalationTriageRankingEngine
{
    public IReadOnlyList<RankedEscalationTriage> Rank(IEnumerable<EscalationTriageRecord> records, DateTimeOffset now, int limit)
    {
        return records.Select(record => RankRecord(record, now)).OrderByDescending(static result => result.Score).ThenBy(static result => result.Record.Name, StringComparer.OrdinalIgnoreCase).Take(Math.Clamp(limit, 1, 500)).ToArray();
    }
    private static RankedEscalationTriage RankRecord(EscalationTriageRecord record, DateTimeOffset now)
    {
        double score = record.Priority * 0.45d + record.RiskScore * 40d; List<string> reasons = new();
        if (record.Priority >= 80) { reasons.Add("high-priority"); }
        if (record.RiskScore >= 0.7d) { reasons.Add("elevated-risk"); }
        if (record.DueAt.HasValue && record.DueAt.Value < now) { score += 30d; reasons.Add("overdue"); }
        else if (record.DueAt.HasValue && (record.DueAt.Value - now).TotalDays <= 7d) { score += 15d; reasons.Add("due-soon"); }
        if (record.State == EscalationTriageState.Paused) { score += 8d; reasons.Add("paused"); }
        return new(record, Math.Round(score, 2), reasons);
    }
}
public sealed class EscalationTriageHistoryJournal
{
    private readonly ConcurrentDictionary<Guid, List<EscalationTriageHistorySnapshot>> _history = new();
    public void Capture(EscalationTriageRecord record, DateTimeOffset capturedAt, string reason)
    {
        List<EscalationTriageHistorySnapshot> snapshots = _history.GetOrAdd(record.Id, static _ => new());
        lock (snapshots) { snapshots.Add(new(record.Id, record.Revision, capturedAt, reason, record)); }
    }
    public IReadOnlyList<EscalationTriageHistorySnapshot> Read(Guid recordId)
    {
        if (!_history.TryGetValue(recordId, out List<EscalationTriageHistorySnapshot>? snapshots)) { return Array.Empty<EscalationTriageHistorySnapshot>(); }
        lock (snapshots) { return snapshots.OrderByDescending(static snapshot => snapshot.Revision).ToArray(); }
    }
    public EscalationTriageRecord? Restore(Guid recordId, long revision, DateTimeOffset restoredAt)
    {
        EscalationTriageHistorySnapshot? snapshot = Read(recordId).FirstOrDefault(item => item.Revision == revision);
        return snapshot?.Record with { UpdatedAt = restoredAt, Revision = snapshot.Record.Revision + 1 };
    }
}