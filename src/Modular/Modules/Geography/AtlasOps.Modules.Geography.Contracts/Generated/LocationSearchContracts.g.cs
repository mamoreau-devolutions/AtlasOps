namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LocationSearchState { Draft, Active, Paused, Completed, Archived }
public sealed record LocationSearchRecord(Guid Id, string Name, string Owner, LocationSearchState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LocationSearchCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LocationSearchEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LocationSearchQuery(string? SearchText, LocationSearchState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LocationSearchPage(IReadOnlyList<LocationSearchRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LocationSearchMutation(bool Succeeded, string Code, string Message, LocationSearchRecord? Record, LocationSearchEvent? Event);
public interface ILocationSearchRepository
{
    ValueTask<LocationSearchRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LocationSearchPage> QueryAsync(LocationSearchQuery query, CancellationToken cancellationToken);
    ValueTask<LocationSearchMutation> SaveAsync(LocationSearchRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILocationSearchEventSink { ValueTask PublishAsync(LocationSearchEvent domainEvent, CancellationToken cancellationToken); }