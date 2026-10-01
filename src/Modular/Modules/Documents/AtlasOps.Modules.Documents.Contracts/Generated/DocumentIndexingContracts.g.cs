namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DocumentIndexingState { Draft, Active, Paused, Completed, Archived }
public sealed record DocumentIndexingRecord(Guid Id, string Name, string Owner, DocumentIndexingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DocumentIndexingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DocumentIndexingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DocumentIndexingQuery(string? SearchText, DocumentIndexingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DocumentIndexingPage(IReadOnlyList<DocumentIndexingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DocumentIndexingMutation(bool Succeeded, string Code, string Message, DocumentIndexingRecord? Record, DocumentIndexingEvent? Event);
public interface IDocumentIndexingRepository
{
    ValueTask<DocumentIndexingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DocumentIndexingPage> QueryAsync(DocumentIndexingQuery query, CancellationToken cancellationToken);
    ValueTask<DocumentIndexingMutation> SaveAsync(DocumentIndexingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDocumentIndexingEventSink { ValueTask PublishAsync(DocumentIndexingEvent domainEvent, CancellationToken cancellationToken); }