namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PolicyAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record PolicyAuditRecord(Guid Id, string Name, string Owner, PolicyAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PolicyAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PolicyAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PolicyAuditQuery(string? SearchText, PolicyAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PolicyAuditPage(IReadOnlyList<PolicyAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PolicyAuditMutation(bool Succeeded, string Code, string Message, PolicyAuditRecord? Record, PolicyAuditEvent? Event);
public interface IPolicyAuditRepository
{
    ValueTask<PolicyAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PolicyAuditPage> QueryAsync(PolicyAuditQuery query, CancellationToken cancellationToken);
    ValueTask<PolicyAuditMutation> SaveAsync(PolicyAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPolicyAuditEventSink { ValueTask PublishAsync(PolicyAuditEvent domainEvent, CancellationToken cancellationToken); }