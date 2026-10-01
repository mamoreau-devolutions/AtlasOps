namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ReleaseRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record ReleaseRecoveryRecord(Guid Id, string Name, string Owner, ReleaseRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ReleaseRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ReleaseRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ReleaseRecoveryQuery(string? SearchText, ReleaseRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ReleaseRecoveryPage(IReadOnlyList<ReleaseRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ReleaseRecoveryMutation(bool Succeeded, string Code, string Message, ReleaseRecoveryRecord? Record, ReleaseRecoveryEvent? Event);
public interface IReleaseRecoveryRepository
{
    ValueTask<ReleaseRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ReleaseRecoveryPage> QueryAsync(ReleaseRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<ReleaseRecoveryMutation> SaveAsync(ReleaseRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IReleaseRecoveryEventSink { ValueTask PublishAsync(ReleaseRecoveryEvent domainEvent, CancellationToken cancellationToken); }