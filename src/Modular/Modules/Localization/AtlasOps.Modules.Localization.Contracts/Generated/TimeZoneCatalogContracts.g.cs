namespace AtlasOps.Modules.Localization.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum TimeZoneCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record TimeZoneCatalogRecord(Guid Id, string Name, string Owner, TimeZoneCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record TimeZoneCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TimeZoneCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record TimeZoneCatalogQuery(string? SearchText, TimeZoneCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record TimeZoneCatalogPage(IReadOnlyList<TimeZoneCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record TimeZoneCatalogMutation(bool Succeeded, string Code, string Message, TimeZoneCatalogRecord? Record, TimeZoneCatalogEvent? Event);
public interface ITimeZoneCatalogRepository
{
    ValueTask<TimeZoneCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<TimeZoneCatalogPage> QueryAsync(TimeZoneCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<TimeZoneCatalogMutation> SaveAsync(TimeZoneCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ITimeZoneCatalogEventSink { ValueTask PublishAsync(TimeZoneCatalogEvent domainEvent, CancellationToken cancellationToken); }