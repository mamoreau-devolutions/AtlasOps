namespace AtlasOps.Modules.Workspaces.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Workspaces.Contracts;

public sealed record PortfolioForecastingAnalytics(int TotalCount, int ActiveCount, int OverdueCount, decimal EstimatedCost, double AverageRisk, IReadOnlyDictionary<PortfolioForecastingState, int> StateCounts, IReadOnlyDictionary<string, int> OwnerCounts);
public sealed record RankedPortfolioForecasting(PortfolioForecastingRecord Record, double Score, IReadOnlyList<string> Reasons);
public sealed record PortfolioForecastingHistorySnapshot(Guid RecordId, long Revision, DateTimeOffset CapturedAt, string Reason, PortfolioForecastingRecord Record);

public sealed class PortfolioForecastingAnalyticsEngine
{
    public PortfolioForecastingAnalytics Analyze(IEnumerable<PortfolioForecastingRecord> records, DateTimeOffset now)
    {
        PortfolioForecastingRecord[] snapshot = records.ToArray();
        Dictionary<PortfolioForecastingState, int> states = Enum.GetValues<PortfolioForecastingState>().ToDictionary(static state => state, state => snapshot.Count(record => record.State == state));
        Dictionary<string, int> owners = snapshot.GroupBy(static record => record.Owner, StringComparer.OrdinalIgnoreCase).OrderByDescending(static group => group.Count()).ToDictionary(static group => group.Key, static group => group.Count(), StringComparer.OrdinalIgnoreCase);
        return new(snapshot.Length, snapshot.Count(static record => record.State == PortfolioForecastingState.Active), snapshot.Count(record => record.DueAt.HasValue && record.DueAt.Value < now && record.State is not PortfolioForecastingState.Completed and not PortfolioForecastingState.Archived), snapshot.Sum(static record => record.EstimatedCost), snapshot.Length == 0 ? 0d : snapshot.Average(static record => record.RiskScore), states, owners);
    }
}
public sealed class PortfolioForecastingRankingEngine
{
    public IReadOnlyList<RankedPortfolioForecasting> Rank(IEnumerable<PortfolioForecastingRecord> records, DateTimeOffset now, int limit)
    {
        return records.Select(record => RankRecord(record, now)).OrderByDescending(static result => result.Score).ThenBy(static result => result.Record.Name, StringComparer.OrdinalIgnoreCase).Take(Math.Clamp(limit, 1, 500)).ToArray();
    }
    private static RankedPortfolioForecasting RankRecord(PortfolioForecastingRecord record, DateTimeOffset now)
    {
        double score = record.Priority * 0.45d + record.RiskScore * 40d; List<string> reasons = new();
        if (record.Priority >= 80) { reasons.Add("high-priority"); }
        if (record.RiskScore >= 0.7d) { reasons.Add("elevated-risk"); }
        if (record.DueAt.HasValue && record.DueAt.Value < now) { score += 30d; reasons.Add("overdue"); }
        else if (record.DueAt.HasValue && (record.DueAt.Value - now).TotalDays <= 7d) { score += 15d; reasons.Add("due-soon"); }
        if (record.State == PortfolioForecastingState.Paused) { score += 8d; reasons.Add("paused"); }
        return new(record, Math.Round(score, 2), reasons);
    }
}
public sealed class PortfolioForecastingHistoryJournal
{
    private readonly ConcurrentDictionary<Guid, List<PortfolioForecastingHistorySnapshot>> _history = new();
    public void Capture(PortfolioForecastingRecord record, DateTimeOffset capturedAt, string reason)
    {
        List<PortfolioForecastingHistorySnapshot> snapshots = _history.GetOrAdd(record.Id, static _ => new());
        lock (snapshots) { snapshots.Add(new(record.Id, record.Revision, capturedAt, reason, record)); }
    }
    public IReadOnlyList<PortfolioForecastingHistorySnapshot> Read(Guid recordId)
    {
        if (!_history.TryGetValue(recordId, out List<PortfolioForecastingHistorySnapshot>? snapshots)) { return Array.Empty<PortfolioForecastingHistorySnapshot>(); }
        lock (snapshots) { return snapshots.OrderByDescending(static snapshot => snapshot.Revision).ToArray(); }
    }
    public PortfolioForecastingRecord? Restore(Guid recordId, long revision, DateTimeOffset restoredAt)
    {
        PortfolioForecastingHistorySnapshot? snapshot = Read(recordId).FirstOrDefault(item => item.Revision == revision);
        return snapshot?.Record with { UpdatedAt = restoredAt, Revision = snapshot.Record.Revision + 1 };
    }
}