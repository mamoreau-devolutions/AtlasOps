namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ObligationCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record ObligationCatalogRecord(Guid Id, string Name, string Owner, ObligationCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ObligationCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ObligationCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ObligationCatalogQuery(string? SearchText, ObligationCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ObligationCatalogPage(IReadOnlyList<ObligationCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ObligationCatalogMutation(bool Succeeded, string Code, string Message, ObligationCatalogRecord? Record, ObligationCatalogEvent? Event);
public interface IObligationCatalogRepository
{
    ValueTask<ObligationCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ObligationCatalogPage> QueryAsync(ObligationCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<ObligationCatalogMutation> SaveAsync(ObligationCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IObligationCatalogEventSink { ValueTask PublishAsync(ObligationCatalogEvent domainEvent, CancellationToken cancellationToken); }