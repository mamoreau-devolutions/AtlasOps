namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RevisionIndexingState { Draft, Active, Paused, Completed, Archived }
public sealed record RevisionIndexingRecord(Guid Id, string Name, string Owner, RevisionIndexingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RevisionIndexingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RevisionIndexingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RevisionIndexingQuery(string? SearchText, RevisionIndexingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RevisionIndexingPage(IReadOnlyList<RevisionIndexingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RevisionIndexingMutation(bool Succeeded, string Code, string Message, RevisionIndexingRecord? Record, RevisionIndexingEvent? Event);
public interface IRevisionIndexingRepository
{
    ValueTask<RevisionIndexingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RevisionIndexingPage> QueryAsync(RevisionIndexingQuery query, CancellationToken cancellationToken);
    ValueTask<RevisionIndexingMutation> SaveAsync(RevisionIndexingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRevisionIndexingEventSink { ValueTask PublishAsync(RevisionIndexingEvent domainEvent, CancellationToken cancellationToken); }