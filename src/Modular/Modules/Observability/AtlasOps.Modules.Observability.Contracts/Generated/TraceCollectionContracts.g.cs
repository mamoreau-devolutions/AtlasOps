namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TraceCollectionState { Draft, Active, Paused, Completed, Archived }
public sealed record TraceCollectionRecord(Guid Id, string Name, string Owner, TraceCollectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TraceCollectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TraceCollectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TraceCollectionQuery(string? SearchText, TraceCollectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TraceCollectionPage(IReadOnlyList<TraceCollectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TraceCollectionMutation(bool Succeeded, string Code, string Message, TraceCollectionRecord? Record, TraceCollectionEvent? Event);
public interface ITraceCollectionRepository
{
    ValueTask<TraceCollectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TraceCollectionPage> QueryAsync(TraceCollectionQuery query, CancellationToken cancellationToken);
    ValueTask<TraceCollectionMutation> SaveAsync(TraceCollectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITraceCollectionEventSink { ValueTask PublishAsync(TraceCollectionEvent domainEvent, CancellationToken cancellationToken); }