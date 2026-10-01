namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ObjectiveCorrelationState { Draft, Active, Paused, Completed, Archived }
public sealed record ObjectiveCorrelationRecord(Guid Id, string Name, string Owner, ObjectiveCorrelationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ObjectiveCorrelationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ObjectiveCorrelationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ObjectiveCorrelationQuery(string? SearchText, ObjectiveCorrelationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ObjectiveCorrelationPage(IReadOnlyList<ObjectiveCorrelationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ObjectiveCorrelationMutation(bool Succeeded, string Code, string Message, ObjectiveCorrelationRecord? Record, ObjectiveCorrelationEvent? Event);
public interface IObjectiveCorrelationRepository
{
    ValueTask<ObjectiveCorrelationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ObjectiveCorrelationPage> QueryAsync(ObjectiveCorrelationQuery query, CancellationToken cancellationToken);
    ValueTask<ObjectiveCorrelationMutation> SaveAsync(ObjectiveCorrelationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IObjectiveCorrelationEventSink { ValueTask PublishAsync(ObjectiveCorrelationEvent domainEvent, CancellationToken cancellationToken); }