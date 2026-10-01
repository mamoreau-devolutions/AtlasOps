namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssignmentReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record AssignmentReportingRecord(Guid Id, string Name, string Owner, AssignmentReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssignmentReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssignmentReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssignmentReportingQuery(string? SearchText, AssignmentReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssignmentReportingPage(IReadOnlyList<AssignmentReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssignmentReportingMutation(bool Succeeded, string Code, string Message, AssignmentReportingRecord? Record, AssignmentReportingEvent? Event);
public interface IAssignmentReportingRepository
{
    ValueTask<AssignmentReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssignmentReportingPage> QueryAsync(AssignmentReportingQuery query, CancellationToken cancellationToken);
    ValueTask<AssignmentReportingMutation> SaveAsync(AssignmentReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssignmentReportingEventSink { ValueTask PublishAsync(AssignmentReportingEvent domainEvent, CancellationToken cancellationToken); }