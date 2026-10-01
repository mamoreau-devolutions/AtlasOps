namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum OfferingSizingState { Draft, Active, Paused, Completed, Archived }
public sealed record OfferingSizingRecord(Guid Id, string Name, string Owner, OfferingSizingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record OfferingSizingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record OfferingSizingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record OfferingSizingQuery(string? SearchText, OfferingSizingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record OfferingSizingPage(IReadOnlyList<OfferingSizingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record OfferingSizingMutation(bool Succeeded, string Code, string Message, OfferingSizingRecord? Record, OfferingSizingEvent? Event);
public interface IOfferingSizingRepository
{
    ValueTask<OfferingSizingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<OfferingSizingPage> QueryAsync(OfferingSizingQuery query, CancellationToken cancellationToken);
    ValueTask<OfferingSizingMutation> SaveAsync(OfferingSizingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IOfferingSizingEventSink { ValueTask PublishAsync(OfferingSizingEvent domainEvent, CancellationToken cancellationToken); }