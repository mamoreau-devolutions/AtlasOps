namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssetReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record AssetReportingRecord(Guid Id, string Name, string Owner, AssetReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssetReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssetReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssetReportingQuery(string? SearchText, AssetReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssetReportingPage(IReadOnlyList<AssetReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssetReportingMutation(bool Succeeded, string Code, string Message, AssetReportingRecord? Record, AssetReportingEvent? Event);
public interface IAssetReportingRepository
{
    ValueTask<AssetReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssetReportingPage> QueryAsync(AssetReportingQuery query, CancellationToken cancellationToken);
    ValueTask<AssetReportingMutation> SaveAsync(AssetReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssetReportingEventSink { ValueTask PublishAsync(AssetReportingEvent domainEvent, CancellationToken cancellationToken); }