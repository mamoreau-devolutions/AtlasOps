namespace AtlasOps.Modules.Connections.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AtlasOps.Modules.Connections.Contracts;

public sealed record TunnelRoutingValidationIssue(string Field, string Code, string Message);
public sealed record TunnelRoutingPolicyDecision(bool Allowed, string Code, string Reason);
public sealed class TunnelRoutingValidator
{
    public IReadOnlyList<TunnelRoutingValidationIssue> Validate(TunnelRoutingRecord record)
    {
        List<TunnelRoutingValidationIssue> issues = new();
        if (record.Id == Guid.Empty) { issues.Add(new(nameof(record.Id), "required", "A stable identifier is required.")); }
        if (string.IsNullOrWhiteSpace(record.Name) || record.Name.Length > 160) { issues.Add(new(nameof(record.Name), "range", "Name must contain between 1 and 160 characters.")); }
        if (string.IsNullOrWhiteSpace(record.Owner) || record.Owner.Length > 120) { issues.Add(new(nameof(record.Owner), "range", "Owner must contain between 1 and 120 characters.")); }
        if (record.Priority is < 0 or > 100) { issues.Add(new(nameof(record.Priority), "range", "Priority must be between 0 and 100.")); }
        if (record.EstimatedCost < 0m) { issues.Add(new(nameof(record.EstimatedCost), "minimum", "Estimated cost cannot be negative.")); }
        if (record.RiskScore is < 0d or > 1d) { issues.Add(new(nameof(record.RiskScore), "range", "Risk score must be between zero and one.")); }
        if (record.UpdatedAt < record.CreatedAt) { issues.Add(new(nameof(record.UpdatedAt), "chronology", "Updated time cannot precede created time.")); }
        if (record.DueAt.HasValue && record.DueAt.Value < record.CreatedAt) { issues.Add(new(nameof(record.DueAt), "chronology", "Due time cannot precede created time.")); }
        return issues;
    }
}
public sealed class TunnelRoutingPolicy
{
    private static readonly IReadOnlyDictionary<TunnelRoutingState, IReadOnlySet<string>> Actions = new Dictionary<TunnelRoutingState, IReadOnlySet<string>>
    {
        [TunnelRoutingState.Draft] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "activate", "archive", "update" },
        [TunnelRoutingState.Active] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "pause", "complete", "archive", "update" },
        [TunnelRoutingState.Paused] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "resume", "archive", "update" },
        [TunnelRoutingState.Completed] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "reopen", "archive" },
        [TunnelRoutingState.Archived] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "restore" }
    };
    public TunnelRoutingPolicyDecision Evaluate(TunnelRoutingRecord record, TunnelRoutingCommand command)
    {
        if (!Actions[record.State].Contains(command.Action)) { return new(false, "transition-denied", "The action is not valid for the current state."); }
        if (record.RiskScore > 0.85d && !command.Parameters.ContainsKey("approval")) { return new(false, "approval-required", "High-risk records require an approval reference."); }
        if (record.Priority >= 90 && !command.Parameters.ContainsKey("changeTicket")) { return new(false, "ticket-required", "Critical-priority records require a change ticket."); }
        if (command.ExpectedRevision != record.Revision) { return new(false, "revision-conflict", "The record changed after the command was prepared."); }
        return new(true, "allowed", "The requested action satisfies the policy.");
    }
}
public sealed class TunnelRoutingStateMachine
{
    public TunnelRoutingMutation Apply(TunnelRoutingRecord current, TunnelRoutingCommand command)
    {
        TunnelRoutingState? next = command.Action.ToLowerInvariant() switch
        {
            "activate" or "resume" or "reopen" or "restore" => TunnelRoutingState.Active,
            "pause" => TunnelRoutingState.Paused,
            "complete" => TunnelRoutingState.Completed,
            "archive" => TunnelRoutingState.Archived,
            "update" => current.State,
            _ => null
        };
        if (!next.HasValue) { return new(false, "unknown-action", "The requested action is not recognized.", current, null); }
        string name = command.Parameters.TryGetValue("name", out string? suppliedName) && !string.IsNullOrWhiteSpace(suppliedName) ? suppliedName : current.Name;
        string owner = command.Parameters.TryGetValue("owner", out string? suppliedOwner) && !string.IsNullOrWhiteSpace(suppliedOwner) ? suppliedOwner : current.Owner;
        TunnelRoutingRecord updated = current with { Name = name, Owner = owner, State = next.Value, UpdatedAt = command.RequestedAt, Revision = current.Revision + 1 };
        Dictionary<string, string> details = new(StringComparer.OrdinalIgnoreCase) { ["previousState"] = current.State.ToString(), ["nextState"] = updated.State.ToString(), ["action"] = command.Action };
        TunnelRoutingEvent domainEvent = new(Guid.NewGuid(), updated.Id, string.Concat("TunnelRouting.", command.Action), command.Actor, updated.Revision, command.RequestedAt, details);
        return new(true, "applied", "The state transition was applied.", updated, domainEvent);
    }
}
public sealed class InMemoryTunnelRoutingRepository : ITunnelRoutingRepository
{
    private readonly ConcurrentDictionary<Guid, TunnelRoutingRecord> _records = new();
    public ValueTask<TunnelRoutingRecord?> GetAsync(Guid id, CancellationToken cancellationToken) { cancellationToken.ThrowIfCancellationRequested(); _records.TryGetValue(id, out TunnelRoutingRecord? record); return ValueTask.FromResult(record); }
    public ValueTask<TunnelRoutingPage> QueryAsync(TunnelRoutingQuery query, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); IEnumerable<TunnelRoutingRecord> result = _records.Values;
        if (!string.IsNullOrWhiteSpace(query.SearchText)) { result = result.Where(record => record.Name.Contains(query.SearchText, StringComparison.OrdinalIgnoreCase) || record.Owner.Contains(query.SearchText, StringComparison.OrdinalIgnoreCase)); }
        if (query.State.HasValue) { result = result.Where(record => record.State == query.State.Value); }
        if (!string.IsNullOrWhiteSpace(query.Owner)) { result = result.Where(record => string.Equals(record.Owner, query.Owner, StringComparison.OrdinalIgnoreCase)); }
        if (query.MinimumPriority.HasValue) { result = result.Where(record => record.Priority >= query.MinimumPriority.Value); }
        if (query.MaximumRisk.HasValue) { result = result.Where(record => record.RiskScore <= query.MaximumRisk.Value); }
        TunnelRoutingRecord[] ordered = result.OrderByDescending(static record => record.Priority).ThenBy(static record => record.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        int offset = Math.Max(0, query.Offset); int limit = Math.Clamp(query.Limit, 1, 500);
        return ValueTask.FromResult(new TunnelRoutingPage(ordered.Skip(offset).Take(limit).ToArray(), offset, limit, ordered.Length));
    }
    public ValueTask<TunnelRoutingMutation> SaveAsync(TunnelRoutingRecord record, long expectedRevision, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        while (true)
        {
            if (!_records.TryGetValue(record.Id, out TunnelRoutingRecord? existing))
            {
                if (expectedRevision != 0) { return ValueTask.FromResult(new TunnelRoutingMutation(false, "revision-conflict", "A new record must use revision zero.", null, null)); }
                if (_records.TryAdd(record.Id, record)) { return ValueTask.FromResult(new TunnelRoutingMutation(true, "created", "The record was created.", record, null)); }
                continue;
            }
            if (existing.Revision != expectedRevision) { return ValueTask.FromResult(new TunnelRoutingMutation(false, "revision-conflict", "The stored revision does not match the expected revision.", existing, null)); }
            if (_records.TryUpdate(record.Id, record, existing)) { return ValueTask.FromResult(new TunnelRoutingMutation(true, "updated", "The record was updated.", record, null)); }
        }
    }
    public ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_records.TryGetValue(id, out TunnelRoutingRecord? existing) || existing.Revision != expectedRevision) { return ValueTask.FromResult(false); }
        return ValueTask.FromResult(_records.TryRemove(new KeyValuePair<Guid, TunnelRoutingRecord>(id, existing)));
    }
}
public sealed class BufferingTunnelRoutingEventSink : ITunnelRoutingEventSink
{
    private readonly List<TunnelRoutingEvent> _events = new(); private readonly object _sync = new();
    public IReadOnlyList<TunnelRoutingEvent> Events { get { lock (_sync) { return _events.ToArray(); } } }
    public ValueTask PublishAsync(TunnelRoutingEvent domainEvent, CancellationToken cancellationToken) { cancellationToken.ThrowIfCancellationRequested(); lock (_sync) { _events.Add(domainEvent); } return ValueTask.CompletedTask; }
}
public sealed class TunnelRoutingService
{
    private readonly ITunnelRoutingRepository _repository; private readonly ITunnelRoutingEventSink _eventSink;
    private readonly TunnelRoutingValidator _validator = new(); private readonly TunnelRoutingPolicy _policy = new(); private readonly TunnelRoutingStateMachine _machine = new();
    public TunnelRoutingService(ITunnelRoutingRepository repository, ITunnelRoutingEventSink eventSink) { _repository = repository; _eventSink = eventSink; }
    public async ValueTask<TunnelRoutingMutation> ExecuteAsync(TunnelRoutingCommand command, CancellationToken cancellationToken)
    {
        TunnelRoutingRecord? current = await _repository.GetAsync(command.RecordId, cancellationToken).ConfigureAwait(false);
        if (current is null) { return new(false, "not-found", "The requested record does not exist.", null, null); }
        TunnelRoutingPolicyDecision decision = _policy.Evaluate(current, command);
        if (!decision.Allowed) { return new(false, decision.Code, decision.Reason, current, null); }
        TunnelRoutingMutation transition = _machine.Apply(current, command);
        if (!transition.Succeeded || transition.Record is null) { return transition; }
        IReadOnlyList<TunnelRoutingValidationIssue> issues = _validator.Validate(transition.Record);
        if (issues.Count > 0) { return new(false, issues[0].Code, issues[0].Message, current, null); }
        TunnelRoutingMutation saved = await _repository.SaveAsync(transition.Record, current.Revision, cancellationToken).ConfigureAwait(false);
        if (saved.Succeeded && transition.Event is not null) { await _eventSink.PublishAsync(transition.Event, cancellationToken).ConfigureAwait(false); }
        return saved.Succeeded ? transition : saved;
    }
}