namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProviderCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record ProviderCatalogRecord(Guid Id, string Name, string Owner, ProviderCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProviderCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProviderCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProviderCatalogQuery(string? SearchText, ProviderCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProviderCatalogPage(IReadOnlyList<ProviderCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProviderCatalogMutation(bool Succeeded, string Code, string Message, ProviderCatalogRecord? Record, ProviderCatalogEvent? Event);
public interface IProviderCatalogRepository
{
    ValueTask<ProviderCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProviderCatalogPage> QueryAsync(ProviderCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<ProviderCatalogMutation> SaveAsync(ProviderCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProviderCatalogEventSink { ValueTask PublishAsync(ProviderCatalogEvent domainEvent, CancellationToken cancellationToken); }