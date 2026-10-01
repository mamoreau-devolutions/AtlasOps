namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssessmentEvaluationState { Draft, Active, Paused, Completed, Archived }
public sealed record AssessmentEvaluationRecord(Guid Id, string Name, string Owner, AssessmentEvaluationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssessmentEvaluationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssessmentEvaluationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssessmentEvaluationQuery(string? SearchText, AssessmentEvaluationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssessmentEvaluationPage(IReadOnlyList<AssessmentEvaluationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssessmentEvaluationMutation(bool Succeeded, string Code, string Message, AssessmentEvaluationRecord? Record, AssessmentEvaluationEvent? Event);
public interface IAssessmentEvaluationRepository
{
    ValueTask<AssessmentEvaluationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssessmentEvaluationPage> QueryAsync(AssessmentEvaluationQuery query, CancellationToken cancellationToken);
    ValueTask<AssessmentEvaluationMutation> SaveAsync(AssessmentEvaluationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssessmentEvaluationEventSink { ValueTask PublishAsync(AssessmentEvaluationEvent domainEvent, CancellationToken cancellationToken); }