namespace AtlasOps.Modules.Geography.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum RegionValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record RegionValidationRecord(Guid Id, string Name, string Owner, RegionValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record RegionValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record RegionValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record RegionValidationQuery(string? SearchText, RegionValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record RegionValidationPage(IReadOnlyList<RegionValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record RegionValidationMutation(bool Succeeded, string Code, string Message, RegionValidationRecord? Record, RegionValidationEvent? Event);
public interface IRegionValidationRepository
{
    ValueTask<RegionValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<RegionValidationPage> QueryAsync(RegionValidationQuery query, CancellationToken cancellationToken);
    ValueTask<RegionValidationMutation> SaveAsync(RegionValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IRegionValidationEventSink { ValueTask PublishAsync(RegionValidationEvent domainEvent, CancellationToken cancellationToken); }