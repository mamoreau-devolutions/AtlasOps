namespace AtlasOps.Modules.Lifecycle.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProductReportingState { Draft, Active, Paused, Completed, Archived }
public sealed record ProductReportingRecord(Guid Id, string Name, string Owner, ProductReportingState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProductReportingCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProductReportingEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProductReportingQuery(string? SearchText, ProductReportingState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProductReportingPage(IReadOnlyList<ProductReportingRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProductReportingMutation(bool Succeeded, string Code, string Message, ProductReportingRecord? Record, ProductReportingEvent? Event);
public interface IProductReportingRepository
{
    ValueTask<ProductReportingRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProductReportingPage> QueryAsync(ProductReportingQuery query, CancellationToken cancellationToken);
    ValueTask<ProductReportingMutation> SaveAsync(ProductReportingRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProductReportingEventSink { ValueTask PublishAsync(ProductReportingEvent domainEvent, CancellationToken cancellationToken); }