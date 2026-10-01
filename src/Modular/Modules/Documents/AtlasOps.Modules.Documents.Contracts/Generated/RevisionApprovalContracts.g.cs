namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RevisionApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record RevisionApprovalRecord(Guid Id, string Name, string Owner, RevisionApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RevisionApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RevisionApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RevisionApprovalQuery(string? SearchText, RevisionApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RevisionApprovalPage(IReadOnlyList<RevisionApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RevisionApprovalMutation(bool Succeeded, string Code, string Message, RevisionApprovalRecord? Record, RevisionApprovalEvent? Event);
public interface IRevisionApprovalRepository
{
    ValueTask<RevisionApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RevisionApprovalPage> QueryAsync(RevisionApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<RevisionApprovalMutation> SaveAsync(RevisionApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRevisionApprovalEventSink { ValueTask PublishAsync(RevisionApprovalEvent domainEvent, CancellationToken cancellationToken); }