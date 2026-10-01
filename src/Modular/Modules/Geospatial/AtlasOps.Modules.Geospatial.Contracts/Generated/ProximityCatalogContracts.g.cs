namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProximityCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record ProximityCatalogRecord(Guid Id, string Name, string Owner, ProximityCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProximityCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProximityCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProximityCatalogQuery(string? SearchText, ProximityCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProximityCatalogPage(IReadOnlyList<ProximityCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProximityCatalogMutation(bool Succeeded, string Code, string Message, ProximityCatalogRecord? Record, ProximityCatalogEvent? Event);
public interface IProximityCatalogRepository
{
    ValueTask<ProximityCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProximityCatalogPage> QueryAsync(ProximityCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<ProximityCatalogMutation> SaveAsync(ProximityCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProximityCatalogEventSink { ValueTask PublishAsync(ProximityCatalogEvent domainEvent, CancellationToken cancellationToken); }