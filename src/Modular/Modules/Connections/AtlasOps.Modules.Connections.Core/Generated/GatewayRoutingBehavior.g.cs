namespace AtlasOps.Modules.Connections.Core;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AtlasOps.Modules.Connections.Contracts;

public sealed record GatewayRoutingValidationIssue(string Field, string Code, string Message);
public sealed record GatewayRoutingPolicyDecision(bool Allowed, string Code, string Reason);
public sealed class GatewayRoutingValidator
{
    public IReadOnlyList<GatewayRoutingValidationIssue> Validate(GatewayRoutingRecord record)
    {
        List<GatewayRoutingValidationIssue> issues = new();
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
public sealed class GatewayRoutingPolicy
{
    private static readonly IReadOnlyDictionary<GatewayRoutingState, IReadOnlySet<string>> Actions = new Dictionary<GatewayRoutingState, IReadOnlySet<string>>
    {
        [GatewayRoutingState.Draft] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "activate", "archive", "update" },
        [GatewayRoutingState.Active] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "pause", "complete", "archive", "update" },
        [GatewayRoutingState.Paused] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "resume", "archive", "update" },
        [GatewayRoutingState.Completed] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "reopen", "archive" },
        [GatewayRoutingState.Archived] = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "restore" }
    };
    public GatewayRoutingPolicyDecision Evaluate(GatewayRoutingRecord record, GatewayRoutingCommand command)
    {
        if (!Actions[record.State].Contains(command.Action)) { return new(false, "transition-denied", "The action is not valid for the current state."); }
        if (record.RiskScore > 0.85d && !command.Parameters.ContainsKey("approval")) { return new(false, "approval-required", "High-risk records require an approval reference."); }
        if (record.Priority >= 90 && !command.Parameters.ContainsKey("changeTicket")) { return new(false, "ticket-required", "Critical-priority records require a change ticket."); }
        if (command.ExpectedRevision != record.Revision) { return new(false, "revision-conflict", "The record changed after the command was prepared."); }
        return new(true, "allowed", "The requested action satisfies the policy.");
    }
}
public sealed class GatewayRoutingStateMachine
{
    public GatewayRoutingMutation Apply(GatewayRoutingRecord current, GatewayRoutingCommand command)
    {
        GatewayRoutingState? next = command.Action.ToLowerInvariant() switch
        {
            "activate" or "resume" or "reopen" or "restore" => GatewayRoutingState.Active,
            "pause" => GatewayRoutingState.Paused,
            "complete" => GatewayRoutingState.Completed,
            "archive" => GatewayRoutingState.Archived,
            "update" => current.State,
            _ => null
        };
        if (!next.HasValue) { return new(false, "unknown-action", "The requested action is not recognized.", current, null); }
        string name = command.Parameters.TryGetValue("name", out string? suppliedName) && !string.IsNullOrWhiteSpace(suppliedName) ? suppliedName : current.Name;
        string owner = command.Parameters.TryGetValue("owner", out string? suppliedOwner) && !string.IsNullOrWhiteSpace(suppliedOwner) ? suppliedOwner : current.Owner;
        GatewayRoutingRecord updated = current with { Name = name, Owner = owner, State = next.Value, UpdatedAt = command.RequestedAt, Revision = current.Revision + 1 };
        Dictionary<string, string> details = new(StringComparer.OrdinalIgnoreCase) { ["previousState"] = current.State.ToString(), ["nextState"] = updated.State.ToString(), ["action"] = command.Action };
        GatewayRoutingEvent domainEvent = new(Guid.NewGuid(), updated.Id, string.Concat("GatewayRouting.", command.Action), command.Actor, updated.Revision, command.RequestedAt, details);
        return new(true, "applied", "The state transition was applied.", updated, domainEvent);
    }
}
public sealed class InMemoryGatewayRoutingRepository : IGatewayRoutingRepository
{
    private readonly ConcurrentDictionary<Guid, GatewayRoutingRecord> _records = new();
    public ValueTask<GatewayRoutingRecord?> GetAsync(Guid id, CancellationToken cancellationToken) { cancellationToken.ThrowIfCancellationRequested(); _records.TryGetValue(id, out GatewayRoutingRecord? record); return ValueTask.FromResult(record); }
    public ValueTask<GatewayRoutingPage> QueryAsync(GatewayRoutingQuery query, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested(); IEnumerable<GatewayRoutingRecord> result = _records.Values;
        if (!string.IsNullOrWhiteSpace(query.SearchText)) { result = result.Where(record => record.Name.Contains(query.SearchText, StringComparison.OrdinalIgnoreCase) || record.Owner.Contains(query.SearchText, StringComparison.OrdinalIgnoreCase)); }
        if (query.State.HasValue) { result = result.Where(record => record.State == query.State.Value); }
        if (!string.IsNullOrWhiteSpace(query.Owner)) { result = result.Where(record => string.Equals(record.Owner, query.Owner, StringComparison.OrdinalIgnoreCase)); }
        if (query.MinimumPriority.HasValue) { result = result.Where(record => record.Priority >= query.MinimumPriority.Value); }
        if (query.MaximumRisk.HasValue) { result = result.Where(record => record.RiskScore <= query.MaximumRisk.Value); }
        GatewayRoutingRecord[] ordered = result.OrderByDescending(static record => record.Priority).ThenBy(static record => record.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        int offset = Math.Max(0, query.Offset); int limit = Math.Clamp(query.Limit, 1, 500);
        return ValueTask.FromResult(new GatewayRoutingPage(ordered.Skip(offset).Take(limit).ToArray(), offset, limit, ordered.Length));
    }
    public ValueTask<GatewayRoutingMutation> SaveAsync(GatewayRoutingRecord record, long expectedRevision, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        while (true)
        {
            if (!_records.TryGetValue(record.Id, out GatewayRoutingRecord? existing))
            {
                if (expectedRevision != 0) { return ValueTask.FromResult(new GatewayRoutingMutation(false, "revision-conflict", "A new record must use revision zero.", null, null)); }
                if (_records.TryAdd(record.Id, record)) { return ValueTask.FromResult(new GatewayRoutingMutation(true, "created", "The record was created.", record, null)); }
                continue;
            }
            if (existing.Revision != expectedRevision) { return ValueTask.FromResult(new GatewayRoutingMutation(false, "revision-conflict", "The stored revision does not match the expected revision.", existing, null)); }
            if (_records.TryUpdate(record.Id, record, existing)) { return ValueTask.FromResult(new GatewayRoutingMutation(true, "updated", "The record was updated.", record, null)); }
        }
    }
    public ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_records.TryGetValue(id, out GatewayRoutingRecord? existing) || existing.Revision != expectedRevision) { return ValueTask.FromResult(false); }
        return ValueTask.FromResult(_records.TryRemove(new KeyValuePair<Guid, GatewayRoutingRecord>(id, existing)));
    }
}
public sealed class BufferingGatewayRoutingEventSink : IGatewayRoutingEventSink
{
    private readonly List<GatewayRoutingEvent> _events = new(); private readonly object _sync = new();
    public IReadOnlyList<GatewayRoutingEvent> Events { get { lock (_sync) { return _events.ToArray(); } } }
    public ValueTask PublishAsync(GatewayRoutingEvent domainEvent, CancellationToken cancellationToken) { cancellationToken.ThrowIfCancellationRequested(); lock (_sync) { _events.Add(domainEvent); } return ValueTask.CompletedTask; }
}
public sealed class GatewayRoutingService
{
    private readonly IGatewayRoutingRepository _repository; private readonly IGatewayRoutingEventSink _eventSink;
    private readonly GatewayRoutingValidator _validator = new(); private readonly GatewayRoutingPolicy _policy = new(); private readonly GatewayRoutingStateMachine _machine = new();
    public GatewayRoutingService(IGatewayRoutingRepository repository, IGatewayRoutingEventSink eventSink) { _repository = repository; _eventSink = eventSink; }
    public async ValueTask<GatewayRoutingMutation> ExecuteAsync(GatewayRoutingCommand command, CancellationToken cancellationToken)
    {
        GatewayRoutingRecord? current = await _repository.GetAsync(command.RecordId, cancellationToken).ConfigureAwait(false);
        if (current is null) { return new(false, "not-found", "The requested record does not exist.", null, null); }
        GatewayRoutingPolicyDecision decision = _policy.Evaluate(current, command);
        if (!decision.Allowed) { return new(false, decision.Code, decision.Reason, current, null); }
        GatewayRoutingMutation transition = _machine.Apply(current, command);
        if (!transition.Succeeded || transition.Record is null) { return transition; }
        IReadOnlyList<GatewayRoutingValidationIssue> issues = _validator.Validate(transition.Record);
        if (issues.Count > 0) { return new(false, issues[0].Code, issues[0].Message, current, null); }
        GatewayRoutingMutation saved = await _repository.SaveAsync(transition.Record, current.Revision, cancellationToken).ConfigureAwait(false);
        if (saved.Succeeded && transition.Event is not null) { await _eventSink.PublishAsync(transition.Event, cancellationToken).ConfigureAwait(false); }
        return saved.Succeeded ? transition : saved;
    }
}