namespace AtlasOps.Modules.Credentials.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RotationRotationState { Draft, Active, Paused, Completed, Archived }
public sealed record RotationRotationRecord(Guid Id, string Name, string Owner, RotationRotationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RotationRotationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RotationRotationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RotationRotationQuery(string? SearchText, RotationRotationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RotationRotationPage(IReadOnlyList<RotationRotationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RotationRotationMutation(bool Succeeded, string Code, string Message, RotationRotationRecord? Record, RotationRotationEvent? Event);
public interface IRotationRotationRepository
{
    ValueTask<RotationRotationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RotationRotationPage> QueryAsync(RotationRotationQuery query, CancellationToken cancellationToken);
    ValueTask<RotationRotationMutation> SaveAsync(RotationRotationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRotationRotationEventSink { ValueTask PublishAsync(RotationRotationEvent domainEvent, CancellationToken cancellationToken); }