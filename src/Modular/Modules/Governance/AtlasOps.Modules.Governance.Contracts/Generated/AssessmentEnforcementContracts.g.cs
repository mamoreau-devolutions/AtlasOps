namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssessmentEnforcementState { Draft, Active, Paused, Completed, Archived }
public sealed record AssessmentEnforcementRecord(Guid Id, string Name, string Owner, AssessmentEnforcementState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssessmentEnforcementCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssessmentEnforcementEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssessmentEnforcementQuery(string? SearchText, AssessmentEnforcementState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssessmentEnforcementPage(IReadOnlyList<AssessmentEnforcementRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssessmentEnforcementMutation(bool Succeeded, string Code, string Message, AssessmentEnforcementRecord? Record, AssessmentEnforcementEvent? Event);
public interface IAssessmentEnforcementRepository
{
    ValueTask<AssessmentEnforcementRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssessmentEnforcementPage> QueryAsync(AssessmentEnforcementQuery query, CancellationToken cancellationToken);
    ValueTask<AssessmentEnforcementMutation> SaveAsync(AssessmentEnforcementRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssessmentEnforcementEventSink { ValueTask PublishAsync(AssessmentEnforcementEvent domainEvent, CancellationToken cancellationToken); }