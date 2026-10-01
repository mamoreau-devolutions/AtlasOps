namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DiscoveryInventoryState { Draft, Active, Paused, Completed, Archived }
public sealed record DiscoveryInventoryRecord(Guid Id, string Name, string Owner, DiscoveryInventoryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DiscoveryInventoryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DiscoveryInventoryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DiscoveryInventoryQuery(string? SearchText, DiscoveryInventoryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DiscoveryInventoryPage(IReadOnlyList<DiscoveryInventoryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DiscoveryInventoryMutation(bool Succeeded, string Code, string Message, DiscoveryInventoryRecord? Record, DiscoveryInventoryEvent? Event);
public interface IDiscoveryInventoryRepository
{
    ValueTask<DiscoveryInventoryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DiscoveryInventoryPage> QueryAsync(DiscoveryInventoryQuery query, CancellationToken cancellationToken);
    ValueTask<DiscoveryInventoryMutation> SaveAsync(DiscoveryInventoryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDiscoveryInventoryEventSink { ValueTask PublishAsync(DiscoveryInventoryEvent domainEvent, CancellationToken cancellationToken); }