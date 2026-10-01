namespace AtlasOps.Modules.Geospatial.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum LayerValidationState { Draft, Active, Paused, Completed, Archived }
public sealed record LayerValidationRecord(Guid Id, string Name, string Owner, LayerValidationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record LayerValidationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record LayerValidationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record LayerValidationQuery(string? SearchText, LayerValidationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record LayerValidationPage(IReadOnlyList<LayerValidationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record LayerValidationMutation(bool Succeeded, string Code, string Message, LayerValidationRecord? Record, LayerValidationEvent? Event);
public interface ILayerValidationRepository
{
    ValueTask<LayerValidationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<LayerValidationPage> QueryAsync(LayerValidationQuery query, CancellationToken cancellationToken);
    ValueTask<LayerValidationMutation> SaveAsync(LayerValidationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface ILayerValidationEventSink { ValueTask PublishAsync(LayerValidationEvent domainEvent, CancellationToken cancellationToken); }