namespace AtlasOps.Modules.Inventory.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DependencyInventoryState { Draft, Active, Paused, Completed, Archived }
public sealed record DependencyInventoryRecord(Guid Id, string Name, string Owner, DependencyInventoryState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DependencyInventoryCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DependencyInventoryEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DependencyInventoryQuery(string? SearchText, DependencyInventoryState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DependencyInventoryPage(IReadOnlyList<DependencyInventoryRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DependencyInventoryMutation(bool Succeeded, string Code, string Message, DependencyInventoryRecord? Record, DependencyInventoryEvent? Event);
public interface IDependencyInventoryRepository
{
    ValueTask<DependencyInventoryRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DependencyInventoryPage> QueryAsync(DependencyInventoryQuery query, CancellationToken cancellationToken);
    ValueTask<DependencyInventoryMutation> SaveAsync(DependencyInventoryRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDependencyInventoryEventSink { ValueTask PublishAsync(DependencyInventoryEvent domainEvent, CancellationToken cancellationToken); }