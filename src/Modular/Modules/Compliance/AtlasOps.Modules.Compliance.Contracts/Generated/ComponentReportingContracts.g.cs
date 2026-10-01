namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ComponentReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record ComponentReportingRecord(Guid Id, string Name, string Owner, ComponentReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ComponentReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ComponentReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ComponentReportingQuery(string? SearchText, ComponentReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ComponentReportingPage(IReadOnlyList<ComponentReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ComponentReportingMutation(bool Succeeded, string Code, string Message, ComponentReportingRecord? Record, ComponentReportingEvent? Event);
public interface IComponentReportingRepository
{
    ValueTask<ComponentReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ComponentReportingPage> QueryAsync(ComponentReportingQuery query, CancellationToken cancellationToken);
    ValueTask<ComponentReportingMutation> SaveAsync(ComponentReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IComponentReportingEventSink { ValueTask PublishAsync(ComponentReportingEvent domainEvent, CancellationToken cancellationToken); }