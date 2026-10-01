namespace AtlasOps.Modules.Documents.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Documents.Contracts;

public sealed record DocumentAuthoringAnalytics(int TotalCount, int ActiveCount, int OverdueCount, decimal EstimatedCost, double AverageRisk, IReadOnlyDictionary<DocumentAuthoringState, int> StateCounts, IReadOnlyDictionary<string, int> OwnerCounts);
public sealed record RankedDocumentAuthoring(DocumentAuthoringRecord Record, double Score, IReadOnlyList<string> Reasons);
public sealed record DocumentAuthoringHistorySnapshot(Guid RecordId, long Revision, DateTimeOffset CapturedAt, string Reason, DocumentAuthoringRecord Record);

public sealed class DocumentAuthoringAnalyticsEngine
{
    public DocumentAuthoringAnalytics Analyze(IEnumerable<DocumentAuthoringRecord> records, DateTimeOffset now)
    {
        DocumentAuthoringRecord[] snapshot = records.ToArray();
        Dictionary<DocumentAuthoringState, int> states = Enum.GetValues<DocumentAuthoringState>().ToDictionary(static state => state, state => snapshot.Count(record => record.State == state));
        Dictionary<string, int> owners = snapshot.GroupBy(static record => record.Owner, StringComparer.OrdinalIgnoreCase).OrderByDescending(static group => group.Count()).ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        return new(snapshot.Length, snapshot.Count(static record => record.State == DocumentAuthoringState.Active), snapshot.Count(record => record.DueAt.HasValue && record.DueAt.Value < now && record.State is not DocumentAuthoringState.Completed and not DocumentAuthoringState.Archived), snapshot.Sum(static record => record.EstimatedCost), snapshot.Length == 0 ? 0d : snapshot.Average(static record => record.RiskScore), states, owners);
    }
}
public sealed class DocumentAuthoringRankingEngine
{
    public IReadOnlyList<RankedDocumentAuthoring> Rank(IEnumerable<DocumentAuthoringRecord> records, DateTimeOffset now, int limit)
    {
        return records.Select(record => RankRecord(record, now)).OrderByDescending(static result => result.Score).ThenBy(static result => result.Record.Name, StringComparer.OrdinalIgnoreCase).Take(Math.Clamp(limit, 1, 500)).ToArray();
    }
    private static RankedDocumentAuthoring RankRecord(DocumentAuthoringRecord record, DateTimeOffset now)
    {
        double score = record.Priority * 0.45d + record.RiskScore * 40d; List<string> reasons = new();
        if (record.Priority >= 80) { reasons.Add("high-priority"); }
        if (record.RiskScore >= 0.7d) { reasons.Add("elevated-risk"); }
        if (record.DueAt.HasValue && record.DueAt.Value < now) { score += 30d; reasons.Add("overdue"); }
        else if (record.DueAt.HasValue && (record.DueAt.Value - now).TotalDays <= 7d) { score += 15d; reasons.Add("due-soon"); }
        if (record.State == DocumentAuthoringState.Paused) { score += 8d; reasons.Add("paused"); }
        return new(record, Math.Round(score, 2), reasons);
    }
}
public sealed class DocumentAuthoringHistoryJournal
{
    private readonly ConcurrentDictionary<Guid, List<DocumentAuthoringHistorySnapshot>> _history = new();
    public void Capture(DocumentAuthoringRecord record, DateTimeOffset capturedAt, string reason)
    {
        List<DocumentAuthoringHistorySnapshot> snapshots = _history.GetOrAdd(record.Id, static _ => new());
        lock (snapshots) { snapshots.Add(new(record.Id, record.Revision, capturedAt, reason, record)); }
    }
    public IReadOnlyList<DocumentAuthoringHistorySnapshot> Read(Guid recordId)
    {
        if (!_history.TryGetValue(recordId, out List<DocumentAuthoringHistorySnapshot>? snapshots)) { return Array.Empty<DocumentAuthoringHistorySnapshot>(); }
        lock (snapshots) { return snapshots.OrderByDescending(static snapshot => snapshot.Revision).ToArray(); }
    }
    public DocumentAuthoringRecord? Restore(Guid recordId, long revision, DateTimeOffset restoredAt)
    {
        DocumentAuthoringHistorySnapshot? snapshot = Read(recordId).FirstOrDefault(item => item.Revision == revision);
        return snapshot?.Record with { UpdatedAt = restoredAt, Revision = snapshot.Record.Revision + 1 };
    }
}