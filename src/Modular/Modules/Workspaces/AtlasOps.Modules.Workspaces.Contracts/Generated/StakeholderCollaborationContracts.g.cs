namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum StakeholderCollaborationState { Draft, Active, Paused, Completed, Archived }
public sealed record StakeholderCollaborationRecord(Guid Id, string Name, string Owner, StakeholderCollaborationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record StakeholderCollaborationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record StakeholderCollaborationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record StakeholderCollaborationQuery(string? SearchText, StakeholderCollaborationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record StakeholderCollaborationPage(IReadOnlyList<StakeholderCollaborationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record StakeholderCollaborationMutation(bool Succeeded, string Code, string Message, StakeholderCollaborationRecord? Record, StakeholderCollaborationEvent? Event);
public interface IStakeholderCollaborationRepository
{
    ValueTask<StakeholderCollaborationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<StakeholderCollaborationPage> QueryAsync(StakeholderCollaborationQuery query, CancellationToken cancellationToken);
    ValueTask<StakeholderCollaborationMutation> SaveAsync(StakeholderCollaborationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IStakeholderCollaborationEventSink { ValueTask PublishAsync(StakeholderCollaborationEvent domainEvent, CancellationToken cancellationToken); }