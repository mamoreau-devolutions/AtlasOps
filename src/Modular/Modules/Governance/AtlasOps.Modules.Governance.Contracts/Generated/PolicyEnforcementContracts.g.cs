namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PolicyEnforcementState { Draft, Active, Paused, Completed, Archived }
public sealed record PolicyEnforcementRecord(Guid Id, string Name, string Owner, PolicyEnforcementState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PolicyEnforcementCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PolicyEnforcementEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PolicyEnforcementQuery(string? SearchText, PolicyEnforcementState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PolicyEnforcementPage(IReadOnlyList<PolicyEnforcementRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PolicyEnforcementMutation(bool Succeeded, string Code, string Message, PolicyEnforcementRecord? Record, PolicyEnforcementEvent? Event);
public interface IPolicyEnforcementRepository
{
    ValueTask<PolicyEnforcementRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PolicyEnforcementPage> QueryAsync(PolicyEnforcementQuery query, CancellationToken cancellationToken);
    ValueTask<PolicyEnforcementMutation> SaveAsync(PolicyEnforcementRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPolicyEnforcementEventSink { ValueTask PublishAsync(PolicyEnforcementEvent domainEvent, CancellationToken cancellationToken); }