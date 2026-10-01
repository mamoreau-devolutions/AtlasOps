namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LocationComparisonState { Draft, Active, Paused, Completed, Archived }
public sealed record LocationComparisonRecord(Guid Id, string Name, string Owner, LocationComparisonState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LocationComparisonCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LocationComparisonEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LocationComparisonQuery(string? SearchText, LocationComparisonState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LocationComparisonPage(IReadOnlyList<LocationComparisonRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LocationComparisonMutation(bool Succeeded, string Code, string Message, LocationComparisonRecord? Record, LocationComparisonEvent? Event);
public interface ILocationComparisonRepository
{
    ValueTask<LocationComparisonRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LocationComparisonPage> QueryAsync(LocationComparisonQuery query, CancellationToken cancellationToken);
    ValueTask<LocationComparisonMutation> SaveAsync(LocationComparisonRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILocationComparisonEventSink { ValueTask PublishAsync(LocationComparisonEvent domainEvent, CancellationToken cancellationToken); }