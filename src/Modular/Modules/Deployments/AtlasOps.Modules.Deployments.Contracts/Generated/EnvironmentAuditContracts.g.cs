namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EnvironmentAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record EnvironmentAuditRecord(Guid Id, string Name, string Owner, EnvironmentAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EnvironmentAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EnvironmentAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EnvironmentAuditQuery(string? SearchText, EnvironmentAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EnvironmentAuditPage(IReadOnlyList<EnvironmentAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EnvironmentAuditMutation(bool Succeeded, string Code, string Message, EnvironmentAuditRecord? Record, EnvironmentAuditEvent? Event);
public interface IEnvironmentAuditRepository
{
    ValueTask<EnvironmentAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EnvironmentAuditPage> QueryAsync(EnvironmentAuditQuery query, CancellationToken cancellationToken);
    ValueTask<EnvironmentAuditMutation> SaveAsync(EnvironmentAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEnvironmentAuditEventSink { ValueTask PublishAsync(EnvironmentAuditEvent domainEvent, CancellationToken cancellationToken); }