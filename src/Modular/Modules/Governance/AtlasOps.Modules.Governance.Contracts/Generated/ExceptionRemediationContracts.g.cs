namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ExceptionRemediationState { Draft, Active, Paused, Completed, Archived }
public sealed record ExceptionRemediationRecord(Guid Id, string Name, string Owner, ExceptionRemediationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ExceptionRemediationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ExceptionRemediationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ExceptionRemediationQuery(string? SearchText, ExceptionRemediationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ExceptionRemediationPage(IReadOnlyList<ExceptionRemediationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ExceptionRemediationMutation(bool Succeeded, string Code, string Message, ExceptionRemediationRecord? Record, ExceptionRemediationEvent? Event);
public interface IExceptionRemediationRepository
{
    ValueTask<ExceptionRemediationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ExceptionRemediationPage> QueryAsync(ExceptionRemediationQuery query, CancellationToken cancellationToken);
    ValueTask<ExceptionRemediationMutation> SaveAsync(ExceptionRemediationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IExceptionRemediationEventSink { ValueTask PublishAsync(ExceptionRemediationEvent domainEvent, CancellationToken cancellationToken); }