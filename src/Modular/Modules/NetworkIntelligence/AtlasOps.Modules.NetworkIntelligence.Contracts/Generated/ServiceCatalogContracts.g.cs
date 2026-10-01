namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ServiceCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record ServiceCatalogRecord(Guid Id, string Name, string Owner, ServiceCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ServiceCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ServiceCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ServiceCatalogQuery(string? SearchText, ServiceCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ServiceCatalogPage(IReadOnlyList<ServiceCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ServiceCatalogMutation(bool Succeeded, string Code, string Message, ServiceCatalogRecord? Record, ServiceCatalogEvent? Event);
public interface IServiceCatalogRepository
{
    ValueTask<ServiceCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ServiceCatalogPage> QueryAsync(ServiceCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<ServiceCatalogMutation> SaveAsync(ServiceCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IServiceCatalogEventSink { ValueTask PublishAsync(ServiceCatalogEvent domainEvent, CancellationToken cancellationToken); }