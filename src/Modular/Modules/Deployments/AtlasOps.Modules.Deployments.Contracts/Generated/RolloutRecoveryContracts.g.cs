namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RolloutRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record RolloutRecoveryRecord(Guid Id, string Name, string Owner, RolloutRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RolloutRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RolloutRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RolloutRecoveryQuery(string? SearchText, RolloutRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RolloutRecoveryPage(IReadOnlyList<RolloutRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RolloutRecoveryMutation(bool Succeeded, string Code, string Message, RolloutRecoveryRecord? Record, RolloutRecoveryEvent? Event);
public interface IRolloutRecoveryRepository
{
    ValueTask<RolloutRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RolloutRecoveryPage> QueryAsync(RolloutRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<RolloutRecoveryMutation> SaveAsync(RolloutRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRolloutRecoveryEventSink { ValueTask PublishAsync(RolloutRecoveryEvent domainEvent, CancellationToken cancellationToken); }