namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ControlApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record ControlApprovalRecord(Guid Id, string Name, string Owner, ControlApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ControlApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ControlApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ControlApprovalQuery(string? SearchText, ControlApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ControlApprovalPage(IReadOnlyList<ControlApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ControlApprovalMutation(bool Succeeded, string Code, string Message, ControlApprovalRecord? Record, ControlApprovalEvent? Event);
public interface IControlApprovalRepository
{
    ValueTask<ControlApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ControlApprovalPage> QueryAsync(ControlApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<ControlApprovalMutation> SaveAsync(ControlApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IControlApprovalEventSink { ValueTask PublishAsync(ControlApprovalEvent domainEvent, CancellationToken cancellationToken); }