namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ObjectiveReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record ObjectiveReportingRecord(Guid Id, string Name, string Owner, ObjectiveReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ObjectiveReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ObjectiveReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ObjectiveReportingQuery(string? SearchText, ObjectiveReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ObjectiveReportingPage(IReadOnlyList<ObjectiveReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ObjectiveReportingMutation(bool Succeeded, string Code, string Message, ObjectiveReportingRecord? Record, ObjectiveReportingEvent? Event);
public interface IObjectiveReportingRepository
{
    ValueTask<ObjectiveReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ObjectiveReportingPage> QueryAsync(ObjectiveReportingQuery query, CancellationToken cancellationToken);
    ValueTask<ObjectiveReportingMutation> SaveAsync(ObjectiveReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IObjectiveReportingEventSink { ValueTask PublishAsync(ObjectiveReportingEvent domainEvent, CancellationToken cancellationToken); }