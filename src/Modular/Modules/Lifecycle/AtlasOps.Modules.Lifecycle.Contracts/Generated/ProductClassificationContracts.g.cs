namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProductClassificationState { Draft, Active, Paused, Completed, Archived }
public sealed record ProductClassificationRecord(Guid Id, string Name, string Owner, ProductClassificationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProductClassificationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProductClassificationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProductClassificationQuery(string? SearchText, ProductClassificationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProductClassificationPage(IReadOnlyList<ProductClassificationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProductClassificationMutation(bool Succeeded, string Code, string Message, ProductClassificationRecord? Record, ProductClassificationEvent? Event);
public interface IProductClassificationRepository
{
    ValueTask<ProductClassificationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProductClassificationPage> QueryAsync(ProductClassificationQuery query, CancellationToken cancellationToken);
    ValueTask<ProductClassificationMutation> SaveAsync(ProductClassificationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProductClassificationEventSink { ValueTask PublishAsync(ProductClassificationEvent domainEvent, CancellationToken cancellationToken); }