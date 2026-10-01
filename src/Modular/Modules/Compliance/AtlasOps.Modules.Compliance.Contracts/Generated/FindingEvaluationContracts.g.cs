namespace AtlasOps.Modules.Compliance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum FindingEvaluationState { Draft, Active, Paused, Completed, Archived }
public sealed record FindingEvaluationRecord(Guid Id, string Name, string Owner, FindingEvaluationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record FindingEvaluationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record FindingEvaluationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record FindingEvaluationQuery(string? SearchText, FindingEvaluationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record FindingEvaluationPage(IReadOnlyList<FindingEvaluationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record FindingEvaluationMutation(bool Succeeded, string Code, string Message, FindingEvaluationRecord? Record, FindingEvaluationEvent? Event);
public interface IFindingEvaluationRepository
{
    ValueTask<FindingEvaluationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<FindingEvaluationPage> QueryAsync(FindingEvaluationQuery query, CancellationToken cancellationToken);
    ValueTask<FindingEvaluationMutation> SaveAsync(FindingEvaluationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IFindingEvaluationEventSink { ValueTask PublishAsync(FindingEvaluationEvent domainEvent, CancellationToken cancellationToken); }