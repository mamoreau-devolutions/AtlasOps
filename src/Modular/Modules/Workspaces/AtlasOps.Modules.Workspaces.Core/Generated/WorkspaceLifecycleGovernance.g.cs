namespace AtlasOps.Modules.Workspaces.Core;

using System;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Workspaces.Contracts;

public enum WorkspaceLifecycleApprovalStage
{
    Requested,
    TeamReview,
    RiskReview,
    OwnerApproval,
    Approved,
    Rejected,
    Expired
}

public sealed record WorkspaceLifecycleApprovalRule(
    string Id,
    string DisplayName,
    int MinimumPriority,
    double MinimumRisk,
    int RequiredApprovals,
    TimeSpan Expiration,
    IReadOnlySet<string> RequiredRoles);

public sealed record WorkspaceLifecycleApprovalVote(
    string Actor,
    string Role,
    bool Approved,
    string Rationale,
    DateTimeOffset CastAt);

public sealed record WorkspaceLifecycleApprovalRequest(
    Guid Id,
    Guid RecordId,
    WorkspaceLifecycleApprovalStage Stage,
    DateTimeOffset RequestedAt,
    DateTimeOffset ExpiresAt,
    IReadOnlyList<WorkspaceLifecycleApprovalRule> Rules,
    IReadOnlyList<WorkspaceLifecycleApprovalVote> Votes);

public sealed record WorkspaceLifecycleApprovalDecision(
    bool Approved,
    WorkspaceLifecycleApprovalStage NextStage,
    string Code,
    string Message,
    int RequiredVotes,
    int AcceptedVotes,
    IReadOnlyList<string> MissingRoles);

public sealed class WorkspaceLifecycleApprovalEngine
{
    public WorkspaceLifecycleApprovalRequest CreateRequest(
        WorkspaceLifecycleRecord record,
        IEnumerable<WorkspaceLifecycleApprovalRule> availableRules,
        DateTimeOffset now)
    {
        WorkspaceLifecycleApprovalRule[] selected = availableRules
            .Where(rule => record.Priority >= rule.MinimumPriority || record.RiskScore >= rule.MinimumRisk)
            .OrderByDescending(static rule => rule.RequiredApprovals)
            .ThenBy(static rule => rule.Id, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        TimeSpan expiration = selected.Length == 0
            ? TimeSpan.FromDays(7)
            : selected.Min(static rule => rule.Expiration);
        return new WorkspaceLifecycleApprovalRequest(
            Guid.NewGuid(),
            record.Id,
            WorkspaceLifecycleApprovalStage.Requested,
            now,
            now.Add(expiration),
            selected,
            Array.Empty<WorkspaceLifecycleApprovalVote>());
    }

    public WorkspaceLifecycleApprovalRequest AddVote(
        WorkspaceLifecycleApprovalRequest request,
        WorkspaceLifecycleApprovalVote vote)
    {
        Dictionary<string, WorkspaceLifecycleApprovalVote> latestVotes = request.Votes
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

    public WorkspaceLifecycleApprovalDecision Evaluate(
        WorkspaceLifecycleApprovalRequest request,
        DateTimeOffset now)
    {
        if (now >= request.ExpiresAt)
        {
            return new WorkspaceLifecycleApprovalDecision(
                false,
                WorkspaceLifecycleApprovalStage.Expired,
                "expired",
                "The approval request expired.",
                0,
                0,
                Array.Empty<string>());
        }
        if (request.Votes.Any(static vote => !vote.Approved))
        {
            return new WorkspaceLifecycleApprovalDecision(
                false,
                WorkspaceLifecycleApprovalStage.Rejected,
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
        return new WorkspaceLifecycleApprovalDecision(
            approved,
            approved ? WorkspaceLifecycleApprovalStage.Approved : WorkspaceLifecycleApprovalStage.TeamReview,
            approved ? "approved" : "pending",
            approved ? "All approval requirements are satisfied." : "Additional approvals are required.",
            requiredVotes,
            accepted,
            missingRoles);
    }
}

public sealed record WorkspaceLifecycleServiceLevel(
    string Id,
    int MinimumPriority,
    TimeSpan ResponseTarget,
    TimeSpan CompletionTarget,
    IReadOnlySet<WorkspaceLifecycleState> PausedStates);

public sealed record WorkspaceLifecycleServiceLevelStatus(
    string PolicyId,
    TimeSpan Age,
    TimeSpan Remaining,
    bool ResponseBreached,
    bool CompletionBreached,
    double CompletionPercent);

public sealed class WorkspaceLifecycleServiceLevelEvaluator
{
    public WorkspaceLifecycleServiceLevelStatus Evaluate(
        WorkspaceLifecycleRecord record,
        WorkspaceLifecycleServiceLevel policy,
        DateTimeOffset now,
        DateTimeOffset? respondedAt)
    {
        TimeSpan age = now <= record.CreatedAt ? TimeSpan.Zero : now - record.CreatedAt;
        bool paused = policy.PausedStates.Contains(record.State);
        bool responseBreached = !respondedAt.HasValue && !paused && age > policy.ResponseTarget;
        bool completionBreached = record.State is not WorkspaceLifecycleState.Completed and not WorkspaceLifecycleState.Archived && !paused && age > policy.CompletionTarget;
        TimeSpan remaining = policy.CompletionTarget - age;
        if (remaining < TimeSpan.Zero)
        {
            remaining = TimeSpan.Zero;
        }
        double progress = policy.CompletionTarget == TimeSpan.Zero
            ? 1d
            : Math.Clamp(age.TotalMilliseconds / policy.CompletionTarget.TotalMilliseconds, 0d, 1d);
        return new WorkspaceLifecycleServiceLevelStatus(
            policy.Id,
            age,
            remaining,
            responseBreached,
            completionBreached,
            Math.Round(progress * 100d, 2));
    }
}

public sealed record WorkspaceLifecycleException(
    Guid Id,
    Guid RecordId,
    string RuleId,
    string Justification,
    string RequestedBy,
    DateTimeOffset RequestedAt,
    DateTimeOffset ExpiresAt,
    bool Approved,
    string? ApprovedBy);

public sealed class WorkspaceLifecycleExceptionRegistry
{
    private readonly Dictionary<Guid, WorkspaceLifecycleException> _exceptions = new();
    private readonly object _sync = new();

    public bool Register(WorkspaceLifecycleException item)
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
            if (!_exceptions.TryGetValue(id, out WorkspaceLifecycleException? current))
            {
                return false;
            }
            _exceptions[id] = current with { Approved = true, ApprovedBy = actor };
            return true;
        }
    }

    public IReadOnlyList<WorkspaceLifecycleException> FindActive(Guid recordId, DateTimeOffset now)
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