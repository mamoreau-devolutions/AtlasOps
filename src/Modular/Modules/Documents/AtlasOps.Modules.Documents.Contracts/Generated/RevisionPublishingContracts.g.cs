namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RevisionPublishingState { Draft, Active, Paused, Completed, Archived }
public sealed record RevisionPublishingRecord(Guid Id, string Name, string Owner, RevisionPublishingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RevisionPublishingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RevisionPublishingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RevisionPublishingQuery(string? SearchText, RevisionPublishingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RevisionPublishingPage(IReadOnlyList<RevisionPublishingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RevisionPublishingMutation(bool Succeeded, string Code, string Message, RevisionPublishingRecord? Record, RevisionPublishingEvent? Event);
public interface IRevisionPublishingRepository
{
    ValueTask<RevisionPublishingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RevisionPublishingPage> QueryAsync(RevisionPublishingQuery query, CancellationToken cancellationToken);
    ValueTask<RevisionPublishingMutation> SaveAsync(RevisionPublishingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRevisionPublishingEventSink { ValueTask PublishAsync(RevisionPublishingEvent domainEvent, CancellationToken cancellationToken); }