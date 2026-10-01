namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ServiceReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record ServiceReportingRecord(Guid Id, string Name, string Owner, ServiceReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ServiceReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ServiceReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ServiceReportingQuery(string? SearchText, ServiceReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ServiceReportingPage(IReadOnlyList<ServiceReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ServiceReportingMutation(bool Succeeded, string Code, string Message, ServiceReportingRecord? Record, ServiceReportingEvent? Event);
public interface IServiceReportingRepository
{
    ValueTask<ServiceReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ServiceReportingPage> QueryAsync(ServiceReportingQuery query, CancellationToken cancellationToken);
    ValueTask<ServiceReportingMutation> SaveAsync(ServiceReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IServiceReportingEventSink { ValueTask PublishAsync(ServiceReportingEvent domainEvent, CancellationToken cancellationToken); }