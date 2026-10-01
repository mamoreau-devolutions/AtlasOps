namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RevisionVersioningState { Draft, Active, Paused, Completed, Archived }
public sealed record RevisionVersioningRecord(Guid Id, string Name, string Owner, RevisionVersioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RevisionVersioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RevisionVersioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RevisionVersioningQuery(string? SearchText, RevisionVersioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RevisionVersioningPage(IReadOnlyList<RevisionVersioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RevisionVersioningMutation(bool Succeeded, string Code, string Message, RevisionVersioningRecord? Record, RevisionVersioningEvent? Event);
public interface IRevisionVersioningRepository
{
    ValueTask<RevisionVersioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RevisionVersioningPage> QueryAsync(RevisionVersioningQuery query, CancellationToken cancellationToken);
    ValueTask<RevisionVersioningMutation> SaveAsync(RevisionVersioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRevisionVersioningEventSink { ValueTask PublishAsync(RevisionVersioningEvent domainEvent, CancellationToken cancellationToken); }