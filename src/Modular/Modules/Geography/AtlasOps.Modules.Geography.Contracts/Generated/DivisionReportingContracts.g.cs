namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DivisionReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record DivisionReportingRecord(Guid Id, string Name, string Owner, DivisionReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DivisionReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DivisionReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DivisionReportingQuery(string? SearchText, DivisionReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DivisionReportingPage(IReadOnlyList<DivisionReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DivisionReportingMutation(bool Succeeded, string Code, string Message, DivisionReportingRecord? Record, DivisionReportingEvent? Event);
public interface IDivisionReportingRepository
{
    ValueTask<DivisionReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DivisionReportingPage> QueryAsync(DivisionReportingQuery query, CancellationToken cancellationToken);
    ValueTask<DivisionReportingMutation> SaveAsync(DivisionReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDivisionReportingEventSink { ValueTask PublishAsync(DivisionReportingEvent domainEvent, CancellationToken cancellationToken); }