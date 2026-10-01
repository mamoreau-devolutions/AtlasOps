namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssetInventoryState { Draft, Active, Paused, Completed, Archived }
public sealed record AssetInventoryRecord(Guid Id, string Name, string Owner, AssetInventoryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssetInventoryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssetInventoryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssetInventoryQuery(string? SearchText, AssetInventoryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssetInventoryPage(IReadOnlyList<AssetInventoryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssetInventoryMutation(bool Succeeded, string Code, string Message, AssetInventoryRecord? Record, AssetInventoryEvent? Event);
public interface IAssetInventoryRepository
{
    ValueTask<AssetInventoryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssetInventoryPage> QueryAsync(AssetInventoryQuery query, CancellationToken cancellationToken);
    ValueTask<AssetInventoryMutation> SaveAsync(AssetInventoryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssetInventoryEventSink { ValueTask PublishAsync(AssetInventoryEvent domainEvent, CancellationToken cancellationToken); }