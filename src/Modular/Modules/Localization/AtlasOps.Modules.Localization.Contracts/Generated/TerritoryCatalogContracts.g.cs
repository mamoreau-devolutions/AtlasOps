namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TerritoryCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record TerritoryCatalogRecord(Guid Id, string Name, string Owner, TerritoryCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TerritoryCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TerritoryCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TerritoryCatalogQuery(string? SearchText, TerritoryCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TerritoryCatalogPage(IReadOnlyList<TerritoryCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TerritoryCatalogMutation(bool Succeeded, string Code, string Message, TerritoryCatalogRecord? Record, TerritoryCatalogEvent? Event);
public interface ITerritoryCatalogRepository
{
    ValueTask<TerritoryCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TerritoryCatalogPage> QueryAsync(TerritoryCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<TerritoryCatalogMutation> SaveAsync(TerritoryCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITerritoryCatalogEventSink { ValueTask PublishAsync(TerritoryCatalogEvent domainEvent, CancellationToken cancellationToken); }