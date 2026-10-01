namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ComponentApprovalState { Draft, Active, Paused, Completed, Archived }
public sealed record ComponentApprovalRecord(Guid Id, string Name, string Owner, ComponentApprovalState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ComponentApprovalCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ComponentApprovalEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ComponentApprovalQuery(string? SearchText, ComponentApprovalState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ComponentApprovalPage(IReadOnlyList<ComponentApprovalRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ComponentApprovalMutation(bool Succeeded, string Code, string Message, ComponentApprovalRecord? Record, ComponentApprovalEvent? Event);
public interface IComponentApprovalRepository
{
    ValueTask<ComponentApprovalRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ComponentApprovalPage> QueryAsync(ComponentApprovalQuery query, CancellationToken cancellationToken);
    ValueTask<ComponentApprovalMutation> SaveAsync(ComponentApprovalRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IComponentApprovalEventSink { ValueTask PublishAsync(ComponentApprovalEvent domainEvent, CancellationToken cancellationToken); }