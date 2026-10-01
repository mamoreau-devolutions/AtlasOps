namespace AtlasOps.Modules.NetworkIntelligence.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssignmentCatalogState { Draft, Active, Paused, Completed, Archived }
public sealed record AssignmentCatalogRecord(Guid Id, string Name, string Owner, AssignmentCatalogState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssignmentCatalogCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssignmentCatalogEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssignmentCatalogQuery(string? SearchText, AssignmentCatalogState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssignmentCatalogPage(IReadOnlyList<AssignmentCatalogRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssignmentCatalogMutation(bool Succeeded, string Code, string Message, AssignmentCatalogRecord? Record, AssignmentCatalogEvent? Event);
public interface IAssignmentCatalogRepository
{
    ValueTask<AssignmentCatalogRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssignmentCatalogPage> QueryAsync(AssignmentCatalogQuery query, CancellationToken cancellationToken);
    ValueTask<AssignmentCatalogMutation> SaveAsync(AssignmentCatalogRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssignmentCatalogEventSink { ValueTask PublishAsync(AssignmentCatalogEvent domainEvent, CancellationToken cancellationToken); }