from __future__ import annotations

import argparse
import json
from pathlib import Path


def write(path: Path, content: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(content.replace("\r\n", "\n").rstrip("\n").replace("\n", "\r\n"), encoding="utf-8", newline="")


def render(template: str, domain: str, capability: str) -> str:
    return template.replace("{{DOMAIN}}", domain).replace("{{CAPABILITY}}", capability)


GOVERNANCE = """namespace AtlasOps.Modules.{{DOMAIN}}.Core;

using System;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.{{DOMAIN}}.Contracts;

public enum {{CAPABILITY}}ApprovalStage
{
    Requested,
    TeamReview,
    RiskReview,
    OwnerApproval,
    Approved,
    Rejected,
    Expired
}

public sealed record {{CAPABILITY}}ApprovalRule(
    string Id,
    string DisplayName,
    int MinimumPriority,
    double MinimumRisk,
    int RequiredApprovals,
    TimeSpan Expiration,
    IReadOnlySet<string> RequiredRoles);

public sealed record {{CAPABILITY}}ApprovalVote(
    string Actor,
    string Role,
    bool Approved,
    string Rationale,
    DateTimeOffset CastAt);

public sealed record {{CAPABILITY}}ApprovalRequest(
    Guid Id,
    Guid RecordId,
    {{CAPABILITY}}ApprovalStage Stage,
    DateTimeOffset RequestedAt,
    DateTimeOffset ExpiresAt,
    IReadOnlyList<{{CAPABILITY}}ApprovalRule> Rules,
    IReadOnlyList<{{CAPABILITY}}ApprovalVote> Votes);

public sealed record {{CAPABILITY}}ApprovalDecision(
    bool Approved,
    {{CAPABILITY}}ApprovalStage NextStage,
    string Code,
    string Message,
    int RequiredVotes,
    int AcceptedVotes,
    IReadOnlyList<string> MissingRoles);

public sealed class {{CAPABILITY}}ApprovalEngine
{
    public {{CAPABILITY}}ApprovalRequest CreateRequest(
        {{CAPABILITY}}Record record,
        IEnumerable<{{CAPABILITY}}ApprovalRule> availableRules,
        DateTimeOffset now)
    {
        {{CAPABILITY}}ApprovalRule[] selected = availableRules
            .Where(rule => record.Priority >= rule.MinimumPriority || record.RiskScore >= rule.MinimumRisk)
            .OrderByDescending(static rule => rule.RequiredApprovals)
            .ThenBy(static rule => rule.Id, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        TimeSpan expiration = selected.Length == 0
            ? TimeSpan.FromDays(7)
            : selected.Min(static rule => rule.Expiration);
        return new {{CAPABILITY}}ApprovalRequest(
            Guid.NewGuid(),
            record.Id,
            {{CAPABILITY}}ApprovalStage.Requested,
            now,
            now.Add(expiration),
            selected,
            Array.Empty<{{CAPABILITY}}ApprovalVote>());
    }

    public {{CAPABILITY}}ApprovalRequest AddVote(
        {{CAPABILITY}}ApprovalRequest request,
        {{CAPABILITY}}ApprovalVote vote)
    {
        Dictionary<string, {{CAPABILITY}}ApprovalVote> latestVotes = request.Votes
            .ToDictionary(static item => item.Actor, StringComparer.OrdinalIgnoreCase);
        latestVotes[vote.Actor] = vote;
        return request with
        {
            Votes = latestVotes.Values
                .OrderBy(static item => item.CastAt)
                .ThenBy(static item => item.Actor, StringComparer.OrdinalIgnoreCase)
                .ToArray()
        };
    }

    public {{CAPABILITY}}ApprovalDecision Evaluate(
        {{CAPABILITY}}ApprovalRequest request,
        DateTimeOffset now)
    {
        if (now >= request.ExpiresAt)
        {
            return new {{CAPABILITY}}ApprovalDecision(
                false,
                {{CAPABILITY}}ApprovalStage.Expired,
                "expired",
                "The approval request expired.",
                0,
                0,
                Array.Empty<string>());
        }
        if (request.Votes.Any(static vote => !vote.Approved))
        {
            return new {{CAPABILITY}}ApprovalDecision(
                false,
                {{CAPABILITY}}ApprovalStage.Rejected,
                "rejected",
                "At least one reviewer rejected the request.",
                0,
                request.Votes.Count(static vote => vote.Approved),
                Array.Empty<string>());
        }
        int requiredVotes = request.Rules.Sum(static rule => rule.RequiredApprovals);
        HashSet<string> approvedRoles = request.Votes
            .Where(static vote => vote.Approved)
            .Select(static vote => vote.Role)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        string[] missingRoles = request.Rules
            .SelectMany(static rule => rule.RequiredRoles)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(role => !approvedRoles.Contains(role))
            .OrderBy(static role => role, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        int accepted = request.Votes.Count(static vote => vote.Approved);
        bool approved = accepted >= requiredVotes && missingRoles.Length == 0;
        return new {{CAPABILITY}}ApprovalDecision(
            approved,
            approved ? {{CAPABILITY}}ApprovalStage.Approved : {{CAPABILITY}}ApprovalStage.TeamReview,
            approved ? "approved" : "pending",
            approved ? "All approval requirements are satisfied." : "Additional approvals are required.",
            requiredVotes,
            accepted,
            missingRoles);
    }
}

public sealed record {{CAPABILITY}}ServiceLevel(
    string Id,
    int MinimumPriority,
    TimeSpan ResponseTarget,
    TimeSpan CompletionTarget,
    IReadOnlySet<{{CAPABILITY}}State> PausedStates);

public sealed record {{CAPABILITY}}ServiceLevelStatus(
    string PolicyId,
    TimeSpan Age,
    TimeSpan Remaining,
    bool ResponseBreached,
    bool CompletionBreached,
    double CompletionPercent);

public sealed class {{CAPABILITY}}ServiceLevelEvaluator
{
    public {{CAPABILITY}}ServiceLevelStatus Evaluate(
        {{CAPABILITY}}Record record,
        {{CAPABILITY}}ServiceLevel policy,
        DateTimeOffset now,
        DateTimeOffset? respondedAt)
    {
        TimeSpan age = now <= record.CreatedAt ? TimeSpan.Zero : now - record.CreatedAt;
        bool paused = policy.PausedStates.Contains(record.State);
        bool responseBreached = !respondedAt.HasValue && !paused && age > policy.ResponseTarget;
        bool completionBreached = record.State is not {{CAPABILITY}}State.Completed and not {{CAPABILITY}}State.Archived && !paused && age > policy.CompletionTarget;
        TimeSpan remaining = policy.CompletionTarget - age;
        if (remaining < TimeSpan.Zero)
        {
            remaining = TimeSpan.Zero;
        }
        double progress = policy.CompletionTarget == TimeSpan.Zero
            ? 1d
            : Math.Clamp(age.TotalMilliseconds / policy.CompletionTarget.TotalMilliseconds, 0d, 1d);
        return new {{CAPABILITY}}ServiceLevelStatus(
            policy.Id,
            age,
            remaining,
            responseBreached,
            completionBreached,
            Math.Round(progress * 100d, 2));
    }
}

public sealed record {{CAPABILITY}}Exception(
    Guid Id,
    Guid RecordId,
    string RuleId,
    string Justification,
    string RequestedBy,
    DateTimeOffset RequestedAt,
    DateTimeOffset ExpiresAt,
    bool Approved,
    string? ApprovedBy);

public sealed class {{CAPABILITY}}ExceptionRegistry
{
    private readonly Dictionary<Guid, {{CAPABILITY}}Exception> _exceptions = new();
    private readonly object _sync = new();

    public bool Register({{CAPABILITY}}Exception item)
    {
        lock (_sync)
        {
            if (item.ExpiresAt <= item.RequestedAt || string.IsNullOrWhiteSpace(item.RuleId))
            {
                return false;
            }
            return _exceptions.TryAdd(item.Id, item);
        }
    }

    public bool Approve(Guid id, string actor)
    {
        lock (_sync)
        {
            if (!_exceptions.TryGetValue(id, out {{CAPABILITY}}Exception? current))
            {
                return false;
            }
            _exceptions[id] = current with { Approved = true, ApprovedBy = actor };
            return true;
        }
    }

    public IReadOnlyList<{{CAPABILITY}}Exception> FindActive(Guid recordId, DateTimeOffset now)
    {
        lock (_sync)
        {
            return _exceptions.Values
                .Where(item => item.RecordId == recordId && item.Approved && item.ExpiresAt > now)
                .OrderBy(static item => item.ExpiresAt)
                .ToArray();
        }
    }
}
"""

PLANNING = """namespace AtlasOps.Modules.{{DOMAIN}}.Core;

using System;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.{{DOMAIN}}.Contracts;

public sealed record {{CAPABILITY}}Dependency(Guid PredecessorId, Guid SuccessorId, string Kind, int Weight, bool Blocking);
public sealed record {{CAPABILITY}}DependencyAnalysis(IReadOnlyList<Guid> Order, IReadOnlyList<IReadOnlyList<Guid>> Cycles, IReadOnlyDictionary<Guid, int> DepthByRecord, IReadOnlySet<Guid> BlockedRecords);

public sealed class {{CAPABILITY}}DependencyGraph
{
    public {{CAPABILITY}}DependencyAnalysis Analyze(
        IEnumerable<{{CAPABILITY}}Record> records,
        IEnumerable<{{CAPABILITY}}Dependency> dependencies)
    {
        {{CAPABILITY}}Record[] nodes = records.ToArray();
        {{CAPABILITY}}Dependency[] edges = dependencies.ToArray();
        Dictionary<Guid, int> indegree = nodes.ToDictionary(static item => item.Id, static _ => 0);
        Dictionary<Guid, List<Guid>> successors = nodes.ToDictionary(static item => item.Id, static _ => new List<Guid>());
        foreach ({{CAPABILITY}}Dependency edge in edges)
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
        HashSet<Guid> completed = nodes.Where(static item => item.State is {{CAPABILITY}}State.Completed or {{CAPABILITY}}State.Archived).Select(static item => item.Id).ToHashSet();
        HashSet<Guid> blocked = edges.Where(edge => edge.Blocking && !completed.Contains(edge.PredecessorId)).Select(static edge => edge.SuccessorId).ToHashSet();
        return new {{CAPABILITY}}DependencyAnalysis(order, cycles, depths, blocked);
    }
}

public sealed record {{CAPABILITY}}ForecastPoint(DateOnly Date, decimal PlannedCost, decimal ExpectedCost, double ExpectedRisk, int ActiveRecords, int CompletedRecords);
public sealed record {{CAPABILITY}}Forecast(IReadOnlyList<{{CAPABILITY}}ForecastPoint> Points, decimal TotalPlannedCost, decimal TotalExpectedCost, DateOnly Horizon, double Confidence);

public sealed class {{CAPABILITY}}ForecastEngine
{
    public {{CAPABILITY}}Forecast Create(
        IEnumerable<{{CAPABILITY}}Record> records,
        DateOnly start,
        int days,
        double costGrowth,
        double completionRate)
    {
        {{CAPABILITY}}Record[] snapshot = records.ToArray();
        int horizonDays = Math.Clamp(days, 1, 730);
        List<{{CAPABILITY}}ForecastPoint> points = new(horizonDays);
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
            points.Add(new {{CAPABILITY}}ForecastPoint(
                start.AddDays(index),
                planned,
                decimal.Round(expected, 2),
                Math.Round(risk, 4),
                active,
                completed));
        }
        decimal totalExpected = points.Count == 0 ? 0m : points[^1].ExpectedCost;
        double confidence = Math.Clamp(1d - Math.Abs(costGrowth) - Math.Abs(completionRate - 0.5d) * 0.25d, 0.1d, 0.99d);
        return new {{CAPABILITY}}Forecast(points, planned, totalExpected, start.AddDays(horizonDays - 1), Math.Round(confidence, 3));
    }
}

public sealed record {{CAPABILITY}}ReconciliationDifference(Guid RecordId, string Field, string? CurrentValue, string? DesiredValue, string Severity, bool AutomaticallyResolvable);
public sealed record {{CAPABILITY}}ReconciliationPlan(IReadOnlyList<{{CAPABILITY}}ReconciliationDifference> Differences, int AutomaticCount, int ManualCount, double DriftScore);

public sealed class {{CAPABILITY}}ReconciliationEngine
{
    public {{CAPABILITY}}ReconciliationPlan Compare(
        IEnumerable<{{CAPABILITY}}Record> current,
        IEnumerable<{{CAPABILITY}}Record> desired)
    {
        Dictionary<Guid, {{CAPABILITY}}Record> currentById = current.ToDictionary(static item => item.Id);
        Dictionary<Guid, {{CAPABILITY}}Record> desiredById = desired.ToDictionary(static item => item.Id);
        List<{{CAPABILITY}}ReconciliationDifference> differences = new();
        foreach ((Guid id, {{CAPABILITY}}Record desiredRecord) in desiredById)
        {
            if (!currentById.TryGetValue(id, out {{CAPABILITY}}Record? currentRecord))
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
        foreach ((Guid id, {{CAPABILITY}}Record currentRecord) in currentById)
        {
            if (!desiredById.ContainsKey(id))
            {
                differences.Add(new(id, "record", currentRecord.Name, null, "medium", false));
            }
        }
        int automatic = differences.Count(static item => item.AutomaticallyResolvable);
        int manual = differences.Count - automatic;
        double score = currentById.Count == 0 ? differences.Count : differences.Count * 100d / currentById.Count;
        return new {{CAPABILITY}}ReconciliationPlan(differences, automatic, manual, Math.Round(score, 2));
    }

    private static void AddDifference(
        ICollection<{{CAPABILITY}}ReconciliationDifference> differences,
        Guid id,
        string field,
        string current,
        string desired,
        string severity,
        bool automatic)
    {
        if (!string.Equals(current, desired, StringComparison.Ordinal))
        {
            differences.Add(new {{CAPABILITY}}ReconciliationDifference(id, field, current, desired, severity, automatic));
        }
    }
}

public sealed record {{CAPABILITY}}Scenario(string Id, string Name, double CostMultiplier, double RiskMultiplier, int PriorityShift, TimeSpan ScheduleShift, IReadOnlyDictionary<string, string> Assumptions);
public sealed record {{CAPABILITY}}ScenarioOutcome(string ScenarioId, decimal EstimatedCost, double AverageRisk, double CompletionProbability, DateTimeOffset? LatestDueAt, IReadOnlyList<Guid> CriticalRecords);

public sealed class {{CAPABILITY}}ScenarioSimulator
{
    public {{CAPABILITY}}ScenarioOutcome Simulate(
        IEnumerable<{{CAPABILITY}}Record> records,
        {{CAPABILITY}}Scenario scenario)
    {
        {{CAPABILITY}}Record[] snapshot = records.ToArray();
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
        return new {{CAPABILITY}}ScenarioOutcome(
            scenario.Id,
            decimal.Round(cost, 2),
            Math.Round(averageRisk, 4),
            Math.Round(completion, 4),
            latestDueAt,
            critical);
    }
}
"""


def generate(root: Path) -> None:
    manifest = json.loads((root / "config" / "atlasops-modules.json").read_text(encoding="utf-8-sig"))
    count = 0
    for domain in manifest["domains"]:
        domain_id = domain["id"]
        core = root / "src" / "Modular" / "Modules" / domain_id / f"AtlasOps.Modules.{domain_id}.Core" / "Generated"
        for noun in domain["nouns"]:
            for concern in domain["concerns"]:
                capability = noun + concern
                write(core / f"{capability}Governance.g.cs", render(GOVERNANCE, domain_id, capability))
                write(core / f"{capability}Planning.g.cs", render(PLANNING, domain_id, capability))
                count += 2
    print(f"Generated {count} domain governance and planning files.")


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--repository-root", type=Path, default=Path(__file__).resolve().parents[1])
    args = parser.parse_args()
    generate(args.repository_root.resolve())


if __name__ == "__main__":
    main()
