namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DocumentApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record DocumentApprovalRecord(Guid Id, string Name, string Owner, DocumentApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DocumentApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DocumentApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DocumentApprovalQuery(string? SearchText, DocumentApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DocumentApprovalPage(IReadOnlyList<DocumentApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DocumentApprovalMutation(bool Succeeded, string Code, string Message, DocumentApprovalRecord? Record, DocumentApprovalEvent? Event);
public interface IDocumentApprovalRepository
{
    ValueTask<DocumentApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DocumentApprovalPage> QueryAsync(DocumentApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<DocumentApprovalMutation> SaveAsync(DocumentApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDocumentApprovalEventSink { ValueTask PublishAsync(DocumentApprovalEvent domainEvent, CancellationToken cancellationToken); }