namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EntitlementDelegationState { Draft, Active, Paused, Completed, Archived }
public sealed record EntitlementDelegationRecord(Guid Id, string Name, string Owner, EntitlementDelegationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EntitlementDelegationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EntitlementDelegationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EntitlementDelegationQuery(string? SearchText, EntitlementDelegationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EntitlementDelegationPage(IReadOnlyList<EntitlementDelegationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EntitlementDelegationMutation(bool Succeeded, string Code, string Message, EntitlementDelegationRecord? Record, EntitlementDelegationEvent? Event);
public interface IEntitlementDelegationRepository
{
    ValueTask<EntitlementDelegationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EntitlementDelegationPage> QueryAsync(EntitlementDelegationQuery query, CancellationToken cancellationToken);
    ValueTask<EntitlementDelegationMutation> SaveAsync(EntitlementDelegationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEntitlementDelegationEventSink { ValueTask PublishAsync(EntitlementDelegationEvent domainEvent, CancellationToken cancellationToken); }