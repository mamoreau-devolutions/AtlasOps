namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RollbackRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record RollbackRecoveryRecord(Guid Id, string Name, string Owner, RollbackRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RollbackRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RollbackRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RollbackRecoveryQuery(string? SearchText, RollbackRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RollbackRecoveryPage(IReadOnlyList<RollbackRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RollbackRecoveryMutation(bool Succeeded, string Code, string Message, RollbackRecoveryRecord? Record, RollbackRecoveryEvent? Event);
public interface IRollbackRecoveryRepository
{
    ValueTask<RollbackRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RollbackRecoveryPage> QueryAsync(RollbackRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<RollbackRecoveryMutation> SaveAsync(RollbackRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRollbackRecoveryEventSink { ValueTask PublishAsync(RollbackRecoveryEvent domainEvent, CancellationToken cancellationToken); }