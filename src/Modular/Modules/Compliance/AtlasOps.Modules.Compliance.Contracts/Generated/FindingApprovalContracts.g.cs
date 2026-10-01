namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum FindingApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record FindingApprovalRecord(Guid Id, string Name, string Owner, FindingApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record FindingApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record FindingApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record FindingApprovalQuery(string? SearchText, FindingApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record FindingApprovalPage(IReadOnlyList<FindingApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record FindingApprovalMutation(bool Succeeded, string Code, string Message, FindingApprovalRecord? Record, FindingApprovalEvent? Event);
public interface IFindingApprovalRepository
{
    ValueTask<FindingApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<FindingApprovalPage> QueryAsync(FindingApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<FindingApprovalMutation> SaveAsync(FindingApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IFindingApprovalEventSink { ValueTask PublishAsync(FindingApprovalEvent domainEvent, CancellationToken cancellationToken); }