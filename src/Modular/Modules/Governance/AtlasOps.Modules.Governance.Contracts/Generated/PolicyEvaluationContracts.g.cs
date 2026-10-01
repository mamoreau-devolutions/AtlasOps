namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PolicyEvaluationState { Draft, Active, Paused, Completed, Archived }
public sealed record PolicyEvaluationRecord(Guid Id, string Name, string Owner, PolicyEvaluationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PolicyEvaluationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PolicyEvaluationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PolicyEvaluationQuery(string? SearchText, PolicyEvaluationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PolicyEvaluationPage(IReadOnlyList<PolicyEvaluationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PolicyEvaluationMutation(bool Succeeded, string Code, string Message, PolicyEvaluationRecord? Record, PolicyEvaluationEvent? Event);
public interface IPolicyEvaluationRepository
{
    ValueTask<PolicyEvaluationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PolicyEvaluationPage> QueryAsync(PolicyEvaluationQuery query, CancellationToken cancellationToken);
    ValueTask<PolicyEvaluationMutation> SaveAsync(PolicyEvaluationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPolicyEvaluationEventSink { ValueTask PublishAsync(PolicyEvaluationEvent domainEvent, CancellationToken cancellationToken); }