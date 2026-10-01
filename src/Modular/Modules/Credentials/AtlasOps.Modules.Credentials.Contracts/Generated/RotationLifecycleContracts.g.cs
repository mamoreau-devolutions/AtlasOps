namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RotationLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record RotationLifecycleRecord(Guid Id, string Name, string Owner, RotationLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RotationLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RotationLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RotationLifecycleQuery(string? SearchText, RotationLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RotationLifecyclePage(IReadOnlyList<RotationLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RotationLifecycleMutation(bool Succeeded, string Code, string Message, RotationLifecycleRecord? Record, RotationLifecycleEvent? Event);
public interface IRotationLifecycleRepository
{
    ValueTask<RotationLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RotationLifecyclePage> QueryAsync(RotationLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<RotationLifecycleMutation> SaveAsync(RotationLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRotationLifecycleEventSink { ValueTask PublishAsync(RotationLifecycleEvent domainEvent, CancellationToken cancellationToken); }