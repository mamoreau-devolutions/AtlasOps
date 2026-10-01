namespace AtlasOps.Modules.Automation.Core;

using System;
using System.Collections.Generic;
using System.Linq;

using AtlasOps.Modules.Automation.Contracts;

public enum ApprovalCancellationApprovalStage
{
    Requested,
    TeamReview,
    RiskReview,
    OwnerApproval,
    Approved,
    Rejected,
    Expired
}

public sealed record ApprovalCancellationApprovalRule(
    string Id,
    string DisplayName,
    int MinimumPriority,
    double MinimumRisk,
    int RequiredApprovals,
    TimeSpan Expiration,
    IReadOnlySet<string> RequiredRoles);

public sealed record ApprovalCancellationApprovalVote(
    string Actor,
    string Role,
    bool Approved,
    string Rationale,
    DateTimeOffset CastAt);

public sealed record ApprovalCancellationApprovalRequest(
    Guid Id,
    Guid RecordId,
    ApprovalCancellationApprovalStage Stage,
    DateTimeOffset RequestedAt,
    DateTimeOffset ExpiresAt,
    IReadOnlyList<ApprovalCancellationApprovalRule> Rules,
    IReadOnlyList<ApprovalCancellationApprovalVote> Votes);

public sealed record ApprovalCancellationApprovalDecision(
    bool Approved,
    ApprovalCancellationApprovalStage NextStage,
    string Code,
    string Message,
    int RequiredVotes,
    int AcceptedVotes,
    IReadOnlyList<string> MissingRoles);

public sealed class ApprovalCancellationApprovalEngine
{
    public ApprovalCancellationApprovalRequest CreateRequest(
        ApprovalCancellationRecord record,
        IEnumerable<ApprovalCancellationApprovalRule> availableRules,
        DateTimeOffset now)
    {
        ApprovalCancellationApprovalRule[] selected = availableRules
            .Where(rule => record.Priority >= rule.MinimumPriority || record.RiskScore >= rule.MinimumRisk)
            .OrderByDescending(static rule => rule.RequiredApprovals)
            .ThenBy(static rule => rule.Id, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        TimeSpan expiration = selected.Length == 0
            ? TimeSpan.FromDays(7)
            : selected.Min(static rule => rule.Expiration);
        return new ApprovalCancellationApprovalRequest(
            Guid.NewGuid(),
            record.Id,
            ApprovalCancellationApprovalStage.Requested,
            now,
            now.Add(expiration),
            selected,
            Array.Empty<ApprovalCancellationApprovalVote>());
    }

    public ApprovalCancellationApprovalRequest AddVote(
        ApprovalCancellationApprovalRequest request,
        ApprovalCancellationApprovalVote vote)
    {
        Dictionary<string, ApprovalCancellationApprovalVote> latestVotes = request.Votes
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

    public ApprovalCancellationApprovalDecision Evaluate(
        ApprovalCancellationApprovalRequest request,
        DateTimeOffset now)
    {
        if (now >= request.ExpiresAt)
        {
            return new ApprovalCancellationApprovalDecision(
                false,
                ApprovalCancellationApprovalStage.Expired,
                "expired",
                "The approval request expired.",
                0,
                0,
                Array.Empty<string>());
        }
        if (request.Votes.Any(static vote => !vote.Approved))
        {
            return new ApprovalCancellationApprovalDecision(
                false,
                ApprovalCancellationApprovalStage.Rejected,
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
        return new ApprovalCancellationApprovalDecision(
            approved,
            approved ? ApprovalCancellationApprovalStage.Approved : ApprovalCancellationApprovalStage.TeamReview,
            approved ? "approved" : "pending",
            approved ? "All approval requirements are satisfied." : "Additional approvals are required.",
            requiredVotes,
            accepted,
            missingRoles);
    }
}

public sealed record ApprovalCancellationServiceLevel(
    string Id,
    int MinimumPriority,
    TimeSpan ResponseTarget,
    TimeSpan CompletionTarget,
    IReadOnlySet<ApprovalCancellationState> PausedStates);

public sealed record ApprovalCancellationServiceLevelStatus(
    string PolicyId,
    TimeSpan Age,
    TimeSpan Remaining,
    bool ResponseBreached,
    bool CompletionBreached,
    double CompletionPercent);

public sealed class ApprovalCancellationServiceLevelEvaluator
{
    public ApprovalCancellationServiceLevelStatus Evaluate(
        ApprovalCancellationRecord record,
        ApprovalCancellationServiceLevel policy,
        DateTimeOffset now,
        DateTimeOffset? respondedAt)
    {
        TimeSpan age = now <= record.CreatedAt ? TimeSpan.Zero : now - record.CreatedAt;
        bool paused = policy.PausedStates.Contains(record.State);
        bool responseBreached = !respondedAt.HasValue && !paused && age > policy.ResponseTarget;
        bool completionBreached = record.State is not ApprovalCancellationState.Completed and not ApprovalCancellationState.Archived && !paused && age > policy.CompletionTarget;
        TimeSpan remaining = policy.CompletionTarget - age;
        if (remaining < TimeSpan.Zero)
        {
            remaining = TimeSpan.Zero;
        }
        double progress = policy.CompletionTarget == TimeSpan.Zero
            ? 1d
            : Math.Clamp(age.TotalMilliseconds / policy.CompletionTarget.TotalMilliseconds, 0d, 1d);
        return new ApprovalCancellationServiceLevelStatus(
            policy.Id,
            age,
            remaining,
            responseBreached,
            completionBreached,
            Math.Round(progress * 100d, 2));
    }
}

public sealed record ApprovalCancellationException(
    Guid Id,
    Guid RecordId,
    string RuleId,
    string Justification,
    string RequestedBy,
    DateTimeOffset RequestedAt,
    DateTimeOffset ExpiresAt,
    bool Approved,
    string? ApprovedBy);

public sealed class ApprovalCancellationExceptionRegistry
{
    private readonly Dictionary<Guid, ApprovalCancellationException> _exceptions = new();
    private readonly object _sync = new();

    public bool Register(ApprovalCancellationException item)
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
            if (!_exceptions.TryGetValue(id, out ApprovalCancellationException? current))
            {
                return false;
            }
            _exceptions[id] = current with { Approved = true, ApprovedBy = actor };
            return true;
        }
    }

    public IReadOnlyList<ApprovalCancellationException> FindActive(Guid recordId, DateTimeOffset now)
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