namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum BoundaryReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record BoundaryReportingRecord(Guid Id, string Name, string Owner, BoundaryReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record BoundaryReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record BoundaryReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record BoundaryReportingQuery(string? SearchText, BoundaryReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record BoundaryReportingPage(IReadOnlyList<BoundaryReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record BoundaryReportingMutation(bool Succeeded, string Code, string Message, BoundaryReportingRecord? Record, BoundaryReportingEvent? Event);
public interface IBoundaryReportingRepository
{
    ValueTask<BoundaryReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<BoundaryReportingPage> QueryAsync(BoundaryReportingQuery query, CancellationToken cancellationToken);
    ValueTask<BoundaryReportingMutation> SaveAsync(BoundaryReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IBoundaryReportingEventSink { ValueTask PublishAsync(BoundaryReportingEvent domainEvent, CancellationToken cancellationToken); }