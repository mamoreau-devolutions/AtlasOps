namespace AtlasOps.Modules.Compliance.Core;

using System;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Compliance.Contracts;

public sealed record ObligationCatalogDependency(Guid PredecessorId, Guid SuccessorId, string Kind, int Weight, bool Blocking);
public sealed record ObligationCatalogDependencyAnalysis(IReadOnlyList<Guid> Order, IReadOnlyList<IReadOnlyList<Guid>> Cycles, IReadOnlyDictionary<Guid, int> DepthByRecord, IReadOnlySet<Guid> BlockedRecords);

public sealed class ObligationCatalogDependencyGraph
{
    public ObligationCatalogDependencyAnalysis Analyze(
        IEnumerable<ObligationCatalogRecord> records,
        IEnumerable<ObligationCatalogDependency> dependencies)
    {
        ObligationCatalogRecord[] nodes = records.ToArray();
        ObligationCatalogDependency[] edges = dependencies.ToArray();
        Dictionary<Guid, int> indegree = nodes.ToDictionary(static item => item.Id, static _ => 0);
        Dictionary<Guid, List<Guid>> successors = nodes.ToDictionary(static item => item.Id, static _ => new List<Guid>());
        foreach (ObligationCatalogDependency edge in edges)
        {
            if (!indegree.ContainsKey(edge.PredecessorId) || !indegree.ContainsKey(edge.SuccessorId))
            {
                continue;
            }
            successors[edge.PredecessorId].Add(edge.SuccessorId);
            indegree[edge.SuccessorId]++;
        }
        Queue<Guid> ready = new(indegree.Where(static item => item.Value == 0).Select(static item => item.Key));
        List<Guid> order = new();
        Dictionary<Guid, int> depths = nodes.ToDictionary(static item => item.Id, static _ => 0);
        while (ready.Count > 0)
        {
            Guid current = ready.Dequeue();
            order.Add(current);
            foreach (Guid successor in successors[current])
            {
                depths[successor] = Math.Max(depths[successor], depths[current] + 1);
                indegree[successor]--;
                if (indegree[successor] == 0)
                {
                    ready.Enqueue(successor);
                }
            }
        }
        Guid[] cyclic = indegree.Where(static item => item.Value > 0).Select(static item => item.Key).ToArray();
        IReadOnlyList<IReadOnlyList<Guid>> cycles = cyclic.Length == 0
            ? Array.Empty<IReadOnlyList<Guid>>()
            : new IReadOnlyList<Guid>[] { cyclic };
        HashSet<Guid> completed = nodes.Where(static item => item.State is ObligationCatalogState.Completed or ObligationCatalogState.Archived).Select(static item => item.Id).ToHashSet();
        HashSet<Guid> blocked = edges.Where(edge => edge.Blocking && !completed.Contains(edge.PredecessorId)).Select(static edge => edge.SuccessorId).ToHashSet();
        return new ObligationCatalogDependencyAnalysis(order, cycles, depths, blocked);
    }
}

public sealed record ObligationCatalogForecastPoint(DateOnly Date, decimal PlannedCost, decimal ExpectedCost, double ExpectedRisk, int ActiveRecords, int CompletedRecords);
public sealed record ObligationCatalogForecast(IReadOnlyList<ObligationCatalogForecastPoint> Points, decimal TotalPlannedCost, decimal TotalExpectedCost, DateOnly Horizon, double Confidence);

public sealed class ObligationCatalogForecastEngine
{
    public ObligationCatalogForecast Create(
        IEnumerable<ObligationCatalogRecord> records,
        DateOnly start,
        int days,
        double costGrowth,
        double completionRate)
    {
        ObligationCatalogRecord[] snapshot = records.ToArray();
        int horizonDays = Math.Clamp(days, 1, 730);
        List<ObligationCatalogForecastPoint> points = new(horizonDays);
        decimal planned = snapshot.Sum(static item => item.EstimatedCost);
        for (int index = 0; index < horizonDays; index++)
        {
            double elapsed = index + 1d;
            double completedRatio = Math.Clamp(completionRate * elapsed / horizonDays, 0d, 1d);
            int completed = (int)Math.Round(snapshot.Length * completedRatio, MidpointRounding.AwayFromZero);
            int active = Math.Max(0, snapshot.Length - completed);
            decimal expected = planned * (decimal)(1d + costGrowth * elapsed / horizonDays);
            double risk = snapshot.Length == 0
                ? 0d
                : snapshot.Average(static item => item.RiskScore) * (1d - completedRatio * 0.6d);
            points.Add(new ObligationCatalogForecastPoint(
                start.AddDays(index),
                planned,
                decimal.Round(expected, 2),
                Math.Round(risk, 4),
                active,
                completed));
        }
        decimal totalExpected = points.Count == 0 ? 0m : points[^1].ExpectedCost;
        double confidence = Math.Clamp(1d - Math.Abs(costGrowth) - Math.Abs(completionRate - 0.5d) * 0.25d, 0.1d, 0.99d);
        return new ObligationCatalogForecast(points, planned, totalExpected, start.AddDays(horizonDays - 1), Math.Round(confidence, 3));
    }
}

