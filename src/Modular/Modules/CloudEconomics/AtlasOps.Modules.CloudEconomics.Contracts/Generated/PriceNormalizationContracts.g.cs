namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum PriceNormalizationState { Draft, Active, Paused, Completed, Archived }
public sealed record PriceNormalizationRecord(Guid Id, string Name, string Owner, PriceNormalizationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record PriceNormalizationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record PriceNormalizationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record PriceNormalizationQuery(string? SearchText, PriceNormalizationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record PriceNormalizationPage(IReadOnlyList<PriceNormalizationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record PriceNormalizationMutation(bool Succeeded, string Code, string Message, PriceNormalizationRecord? Record, PriceNormalizationEvent? Event);
public interface IPriceNormalizationRepository
{
    ValueTask<PriceNormalizationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<PriceNormalizationPage> QueryAsync(PriceNormalizationQuery query, CancellationToken cancellationToken);
    ValueTask<PriceNormalizationMutation> SaveAsync(PriceNormalizationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IPriceNormalizationEventSink { ValueTask PublishAsync(PriceNormalizationEvent domainEvent, CancellationToken cancellationToken); }