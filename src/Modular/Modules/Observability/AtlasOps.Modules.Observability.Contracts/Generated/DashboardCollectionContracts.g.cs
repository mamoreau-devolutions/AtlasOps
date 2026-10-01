namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum DashboardCollectionState { Draft, Active, Paused, Completed, Archived }
public sealed record DashboardCollectionRecord(Guid Id, string Name, string Owner, DashboardCollectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record DashboardCollectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DashboardCollectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record DashboardCollectionQuery(string? SearchText, DashboardCollectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record DashboardCollectionPage(IReadOnlyList<DashboardCollectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record DashboardCollectionMutation(bool Succeeded, string Code, string Message, DashboardCollectionRecord? Record, DashboardCollectionEvent? Event);
public interface IDashboardCollectionRepository
{
    ValueTask<DashboardCollectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<DashboardCollectionPage> QueryAsync(DashboardCollectionQuery query, CancellationToken cancellationToken);
    ValueTask<DashboardCollectionMutation> SaveAsync(DashboardCollectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IDashboardCollectionEventSink { ValueTask PublishAsync(DashboardCollectionEvent domainEvent, CancellationToken cancellationToken); }