namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RolloutAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record RolloutAuditRecord(Guid Id, string Name, string Owner, RolloutAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RolloutAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RolloutAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RolloutAuditQuery(string? SearchText, RolloutAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RolloutAuditPage(IReadOnlyList<RolloutAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RolloutAuditMutation(bool Succeeded, string Code, string Message, RolloutAuditRecord? Record, RolloutAuditEvent? Event);
public interface IRolloutAuditRepository
{
    ValueTask<RolloutAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RolloutAuditPage> QueryAsync(RolloutAuditQuery query, CancellationToken cancellationToken);
    ValueTask<RolloutAuditMutation> SaveAsync(RolloutAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRolloutAuditEventSink { ValueTask PublishAsync(RolloutAuditEvent domainEvent, CancellationToken cancellationToken); }