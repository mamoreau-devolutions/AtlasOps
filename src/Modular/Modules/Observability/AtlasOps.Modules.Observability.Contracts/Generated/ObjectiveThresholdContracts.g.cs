namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ObjectiveThresholdState { Draft, Active, Paused, Completed, Archived }
public sealed record ObjectiveThresholdRecord(Guid Id, string Name, string Owner, ObjectiveThresholdState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ObjectiveThresholdCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ObjectiveThresholdEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ObjectiveThresholdQuery(string? SearchText, ObjectiveThresholdState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ObjectiveThresholdPage(IReadOnlyList<ObjectiveThresholdRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ObjectiveThresholdMutation(bool Succeeded, string Code, string Message, ObjectiveThresholdRecord? Record, ObjectiveThresholdEvent? Event);
public interface IObjectiveThresholdRepository
{
    ValueTask<ObjectiveThresholdRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ObjectiveThresholdPage> QueryAsync(ObjectiveThresholdQuery query, CancellationToken cancellationToken);
    ValueTask<ObjectiveThresholdMutation> SaveAsync(ObjectiveThresholdRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IObjectiveThresholdEventSink { ValueTask PublishAsync(ObjectiveThresholdEvent domainEvent, CancellationToken cancellationToken); }