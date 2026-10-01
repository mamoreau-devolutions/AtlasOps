namespace AtlasOps.Modules.Connections.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SessionRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record SessionRecoveryRecord(Guid Id, string Name, string Owner, SessionRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SessionRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SessionRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SessionRecoveryQuery(string? SearchText, SessionRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SessionRecoveryPage(IReadOnlyList<SessionRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SessionRecoveryMutation(bool Succeeded, string Code, string Message, SessionRecoveryRecord? Record, SessionRecoveryEvent? Event);
public interface ISessionRecoveryRepository
{
    ValueTask<SessionRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SessionRecoveryPage> QueryAsync(SessionRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<SessionRecoveryMutation> SaveAsync(SessionRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISessionRecoveryEventSink { ValueTask PublishAsync(SessionRecoveryEvent domainEvent, CancellationToken cancellationToken); }