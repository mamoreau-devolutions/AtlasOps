namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum OfferingNormalizationState { Draft, Active, Paused, Completed, Archived }
public sealed record OfferingNormalizationRecord(Guid Id, string Name, string Owner, OfferingNormalizationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record OfferingNormalizationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record OfferingNormalizationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record OfferingNormalizationQuery(string? SearchText, OfferingNormalizationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record OfferingNormalizationPage(IReadOnlyList<OfferingNormalizationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record OfferingNormalizationMutation(bool Succeeded, string Code, string Message, OfferingNormalizationRecord? Record, OfferingNormalizationEvent? Event);
public interface IOfferingNormalizationRepository
{
    ValueTask<OfferingNormalizationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<OfferingNormalizationPage> QueryAsync(OfferingNormalizationQuery query, CancellationToken cancellationToken);
    ValueTask<OfferingNormalizationMutation> SaveAsync(OfferingNormalizationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IOfferingNormalizationEventSink { ValueTask PublishAsync(OfferingNormalizationEvent domainEvent, CancellationToken cancellationToken); }