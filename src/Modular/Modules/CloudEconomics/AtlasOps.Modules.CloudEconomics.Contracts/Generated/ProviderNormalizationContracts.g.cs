namespace AtlasOps.Modules.CloudEconomics.Contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public enum ProviderNormalizationState { Draft, Active, Paused, Completed, Archived }
public sealed record ProviderNormalizationRecord(Guid Id, string Name, string Owner, ProviderNormalizationState State, int Priority, decimal EstimatedCost, double RiskScore, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? DueAt, long Revision, IReadOnlyDictionary<string, string> Attributes);
public sealed record ProviderNormalizationCommand(Guid RecordId, string Action, string Actor, long ExpectedRevision, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ProviderNormalizationEvent(Guid EventId, Guid RecordId, string EventType, string Actor, long Revision, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Details);
public sealed record ProviderNormalizationQuery(string? SearchText, ProviderNormalizationState? State, string? Owner, int? MinimumPriority, double? MaximumRisk, int Offset, int Limit);
public sealed record ProviderNormalizationPage(IReadOnlyList<ProviderNormalizationRecord> Items, int Offset, int Limit, int TotalCount);
public sealed record ProviderNormalizationMutation(bool Succeeded, string Code, string Message, ProviderNormalizationRecord? Record, ProviderNormalizationEvent? Event);
public interface IProviderNormalizationRepository
{
    ValueTask<ProviderNormalizationRecord?> GetAsync(Guid id, CancellationToken cancellationToken);
    ValueTask<ProviderNormalizationPage> QueryAsync(ProviderNormalizationQuery query, CancellationToken cancellationToken);
    ValueTask<ProviderNormalizationMutation> SaveAsync(ProviderNormalizationRecord record, long expectedRevision, CancellationToken cancellationToken);
    ValueTask<bool> DeleteAsync(Guid id, long expectedRevision, CancellationToken cancellationToken);
}
public interface IProviderNormalizationEventSink { ValueTask PublishAsync(ProviderNormalizationEvent domainEvent, CancellationToken cancellationToken); }