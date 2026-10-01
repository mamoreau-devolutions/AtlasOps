namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ExceptionEvaluationState { Draft, Active, Paused, Completed, Archived }
public sealed record ExceptionEvaluationRecord(Guid Id, string Name, string Owner, ExceptionEvaluationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ExceptionEvaluationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ExceptionEvaluationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ExceptionEvaluationQuery(string? SearchText, ExceptionEvaluationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ExceptionEvaluationPage(IReadOnlyList<ExceptionEvaluationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ExceptionEvaluationMutation(bool Succeeded, string Code, string Message, ExceptionEvaluationRecord? Record, ExceptionEvaluationEvent? Event);
public interface IExceptionEvaluationRepository
{
    ValueTask<ExceptionEvaluationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ExceptionEvaluationPage> QueryAsync(ExceptionEvaluationQuery query, CancellationToken cancellationToken);
    ValueTask<ExceptionEvaluationMutation> SaveAsync(ExceptionEvaluationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IExceptionEvaluationEventSink { ValueTask PublishAsync(ExceptionEvaluationEvent domainEvent, CancellationToken cancellationToken); }