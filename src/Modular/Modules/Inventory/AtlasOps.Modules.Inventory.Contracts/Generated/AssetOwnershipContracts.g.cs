namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssetOwnershipState { Draft, Active, Paused, Completed, Archived }
public sealed record AssetOwnershipRecord(Guid Id, string Name, string Owner, AssetOwnershipState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssetOwnershipCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssetOwnershipEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssetOwnershipQuery(string? SearchText, AssetOwnershipState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssetOwnershipPage(IReadOnlyList<AssetOwnershipRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssetOwnershipMutation(bool Succeeded, string Code, string Message, AssetOwnershipRecord? Record, AssetOwnershipEvent? Event);
public interface IAssetOwnershipRepository
{
    ValueTask<AssetOwnershipRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssetOwnershipPage> QueryAsync(AssetOwnershipQuery query, CancellationToken cancellationToken);
    ValueTask<AssetOwnershipMutation> SaveAsync(AssetOwnershipRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssetOwnershipEventSink { ValueTask PublishAsync(AssetOwnershipEvent domainEvent, CancellationToken cancellationToken); }