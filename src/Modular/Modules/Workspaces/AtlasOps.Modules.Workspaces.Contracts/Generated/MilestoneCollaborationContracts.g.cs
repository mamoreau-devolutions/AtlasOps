namespace AtlasOps.Modules.Workspaces.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum MilestoneCollaborationState { Draft, Active, Paused, Completed, Archived }
public sealed record MilestoneCollaborationRecord(Guid Id, string Name, string Owner, MilestoneCollaborationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record MilestoneCollaborationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record MilestoneCollaborationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record MilestoneCollaborationQuery(string? SearchText, MilestoneCollaborationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record MilestoneCollaborationPage(IReadOnlyList<MilestoneCollaborationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record MilestoneCollaborationMutation(bool Succeeded, string Code, string Message, MilestoneCollaborationRecord? Record, MilestoneCollaborationEvent? Event);
public interface IMilestoneCollaborationRepository
{
    ValueTask<MilestoneCollaborationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<MilestoneCollaborationPage> QueryAsync(MilestoneCollaborationQuery query, CancellationToken cancellationToken);
    ValueTask<MilestoneCollaborationMutation> SaveAsync(MilestoneCollaborationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IMilestoneCollaborationEventSink { ValueTask PublishAsync(MilestoneCollaborationEvent domainEvent, CancellationToken cancellationToken); }