namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum AssessmentAuthoringState { Draft, Active, Paused, Completed, Archived }
public sealed record AssessmentAuthoringRecord(Guid Id, string Name, string Owner, AssessmentAuthoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record AssessmentAuthoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AssessmentAuthoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record AssessmentAuthoringQuery(string? SearchText, AssessmentAuthoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record AssessmentAuthoringPage(IReadOnlyList<AssessmentAuthoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record AssessmentAuthoringMutation(bool Succeeded, string Code, string Message, AssessmentAuthoringRecord? Record, AssessmentAuthoringEvent? Event);
public interface IAssessmentAuthoringRepository
{
    ValueTask<AssessmentAuthoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<AssessmentAuthoringPage> QueryAsync(AssessmentAuthoringQuery query, CancellationToken cancellationToken);
    ValueTask<AssessmentAuthoringMutation> SaveAsync(AssessmentAuthoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IAssessmentAuthoringEventSink { ValueTask PublishAsync(AssessmentAuthoringEvent domainEvent, CancellationToken cancellationToken); }