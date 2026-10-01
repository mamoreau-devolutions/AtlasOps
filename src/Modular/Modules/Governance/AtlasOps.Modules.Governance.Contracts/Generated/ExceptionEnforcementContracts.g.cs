namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ExceptionEnforcementState { Draft, Active, Paused, Completed, Archived }
public sealed record ExceptionEnforcementRecord(Guid Id, string Name, string Owner, ExceptionEnforcementState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ExceptionEnforcementCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ExceptionEnforcementEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ExceptionEnforcementQuery(string? SearchText, ExceptionEnforcementState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ExceptionEnforcementPage(IReadOnlyList<ExceptionEnforcementRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ExceptionEnforcementMutation(bool Succeeded, string Code, string Message, ExceptionEnforcementRecord? Record, ExceptionEnforcementEvent? Event);
public interface IExceptionEnforcementRepository
{
    ValueTask<ExceptionEnforcementRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ExceptionEnforcementPage> QueryAsync(ExceptionEnforcementQuery query, CancellationToken cancellationToken);
    ValueTask<ExceptionEnforcementMutation> SaveAsync(ExceptionEnforcementRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IExceptionEnforcementEventSink { ValueTask PublishAsync(ExceptionEnforcementEvent domainEvent, CancellationToken cancellationToken); }