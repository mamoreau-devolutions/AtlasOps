namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum IdentityDelegationState { Draft, Active, Paused, Completed, Archived }
public sealed record IdentityDelegationRecord(Guid Id, string Name, string Owner, IdentityDelegationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record IdentityDelegationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record IdentityDelegationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record IdentityDelegationQuery(string? SearchText, IdentityDelegationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record IdentityDelegationPage(IReadOnlyList<IdentityDelegationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record IdentityDelegationMutation(bool Succeeded, string Code, string Message, IdentityDelegationRecord? Record, IdentityDelegationEvent? Event);
public interface IIdentityDelegationRepository
{
    ValueTask<IdentityDelegationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<IdentityDelegationPage> QueryAsync(IdentityDelegationQuery query, CancellationToken cancellationToken);
    ValueTask<IdentityDelegationMutation> SaveAsync(IdentityDelegationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IIdentityDelegationEventSink { ValueTask PublishAsync(IdentityDelegationEvent domainEvent, CancellationToken cancellationToken); }