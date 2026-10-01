namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RoleDelegationState { Draft, Active, Paused, Completed, Archived }
public sealed record RoleDelegationRecord(Guid Id, string Name, string Owner, RoleDelegationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RoleDelegationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RoleDelegationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RoleDelegationQuery(string? SearchText, RoleDelegationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RoleDelegationPage(IReadOnlyList<RoleDelegationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RoleDelegationMutation(bool Succeeded, string Code, string Message, RoleDelegationRecord? Record, RoleDelegationEvent? Event);
public interface IRoleDelegationRepository
{
    ValueTask<RoleDelegationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RoleDelegationPage> QueryAsync(RoleDelegationQuery query, CancellationToken cancellationToken);
    ValueTask<RoleDelegationMutation> SaveAsync(RoleDelegationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRoleDelegationEventSink { ValueTask PublishAsync(RoleDelegationEvent domainEvent, CancellationToken cancellationToken); }