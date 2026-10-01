namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PriceSizingState { Draft, Active, Paused, Completed, Archived }
public sealed record PriceSizingRecord(Guid Id, string Name, string Owner, PriceSizingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PriceSizingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PriceSizingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PriceSizingQuery(string? SearchText, PriceSizingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PriceSizingPage(IReadOnlyList<PriceSizingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PriceSizingMutation(bool Succeeded, string Code, string Message, PriceSizingRecord? Record, PriceSizingEvent? Event);
public interface IPriceSizingRepository
{
    ValueTask<PriceSizingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PriceSizingPage> QueryAsync(PriceSizingQuery query, CancellationToken cancellationToken);
    ValueTask<PriceSizingMutation> SaveAsync(PriceSizingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPriceSizingEventSink { ValueTask PublishAsync(PriceSizingEvent domainEvent, CancellationToken cancellationToken); }