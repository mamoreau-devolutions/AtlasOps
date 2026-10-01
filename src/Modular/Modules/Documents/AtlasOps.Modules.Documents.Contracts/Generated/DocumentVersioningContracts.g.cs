namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DocumentVersioningState { Draft, Active, Paused, Completed, Archived }
public sealed record DocumentVersioningRecord(Guid Id, string Name, string Owner, DocumentVersioningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DocumentVersioningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DocumentVersioningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DocumentVersioningQuery(string? SearchText, DocumentVersioningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DocumentVersioningPage(IReadOnlyList<DocumentVersioningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DocumentVersioningMutation(bool Succeeded, string Code, string Message, DocumentVersioningRecord? Record, DocumentVersioningEvent? Event);
public interface IDocumentVersioningRepository
{
    ValueTask<DocumentVersioningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DocumentVersioningPage> QueryAsync(DocumentVersioningQuery query, CancellationToken cancellationToken);
    ValueTask<DocumentVersioningMutation> SaveAsync(DocumentVersioningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDocumentVersioningEventSink { ValueTask PublishAsync(DocumentVersioningEvent domainEvent, CancellationToken cancellationToken); }