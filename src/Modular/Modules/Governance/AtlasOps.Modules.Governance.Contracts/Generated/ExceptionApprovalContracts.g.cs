namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ExceptionApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record ExceptionApprovalRecord(Guid Id, string Name, string Owner, ExceptionApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ExceptionApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ExceptionApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ExceptionApprovalQuery(string? SearchText, ExceptionApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ExceptionApprovalPage(IReadOnlyList<ExceptionApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ExceptionApprovalMutation(bool Succeeded, string Code, string Message, ExceptionApprovalRecord? Record, ExceptionApprovalEvent? Event);
public interface IExceptionApprovalRepository
{
    ValueTask<ExceptionApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ExceptionApprovalPage> QueryAsync(ExceptionApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<ExceptionApprovalMutation> SaveAsync(ExceptionApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IExceptionApprovalEventSink { ValueTask PublishAsync(ExceptionApprovalEvent domainEvent, CancellationToken cancellationToken); }