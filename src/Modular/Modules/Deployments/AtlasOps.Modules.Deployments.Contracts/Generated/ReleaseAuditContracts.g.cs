namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ReleaseAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record ReleaseAuditRecord(Guid Id, string Name, string Owner, ReleaseAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ReleaseAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ReleaseAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ReleaseAuditQuery(string? SearchText, ReleaseAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ReleaseAuditPage(IReadOnlyList<ReleaseAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ReleaseAuditMutation(bool Succeeded, string Code, string Message, ReleaseAuditRecord? Record, ReleaseAuditEvent? Event);
public interface IReleaseAuditRepository
{
    ValueTask<ReleaseAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ReleaseAuditPage> QueryAsync(ReleaseAuditQuery query, CancellationToken cancellationToken);
    ValueTask<ReleaseAuditMutation> SaveAsync(ReleaseAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IReleaseAuditEventSink { ValueTask PublishAsync(ReleaseAuditEvent domainEvent, CancellationToken cancellationToken); }