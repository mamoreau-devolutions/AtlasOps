namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LicenseCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record LicenseCatalogRecord(Guid Id, string Name, string Owner, LicenseCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LicenseCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LicenseCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LicenseCatalogQuery(string? SearchText, LicenseCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LicenseCatalogPage(IReadOnlyList<LicenseCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LicenseCatalogMutation(bool Succeeded, string Code, string Message, LicenseCatalogRecord? Record, LicenseCatalogEvent? Event);
public interface ILicenseCatalogRepository
{
    ValueTask<LicenseCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LicenseCatalogPage> QueryAsync(LicenseCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<LicenseCatalogMutation> SaveAsync(LicenseCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILicenseCatalogEventSink { ValueTask PublishAsync(LicenseCatalogEvent domainEvent, CancellationToken cancellationToken); }