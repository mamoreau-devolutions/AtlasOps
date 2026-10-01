namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ObjectiveCollectionState { Draft, Active, Paused, Completed, Archived }
public sealed record ObjectiveCollectionRecord(Guid Id, string Name, string Owner, ObjectiveCollectionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ObjectiveCollectionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ObjectiveCollectionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ObjectiveCollectionQuery(string? SearchText, ObjectiveCollectionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ObjectiveCollectionPage(IReadOnlyList<ObjectiveCollectionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ObjectiveCollectionMutation(bool Succeeded, string Code, string Message, ObjectiveCollectionRecord? Record, ObjectiveCollectionEvent? Event);
public interface IObjectiveCollectionRepository
{
    ValueTask<ObjectiveCollectionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ObjectiveCollectionPage> QueryAsync(ObjectiveCollectionQuery query, CancellationToken cancellationToken);
    ValueTask<ObjectiveCollectionMutation> SaveAsync(ObjectiveCollectionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IObjectiveCollectionEventSink { ValueTask PublishAsync(ObjectiveCollectionEvent domainEvent, CancellationToken cancellationToken); }