namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RolloutPlanningState { Draft, Active, Paused, Completed, Archived }
public sealed record RolloutPlanningRecord(Guid Id, string Name, string Owner, RolloutPlanningState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RolloutPlanningCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RolloutPlanningEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RolloutPlanningQuery(string? SearchText, RolloutPlanningState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RolloutPlanningPage(IReadOnlyList<RolloutPlanningRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RolloutPlanningMutation(bool Succeeded, string Code, string Message, RolloutPlanningRecord? Record, RolloutPlanningEvent? Event);
public interface IRolloutPlanningRepository
{
    ValueTask<RolloutPlanningRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RolloutPlanningPage> QueryAsync(RolloutPlanningQuery query, CancellationToken cancellationToken);
    ValueTask<RolloutPlanningMutation> SaveAsync(RolloutPlanningRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRolloutPlanningEventSink { ValueTask PublishAsync(RolloutPlanningEvent domainEvent, CancellationToken cancellationToken); }