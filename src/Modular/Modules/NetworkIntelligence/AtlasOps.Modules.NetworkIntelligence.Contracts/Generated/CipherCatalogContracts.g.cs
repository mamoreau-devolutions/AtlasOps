namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CipherCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record CipherCatalogRecord(Guid Id, string Name, string Owner, CipherCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CipherCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CipherCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CipherCatalogQuery(string? SearchText, CipherCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CipherCatalogPage(IReadOnlyList<CipherCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CipherCatalogMutation(bool Succeeded, string Code, string Message, CipherCatalogRecord? Record, CipherCatalogEvent? Event);
public interface ICipherCatalogRepository
{
    ValueTask<CipherCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CipherCatalogPage> QueryAsync(CipherCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<CipherCatalogMutation> SaveAsync(CipherCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICipherCatalogEventSink { ValueTask PublishAsync(CipherCatalogEvent domainEvent, CancellationToken cancellationToken); }