namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssessmentRemediationState { Draft, Active, Paused, Completed, Archived }
public sealed record AssessmentRemediationRecord(Guid Id, string Name, string Owner, AssessmentRemediationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssessmentRemediationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssessmentRemediationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssessmentRemediationQuery(string? SearchText, AssessmentRemediationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssessmentRemediationPage(IReadOnlyList<AssessmentRemediationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssessmentRemediationMutation(bool Succeeded, string Code, string Message, AssessmentRemediationRecord? Record, AssessmentRemediationEvent? Event);
public interface IAssessmentRemediationRepository
{
    ValueTask<AssessmentRemediationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssessmentRemediationPage> QueryAsync(AssessmentRemediationQuery query, CancellationToken cancellationToken);
    ValueTask<AssessmentRemediationMutation> SaveAsync(AssessmentRemediationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssessmentRemediationEventSink { ValueTask PublishAsync(AssessmentRemediationEvent domainEvent, CancellationToken cancellationToken); }