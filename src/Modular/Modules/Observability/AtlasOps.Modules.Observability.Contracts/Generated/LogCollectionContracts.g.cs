namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LogCollectionState { Draft, Active, Paused, Completed, Archived }
public sealed record LogCollectionRecord(Guid Id, string Name, string Owner, LogCollectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LogCollectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LogCollectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LogCollectionQuery(string? SearchText, LogCollectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LogCollectionPage(IReadOnlyList<LogCollectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LogCollectionMutation(bool Succeeded, string Code, string Message, LogCollectionRecord? Record, LogCollectionEvent? Event);
public interface ILogCollectionRepository
{
    ValueTask<LogCollectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LogCollectionPage> QueryAsync(LogCollectionQuery query, CancellationToken cancellationToken);
    ValueTask<LogCollectionMutation> SaveAsync(LogCollectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILogCollectionEventSink { ValueTask PublishAsync(LogCollectionEvent domainEvent, CancellationToken cancellationToken); }