namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ControlEvaluationState { Draft, Active, Paused, Completed, Archived }
public sealed record ControlEvaluationRecord(Guid Id, string Name, string Owner, ControlEvaluationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ControlEvaluationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ControlEvaluationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ControlEvaluationQuery(string? SearchText, ControlEvaluationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ControlEvaluationPage(IReadOnlyList<ControlEvaluationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ControlEvaluationMutation(bool Succeeded, string Code, string Message, ControlEvaluationRecord? Record, ControlEvaluationEvent? Event);
public interface IControlEvaluationRepository
{
    ValueTask<ControlEvaluationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ControlEvaluationPage> QueryAsync(ControlEvaluationQuery query, CancellationToken cancellationToken);
    ValueTask<ControlEvaluationMutation> SaveAsync(ControlEvaluationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IControlEvaluationEventSink { ValueTask PublishAsync(ControlEvaluationEvent domainEvent, CancellationToken cancellationToken); }