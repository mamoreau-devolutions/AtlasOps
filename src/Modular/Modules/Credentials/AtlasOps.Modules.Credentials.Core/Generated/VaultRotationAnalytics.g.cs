namespace AtlasOps.Modules.Credentials.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Credentials.Contracts;

public sealed record VaultRotationAnalytics(int TotalCount, int ActiveCount, int OverdueCount, decimal EstimatedCost, double AverageRisk, IReadOnlyDictionary<VaultRotationState, int> StateCounts, IReadOnlyDictionary<string, int> OwnerCounts);
public sealed record RankedVaultRotation(VaultRotationRecord Record, double Score, IReadOnlyList<string> Reasons);
public sealed record VaultRotationHistorySnapshot(Guid RecordId, long Revision, DateTimeOffset CapturedAt, string Reason, VaultRotationRecord Record);

public sealed class VaultRotationAnalyticsEngine
{
    public VaultRotationAnalytics Analyze(IEnumerable<VaultRotationRecord> records, DateTimeOffset now)
    {
        VaultRotationRecord[] snapshot = records.ToArray();
        Dictionary<VaultRotationState, int> states = Enum.GetValues<VaultRotationState>().ToDictionary(static state => state, state => snapshot.Count(record => record.State == state));
        Dictionary<string, int> owners = snapshot.GroupBy(static record => record.Owner, StringComparer.OrdinalIgnoreCase).OrderByDescending(static group => group.Count()).ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        return new(snapshot.Length, snapshot.Count(static record => record.State == VaultRotationState.Active), snapshot.Count(record => record.DueAt.HasValue && record.DueAt.Value < now && record.State is not VaultRotationState.Completed and not VaultRotationState.Archived), snapshot.Sum(static record => record.EstimatedCost), snapshot.Length == 0 ? 0d : snapshot.Average(static record => record.RiskScore), states, owners);
    }
}
public sealed class VaultRotationRankingEngine
{
    public IReadOnlyList<RankedVaultRotation> Rank(IEnumerable<VaultRotationRecord> records, DateTimeOffset now, int limit)
    {
        return records.Select(record => RankRecord(record, now)).OrderByDescending(static result => result.Score).ThenBy(static result => result.Record.Name, StringComparer.OrdinalIgnoreCase).Take(Math.Clamp(limit, 1, 500)).ToArray();
    }
    private static RankedVaultRotation RankRecord(VaultRotationRecord record, DateTimeOffset now)
    {
        double score = record.Priority * 0.45d + record.RiskScore * 40d; List<string> reasons = new();
        if (record.Priority >= 80) { reasons.Add("high-priority"); }
        if (record.RiskScore >= 0.7d) { reasons.Add("elevated-risk"); }
        if (record.DueAt.HasValue && record.DueAt.Value < now) { score += 30d; reasons.Add("overdue"); }
        else if (record.DueAt.HasValue && (record.DueAt.Value - now).TotalDays <= 7d) { score += 15d; reasons.Add("due-soon"); }
        if (record.State == VaultRotationState.Paused) { score += 8d; reasons.Add("paused"); }
        return new(record, Math.Round(score, 2), reasons);
    }
}
public sealed class VaultRotationHistoryJournal
{
    private readonly ConcurrentDictionary<Guid, List<VaultRotationHistorySnapshot>> _history = new();
    public void Capture(VaultRotationRecord record, DateTimeOffset capturedAt, string reason)
    {
        List<VaultRotationHistorySnapshot> snapshots = _history.GetOrAdd(record.Id, static _ => new());
        lock (snapshots) { snapshots.Add(new(record.Id, record.Revision, capturedAt, reason, record)); }
    }
    public IReadOnlyList<VaultRotationHistorySnapshot> Read(Guid recordId)
    {
        if (!_history.TryGetValue(recordId, out List<VaultRotationHistorySnapshot>? snapshots)) { return Array.Empty<VaultRotationHistorySnapshot>(); }
        lock (snapshots) { return snapshots.OrderByDescending(static snapshot => snapshot.Revision).ToArray(); }
    }
    public VaultRotationRecord? Restore(Guid recordId, long revision, DateTimeOffset restoredAt)
    {
        VaultRotationHistorySnapshot? snapshot = Read(recordId).FirstOrDefault(item => item.Revision == revision);
        return snapshot?.Record with { UpdatedAt = restoredAt, Revision = snapshot.Record.Revision + 1 };
    }
}