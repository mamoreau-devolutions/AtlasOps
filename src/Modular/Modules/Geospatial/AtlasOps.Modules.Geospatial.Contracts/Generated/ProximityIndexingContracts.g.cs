namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProximityIndexingState { Draft, Active, Paused, Completed, Archived }
public sealed record ProximityIndexingRecord(Guid Id, string Name, string Owner, ProximityIndexingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProximityIndexingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProximityIndexingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProximityIndexingQuery(string? SearchText, ProximityIndexingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProximityIndexingPage(IReadOnlyList<ProximityIndexingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProximityIndexingMutation(bool Succeeded, string Code, string Message, ProximityIndexingRecord? Record, ProximityIndexingEvent? Event);
public interface IProximityIndexingRepository
{
    ValueTask<ProximityIndexingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProximityIndexingPage> QueryAsync(ProximityIndexingQuery query, CancellationToken cancellationToken);
    ValueTask<ProximityIndexingMutation> SaveAsync(ProximityIndexingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProximityIndexingEventSink { ValueTask PublishAsync(ProximityIndexingEvent domainEvent, CancellationToken cancellationToken); }