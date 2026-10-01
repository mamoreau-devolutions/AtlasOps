namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ComponentCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record ComponentCatalogRecord(Guid Id, string Name, string Owner, ComponentCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ComponentCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ComponentCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ComponentCatalogQuery(string? SearchText, ComponentCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ComponentCatalogPage(IReadOnlyList<ComponentCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ComponentCatalogMutation(bool Succeeded, string Code, string Message, ComponentCatalogRecord? Record, ComponentCatalogEvent? Event);
public interface IComponentCatalogRepository
{
    ValueTask<ComponentCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ComponentCatalogPage> QueryAsync(ComponentCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<ComponentCatalogMutation> SaveAsync(ComponentCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IComponentCatalogEventSink { ValueTask PublishAsync(ComponentCatalogEvent domainEvent, CancellationToken cancellationToken); }