namespace AtlasOps.Modules.Incidents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AlertAssignmentState { Draft, Active, Paused, Completed, Archived }
public sealed record AlertAssignmentRecord(Guid Id, string Name, string Owner, AlertAssignmentState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AlertAssignmentCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AlertAssignmentEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AlertAssignmentQuery(string? SearchText, AlertAssignmentState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AlertAssignmentPage(IReadOnlyList<AlertAssignmentRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AlertAssignmentMutation(bool Succeeded, string Code, string Message, AlertAssignmentRecord? Record, AlertAssignmentEvent? Event);
public interface IAlertAssignmentRepository
{
    ValueTask<AlertAssignmentRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AlertAssignmentPage> QueryAsync(AlertAssignmentQuery query, CancellationToken cancellationToken);
    ValueTask<AlertAssignmentMutation> SaveAsync(AlertAssignmentRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAlertAssignmentEventSink { ValueTask PublishAsync(AlertAssignmentEvent domainEvent, CancellationToken cancellationToken); }