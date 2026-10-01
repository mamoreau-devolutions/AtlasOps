namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ObligationApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record ObligationApprovalRecord(Guid Id, string Name, string Owner, ObligationApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ObligationApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ObligationApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ObligationApprovalQuery(string? SearchText, ObligationApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ObligationApprovalPage(IReadOnlyList<ObligationApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ObligationApprovalMutation(bool Succeeded, string Code, string Message, ObligationApprovalRecord? Record, ObligationApprovalEvent? Event);
public interface IObligationApprovalRepository
{
    ValueTask<ObligationApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ObligationApprovalPage> QueryAsync(ObligationApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<ObligationApprovalMutation> SaveAsync(ObligationApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IObligationApprovalEventSink { ValueTask PublishAsync(ObligationApprovalEvent domainEvent, CancellationToken cancellationToken); }