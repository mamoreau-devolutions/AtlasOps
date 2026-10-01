namespace AtlasOps.Modules.Identity.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TeamAuditState { Draft, Active, Paused, Completed, Archived }
public sealed record TeamAuditRecord(Guid Id, string Name, string Owner, TeamAuditState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TeamAuditCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TeamAuditEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TeamAuditQuery(string? SearchText, TeamAuditState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TeamAuditPage(IReadOnlyList<TeamAuditRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TeamAuditMutation(bool Succeeded, string Code, string Message, TeamAuditRecord? Record, TeamAuditEvent? Event);
public interface ITeamAuditRepository
{
    ValueTask<TeamAuditRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TeamAuditPage> QueryAsync(TeamAuditQuery query, CancellationToken cancellationToken);
    ValueTask<TeamAuditMutation> SaveAsync(TeamAuditRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITeamAuditEventSink { ValueTask PublishAsync(TeamAuditEvent domainEvent, CancellationToken cancellationToken); }