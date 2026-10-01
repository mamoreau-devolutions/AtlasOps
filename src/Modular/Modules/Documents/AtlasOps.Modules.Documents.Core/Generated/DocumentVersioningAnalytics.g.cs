namespace AtlasOps.Modules.Documents.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Documents.Contracts;

public sealed record DocumentVersioningAnalytics(int TotalCount, int ActiveCount, int OverdueCount, decimal EstimatedCost, double AverageRisk, IReadOnlyDictionary<DocumentVersioningState, int> StateCounts, IReadOnlyDictionary<string, int> OwnerCounts);
public sealed record RankedDocumentVersioning(DocumentVersioningRecord Record, double Score, IReadOnlyList<string> Reasons);
public sealed record DocumentVersioningHistorySnapshot(Guid RecordId, long Revision, DateTimeOffset CapturedAt, string Reason, DocumentVersioningRecord Record);

public sealed class DocumentVersioningAnalyticsEngine
{
    public DocumentVersioningAnalytics Analyze(IEnumerable<DocumentVersioningRecord> records, DateTimeOffset now)
    {
        DocumentVersioningRecord[] snapshot = records.ToArray();
        Dictionary<DocumentVersioningState, int> states = Enum.GetValues<DocumentVersioningState>().ToDictionary(static state => state, state => snapshot.Count(record => record.State == state));
        Dictionary<string, int> owners = snapshot.GroupBy(static record => record.Owner, StringComparer.OrdinalIgnoreCase).OrderByDescending(static group => group.Count()).ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        return new(snapshot.Length, snapshot.Count(static record => record.State == DocumentVersioningState.Active), snapshot.Count(record => record.DueAt.HasValue && record.DueAt.Value < now && record.State is not DocumentVersioningState.Completed and not DocumentVersioningState.Archived), snapshot.Sum(static record => record.EstimatedCost), snapshot.Length == 0 ? 0d : snapshot.Average(static record => record.RiskScore), states, owners);
    }
}
public sealed class DocumentVersioningRankingEngine
{
    public IReadOnlyList<RankedDocumentVersioning> Rank(IEnumerable<DocumentVersioningRecord> records, DateTimeOffset now, int limit)
    {
        return records.Select(record => RankRecord(record, now)).OrderByDescending(static result => result.Score).ThenBy(static result => result.Record.Name, StringComparer.OrdinalIgnoreCase).Take(Math.Clamp(limit, 1, 500)).ToArray();
    }
    private static RankedDocumentVersioning RankRecord(DocumentVersioningRecord record, DateTimeOffset now)
    {
        double score = record.Priority * 0.45d + record.RiskScore * 40d; List<string> reasons = new();
        if (record.Priority >= 80) { reasons.Add("high-priority"); }
        if (record.RiskScore >= 0.7d) { reasons.Add("elevated-risk"); }
        if (record.DueAt.HasValue && record.DueAt.Value < now) { score += 30d; reasons.Add("overdue"); }
        else if (record.DueAt.HasValue && (record.DueAt.Value - now).TotalDays <= 7d) { score += 15d; reasons.Add("due-soon"); }
        if (record.State == DocumentVersioningState.Paused) { score += 8d; reasons.Add("paused"); }
        return new(record, Math.Round(score, 2), reasons);
    }
}
public sealed class DocumentVersioningHistoryJournal
{
    private readonly ConcurrentDictionary<Guid, List<DocumentVersioningHistorySnapshot>> _history = new();
    public void Capture(DocumentVersioningRecord record, DateTimeOffset capturedAt, string reason)
    {
        List<DocumentVersioningHistorySnapshot> snapshots = _history.GetOrAdd(record.Id, static _ => new());
        lock (snapshots) { snapshots.Add(new(record.Id, record.Revision, capturedAt, reason, record)); }
    }
    public IReadOnlyList<DocumentVersioningHistorySnapshot> Read(Guid recordId)
    {
        if (!_history.TryGetValue(recordId, out List<DocumentVersioningHistorySnapshot>? snapshots)) { return Array.Empty<DocumentVersioningHistorySnapshot>(); }
        lock (snapshots) { return snapshots.OrderByDescending(static snapshot => snapshot.Revision).ToArray(); }
    }
    public DocumentVersioningRecord? Restore(Guid recordId, long revision, DateTimeOffset restoredAt)
    {
        DocumentVersioningHistorySnapshot? snapshot = Read(recordId).FirstOrDefault(item => item.Revision == revision);
        return snapshot?.Record with { UpdatedAt = restoredAt, Revision = snapshot.Record.Revision + 1 };
    }
}