namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RotationRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record RotationRecoveryRecord(Guid Id, string Name, string Owner, RotationRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RotationRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RotationRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RotationRecoveryQuery(string? SearchText, RotationRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RotationRecoveryPage(IReadOnlyList<RotationRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RotationRecoveryMutation(bool Succeeded, string Code, string Message, RotationRecoveryRecord? Record, RotationRecoveryEvent? Event);
public interface IRotationRecoveryRepository
{
    ValueTask<RotationRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RotationRecoveryPage> QueryAsync(RotationRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<RotationRecoveryMutation> SaveAsync(RotationRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRotationRecoveryEventSink { ValueTask PublishAsync(RotationRecoveryEvent domainEvent, CancellationToken cancellationToken); }