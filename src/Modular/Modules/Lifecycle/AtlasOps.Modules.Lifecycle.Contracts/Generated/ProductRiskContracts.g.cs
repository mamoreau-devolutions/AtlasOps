namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProductRiskState { Draft, Active, Paused, Completed, Archived }
public sealed record ProductRiskRecord(Guid Id, string Name, string Owner, ProductRiskState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProductRiskCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProductRiskEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProductRiskQuery(string? SearchText, ProductRiskState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProductRiskPage(IReadOnlyList<ProductRiskRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProductRiskMutation(bool Succeeded, string Code, string Message, ProductRiskRecord? Record, ProductRiskEvent? Event);
public interface IProductRiskRepository
{
    ValueTask<ProductRiskRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProductRiskPage> QueryAsync(ProductRiskQuery query, CancellationToken cancellationToken);
    ValueTask<ProductRiskMutation> SaveAsync(ProductRiskRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProductRiskEventSink { ValueTask PublishAsync(ProductRiskEvent domainEvent, CancellationToken cancellationToken); }