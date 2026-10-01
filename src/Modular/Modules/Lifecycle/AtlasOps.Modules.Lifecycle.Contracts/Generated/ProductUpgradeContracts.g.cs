namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProductUpgradeState { Draft, Active, Paused, Completed, Archived }
public sealed record ProductUpgradeRecord(Guid Id, string Name, string Owner, ProductUpgradeState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProductUpgradeCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProductUpgradeEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProductUpgradeQuery(string? SearchText, ProductUpgradeState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProductUpgradePage(IReadOnlyList<ProductUpgradeRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProductUpgradeMutation(bool Succeeded, string Code, string Message, ProductUpgradeRecord? Record, ProductUpgradeEvent? Event);
public interface IProductUpgradeRepository
{
    ValueTask<ProductUpgradeRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProductUpgradePage> QueryAsync(ProductUpgradeQuery query, CancellationToken cancellationToken);
    ValueTask<ProductUpgradeMutation> SaveAsync(ProductUpgradeRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProductUpgradeEventSink { ValueTask PublishAsync(ProductUpgradeEvent domainEvent, CancellationToken cancellationToken); }