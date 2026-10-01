namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RollbackPlanningState { Draft, Active, Paused, Completed, Archived }
public sealed record RollbackPlanningRecord(Guid Id, string Name, string Owner, RollbackPlanningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RollbackPlanningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RollbackPlanningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RollbackPlanningQuery(string? SearchText, RollbackPlanningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RollbackPlanningPage(IReadOnlyList<RollbackPlanningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RollbackPlanningMutation(bool Succeeded, string Code, string Message, RollbackPlanningRecord? Record, RollbackPlanningEvent? Event);
public interface IRollbackPlanningRepository
{
    ValueTask<RollbackPlanningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RollbackPlanningPage> QueryAsync(RollbackPlanningQuery query, CancellationToken cancellationToken);
    ValueTask<RollbackPlanningMutation> SaveAsync(RollbackPlanningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRollbackPlanningEventSink { ValueTask PublishAsync(RollbackPlanningEvent domainEvent, CancellationToken cancellationToken); }