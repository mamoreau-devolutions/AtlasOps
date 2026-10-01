namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssignmentCollisionState { Draft, Active, Paused, Completed, Archived }
public sealed record AssignmentCollisionRecord(Guid Id, string Name, string Owner, AssignmentCollisionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssignmentCollisionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssignmentCollisionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssignmentCollisionQuery(string? SearchText, AssignmentCollisionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssignmentCollisionPage(IReadOnlyList<AssignmentCollisionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssignmentCollisionMutation(bool Succeeded, string Code, string Message, AssignmentCollisionRecord? Record, AssignmentCollisionEvent? Event);
public interface IAssignmentCollisionRepository
{
    ValueTask<AssignmentCollisionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssignmentCollisionPage> QueryAsync(AssignmentCollisionQuery query, CancellationToken cancellationToken);
    ValueTask<AssignmentCollisionMutation> SaveAsync(AssignmentCollisionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssignmentCollisionEventSink { ValueTask PublishAsync(AssignmentCollisionEvent domainEvent, CancellationToken cancellationToken); }