namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CurrencyMappingState { Draft, Active, Paused, Completed, Archived }
public sealed record CurrencyMappingRecord(Guid Id, string Name, string Owner, CurrencyMappingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CurrencyMappingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CurrencyMappingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CurrencyMappingQuery(string? SearchText, CurrencyMappingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CurrencyMappingPage(IReadOnlyList<CurrencyMappingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CurrencyMappingMutation(bool Succeeded, string Code, string Message, CurrencyMappingRecord? Record, CurrencyMappingEvent? Event);
public interface ICurrencyMappingRepository
{
    ValueTask<CurrencyMappingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CurrencyMappingPage> QueryAsync(CurrencyMappingQuery query, CancellationToken cancellationToken);
    ValueTask<CurrencyMappingMutation> SaveAsync(CurrencyMappingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICurrencyMappingEventSink { ValueTask PublishAsync(CurrencyMappingEvent domainEvent, CancellationToken cancellationToken); }