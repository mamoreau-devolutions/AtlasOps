namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RolloutValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record RolloutValidationRecord(Guid Id, string Name, string Owner, RolloutValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RolloutValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RolloutValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RolloutValidationQuery(string? SearchText, RolloutValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RolloutValidationPage(IReadOnlyList<RolloutValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RolloutValidationMutation(bool Succeeded, string Code, string Message, RolloutValidationRecord? Record, RolloutValidationEvent? Event);
public interface IRolloutValidationRepository
{
    ValueTask<RolloutValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RolloutValidationPage> QueryAsync(RolloutValidationQuery query, CancellationToken cancellationToken);
    ValueTask<RolloutValidationMutation> SaveAsync(RolloutValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRolloutValidationEventSink { ValueTask PublishAsync(RolloutValidationEvent domainEvent, CancellationToken cancellationToken); }