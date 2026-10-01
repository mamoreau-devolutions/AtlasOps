namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProtocolCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record ProtocolCatalogRecord(Guid Id, string Name, string Owner, ProtocolCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProtocolCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProtocolCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProtocolCatalogQuery(string? SearchText, ProtocolCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProtocolCatalogPage(IReadOnlyList<ProtocolCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProtocolCatalogMutation(bool Succeeded, string Code, string Message, ProtocolCatalogRecord? Record, ProtocolCatalogEvent? Event);
public interface IProtocolCatalogRepository
{
    ValueTask<ProtocolCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProtocolCatalogPage> QueryAsync(ProtocolCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<ProtocolCatalogMutation> SaveAsync(ProtocolCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProtocolCatalogEventSink { ValueTask PublishAsync(ProtocolCatalogEvent domainEvent, CancellationToken cancellationToken); }