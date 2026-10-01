namespace AtlasOps.Modules.Documents.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LinkApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record LinkApprovalRecord(Guid Id, string Name, string Owner, LinkApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LinkApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LinkApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LinkApprovalQuery(string? SearchText, LinkApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LinkApprovalPage(IReadOnlyList<LinkApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LinkApprovalMutation(bool Succeeded, string Code, string Message, LinkApprovalRecord? Record, LinkApprovalEvent? Event);
public interface ILinkApprovalRepository
{
    ValueTask<LinkApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LinkApprovalPage> QueryAsync(LinkApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<LinkApprovalMutation> SaveAsync(LinkApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILinkApprovalEventSink { ValueTask PublishAsync(LinkApprovalEvent domainEvent, CancellationToken cancellationToken); }