namespace AtlasOps.Modules.Governance.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ExceptionAuthoringState { Draft, Active, Paused, Completed, Archived }
public sealed record ExceptionAuthoringRecord(Guid Id, string Name, string Owner, ExceptionAuthoringState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ExceptionAuthoringCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ExceptionAuthoringEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ExceptionAuthoringQuery(string? SearchText, ExceptionAuthoringState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ExceptionAuthoringPage(IReadOnlyList<ExceptionAuthoringRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ExceptionAuthoringMutation(bool Succeeded, string Code, string Message, ExceptionAuthoringRecord? Record, ExceptionAuthoringEvent? Event);
public interface IExceptionAuthoringRepository
{
    ValueTask<ExceptionAuthoringRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ExceptionAuthoringPage> QueryAsync(ExceptionAuthoringQuery query, CancellationToken cancellationToken);
    ValueTask<ExceptionAuthoringMutation> SaveAsync(ExceptionAuthoringRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IExceptionAuthoringEventSink { ValueTask PublishAsync(ExceptionAuthoringEvent domainEvent, CancellationToken cancellationToken); }