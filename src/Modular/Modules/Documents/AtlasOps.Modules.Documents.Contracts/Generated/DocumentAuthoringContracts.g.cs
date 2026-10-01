namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DocumentAuthoringState { Draft, Active, Paused, Completed, Archived }
public sealed record DocumentAuthoringRecord(Guid Id, string Name, string Owner, DocumentAuthoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DocumentAuthoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DocumentAuthoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DocumentAuthoringQuery(string? SearchText, DocumentAuthoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DocumentAuthoringPage(IReadOnlyList<DocumentAuthoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DocumentAuthoringMutation(bool Succeeded, string Code, string Message, DocumentAuthoringRecord? Record, DocumentAuthoringEvent? Event);
public interface IDocumentAuthoringRepository
{
    ValueTask<DocumentAuthoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DocumentAuthoringPage> QueryAsync(DocumentAuthoringQuery query, CancellationToken cancellationToken);
    ValueTask<DocumentAuthoringMutation> SaveAsync(DocumentAuthoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDocumentAuthoringEventSink { ValueTask PublishAsync(DocumentAuthoringEvent domainEvent, CancellationToken cancellationToken); }