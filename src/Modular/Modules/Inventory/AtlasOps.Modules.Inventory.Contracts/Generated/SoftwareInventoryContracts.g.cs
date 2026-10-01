namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum SoftwareInventoryState { Draft, Active, Paused, Completed, Archived }
public sealed record SoftwareInventoryRecord(Guid Id, string Name, string Owner, SoftwareInventoryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record SoftwareInventoryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SoftwareInventoryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record SoftwareInventoryQuery(string? SearchText, SoftwareInventoryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record SoftwareInventoryPage(IReadOnlyList<SoftwareInventoryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record SoftwareInventoryMutation(bool Succeeded, string Code, string Message, SoftwareInventoryRecord? Record, SoftwareInventoryEvent? Event);
public interface ISoftwareInventoryRepository
{
    ValueTask<SoftwareInventoryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<SoftwareInventoryPage> QueryAsync(SoftwareInventoryQuery query, CancellationToken cancellationToken);
    ValueTask<SoftwareInventoryMutation> SaveAsync(SoftwareInventoryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ISoftwareInventoryEventSink { ValueTask PublishAsync(SoftwareInventoryEvent domainEvent, CancellationToken cancellationToken); }