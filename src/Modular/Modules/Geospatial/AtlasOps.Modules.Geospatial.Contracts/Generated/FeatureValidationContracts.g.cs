namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum FeatureValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record FeatureValidationRecord(Guid Id, string Name, string Owner, FeatureValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record FeatureValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record FeatureValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record FeatureValidationQuery(string? SearchText, FeatureValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record FeatureValidationPage(IReadOnlyList<FeatureValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record FeatureValidationMutation(bool Succeeded, string Code, string Message, FeatureValidationRecord? Record, FeatureValidationEvent? Event);
public interface IFeatureValidationRepository
{
    ValueTask<FeatureValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<FeatureValidationPage> QueryAsync(FeatureValidationQuery query, CancellationToken cancellationToken);
    ValueTask<FeatureValidationMutation> SaveAsync(FeatureValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IFeatureValidationEventSink { ValueTask PublishAsync(FeatureValidationEvent domainEvent, CancellationToken cancellationToken); }