namespace AtlasOps.Modules.Deployments.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ReleaseValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record ReleaseValidationRecord(Guid Id, string Name, string Owner, ReleaseValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ReleaseValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ReleaseValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ReleaseValidationQuery(string? SearchText, ReleaseValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ReleaseValidationPage(IReadOnlyList<ReleaseValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ReleaseValidationMutation(bool Succeeded, string Code, string Message, ReleaseValidationRecord? Record, ReleaseValidationEvent? Event);
public interface IReleaseValidationRepository
{
    ValueTask<ReleaseValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ReleaseValidationPage> QueryAsync(ReleaseValidationQuery query, CancellationToken cancellationToken);
    ValueTask<ReleaseValidationMutation> SaveAsync(ReleaseValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IReleaseValidationEventSink { ValueTask PublishAsync(ReleaseValidationEvent domainEvent, CancellationToken cancellationToken); }