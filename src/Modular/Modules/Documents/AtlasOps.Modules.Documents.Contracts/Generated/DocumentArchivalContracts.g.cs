namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DocumentArchivalState { Draft, Active, Paused, Completed, Archived }
public sealed record DocumentArchivalRecord(Guid Id, string Name, string Owner, DocumentArchivalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DocumentArchivalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DocumentArchivalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DocumentArchivalQuery(string? SearchText, DocumentArchivalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DocumentArchivalPage(IReadOnlyList<DocumentArchivalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DocumentArchivalMutation(bool Succeeded, string Code, string Message, DocumentArchivalRecord? Record, DocumentArchivalEvent? Event);
public interface IDocumentArchivalRepository
{
    ValueTask<DocumentArchivalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DocumentArchivalPage> QueryAsync(DocumentArchivalQuery query, CancellationToken cancellationToken);
    ValueTask<DocumentArchivalMutation> SaveAsync(DocumentArchivalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDocumentArchivalEventSink { ValueTask PublishAsync(DocumentArchivalEvent domainEvent, CancellationToken cancellationToken); }