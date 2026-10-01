namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProfileAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record ProfileAuditRecord(Guid Id, string Name, string Owner, ProfileAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProfileAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProfileAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProfileAuditQuery(string? SearchText, ProfileAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProfileAuditPage(IReadOnlyList<ProfileAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProfileAuditMutation(bool Succeeded, string Code, string Message, ProfileAuditRecord? Record, ProfileAuditEvent? Event);
public interface IProfileAuditRepository
{
    ValueTask<ProfileAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProfileAuditPage> QueryAsync(ProfileAuditQuery query, CancellationToken cancellationToken);
    ValueTask<ProfileAuditMutation> SaveAsync(ProfileAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProfileAuditEventSink { ValueTask PublishAsync(ProfileAuditEvent domainEvent, CancellationToken cancellationToken); }