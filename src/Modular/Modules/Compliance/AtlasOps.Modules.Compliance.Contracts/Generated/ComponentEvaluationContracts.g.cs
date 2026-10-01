namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ComponentEvaluationState { Draft, Active, Paused, Completed, Archived }
public sealed record ComponentEvaluationRecord(Guid Id, string Name, string Owner, ComponentEvaluationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ComponentEvaluationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ComponentEvaluationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ComponentEvaluationQuery(string? SearchText, ComponentEvaluationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ComponentEvaluationPage(IReadOnlyList<ComponentEvaluationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ComponentEvaluationMutation(bool Succeeded, string Code, string Message, ComponentEvaluationRecord? Record, ComponentEvaluationEvent? Event);
public interface IComponentEvaluationRepository
{
    ValueTask<ComponentEvaluationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ComponentEvaluationPage> QueryAsync(ComponentEvaluationQuery query, CancellationToken cancellationToken);
    ValueTask<ComponentEvaluationMutation> SaveAsync(ComponentEvaluationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IComponentEvaluationEventSink { ValueTask PublishAsync(ComponentEvaluationEvent domainEvent, CancellationToken cancellationToken); }