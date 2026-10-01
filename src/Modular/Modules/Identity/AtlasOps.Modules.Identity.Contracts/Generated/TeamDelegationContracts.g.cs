namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TeamDelegationState { Draft, Active, Paused, Completed, Archived }
public sealed record TeamDelegationRecord(Guid Id, string Name, string Owner, TeamDelegationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TeamDelegationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TeamDelegationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TeamDelegationQuery(string? SearchText, TeamDelegationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TeamDelegationPage(IReadOnlyList<TeamDelegationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TeamDelegationMutation(bool Succeeded, string Code, string Message, TeamDelegationRecord? Record, TeamDelegationEvent? Event);
public interface ITeamDelegationRepository
{
    ValueTask<TeamDelegationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TeamDelegationPage> QueryAsync(TeamDelegationQuery query, CancellationToken cancellationToken);
    ValueTask<TeamDelegationMutation> SaveAsync(TeamDelegationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITeamDelegationEventSink { ValueTask PublishAsync(TeamDelegationEvent domainEvent, CancellationToken cancellationToken); }