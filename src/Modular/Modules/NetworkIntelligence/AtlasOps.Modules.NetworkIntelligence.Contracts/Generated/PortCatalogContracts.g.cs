namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PortCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record PortCatalogRecord(Guid Id, string Name, string Owner, PortCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PortCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PortCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PortCatalogQuery(string? SearchText, PortCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PortCatalogPage(IReadOnlyList<PortCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PortCatalogMutation(bool Succeeded, string Code, string Message, PortCatalogRecord? Record, PortCatalogEvent? Event);
public interface IPortCatalogRepository
{
    ValueTask<PortCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PortCatalogPage> QueryAsync(PortCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<PortCatalogMutation> SaveAsync(PortCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPortCatalogEventSink { ValueTask PublishAsync(PortCatalogEvent domainEvent, CancellationToken cancellationToken); }