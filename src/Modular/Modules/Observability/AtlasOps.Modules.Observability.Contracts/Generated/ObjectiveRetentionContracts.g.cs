namespace AtlasOps.Modules.Observability.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ObjectiveRetentionState { Draft, Active, Paused, Completed, Archived }
public sealed record ObjectiveRetentionRecord(Guid Id, string Name, string Owner, ObjectiveRetentionState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ObjectiveRetentionCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ObjectiveRetentionEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ObjectiveRetentionQuery(string? SearchText, ObjectiveRetentionState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ObjectiveRetentionPage(IReadOnlyList<ObjectiveRetentionRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ObjectiveRetentionMutation(bool Succeeded, string Code, string Message, ObjectiveRetentionRecord? Record, ObjectiveRetentionEvent? Event);
public interface IObjectiveRetentionRepository
{
    ValueTask<ObjectiveRetentionRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ObjectiveRetentionPage> QueryAsync(ObjectiveRetentionQuery query, CancellationToken cancellationToken);
    ValueTask<ObjectiveRetentionMutation> SaveAsync(ObjectiveRetentionRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IObjectiveRetentionEventSink { ValueTask PublishAsync(ObjectiveRetentionEvent domainEvent, CancellationToken cancellationToken); }