namespace AtlasOps.Modules.Deployments.Core;

using System;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Deployments.Contracts;

public enum EnvironmentMonitoringApprovalStage
{
    Requested,
    TeamReview,
    RiskReview,
    OwnerApproval,
    Approved,
    Rejected,
    Expired
}

public sealed record EnvironmentMonitoringApprovalRule(
    string Id,
    string DisplayName,
    int MinimumPriority,
    double MinimumRisk,
    int RequiredApprovals,
    TimeSpan Expiration,
    IReadOnlySet<string> RequiredRoles);

public sealed record EnvironmentMonitoringApprovalVote(
    string Actor,
    string Role,
    bool Approved,
    string Rationale,
    DateTimeOffset CastAt);

public sealed record EnvironmentMonitoringApprovalRequest(
    Guid Id,
    Guid RecordId,
    EnvironmentMonitoringApprovalStage Stage,
    DateTimeOffset RequestedAt,
    DateTimeOffset ExpiresAt,
    IReadOnlyList<EnvironmentMonitoringApprovalRule> Rules,
    IReadOnlyList<EnvironmentMonitoringApprovalVote> Votes);

public sealed record EnvironmentMonitoringApprovalDecision(
    bool Approved,
    EnvironmentMonitoringApprovalStage NextStage,
    string Code,
    string Message,
    int RequiredVotes,
    int AcceptedVotes,
    IReadOnlyList<string> MissingRoles);

public sealed class EnvironmentMonitoringApprovalEngine
{
    public EnvironmentMonitoringApprovalRequest CreateRequest(
        EnvironmentMonitoringRecord record,
        IEnumerable<EnvironmentMonitoringApprovalRule> availableRules,
        DateTimeOffset now)
    {
        EnvironmentMonitoringApprovalRule[] selected = availableRules
            .Where(rule => record.Priority >= rule.MinimumPriority || record.RiskScore >= rule.MinimumRisk)
            .OrderByDescending(static rule => rule.RequiredApprovals)
            .ThenBy(static rule => rule.Id, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        TimeSpan expiration = selected.Length == 0
            ? TimeSpan.FromDays(7)
            : selected.Min(static rule => rule.Expiration);
        return new EnvironmentMonitoringApprovalRequest(
            Guid.NewGuid(),
            record.Id,
            EnvironmentMonitoringApprovalStage.Requested,
            now,
            now.Add(expiration),
            selected,
            Array.Empty<EnvironmentMonitoringApprovalVote>());
    }

    public EnvironmentMonitoringApprovalRequest AddVote(
        EnvironmentMonitoringApprovalRequest request,
        EnvironmentMonitoringApprovalVote vote)
    {
        Dictionary<string, EnvironmentMonitoringApprovalVote> latestVotes = request.Votes
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

    public EnvironmentMonitoringApprovalDecision Evaluate(
        EnvironmentMonitoringApprovalRequest request,
        DateTimeOffset now)
    {
        if (now >= request.ExpiresAt)
        {
            return new EnvironmentMonitoringApprovalDecision(
                false,
                EnvironmentMonitoringApprovalStage.Expired,
                "expired",
                "The approval request expired.",
                0,
                0,
                Array.Empty<string>());
        }
        if (request.Votes.Any(static vote => !vote.Approved))
        {
            return new EnvironmentMonitoringApprovalDecision(
                false,
                EnvironmentMonitoringApprovalStage.Rejected,
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
        return new EnvironmentMonitoringApprovalDecision(
            approved,
            approved ? EnvironmentMonitoringApprovalStage.Approved : EnvironmentMonitoringApprovalStage.TeamReview,
            approved ? "approved" : "pending",
            approved ? "All approval requirements are satisfied." : "Additional approvals are required.",
            requiredVotes,
            accepted,
            missingRoles);
    }
}

public sealed record EnvironmentMonitoringServiceLevel(
    string Id,
    int MinimumPriority,
    TimeSpan ResponseTarget,
    TimeSpan CompletionTarget,
    IReadOnlySet<EnvironmentMonitoringState> PausedStates);

public sealed record EnvironmentMonitoringServiceLevelStatus(
    string PolicyId,
    TimeSpan Age,
    TimeSpan Remaining,
    bool ResponseBreached,
    bool CompletionBreached,
    double CompletionPercent);

public sealed class EnvironmentMonitoringServiceLevelEvaluator
{
    public EnvironmentMonitoringServiceLevelStatus Evaluate(
        EnvironmentMonitoringRecord record,
        EnvironmentMonitoringServiceLevel policy,
        DateTimeOffset now,
        DateTimeOffset? respondedAt)
    {
        TimeSpan age = now <= record.CreatedAt ? TimeSpan.Zero : now - record.CreatedAt;
        bool paused = policy.PausedStates.Contains(record.State);
        bool responseBreached = !respondedAt.HasValue && !paused && age > policy.ResponseTarget;
        bool completionBreached = record.State is not EnvironmentMonitoringState.Completed and not EnvironmentMonitoringState.Archived && !paused && age > policy.CompletionTarget;
        TimeSpan remaining = policy.CompletionTarget - age;
        if (remaining < TimeSpan.Zero)
        {
            remaining = TimeSpan.Zero;
        }
        double progress = policy.CompletionTarget == TimeSpan.Zero
            ? 1d
            : Math.Clamp(age.TotalMilliseconds / policy.CompletionTarget.TotalMilliseconds, 0d, 1d);
        return new EnvironmentMonitoringServiceLevelStatus(
            policy.Id,
            age,
            remaining,
            responseBreached,
            completionBreached,
            Math.Round(progress * 100d, 2));
    }
}

public sealed record EnvironmentMonitoringException(
    Guid Id,
    Guid RecordId,
    string RuleId,
    string Justification,
    string RequestedBy,
    DateTimeOffset RequestedAt,
    DateTimeOffset ExpiresAt,
    bool Approved,
    string? ApprovedBy);

public sealed class EnvironmentMonitoringExceptionRegistry
{
    private readonly Dictionary<Guid, EnvironmentMonitoringException> _exceptions = new();
    private readonly object _sync = new();

    public bool Register(EnvironmentMonitoringException item)
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
            if (!_exceptions.TryGetValue(id, out EnvironmentMonitoringException? current))
            {
                return false;
            }
            _exceptions[id] = current with { Approved = true, ApprovedBy = actor };
            return true;
        }
    }

    public IReadOnlyList<EnvironmentMonitoringException> FindActive(Guid recordId, DateTimeOffset now)
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