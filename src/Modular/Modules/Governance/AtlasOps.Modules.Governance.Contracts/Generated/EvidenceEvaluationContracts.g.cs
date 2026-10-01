namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum EvidenceEvaluationState { Draft, Active, Paused, Completed, Archived }
public sealed record EvidenceEvaluationRecord(Guid Id, string Name, string Owner, EvidenceEvaluationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record EvidenceEvaluationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record EvidenceEvaluationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record EvidenceEvaluationQuery(string? SearchText, EvidenceEvaluationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record EvidenceEvaluationPage(IReadOnlyList<EvidenceEvaluationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record EvidenceEvaluationMutation(bool Succeeded, string Code, string Message, EvidenceEvaluationRecord? Record, EvidenceEvaluationEvent? Event);
public interface IEvidenceEvaluationRepository
{
    ValueTask<EvidenceEvaluationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<EvidenceEvaluationPage> QueryAsync(EvidenceEvaluationQuery query, CancellationToken cancellationToken);
    ValueTask<EvidenceEvaluationMutation> SaveAsync(EvidenceEvaluationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IEvidenceEvaluationEventSink { ValueTask PublishAsync(EvidenceEvaluationEvent domainEvent, CancellationToken cancellationToken); }