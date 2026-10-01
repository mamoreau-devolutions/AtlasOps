namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DashboardReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record DashboardReportingRecord(Guid Id, string Name, string Owner, DashboardReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DashboardReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DashboardReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DashboardReportingQuery(string? SearchText, DashboardReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DashboardReportingPage(IReadOnlyList<DashboardReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DashboardReportingMutation(bool Succeeded, string Code, string Message, DashboardReportingRecord? Record, DashboardReportingEvent? Event);
public interface IDashboardReportingRepository
{
    ValueTask<DashboardReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DashboardReportingPage> QueryAsync(DashboardReportingQuery query, CancellationToken cancellationToken);
    ValueTask<DashboardReportingMutation> SaveAsync(DashboardReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDashboardReportingEventSink { ValueTask PublishAsync(DashboardReportingEvent domainEvent, CancellationToken cancellationToken); }