public sealed record ObligationCatalogReconciliationDifference(Guid RecordId, string Field, string? CurrentValue, string? DesiredValue, string Severity, bool AutomaticallyResolvable);
public sealed record ObligationCatalogReconciliationPlan(IReadOnlyList<ObligationCatalogReconciliationDifference> Differences, int AutomaticCount, int ManualCount, double DriftScore);

public sealed class ObligationCatalogReconciliationEngine
{
    public ObligationCatalogReconciliationPlan Compare(
        IEnumerable<ObligationCatalogRecord> current,
        IEnumerable<ObligationCatalogRecord> desired)
    {
        Dictionary<Guid, ObligationCatalogRecord> currentById = current.ToDictionary(static item => item.Id);
        Dictionary<Guid, ObligationCatalogRecord> desiredById = desired.ToDictionary(static item => item.Id);
        List<ObligationCatalogReconciliationDifference> differences = new();
        foreach ((Guid id, ObligationCatalogRecord desiredRecord) in desiredById)
        {
            if (!currentById.TryGetValue(id, out ObligationCatalogRecord? currentRecord))
            {
                differences.Add(new(id, "record", null, desiredRecord.Name, "high", true));
                continue;
            }
            AddDifference(differences, id, "name", currentRecord.Name, desiredRecord.Name, "medium", true);
            AddDifference(differences, id, "owner", currentRecord.Owner, desiredRecord.Owner, "medium", true);
            AddDifference(differences, id, "state", currentRecord.State.ToString(), desiredRecord.State.ToString(), "high", false);
            AddDifference(differences, id, "priority", currentRecord.Priority.ToString(), desiredRecord.Priority.ToString(), "low", true);
            AddDifference(differences, id, "risk", currentRecord.RiskScore.ToString("0.000"), desiredRecord.RiskScore.ToString("0.000"), "high", false);
        }
        foreach ((Guid id, ObligationCatalogRecord currentRecord) in currentById)
        {
            if (!desiredById.ContainsKey(id))
            {
                differences.Add(new(id, "record", currentRecord.Name, null, "medium", false));
            }
        }
        int automatic = differences.Count(static item => item.AutomaticallyResolvable);
        int manual = differences.Count - automatic;
        double score = currentById.Count == 0 ? differences.Count : differences.Count * 100d / currentById.Count;
        return new ObligationCatalogReconciliationPlan(differences, automatic, manual, Math.Round(score, 2));
    }

    private static void AddDifference(
        ICollection<ObligationCatalogReconciliationDifference> differences,
        Guid id,
        string field,
        string current,
        string desired,
        string severity,
        bool automatic)
    {
        if (!string.Equals(current, desired, StringComparison.Ordinal))
        {
            differences.Add(new ObligationCatalogReconciliationDifference(id, field, current, desired, severity, automatic));
        }
    }
}

public sealed record ObligationCatalogScenario(string Id, string Name, double CostMultiplier, double RiskMultiplier, int PriorityShift, TimeSpan ScheduleShift, IReadOnlyDictionary<string, string> Assumptions);
public sealed record ObligationCatalogScenarioOutcome(string ScenarioId, decimal EstimatedCost, double AverageRisk, double CompletionProbability, DateTimeOffset? LatestDueAt, IReadOnlyList<Guid> CriticalRecords);

public sealed class ObligationCatalogScenarioSimulator
{
    public ObligationCatalogScenarioOutcome Simulate(
        IEnumerable<ObligationCatalogRecord> records,
        ObligationCatalogScenario scenario)
    {
        ObligationCatalogRecord[] snapshot = records.ToArray();
        decimal cost = snapshot.Sum(static item => item.EstimatedCost) * (decimal)Math.Max(0d, scenario.CostMultiplier);
        double averageRisk = snapshot.Length == 0
            ? 0d
            : snapshot.Average(static item => item.RiskScore) * Math.Max(0d, scenario.RiskMultiplier);
        double prioritySignal = snapshot.Length == 0
            ? 0d
            : snapshot.Average(item => Math.Clamp(item.Priority + scenario.PriorityShift, 0, 100)) / 100d;
        double completion = Math.Clamp(1d - averageRisk * 0.7d + prioritySignal * 0.2d, 0d, 1d);
        DateTimeOffset? latestDueAt = snapshot
            .Where(static item => item.DueAt.HasValue)
            .Select(item => item.DueAt!.Value.Add(scenario.ScheduleShift))
            .Cast<DateTimeOffset?>()
            .Max();
        Guid[] critical = snapshot
            .Where(item => item.RiskScore * scenario.RiskMultiplier >= 0.75d || item.Priority + scenario.PriorityShift >= 85)
            .OrderByDescending(static item => item.RiskScore)
            .ThenByDescending(static item => item.Priority)
            .Select(static item => item.Id)
            .ToArray();
        return new ObligationCatalogScenarioOutcome(
            scenario.Id,
            decimal.Round(cost, 2),
            Math.Round(averageRisk, 4),
            Math.Round(completion, 4),
            latestDueAt,
            critical);
    }
}