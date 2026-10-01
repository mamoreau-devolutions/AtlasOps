namespace AtlasOps.Modules.Identity.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Identity.Contracts;

public sealed record MembershipRevocationAnalytics(int TotalCount, int ActiveCount, int OverdueCount, decimal EstimatedCost, double AverageRisk, IReadOnlyDictionary<MembershipRevocationState, int> StateCounts, IReadOnlyDictionary<string, int> OwnerCounts);
public sealed record RankedMembershipRevocation(MembershipRevocationRecord Record, double Score, IReadOnlyList<string> Reasons);
public sealed record MembershipRevocationHistorySnapshot(Guid RecordId, long Revision, DateTimeOffset CapturedAt, string Reason, MembershipRevocationRecord Record);

public sealed class MembershipRevocationAnalyticsEngine
{
    public MembershipRevocationAnalytics Analyze(IEnumerable<MembershipRevocationRecord> records, DateTimeOffset now)
    {
        MembershipRevocationRecord[] snapshot = records.ToArray();
        Dictionary<MembershipRevocationState, int> states = Enum.GetValues<MembershipRevocationState>().ToDictionary(static state => state, state => snapshot.Count(record => record.State == state));
        Dictionary<string, int> owners = snapshot.GroupBy(static record => record.Owner, StringComparer.OrdinalIgnoreCase).OrderByDescending(static group => group.Count()).ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        return new(snapshot.Length, snapshot.Count(static record => record.State == MembershipRevocationState.Active), snapshot.Count(record => record.DueAt.HasValue && record.DueAt.Value < now && record.State is not MembershipRevocationState.Completed and not MembershipRevocationState.Archived), snapshot.Sum(static record => record.EstimatedCost), snapshot.Length == 0 ? 0d : snapshot.Average(static record => record.RiskScore), states, owners);
    }
}
public sealed class MembershipRevocationRankingEngine
{
    public IReadOnlyList<RankedMembershipRevocation> Rank(IEnumerable<MembershipRevocationRecord> records, DateTimeOffset now, int limit)
    {
        return records.Select(record => RankRecord(record, now)).OrderByDescending(static result => result.Score).ThenBy(static result => result.Record.Name, StringComparer.OrdinalIgnoreCase).Take(Math.Clamp(limit, 1, 500)).ToArray();
    }
    private static RankedMembershipRevocation RankRecord(MembershipRevocationRecord record, DateTimeOffset now)
    {
        double score = record.Priority * 0.45d + record.RiskScore * 40d; List<string> reasons = new();
        if (record.Priority >= 80) { reasons.Add("high-priority"); }
        if (record.RiskScore >= 0.7d) { reasons.Add("elevated-risk"); }
        if (record.DueAt.HasValue && record.DueAt.Value < now) { score += 30d; reasons.Add("overdue"); }
        else if (record.DueAt.HasValue && (record.DueAt.Value - now).TotalDays <= 7d) { score += 15d; reasons.Add("due-soon"); }
        if (record.State == MembershipRevocationState.Paused) { score += 8d; reasons.Add("paused"); }
        return new(record, Math.Round(score, 2), reasons);
    }
}
public sealed class MembershipRevocationHistoryJournal
{
    private readonly ConcurrentDictionary<Guid, List<MembershipRevocationHistorySnapshot>> _history = new();
    public void Capture(MembershipRevocationRecord record, DateTimeOffset capturedAt, string reason)
    {
        List<MembershipRevocationHistorySnapshot> snapshots = _history.GetOrAdd(record.Id, static _ => new());
        lock (snapshots) { snapshots.Add(new(record.Id, record.Revision, capturedAt, reason, record)); }
    }
    public IReadOnlyList<MembershipRevocationHistorySnapshot> Read(Guid recordId)
    {
        if (!_history.TryGetValue(recordId, out List<MembershipRevocationHistorySnapshot>? snapshots)) { return Array.Empty<MembershipRevocationHistorySnapshot>(); }
        lock (snapshots) { return snapshots.OrderByDescending(static snapshot => snapshot.Revision).ToArray(); }
    }
    public MembershipRevocationRecord? Restore(Guid recordId, long revision, DateTimeOffset restoredAt)
    {
        MembershipRevocationHistorySnapshot? snapshot = Read(recordId).FirstOrDefault(item => item.Revision == revision);
        return snapshot?.Record with { UpdatedAt = restoredAt, Revision = snapshot.Record.Revision + 1 };
    }
}