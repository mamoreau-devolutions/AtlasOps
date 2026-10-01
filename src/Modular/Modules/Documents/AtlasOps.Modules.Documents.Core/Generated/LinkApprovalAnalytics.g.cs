namespace AtlasOps.Modules.Documents.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Documents.Contracts;

public sealed record LinkApprovalAnalytics(int TotalCount, int ActiveCount, int OverdueCount, decimal EstimatedCost, double AverageRisk, IReadOnlyDictionary<LinkApprovalState, int> StateCounts, IReadOnlyDictionary<string, int> OwnerCounts);
public sealed record RankedLinkApproval(LinkApprovalRecord Record, double Score, IReadOnlyList<string> Reasons);
public sealed record LinkApprovalHistorySnapshot(Guid RecordId, long Revision, DateTimeOffset CapturedAt, string Reason, LinkApprovalRecord Record);

public sealed class LinkApprovalAnalyticsEngine
{
    public LinkApprovalAnalytics Analyze(IEnumerable<LinkApprovalRecord> records, DateTimeOffset now)
    {
        LinkApprovalRecord[] snapshot = records.ToArray();
        Dictionary<LinkApprovalState, int> states = Enum.GetValues<LinkApprovalState>().ToDictionary(static state => state, state => snapshot.Count(record => record.State == state));
        Dictionary<string, int> owners = snapshot.GroupBy(static record => record.Owner, StringComparer.OrdinalIgnoreCase).OrderByDescending(static group => group.Count()).ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        return new(snapshot.Length, snapshot.Count(static record => record.State == LinkApprovalState.Active), snapshot.Count(record => record.DueAt.HasValue && record.DueAt.Value < now && record.State is not LinkApprovalState.Completed and not LinkApprovalState.Archived), snapshot.Sum(static record => record.EstimatedCost), snapshot.Length == 0 ? 0d : snapshot.Average(static record => record.RiskScore), states, owners);
    }
}
public sealed class LinkApprovalRankingEngine
{
    public IReadOnlyList<RankedLinkApproval> Rank(IEnumerable<LinkApprovalRecord> records, DateTimeOffset now, int limit)
    {
        return records.Select(record => RankRecord(record, now)).OrderByDescending(static result => result.Score).ThenBy(static result => result.Record.Name, StringComparer.OrdinalIgnoreCase).Take(Math.Clamp(limit, 1, 500)).ToArray();
    }
    private static RankedLinkApproval RankRecord(LinkApprovalRecord record, DateTimeOffset now)
    {
        double score = record.Priority * 0.45d + record.RiskScore * 40d; List<string> reasons = new();
        if (record.Priority >= 80) { reasons.Add("high-priority"); }
        if (record.RiskScore >= 0.7d) { reasons.Add("elevated-risk"); }
        if (record.DueAt.HasValue && record.DueAt.Value < now) { score += 30d; reasons.Add("overdue"); }
        else if (record.DueAt.HasValue && (record.DueAt.Value - now).TotalDays <= 7d) { score += 15d; reasons.Add("due-soon"); }
        if (record.State == LinkApprovalState.Paused) { score += 8d; reasons.Add("paused"); }
        return new(record, Math.Round(score, 2), reasons);
    }
}
public sealed class LinkApprovalHistoryJournal
{
    private readonly ConcurrentDictionary<Guid, List<LinkApprovalHistorySnapshot>> _history = new();
    public void Capture(LinkApprovalRecord record, DateTimeOffset capturedAt, string reason)
    {
        List<LinkApprovalHistorySnapshot> snapshots = _history.GetOrAdd(record.Id, static _ => new());
        lock (snapshots) { snapshots.Add(new(record.Id, record.Revision, capturedAt, reason, record)); }
    }
    public IReadOnlyList<LinkApprovalHistorySnapshot> Read(Guid recordId)
    {
        if (!_history.TryGetValue(recordId, out List<LinkApprovalHistorySnapshot>? snapshots)) { return Array.Empty<LinkApprovalHistorySnapshot>(); }
        lock (snapshots) { return snapshots.OrderByDescending(static snapshot => snapshot.Revision).ToArray(); }
    }
    public LinkApprovalRecord? Restore(Guid recordId, long revision, DateTimeOffset restoredAt)
    {
        LinkApprovalHistorySnapshot? snapshot = Read(recordId).FirstOrDefault(item => item.Revision == revision);
        return snapshot?.Record with { UpdatedAt = restoredAt, Revision = snapshot.Record.Revision + 1 };
    }
}