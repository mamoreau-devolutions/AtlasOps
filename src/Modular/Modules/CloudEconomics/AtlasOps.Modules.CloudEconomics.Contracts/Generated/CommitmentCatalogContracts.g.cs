namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum CommitmentCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record CommitmentCatalogRecord(Guid Id, string Name, string Owner, CommitmentCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record CommitmentCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CommitmentCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record CommitmentCatalogQuery(string? SearchText, CommitmentCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record CommitmentCatalogPage(IReadOnlyList<CommitmentCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record CommitmentCatalogMutation(bool Succeeded, string Code, string Message, CommitmentCatalogRecord? Record, CommitmentCatalogEvent? Event);
public interface ICommitmentCatalogRepository
{
    ValueTask<CommitmentCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<CommitmentCatalogPage> QueryAsync(CommitmentCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<CommitmentCatalogMutation> SaveAsync(CommitmentCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ICommitmentCatalogEventSink { ValueTask PublishAsync(CommitmentCatalogEvent domainEvent, CancellationToken cancellationToken); }