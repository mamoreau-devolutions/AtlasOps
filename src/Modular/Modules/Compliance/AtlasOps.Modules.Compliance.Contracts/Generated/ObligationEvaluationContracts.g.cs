namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ObligationEvaluationState { Draft, Active, Paused, Completed, Archived }
public sealed record ObligationEvaluationRecord(Guid Id, string Name, string Owner, ObligationEvaluationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ObligationEvaluationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ObligationEvaluationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ObligationEvaluationQuery(string? SearchText, ObligationEvaluationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ObligationEvaluationPage(IReadOnlyList<ObligationEvaluationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ObligationEvaluationMutation(bool Succeeded, string Code, string Message, ObligationEvaluationRecord? Record, ObligationEvaluationEvent? Event);
public interface IObligationEvaluationRepository
{
    ValueTask<ObligationEvaluationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ObligationEvaluationPage> QueryAsync(ObligationEvaluationQuery query, CancellationToken cancellationToken);
    ValueTask<ObligationEvaluationMutation> SaveAsync(ObligationEvaluationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IObligationEvaluationEventSink { ValueTask PublishAsync(ObligationEvaluationEvent domainEvent, CancellationToken cancellationToken); }