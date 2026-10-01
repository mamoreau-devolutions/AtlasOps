namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssetClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record AssetClassificationRecord(Guid Id, string Name, string Owner, AssetClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssetClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssetClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssetClassificationQuery(string? SearchText, AssetClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssetClassificationPage(IReadOnlyList<AssetClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssetClassificationMutation(bool Succeeded, string Code, string Message, AssetClassificationRecord? Record, AssetClassificationEvent? Event);
public interface IAssetClassificationRepository
{
    ValueTask<AssetClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssetClassificationPage> QueryAsync(AssetClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<AssetClassificationMutation> SaveAsync(AssetClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssetClassificationEventSink { ValueTask PublishAsync(AssetClassificationEvent domainEvent, CancellationToken cancellationToken); }