namespace AtlasOps.Modules.Deployments.Core;

using System;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Deployments.Contracts;

public enum RollbackMonitoringApprovalStage
{
    Requested,
    TeamReview,
    RiskReview,
    OwnerApproval,
    Approved,
    Rejected,
    Expired
}

public sealed record RollbackMonitoringApprovalRule(
    string Id,
    string DisplayName,
    int MinimumPriority,
    double MinimumRisk,
    int RequiredApprovals,
    TimeSpan Expiration,
    IReadOnlySet<string> RequiredRoles);

public sealed record RollbackMonitoringApprovalVote(
    string Actor,
    string Role,
    bool Approved,
    string Rationale,
    DateTimeOffset CastAt);

public sealed record RollbackMonitoringApprovalRequest(
    Guid Id,
    Guid RecordId,
    RollbackMonitoringApprovalStage Stage,
    DateTimeOffset RequestedAt,
    DateTimeOffset ExpiresAt,
    IReadOnlyList<RollbackMonitoringApprovalRule> Rules,
    IReadOnlyList<RollbackMonitoringApprovalVote> Votes);

public sealed record RollbackMonitoringApprovalDecision(
    bool Approved,
    RollbackMonitoringApprovalStage NextStage,
    string Code,
    string Message,
    int RequiredVotes,
    int AcceptedVotes,
    IReadOnlyList<string> MissingRoles);

public sealed class RollbackMonitoringApprovalEngine
{
    public RollbackMonitoringApprovalRequest CreateRequest(
        RollbackMonitoringRecord record,
        IEnumerable<RollbackMonitoringApprovalRule> availableRules,
        DateTimeOffset now)
    {
        RollbackMonitoringApprovalRule[] selected = availableRules
            .Where(rule => record.Priority >= rule.MinimumPriority || record.RiskScore >= rule.MinimumRisk)
            .OrderByDescending(static rule => rule.RequiredApprovals)
            .ThenBy(static rule => rule.Id, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        TimeSpan expiration = selected.Length == 0
            ? TimeSpan.FromDays(7)
            : selected.Min(static rule => rule.Expiration);
        return new RollbackMonitoringApprovalRequest(
            Guid.NewGuid(),
            record.Id,
            RollbackMonitoringApprovalStage.Requested,
            now,
            now.Add(expiration),
            selected,
            Array.Empty<RollbackMonitoringApprovalVote>());
    }

    public RollbackMonitoringApprovalRequest AddVote(
        RollbackMonitoringApprovalRequest request,
        RollbackMonitoringApprovalVote vote)
    {
        Dictionary<string, RollbackMonitoringApprovalVote> latestVotes = request.Votes
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

    public RollbackMonitoringApprovalDecision Evaluate(
        RollbackMonitoringApprovalRequest request,
        DateTimeOffset now)
    {
        if (now >= request.ExpiresAt)
        {
            return new RollbackMonitoringApprovalDecision(
                false,
                RollbackMonitoringApprovalStage.Expired,
                "expired",
                "The approval request expired.",
                0,
                0,
                Array.Empty<string>());
        }
        if (request.Votes.Any(static vote => !vote.Approved))
        {
            return new RollbackMonitoringApprovalDecision(
                false,
                RollbackMonitoringApprovalStage.Rejected,
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
        return new RollbackMonitoringApprovalDecision(
            approved,
            approved ? RollbackMonitoringApprovalStage.Approved : RollbackMonitoringApprovalStage.TeamReview,
            approved ? "approved" : "pending",
            approved ? "All approval requirements are satisfied." : "Additional approvals are required.",
            requiredVotes,
            accepted,
            missingRoles);
    }
}

public sealed record RollbackMonitoringServiceLevel(
    string Id,
    int MinimumPriority,
    TimeSpan ResponseTarget,
    TimeSpan CompletionTarget,
    IReadOnlySet<RollbackMonitoringState> PausedStates);

public sealed record RollbackMonitoringServiceLevelStatus(
    string PolicyId,
    TimeSpan Age,
    TimeSpan Remaining,
    bool ResponseBreached,
    bool CompletionBreached,
    double CompletionPercent);

public sealed class RollbackMonitoringServiceLevelEvaluator
{
    public RollbackMonitoringServiceLevelStatus Evaluate(
        RollbackMonitoringRecord record,
        RollbackMonitoringServiceLevel policy,
        DateTimeOffset now,
        DateTimeOffset? respondedAt)
    {
        TimeSpan age = now <= record.CreatedAt ? TimeSpan.Zero : now - record.CreatedAt;
        bool paused = policy.PausedStates.Contains(record.State);
        bool responseBreached = !respondedAt.HasValue && !paused && age > policy.ResponseTarget;
        bool completionBreached = record.State is not RollbackMonitoringState.Completed and not RollbackMonitoringState.Archived && !paused && age > policy.CompletionTarget;
        TimeSpan remaining = policy.CompletionTarget - age;
        if (remaining < TimeSpan.Zero)
        {
            remaining = TimeSpan.Zero;
        }
        double progress = policy.CompletionTarget == TimeSpan.Zero
            ? 1d
            : Math.Clamp(age.TotalMilliseconds / policy.CompletionTarget.TotalMilliseconds, 0d, 1d);
        return new RollbackMonitoringServiceLevelStatus(
            policy.Id,
            age,
            remaining,
            responseBreached,
            completionBreached,
            Math.Round(progress * 100d, 2));
    }
}

public sealed record RollbackMonitoringException(
    Guid Id,
    Guid RecordId,
    string RuleId,
    string Justification,
    string RequestedBy,
    DateTimeOffset RequestedAt,
    DateTimeOffset ExpiresAt,
    bool Approved,
    string? ApprovedBy);

public sealed class RollbackMonitoringExceptionRegistry
{
    private readonly Dictionary<Guid, RollbackMonitoringException> _exceptions = new();
    private readonly object _sync = new();

    public bool Register(RollbackMonitoringException item)
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
            if (!_exceptions.TryGetValue(id, out RollbackMonitoringException? current))
            {
                return false;
            }
            _exceptions[id] = current with { Approved = true, ApprovedBy = actor };
            return true;
        }
    }

    public IReadOnlyList<RollbackMonitoringException> FindActive(Guid recordId, DateTimeOffset now)
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