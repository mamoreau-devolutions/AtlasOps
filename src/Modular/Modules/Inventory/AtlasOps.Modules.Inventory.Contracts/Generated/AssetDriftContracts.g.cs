namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssetDriftState { Draft, Active, Paused, Completed, Archived }
public sealed record AssetDriftRecord(Guid Id, string Name, string Owner, AssetDriftState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssetDriftCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssetDriftEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssetDriftQuery(string? SearchText, AssetDriftState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssetDriftPage(IReadOnlyList<AssetDriftRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssetDriftMutation(bool Succeeded, string Code, string Message, AssetDriftRecord? Record, AssetDriftEvent? Event);
public interface IAssetDriftRepository
{
    ValueTask<AssetDriftRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssetDriftPage> QueryAsync(AssetDriftQuery query, CancellationToken cancellationToken);
    ValueTask<AssetDriftMutation> SaveAsync(AssetDriftRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssetDriftEventSink { ValueTask PublishAsync(AssetDriftEvent domainEvent, CancellationToken cancellationToken); }