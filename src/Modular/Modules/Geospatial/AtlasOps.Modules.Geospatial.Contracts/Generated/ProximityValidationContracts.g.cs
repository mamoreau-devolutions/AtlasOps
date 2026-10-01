namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProximityValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record ProximityValidationRecord(Guid Id, string Name, string Owner, ProximityValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProximityValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProximityValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProximityValidationQuery(string? SearchText, ProximityValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProximityValidationPage(IReadOnlyList<ProximityValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProximityValidationMutation(bool Succeeded, string Code, string Message, ProximityValidationRecord? Record, ProximityValidationEvent? Event);
public interface IProximityValidationRepository
{
    ValueTask<ProximityValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProximityValidationPage> QueryAsync(ProximityValidationQuery query, CancellationToken cancellationToken);
    ValueTask<ProximityValidationMutation> SaveAsync(ProximityValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProximityValidationEventSink { ValueTask PublishAsync(ProximityValidationEvent domainEvent, CancellationToken cancellationToken); }