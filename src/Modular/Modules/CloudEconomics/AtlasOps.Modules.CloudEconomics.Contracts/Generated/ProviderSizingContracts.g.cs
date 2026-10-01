namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProviderSizingState { Draft, Active, Paused, Completed, Archived }
public sealed record ProviderSizingRecord(Guid Id, string Name, string Owner, ProviderSizingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProviderSizingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProviderSizingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProviderSizingQuery(string? SearchText, ProviderSizingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProviderSizingPage(IReadOnlyList<ProviderSizingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProviderSizingMutation(bool Succeeded, string Code, string Message, ProviderSizingRecord? Record, ProviderSizingEvent? Event);
public interface IProviderSizingRepository
{
    ValueTask<ProviderSizingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProviderSizingPage> QueryAsync(ProviderSizingQuery query, CancellationToken cancellationToken);
    ValueTask<ProviderSizingMutation> SaveAsync(ProviderSizingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProviderSizingEventSink { ValueTask PublishAsync(ProviderSizingEvent domainEvent, CancellationToken cancellationToken); }