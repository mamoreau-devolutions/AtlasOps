namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssetLifecycleState { Draft, Active, Paused, Completed, Archived }
public sealed record AssetLifecycleRecord(Guid Id, string Name, string Owner, AssetLifecycleState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssetLifecycleCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssetLifecycleEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssetLifecycleQuery(string? SearchText, AssetLifecycleState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssetLifecyclePage(IReadOnlyList<AssetLifecycleRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssetLifecycleMutation(bool Succeeded, string Code, string Message, AssetLifecycleRecord? Record, AssetLifecycleEvent? Event);
public interface IAssetLifecycleRepository
{
    ValueTask<AssetLifecycleRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssetLifecyclePage> QueryAsync(AssetLifecycleQuery query, CancellationToken cancellationToken);
    ValueTask<AssetLifecycleMutation> SaveAsync(AssetLifecycleRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssetLifecycleEventSink { ValueTask PublishAsync(AssetLifecycleEvent domainEvent, CancellationToken cancellationToken); }