namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SoftwareLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record SoftwareLifecycleRecord(Guid Id, string Name, string Owner, SoftwareLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SoftwareLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SoftwareLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SoftwareLifecycleQuery(string? SearchText, SoftwareLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SoftwareLifecyclePage(IReadOnlyList<SoftwareLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SoftwareLifecycleMutation(bool Succeeded, string Code, string Message, SoftwareLifecycleRecord? Record, SoftwareLifecycleEvent? Event);
public interface ISoftwareLifecycleRepository
{
    ValueTask<SoftwareLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SoftwareLifecyclePage> QueryAsync(SoftwareLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<SoftwareLifecycleMutation> SaveAsync(SoftwareLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISoftwareLifecycleEventSink { ValueTask PublishAsync(SoftwareLifecycleEvent domainEvent, CancellationToken cancellationToken); }