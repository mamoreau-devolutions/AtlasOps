namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DocumentPublishingState { Draft, Active, Paused, Completed, Archived }
public sealed record DocumentPublishingRecord(Guid Id, string Name, string Owner, DocumentPublishingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DocumentPublishingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DocumentPublishingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DocumentPublishingQuery(string? SearchText, DocumentPublishingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DocumentPublishingPage(IReadOnlyList<DocumentPublishingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DocumentPublishingMutation(bool Succeeded, string Code, string Message, DocumentPublishingRecord? Record, DocumentPublishingEvent? Event);
public interface IDocumentPublishingRepository
{
    ValueTask<DocumentPublishingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DocumentPublishingPage> QueryAsync(DocumentPublishingQuery query, CancellationToken cancellationToken);
    ValueTask<DocumentPublishingMutation> SaveAsync(DocumentPublishingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDocumentPublishingEventSink { ValueTask PublishAsync(DocumentPublishingEvent domainEvent, CancellationToken cancellationToken); }