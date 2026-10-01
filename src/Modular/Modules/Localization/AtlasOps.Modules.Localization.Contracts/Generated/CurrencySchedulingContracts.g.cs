namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CurrencySchedulingState { Draft, Active, Paused, Completed, Archived }
public sealed record CurrencySchedulingRecord(Guid Id, string Name, string Owner, CurrencySchedulingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CurrencySchedulingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CurrencySchedulingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CurrencySchedulingQuery(string? SearchText, CurrencySchedulingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CurrencySchedulingPage(IReadOnlyList<CurrencySchedulingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CurrencySchedulingMutation(bool Succeeded, string Code, string Message, CurrencySchedulingRecord? Record, CurrencySchedulingEvent? Event);
public interface ICurrencySchedulingRepository
{
    ValueTask<CurrencySchedulingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CurrencySchedulingPage> QueryAsync(CurrencySchedulingQuery query, CancellationToken cancellationToken);
    ValueTask<CurrencySchedulingMutation> SaveAsync(CurrencySchedulingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICurrencySchedulingEventSink { ValueTask PublishAsync(CurrencySchedulingEvent domainEvent, CancellationToken cancellationToken); }