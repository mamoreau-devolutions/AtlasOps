namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EnvironmentRecoveryState { Draft, Active, Paused, Completed, Archived }
public sealed record EnvironmentRecoveryRecord(Guid Id, string Name, string Owner, EnvironmentRecoveryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EnvironmentRecoveryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EnvironmentRecoveryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EnvironmentRecoveryQuery(string? SearchText, EnvironmentRecoveryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EnvironmentRecoveryPage(IReadOnlyList<EnvironmentRecoveryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EnvironmentRecoveryMutation(bool Succeeded, string Code, string Message, EnvironmentRecoveryRecord? Record, EnvironmentRecoveryEvent? Event);
public interface IEnvironmentRecoveryRepository
{
    ValueTask<EnvironmentRecoveryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EnvironmentRecoveryPage> QueryAsync(EnvironmentRecoveryQuery query, CancellationToken cancellationToken);
    ValueTask<EnvironmentRecoveryMutation> SaveAsync(EnvironmentRecoveryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEnvironmentRecoveryEventSink { ValueTask PublishAsync(EnvironmentRecoveryEvent domainEvent, CancellationToken cancellationToken); }