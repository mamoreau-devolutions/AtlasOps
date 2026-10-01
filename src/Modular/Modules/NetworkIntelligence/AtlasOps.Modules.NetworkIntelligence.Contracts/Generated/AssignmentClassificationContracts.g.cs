namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssignmentClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record AssignmentClassificationRecord(Guid Id, string Name, string Owner, AssignmentClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssignmentClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssignmentClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssignmentClassificationQuery(string? SearchText, AssignmentClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssignmentClassificationPage(IReadOnlyList<AssignmentClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssignmentClassificationMutation(bool Succeeded, string Code, string Message, AssignmentClassificationRecord? Record, AssignmentClassificationEvent? Event);
public interface IAssignmentClassificationRepository
{
    ValueTask<AssignmentClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssignmentClassificationPage> QueryAsync(AssignmentClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<AssignmentClassificationMutation> SaveAsync(AssignmentClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssignmentClassificationEventSink { ValueTask PublishAsync(AssignmentClassificationEvent domainEvent, CancellationToken cancellationToken); }