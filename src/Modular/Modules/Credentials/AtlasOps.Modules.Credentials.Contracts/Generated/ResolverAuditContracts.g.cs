namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ResolverAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record ResolverAuditRecord(Guid Id, string Name, string Owner, ResolverAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ResolverAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ResolverAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ResolverAuditQuery(string? SearchText, ResolverAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ResolverAuditPage(IReadOnlyList<ResolverAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ResolverAuditMutation(bool Succeeded, string Code, string Message, ResolverAuditRecord? Record, ResolverAuditEvent? Event);
public interface IResolverAuditRepository
{
    ValueTask<ResolverAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ResolverAuditPage> QueryAsync(ResolverAuditQuery query, CancellationToken cancellationToken);
    ValueTask<ResolverAuditMutation> SaveAsync(ResolverAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IResolverAuditEventSink { ValueTask PublishAsync(ResolverAuditEvent domainEvent, CancellationToken cancellationToken); }