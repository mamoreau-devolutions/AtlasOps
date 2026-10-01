namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum OfferingReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record OfferingReportingRecord(Guid Id, string Name, string Owner, OfferingReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record OfferingReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record OfferingReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record OfferingReportingQuery(string? SearchText, OfferingReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record OfferingReportingPage(IReadOnlyList<OfferingReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record OfferingReportingMutation(bool Succeeded, string Code, string Message, OfferingReportingRecord? Record, OfferingReportingEvent? Event);
public interface IOfferingReportingRepository
{
    ValueTask<OfferingReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<OfferingReportingPage> QueryAsync(OfferingReportingQuery query, CancellationToken cancellationToken);
    ValueTask<OfferingReportingMutation> SaveAsync(OfferingReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IOfferingReportingEventSink { ValueTask PublishAsync(OfferingReportingEvent domainEvent, CancellationToken cancellationToken); }