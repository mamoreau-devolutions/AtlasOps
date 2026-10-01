namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PriceReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record PriceReportingRecord(Guid Id, string Name, string Owner, PriceReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PriceReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PriceReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PriceReportingQuery(string? SearchText, PriceReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PriceReportingPage(IReadOnlyList<PriceReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PriceReportingMutation(bool Succeeded, string Code, string Message, PriceReportingRecord? Record, PriceReportingEvent? Event);
public interface IPriceReportingRepository
{
    ValueTask<PriceReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PriceReportingPage> QueryAsync(PriceReportingQuery query, CancellationToken cancellationToken);
    ValueTask<PriceReportingMutation> SaveAsync(PriceReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPriceReportingEventSink { ValueTask PublishAsync(PriceReportingEvent domainEvent, CancellationToken cancellationToken); }