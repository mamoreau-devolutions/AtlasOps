namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MembershipDelegationState { Draft, Active, Paused, Completed, Archived }
public sealed record MembershipDelegationRecord(Guid Id, string Name, string Owner, MembershipDelegationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MembershipDelegationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MembershipDelegationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MembershipDelegationQuery(string? SearchText, MembershipDelegationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MembershipDelegationPage(IReadOnlyList<MembershipDelegationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MembershipDelegationMutation(bool Succeeded, string Code, string Message, MembershipDelegationRecord? Record, MembershipDelegationEvent? Event);
public interface IMembershipDelegationRepository
{
    ValueTask<MembershipDelegationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MembershipDelegationPage> QueryAsync(MembershipDelegationQuery query, CancellationToken cancellationToken);
    ValueTask<MembershipDelegationMutation> SaveAsync(MembershipDelegationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMembershipDelegationEventSink { ValueTask PublishAsync(MembershipDelegationEvent domainEvent, CancellationToken cancellationToken); }